using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DScript {
	public Value evaluate(string input) {
		return interpret(desugar(parse(input)));
	}

	Expression parse(string input) {

	}

	Expression desugar(Expression sugaredExpression) {

	}

	Result interpret(Expression expression, Dictionary<string, string> enviroment, Dictionary<string, Value> store) {
		switch (returnValue.valueType) {
            case 1: //e-int
            	Value returnValue = new Value;
                returnValue.valueType = 1;
                returnValue.vInt = expression.eInt;
                return new Result(returnValue, store);
                break;
            case 2: //e-float
            	Value returnValue = new Value;
                returnValue.valueType = 2;
                returnValue.vFloat = expression.eFloat;
                return new Result(returnValue, store);
                break;
            case 3: //e-string
            	Value returnValue = new Value;
            	returnValue.valueType = 3;
            	returnValue.vString = expression.eString;
            	return new Result(returnValue, store);
            	break;
            case 4: //e-bool
            	Value returnValue = new Value;
            	returnValue.valueType = 4;
            	returnValue.vBool = expression.eBool;
            	return new Result(returnValue, store);
            	break;
            case 5: //e-op
            	returnValue = interpOperatpr(expression.eOperatorOp, expression.eOperatorLeft, expression.eOperatorRight, enviroment, store);
            	break;
            case 6: //e-if
            	returnValue = interpIf(expression.eIfCond, expression.eIfConsq, expression.eIfAlter, enviroment, store);
            	break;
            case 7: //e-lam
            	returnValue = interpLam(expression.eLamParam, expression.eLamBody, enviroment, store);
            	break;
            case 8: //e-app
            	returnValue = interpApp(expression.eAppFun, expression.eAppArg, enviroment, store);
            	break;
            case 9: //e-set
            	returnValue = interpSet(expression.eSetNAme, expression.eSetValue, enviroment, store);
            	break;
            case 10: //e-do
            	returnValue = interpDo(expression.eDo, enviroment, store);
            	break;	
            case 11: //e-while
            	returnValue = interpWhile(expression.eWhileCond, expression.eWhileBody, new Value(), false, enviroment, store);
            	break;	
        }
	}

	Result interpOperator(Operator op, Expression left, Expression right, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		switch (op.operatorType) {
			case 1: //Addition
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 2;
						returnValue.vFloat = l_val + r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 2: //Subtraction
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 2;
						returnValue.vFloat = l_val - r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 3: //Multiplication
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 2;
						returnValue.vFloat = l_val * r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 4: //Division
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 2;
						returnValue.vFloat = l_val / r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 5: //Exponent
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 2;
						returnValue.vFloat = Math.Pow(l_val, r_val);
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 6: //Modulo
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 2;
						returnValue.vFloat = l_val % r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 7: //List Append
				// List Append
				break;
			case 8: // String Append
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 3) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 3) {
						returnValue.valueType = 3;
						returnValue.vString = l_val + r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 9: // string equal
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 3) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 3) {
						returnValue.valueType = 4;
						returnValue.vBool = l_val == r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 10: // num equal
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 4;
						returnValue.vBool = l_val == r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 11: // num greater than
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 4;
						returnValue.vBool = l_val > r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 12: // num less than
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 4;
						returnValue.vBool = l_val < r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 13: // num geq
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 4;
						returnValue.vBool = l_val >= r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
			case 14: // num leq
				Result l_res = interpret(left, env, store);
				Value l_val = l_res.v;
				if (l_val.valueType == 2) {
					Result r_res = interpret(right, env, store);
					Value r_val = r_res.v;
					Dictionary<string, Value> r_store = r_res.s;
					if (r_val.valueType == 2) {
						returnValue.valueType = 4;
						returnValue.vBool = l_val <= r_val;
						return new Result(returnValue, store);
					} else {
						//Throw Error
					}
				} else {
					//Throw Error
				}
				break;
		}
		return returnValue;
	}

	Result interpIf(Expression cond, Expression consq, Expression alter, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result cond_Res = interpret(cond, env, store);
		Value cond_val = cond_Res.v;
		Dictionary<string, Value> cond_store = cond_Res.s;
		if (cond_val.valueType == 4) {
			if (cond_val.vBool) {
				return interpret(consq, env, cond_store);
			} else {
				return interpret(alter, env, cond_store);
			}
		} else {
			//Throw Error
		}
	}

	Result interpLam(string param, Expression body, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		Value.vFunParam = param;
		Value.vFunBody = body;
		Value.vFunEnviroment = env;
		return new Result(returnValue, store);
	}

	Result interpApp(Expression fun, Expression arg, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result fun_res = interpret(fun, env, store);
		Value fun_val = fun_res.v;
		Dictionary<string, Value> fun_store = fun_res.s;
		if (fun_val.valueType == 5) {
			Result arg_res = interpret(arg, env, funValue.store);
			Value arg_val = arg_res.v;
			Dictionary<string, Value> arg_store = arg_res.s;

			string loc = System.Guid.NewGuid().ToString();
			fun_val.vFunEnviroment.Add(fun_val.vFunParam, loc)
			arg_store.Add(loc, arg_val);

			return interpret(fun_val.vFunBody, fun_val.vFunEnviroment, arg_store)
		} else {
			//Throw Error
		}
	}

	Result interpSet(string name, Expression newValue, Dictionary<string, string> env, Dictionary<string, Value> store) {
		if (env.ContainsKey(name)) {
			string pointer = env[name];
			if (stire.ContainsKey(pointer)) {
				newValue_res = interpret(newValue, env, store);
				newValue_val = newValue_res.v;
				newValue_store = newValue_res.s;
				newValue_store[pointer] = newValue_val;
				return new Result(newValue_val, newValue_store);
			} else {
				//Throw Error, unbound
			}
		} else {
			//Throw Error, unbound
		}
	}

	Result interpDo(List<Expression> expressionList, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result cur_res;
		foreach (Expression expression in expressionList) {
			cur_res = interpret(expression, env, store);
			Value cur_val = cur_res.v;
			Dictionary<string, Value> cur_store = cur_res.s;
			store = cur_store;
		}
		return cur_res;
	}

	Result interpWhile(Expression cond, Expression body, Value lastValue, bool useLast, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result cond_res = interpret(cond, env, store);
		Value cond_val = cond_res.v;
		Dictionary<string, Value> cond_store = cond_res.s;
		if (cond_val.valueType == 4) {
			if (cond_val.vBool) {
				Result body_res = interpret(body, env, cond_store);
				Value body_val = body_res.v;
				Dictionary<string, Value> body_store = body_res.s;
				return interpWhile(cond, body, body_val, true, env, body_store);
			} else if (useLast) {
				return new Result(lastValue, cond_store);
			} else {
				return cond_res;
			}
		} else {
			// Throw Error
		}
	}
}

public class Result {
	Value v;
	Dictionary<string, Value> s;

	public Result(Value val, Dictionary<string, Value> store) {
		v = val;
		s = store;
	}
}

public class Value {

	public Value() {

	}

	int valueType; 

	int vInt; // type=1
	float vFloat; // type=2
	string vString; // type=3
	bool vBool; // type=4

	string vFunParam; // type=5
	Expression vFunBody;
	Dictionary<String, String> vFunEnviroment;	

	List<Value> vList; //type=5
}

public class Expression {
	int expressionType;

	int eInt; // type=1
	float eFloat; // type=2
	string eString; // type=3
	bool eBool; // type=4

	Operator eOperatorOp; // type=5
	Expression eOperatorLeft;
	Expression eOperatorRight;

	Expression eIfCond; // type=6
	Expression eIfConsq;
	Expression eIfAlter;

	string eLamParam; // type=7
	Expression eLamBody;

	Expression eAppFunc; // type=8
	Expression eAppArg;

	string eSetName; // type=9
	Expression eSetValue;

	List<Expression> eDo; //type=10

	Expression eWhileCond; //type=11
	Expression eWhileBody; 

	Expression eIndexListList; //type=12
	int eIndexListIndex;

	Expression eSublistList; //type=13
	int eSublistStart;
	int eSublistEnd;

	Expression eSubstringString; //type=13
	int eSubstringStart;
	int eSubstringEnd;

	//Sugar

	Expression eAndLeft; //type=100
	Expression eAndRight;

	Expression eOrLeft; //type=101
	Expression eOrRight;

	List<Expression> eForIter; //type=102
	string eForVariable;
	Expression eForBody;

	Expression eNot //type=103

	Expression eStringIndexString; //type=104
	int eStringIndexIndex;

	Expression eStringIndexOfString; //type=14
}

public class Operator {
	int operatorType;
	// Basic Math Operators
	// op-plus, type=1
	// op-minus, type=2
	// op-multiply, type=3
	// op-divide, type=4
	// op-exponent, type=5
	// op-modulus, type=6

	// op-list-append, type=7
	// op-str-append, type=8

	// op-str-eq, type=9
	// op-num-eq, type=10
	// op-num-g, type=11
	// op-num-l, type=12
	// op-num-geq, type=13
	// op-num-leq, type=14
}

public class BuiltInFunctions {
	// Expression eIndexListList; //type=12
	// int eIndexListIndex;

	// Expression eSubstringString; //type=13
	// Expression eSubstringStart;
	// Expression eSubstringEnd;

	// Expression eStringIndexOfString; //type=14

	// Other Math Operators
	// op-absoluteValue, type=9
	// op-max, type=10
	// op-min, type=11
	// op-floor, type=12
	// op-ceil, type=13
	// op-factorial, type=14
	// op-logarithm, type=6
	// op-round
}
