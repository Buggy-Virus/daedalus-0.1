/*
provide *

import string-dict as D
include shared-gdrive("interp-state-definitions.arr", "0Bxr4FfLa3goOc2dma0VuSFJQX00")

fun eval(str :: String) -> Value:
  doc: "Parse, desugar, and interpret a Paret program."
  {value; _} = interp(desugar(parse(str)), [D.string-dict: ], [D.string-dict: ])
  value
end

fun desugar(expr :: Expr) -> Expr%(is-desugared):
  doc: "desugar 'sugar-and','sugar-or', 'sugar-let', and 'sugar-rec-lam' in an expression."
  cases(Expr) expr:
    | e-num(v) => e-num(v)
    | e-str(v) => e-str(v)
    | e-bool(v) => e-bool(v)
    | e-op(o, l, r) => e-op(o, desugar(l), desugar(r))
    | e-if(c, con, alt) => e-if(desugar(c), desugar(con), desugar(alt))
    | e-lam(p, b) => e-lam(p, desugar(b))
    | e-app(f, a) => e-app(desugar(f), desugar(a))
    | e-id(n) => e-id(n)
    | e-set(n, v) => e-set(n, v)
    | e-do(s) => e-do(s)
    | sugar-and(l, r) =>
      e-if(
        desugar(l), 
        e-if(desugar(r), e-bool(true), e-bool(false)), 
        e-bool(false))
    | sugar-or(l, r) =>
      e-if(
        desugar(l), 
        e-bool(true), 
        e-if(desugar(r), e-bool(true), e-bool(false)))
    | sugar-let(id, v, b) =>
      e-app(e-lam(id, desugar(b)), desugar(v))
    | sugar-rec-lam(f, p, b) =>
      desugar(sugar-let(f, e-str("dummy"), e-set(f, e-lam(p, b))))
  end
end

fun interp(expr :: Expr%(is-desugared), env :: Env, store :: Store) -> Result:
  doc: "Compute the result of evaluating the given expression."
  cases(Expr) expr:
    | e-num(v) => {v-num(v); store}
    | e-str(v) => {v-str(v); store}
    | e-bool(v) => {v-bool(v); store}
    | e-op(o, l, r) =>
      cases(Operator) o:
        | op-plus => 
          l_pair = interp(l, env, store)
          {l_val; l_str} = l_pair
          cases(Value) l_val:
            | v-num(v) =>
              r_pair = interp(r, env, l_str)
              {r_val; r_str} = r_pair
              cases(Value) r_val:
                | v-num(v2) =>
                  {v-num(v + v2); r_str}
                | else =>
                  raise(err-bad-arg-to-op(o, r_val))
              end
            | else =>
              raise(err-bad-arg-to-op(o, l_val))
          end
        | op-num-eq =>
          l_pair = interp(l, env, store)
          {l_val; l_str} = l_pair
          cases(Value) l_val:
            | v-num(v) =>
              r_pair = interp(r, env, l_str)
              {r_val; r_str} = r_pair
              cases(Value) r_val:
                | v-num(v2) =>
                  {v-bool(v == v2); r_str}
                | else =>
                  raise(err-bad-arg-to-op(o, r_val))
              end
            | else =>
              raise(err-bad-arg-to-op(o, l_val))
          end
        | op-append =>
          l_pair = interp(l, env, store)
          {l_val; l_str} = l_pair
          cases(Value) l_val:
            | v-str(v) =>
              r_pair = interp(r, env, l_str)
              {r_val; r_str} = r_pair
              cases(Value) r_val:
                | v-str(v2) =>
                  {v-str(string-append(v, v2)); r_str}
                | else =>
                  raise(err-bad-arg-to-op(o, r_val))
              end
            | else =>
              raise(err-bad-arg-to-op(o, l_val))
          end
        | op-str-eq =>
          l_pair = interp(l, env, store)
          {l_val; l_str} = l_pair
          cases(Value) l_val:
            | v-str(v) =>
              r_pair = interp(r, env, l_str)
              {r_val; r_str} = r_pair
              cases(Value) r_val:
                | v-str(v2) =>
                  {v-bool(v == v2); r_str}
                | else =>
                  raise(err-bad-arg-to-op(o, r_val))
              end
            | else =>
              raise(err-bad-arg-to-op(o, l_val))
          end
      end
    | e-if(cond, cons, alt) =>
      cond_pair = interp(cond, env, store)
      {cond_val; cond_str} = cond_pair
      cases(Value) cond_val:
        | v-bool(v) =>
          if v:
            interp(cons, env, cond_str)
          else:
            interp(alt, env, cond_str)
          end
        | else =>
          raise(err-if-got-non-boolean(cond_val))
      end
    | e-id(n) =>
      if env.has-key(n):
        pointer = env.get-value(n)
        if store.has-key(pointer):
          {store.get-value(pointer); store}
        else:
          raise(err-unbound-id(n))
        end
      else:
        raise(err-unbound-id(n))
      end
    | e-set(n, v) => 
      if env.has-key(n):
        pointer = env.get-value(n)
        if store.has-key(pointer):
          v_pair = interp(v, env, store)
          {v_val; v_str} = v_pair
          {v_val; v_str.set(pointer, v_val)}
        else:
          raise(err-unbound-id(n))
        end
      else:
        raise(err-unbound-id(n))
      end
    | e-do(s) => helper-do(s, env, store)
    | e-lam(p, b) =>
      {v-fun(p, b, env); store}
    | e-app(f, a) =>
      f_pair = interp(f, env, store)
      {f_val; f_str} = f_pair
      cases(Value) f_val:
        | v-fun(p, b, e) =>
          a_pair = interp(a, env, f_str)
          {a_val; a_str} = a_pair
          loc = gensym("loc")
          interp(b, e.set(p, loc), a_str.set(loc, a_val))
        | else => 
          raise(err-not-a-function(f_val))
      end
  end
end

fun helper-do(s :: List<Expr>, env :: Env, store :: Store) -> Result:
  doc: "Takes in a do statement and evaluates each expr in its list, then returns the result of the final expression"
  cases(List) s:
    | link(f, r) =>
      cases(List) r:
        | link(f2, r2) => 
          pair = interp(f, env, store)
          {val; str} = pair
          helper-do(r, env, str)
        | empty => 
          interp(f, env, store)
      end
    | empty => raise(err-unbound-id("Should never hit this"))
  end
end
*/