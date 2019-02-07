using System;
using System.Linq;
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

	Result interpret(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		switch (expression.expressionType) {
			case 1: //e-int
				return interpInt(expression.eInt, env, store);
			case 2: //e-float
				return interpFloat(expression.eFloat, env, store);
			case 3: //e-string
				return interpFloat(expression.eString, env, store);
			case 4: //e-bool
				return interpFloat(expression.eBool, env, store);
			case 5: // e-list
				return interpList(expression.eList, env, store);
			case 6: //e-op
				return interpOperator(expression.eOperatorOp, expression.eOperatorLeft, expression.eOperatorRight, env, store);
			case 7: //e-if
				return interpIf(expression.eIfCond, expression.eIfConsq, expression.eIfAlter, env, store);
			case 8: //e-lam
				return interpLam(expression.eLamParam, expression.eLamBody, env, store);
			case 9: //e-app
				return interpApp(expression.eAppFunc, expression.eAppArg, env, store);
			case 10: //e-set
				return interpSet(expression.eSetName, expression.eSetValue, env, store);
			case 11: //e-do
				return interpDo(expression.eDo, env, store);	
			case 12: //e-while
				return interpWhile(expression.eWhileCond, expression.eWhileBody, new Value(), false, env, store);
			case 13: //e-define
				return interpret(expression.eDefineValue, env, store);
		}
	}

	Result interpInt(int eInt, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		returnValue.valueType = 1;
		returnValue.vInt = eInt;
		return new Result(returnValue, store);
	}

	Result interpFloat(float eFloat, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		returnValue.valueType = 2;
		returnValue.vFloat = eFloat;
		return new Result(returnValue, store);
	}

	Result interpString(string eString, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		returnValue.valueType = 3;
		returnValue.vString = eString;
		return new Result(returnValue, store);
	}

	Result interpBool(bool eBool, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = eBool;
		return new Result(returnValue, store);
	}

	Result interpList(List<Expression> expressionList, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		returnValue.valueType = 5;
		returnValue.vList = new List<Value>();
		Result last_result = new Result(new Value(), store);
		foreach (Expression expression in expressionList) {
			last_result = interpret(expression, env, last_result.store);
			returnValue.vList.Add(last_result.value);
		}
		return new Result(returnValue, last_result.store);
	}
	
	Result interpOperator(Operator op, Expression left, Expression right, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		switch (op.operatorType) {
			case 1: //Addition
				return interpOperatorHelper(additionValue, left, right, 2, 2);
			case 2: //Subtraction
				return interpOperatorHelper(subtractionValue, left, right, 2, 2);
			case 3: //Multiplication
				return interpOperatorHelper(multiplicationValue, left, right, 2, 2);
			case 4: //Division
				return interpOperatorHelper(divisionValue, left, right, 2, 2);
			case 5: //Exponent
				return interpOperatorHelper(exponentValue, left, right, 2, 2);
			case 6: //Modulo
				return interpOperatorHelper(moduloValue, left, right, 2, 2);
			case 7: //List Concat
				return interpOperatorHelper(listConcatValue, left, right, 5, 5);
			case 8: // String Concant
				return interpOperatorHelper(stringConcatValue, left, right, 3, 3);
			case 9: // string equal
				return interpOperatorHelper(stringEqualValue, left, right, 3, 3);
			case 10: // num equal
				return interpOperatorHelper(floatEqualValue, left, right, 2, 2);
			case 11: // num greater than
				return interpOperatorHelper(floatGreaterValue, left, right, 2, 2);
			case 12: // num less than
				return interpOperatorHelper(floatLesserValue, left, right, 2, 2);
			case 13: // num geq
				return interpOperatorHelper(floatGeqValue, left, right, 2, 2);
			case 14: // num leq
				return interpOperatorHelper(floatLeqValue, left, right, 2, 2);
			case 15: // List Index
				return interpOperatorHelper(listIndexValue, left, right, 5, 2);
			case 16: // String Index
				return interpOperatorHelper(stringIndexValue, left, right, 3, 2);
		}
	}

	Result interpOperatorHelper(Func<Value, Value, Value> valueFunc, Expression left, Expression right, Expression leftType, Expression rightType) {
		Result l_result = interpret(left, env, store);
		if (l_result.value.valueType == leftType) {
			Result r_result = interpret(right, env, store);
			if (r_result.value.valueType == rightType) {
				Value returnValue = valueFunc(l_result.value, r_result.value);
				return new Result(returnValue, r_result.store);
			} else {
				return new Result(new Value(rightType, r_result.value.valueType), r_result.store); //Throw Error
			}
		} else {
			return new Result(new Value(leftType, l_result.value.valueType), l_result.store); //Throw Error
		}
	}

	Value additionValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 2;
		returnValue.vFloat = leftValue.vFloat + rightValue.vFloat;
		return returnValue;
	}

	Value subtractionValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 2;
		returnValue.vFloat = leftValue.vFloat - rightValue.vFloat;
		return returnValue;
	}

	Value multiplicationValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 2;
		returnValue.vFloat = leftValue.vFloat * rightValue.vFloat;
		return returnValue;
	}

	Value divisionValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		
		if (rightValue.vFloat == 9) {
			returnValue.valueType = 0;
			returnValue.errorMessage = "Divide by zero error";
		} else {
			returnValue.valueType = 2;
			returnValue.vFloat = leftValue.vFloat / rightValue.vFloat;
		}
		return returnValue;
	}

	Value exponentValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 2;
		returnValue.vFloat = (float)Math.Pow(leftValue.vFloat, rightValue.vFloat);
		return returnValue;
	}

	Value moduloValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 2;
		returnValue.vFloat = leftValue.vFloat % rightValue.vFloat;
		return returnValue;
	}

	Value listConcatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 5;
		returnValue.vList = l_result.value.vList.Concat(r_result.value.vList).ToList();
		return returnValue;
	}

	Value strintConcatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 3;
		returnValue.vString = leftValue.vString + rightValue.vString;
		return returnValue;
	}

	Value strintEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = leftValue.vString == rightValue.vString;
		return returnValue;
	}

	Value floatEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = leftValue.vFloat == rightValue.vFloat;
		return returnValue;
	}

	Value floatGreaterValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = leftValue.vFloat > rightValue.vFloat;
		return returnValue;
	}

	Value floatLesserValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = leftValue.vFloat < rightValue.vFloat;
		return returnValue;
	}

	Value floatGeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = leftValue.vFloat >= rightValue.vFloat;
		return returnValue;
	}

	Value floatLeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 4;
		returnValue.vBool = leftValue.vFloat <= rightValue.vFloat;
		return returnValue;
	}

	Value listIndexValue(Value leftValue, Value rightValue) {
		return leftValue.value.vList[(int)rightValue.value.vFloat];
	}

	Value stringIndexValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value();
		returnValue.valueType = 3;
		returnValue.vString = leftValue.value.vString[(int)rightValue.value.vFloat].ToString();
		return returnValue;
	}

	Result interpIf(Expression cond, Expression consq, Expression alter, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result cond_result = interpret(cond, env, store);
		if (cond_result.value.valueType == 4) {
			if (cond_result.value.vBool) {
				return interpret(consq, env, cond_result.store);
			} else {
				return interpret(alter, env, cond_result.store);
			}
		} else {
			return new Result(new Value(4, cond_result.value.valueType), store);
		}
	}

	Result interpLam(string param, Expression body, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value();
		returnValue.vFunParam = param;
		returnValue.vFunBody = body;
		returnValue.vFunEnviroment = env;
		return new Result(returnValue, store);
	}

	Result interpApp(Expression func, Expression arg, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result func_result = interpret(func, env, store);
		if (func_result.value.valueType == 5) {
			Result arg_result = interpret(arg, env, func_result.store);
			string loc = System.Guid.NewGuid().ToString();
			func_result.value.vFunEnviroment.Add(func_result.value.vFunParam, loc);
			arg_result.store.Add(loc, arg_result.value);
			return interpret(func_result.value.vFunBody, func_result.value.vFunEnviroment, arg_result.store); 
		} else {
			return new Result(new Value(5, func_result.value.valueType), store); //Throw Error
		}
	}

	Result interpSet(string name, Expression newValue, Dictionary<string, string> env, Dictionary<string, Value> store) {
		if (env.ContainsKey(name)) {
			string pointer = env[name];
			if (store.ContainsKey(pointer)) {
				Result newValue_result = interpret(newValue, env, store);
				newValue_result.store[pointer] = newValue_result.value;
				return new Result(newValue_result.value, newValue_result.store);
			} else {
				Value errorValue = new Value();
				errorValue.errorMessage = "Identifier Pointing To Nothing: " + name;
				return new Result(errorValue, store);
			}
		} else {
			Value errorValue = new Value();
			errorValue.errorMessage = "Unbound identifier: " + name;
			return new Result(errorValue, store);
		}
	}

	Result interpDo(List<Expression> expressionList, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result last_result = new Result(new Value(), store);
		foreach (Expression expression in expressionList) {
			// If statement in Do handles define statements. Defines are only relevent in Do
			// The change to the env is only relevent across other Do expressions
			// In the same Do statement
			if (expression.expressionType == 12) {
				Result define_result = interpret(expression.eDefineValue, env, last_result.store);
				string loc = System.Guid.NewGuid().ToString();
				env.Add(expression.eDefineName, loc);
				define_result.store.Add(loc, define_result.value);
				last_result = define_result;
			} else {
				last_result = interpret(expression, env, last_result.store);
			}
			
		}
		return last_result;
	}

	Result interpWhile(Expression cond, Expression body, Value lastValue, bool useLast, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Result cond_result = interpret(cond, env, store);
		if (cond_result.value.valueType == 4) {
			if (cond_result.value.vBool) {
				Result body_result = interpret(body, env, cond_result.store);
				return interpWhile(cond, body, body_result.value, true, env, body_result.store);
			} else if (useLast) {
				return new Result(lastValue, cond_result.store);
			} else {
				return cond_result;
			}
		} else {
			return new Result(new Value(4, cond_result.value.valueType), store); //Throw Error
		}
	}
}

public class Result {
	public Value value;
	public Dictionary<string, Value> store;

	public Result(Value val, Dictionary<string, Value> str) {
		value = val;
		store = str;
	}
}

public class Value {

	public int valueType; 

	public int errorExpected; // type=0
	public int errorGot;
	public string errorMessage;

	public int vInt; // type=1
	public float vFloat; // type=2
	public string vString; // type=3
	public bool vBool; // type=4

	public string vFunParam; // type=5
	public Expression vFunBody;
	public Dictionary<String, String> vFunEnviroment;	

	public List<Value> vList; //type=5

	public Value() {

	}

	public Value(int expected, int got) {
		valueType = 0;
		errorExpected = expected;
		errorGot = got;
	}
}

public class Expression {
	public int expressionType;

	public int eInt; // type=1
	public float eFloat; // type=2
	public string eString; // type=3
	public bool eBool; // type=4
	public List<Expression> eList; // type=5

	public Operator eOperatorOp; // type=6
	public Expression eOperatorLeft;
	public Expression eOperatorRight;

	public Expression eIfCond; // type=7
	public Expression eIfConsq;
	public Expression eIfAlter;

	public string eLamParam; // type=8
	public Expression eLamBody;

	public Expression eAppFunc; // type=9
	public Expression eAppArg;

	public string eSetName; // type=10
	public Expression eSetValue;

	public List<Expression> eDo; //type=11

	public Expression eWhileCond; //type=12
	public Expression eWhileBody; 

	public string eDefineName; //type=13
	public Expression eDefineValue;

	//Sugar

	public Expression eAndLeft; //type=100
	public Expression eAndRight;

	public Expression eOrLeft; //type=101
	public Expression eOrRight;

	public List<Expression> eForIter; //type=102
	public string eForVariable;
	public Expression eForBody;

	public Expression eNot; //type=103

	public Expression eStringIndexString; //type=104
	public int eStringIndexIndex;

	public Expression eStringIndexOfString; //type=14
}

public class Operator {
	public int operatorType;
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

	// op-list-index type=15
	// op-string-index type=16	
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


	// public Expression eSublistList; //type=13
	// public int eSublistStart;
	// public int eSublistEnd;

	// public Expression eSubstringString; //type=13
	// public int eSubstringStart;
	// public int eSubstringEnd;
}
