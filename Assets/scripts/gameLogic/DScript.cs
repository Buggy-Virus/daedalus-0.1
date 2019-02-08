using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DScript {

	IgListScript igListScript;
	public Dictionary<string, Token> tokenEnv;
    public Dictionary<string, Cube> cubeEnv;

	void Start(){
		igListScript = GameObject.Find("GameLogic").GetComponent<IgListScript>();
		tokenEnv = igListScript.tokenDict;
		cubeEnv = igListScript.cubeDict;
	}

	public Value evaluate(string input) {
		return interpret(desugar(parse(input)));
	}

	Expression parse(string input) {

	}

	Expression desugar(Expression sugaredExpression) {

	}

	Result interpret(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store,
		Dictionary<string, Token> tokenEnv,
		Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (expression.expressionType) {
			case 1: //e-int
				return interpInt(expression.eInt, env, store, tokenEnv, cubeEnv);
			case 2: //e-float
				return interpFloat(expression.eFloat, env, store, tokenEnv, cubeEnv);
			case 3: //e-string
				return interpString(expression.eString, env, store, tokenEnv, cubeEnv);
			case 4: //e-bool
				return interpBool(expression.eBool, env, store, tokenEnv, cubeEnv);
			case 5: // e-list
				return interpList(expression.eList, env, store, ref tokenEnv, ref cubeEnv);
			case 6: //e-op
				return interpOperator(expression.eOperatorOp, expression.eOperatorLeft, expression.eOperatorRight, env, store, ref tokenEnv, ref cubeEnv);
			case 7: //e-triOp
				return interpTriOperator(expression.eTriOperatorOp, expression.eTriOperatorTarget, expression.eTriOperatorLeft, expression.eTriOperatorRight, env, store, ref tokenEnv, ref cubeEnv);
			case 8: //e-if
				return interpIf(expression.eIfCond, expression.eIfConsq, expression.eIfAlter, env, store, ref tokenEnv, ref cubeEnv);
			case 9: //e-lam
				return interpLam(expression.eLamParam, expression.eLamBody, env, store, ref tokenEnv, ref cubeEnv);
			case 10: //e-app
				return interpApp(expression.eAppFunc, expression.eAppArg, env, store, ref tokenEnv, ref cubeEnv);
			case 11: //e-set
				return interpSet(expression.eSetName, expression.eSetValue, env, store, ref tokenEnv, ref cubeEnv);
			case 12: //e-do
				return interpDo(expression.eDo, env, store, ref tokenEnv, ref cubeEnv);	
			case 13: //e-while
				return interpWhile(expression.eWhileCond, expression.eWhileBody, new Value(), false, env, store, ref tokenEnv, ref cubeEnv);
			case 14: //e-define
				return interpret(expression.eDefineValue, env, store, ref tokenEnv, ref cubeEnv);
			case 15: // e-id
				return interpretId(expression.eId, env, store, ref tokenEnv, ref cubeEnv)
			case 16: //e-ig-variable
				return interpretIgVariable(expression.eIgName, expression.eIgVariable, env, store, ref tokenEnv, ref cubeEnv);
			case 17: //e-set-ig-variable
				return interpretSetIfVariable(expression.eSetIgName, expression.eSetIgVariable, expression.eSetIgValue, env, store, ref tokenEnv, ref cubeEnv);
		}
	}

	// ================================= Interpret Values Functions =================================

	Result interpInt(int eInt, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value(1);
		returnValue.vInt = eInt;
		return new Result(returnValue, store);
	}

	Result interpFloat(float eFloat, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value(2);
		returnValue.vFloat = eFloat;
		return new Result(returnValue, store);
	}

	Result interpString(string eString, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value(3);
		returnValue.vString = eString;
		return new Result(returnValue, store);
	}

	Result interpBool(bool eBool, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value(4);
		returnValue.vBool = eBool;
		return new Result(returnValue, store);
	}

	Result interpList(
		List<Expression> expressionList, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Value returnValue = new Value(5);
		returnValue.vList = new List<Value>();
		Result last_result = new Result(new Value(), store);
		foreach (Expression expression in expressionList) {
			last_result = interpret(expression, env, last_result.store, ref tokenEnv, ref cubeEnv);
			returnValue.vList.Add(last_result.value);
		}
		return new Result(returnValue, last_result.store);
	}

	// ================================= Interpret Operator Functions =================================
	
	Result interpOperator(
		Operator op, 
		Expression left, 
		Expression right, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (op.operatorType) {
			case 1: //Addition
				return interpOperatorHelper(additionFloatValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 2: //Subtraction
				return interpOperatorHelper(subtractionFloatValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 3: //Multiplication
				return interpOperatorHelper(multiplicationFloatValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 4: //Division
				return interpOperatorHelper(divisionFloatValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 5: //Exponent
				return interpOperatorHelper(exponentFloatValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 6: //Modulo
				return interpOperatorHelper(moduloFloatValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 7: //List Concat
				return interpOperatorHelper(listConcatValue, left, right, 5, 5, env, store, ref tokenEnv, ref cubeEnv);
			case 8: // String Concant
				return interpOperatorHelper(stringConcatValue, left, right, 3, 3, env, store, ref tokenEnv, ref cubeEnv);
			case 9: // string equal
				return interpOperatorHelper(stringEqualValue, left, right, 3, 3, env, store, ref tokenEnv, ref cubeEnv);
			case 10: // num equal
				return interpOperatorHelper(floatEqualValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 11: // num greater than
				return interpOperatorHelper(floatGreaterValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 12: // num less than
				return interpOperatorHelper(floatLesserValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 13: // num geq
				return interpOperatorHelper(floatGeqValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 14: // num leq
				return interpOperatorHelper(floatLeqValue, left, right, 2, 2, env, store, ref tokenEnv, ref cubeEnv);
			case 15: // List Index
				return interpOperatorHelper(listIndexValue, left, right, 5, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 16: // String Index
				return interpOperatorHelper(stringIndexValue, left, right, 3, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 19: //Addition
				return interpOperatorHelper(additionIntValue, left, right, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 20: //Subtraction
				return interpOperatorHelper(subtractionIntValue, left, right, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 21: //Multiplication
				return interpOperatorHelper(multiplicationIntValue, left, right, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 22: //Division
				return interpOperatorHelper(divisionIntValue, left, right, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 23: //Exponent
				return interpOperatorHelper(exponentIntValue, left, right, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
			case 24: //Modulo
				return interpOperatorHelper(moduloIntValue, left, right, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
		}
	}

	Result interpOperatorHelper(
		Func<Value, Value, Value> valueFunc, 
		Expression left, 
		Expression right, 
		int leftType, 
		int rightType, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		if (l_result.value.valueType == leftType) {
			Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
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

	// SPECIFIC OPERATOR FUNCTIONS

	Value additionFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(2);
		returnValue.vFloat = leftValue.vFloat + rightValue.vFloat;
		return returnValue;
	}

	Value subtractionFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(2);
		returnValue.vFloat = leftValue.vFloat - rightValue.vFloat;
		return returnValue;
	}

	Value multiplicationFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(2);
		returnValue.vFloat = leftValue.vFloat * rightValue.vFloat;
		return returnValue;
	}

	Value divisionFloatValue(Value leftValue, Value rightValue) {
		if (rightValue.vFloat == 0) {
			Value returnValue = new Value(0);
			returnValue.errorMessage = "Divide by zero error";
			return returnValue;
		} else {
			Value returnValue = new Value(2);
			returnValue.vFloat = leftValue.vFloat / rightValue.vFloat;
			return returnValue;
		}
	}

	Value exponentFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(2);
		returnValue.vFloat = (float)Math.Pow(leftValue.vFloat, rightValue.vFloat);
		return returnValue;
	}

	Value moduloFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(2);
		returnValue.vFloat = leftValue.vFloat % rightValue.vFloat;
		return returnValue;
	}

	Value listConcatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(5);
		returnValue.vList = leftValue.vList.Concat(rightValue.vList).ToList();
		return returnValue;
	}

	Value stringConcatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(3);
		returnValue.vString = leftValue.vString + rightValue.vString;
		return returnValue;
	}

	Value stringEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(4);
		returnValue.vBool = leftValue.vString == rightValue.vString;
		return returnValue;
	}

	Value floatEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(4);
		returnValue.vBool = leftValue.vFloat == rightValue.vFloat;
		return returnValue;
	}

	Value floatGreaterValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(4);
		returnValue.vBool = leftValue.vFloat > rightValue.vFloat;
		return returnValue;
	}

	Value floatLesserValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(4);
		returnValue.vBool = leftValue.vFloat < rightValue.vFloat;
		return returnValue;
	}

	Value floatGeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(4);
		returnValue.vBool = leftValue.vFloat >= rightValue.vFloat;
		return returnValue;
	}

	Value floatLeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(4);
		returnValue.vBool = leftValue.vFloat <= rightValue.vFloat;
		return returnValue;
	}

	Value listIndexValue(Value leftValue, Value rightValue) {
		if (Math.Abs(rightValue.vFloat) < leftValue.vList.Count) {
			return leftValue.vList[rightValue.vInt];
		} else {
			return indexOutOfRangeError();
		}
		
	}

	Value stringIndexValue(Value leftValue, Value rightValue) {
		if (Math.Abs(rightValue.vFloat) < leftValue.vString.Length) {
			Value returnValue = new Value(3);
			returnValue.vString = leftValue.vString[rightValue.vInt].ToString();
			return returnValue;
		} else {
			return indexOutOfRangeError();
		}
	}

	Value indexOutOfRangeError() {
		Value returnValue = new Value(0);
		returnValue.errorMessage = "Index out of range";
		return returnValue;
	}

	Value additionIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(1);
		returnValue.vInt = leftValue.vInt + rightValue.vInt;
		return returnValue;
	}

	Value subtractionIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(1);
		returnValue.vInt = leftValue.vInt - rightValue.vInt;
		return returnValue;
	}

	Value multiplicationIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(1);
		returnValue.vInt = leftValue.vInt * rightValue.vInt;
		return returnValue;
	}

	Value divisionIntValue(Value leftValue, Value rightValue) {
		if (rightValue.vInt == 0) {
			Value returnValue = new Value(0);
			returnValue.errorMessage = "Divide by zero error";
			return returnValue;
		} else {
			Value returnValue = new Value(1);
			returnValue.vInt = leftValue.vInt / rightValue.vInt;
			return returnValue;
		}
	}

	Value exponentIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(1);
		returnValue.vInt = (float)Math.Pow(leftValue.vInt, rightValue.vInt);
		return returnValue;
	}

	Value moduloIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value(1);
		returnValue.vInt = leftValue.vInt % rightValue.vInt;
		return returnValue;
	}

	// ================================= Interpret triOperator Functions =================================

	Result interpTriOperator(
		Operator op, 
		Expression target, 
		Expression left, 
		Expression right, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (op.operatorType) {
			case(17):
				return interpTriOperatorHelper(listSublistValue, target, left, right, 5, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
			case(18):
				return interpTriOperatorHelper(stringSubstringValue, target, left, right, 3, 1, 1, env, store, ref tokenEnv, ref cubeEnv);
		}
	}

	Result interpTriOperatorHelper(
		Func<Value, Value, Value, Value> valueFunc, 
		Expression target, 
		Expression left, 
		Expression right, 
		int targetType, 
		int leftType, 
		int rightType, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		)
	{
		Result t_result = interpret(target, env, store, ref tokenEnv, ref cubeEnv);
		if (t_result.value.valueType == targetType) {
			Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
			if (l_result.value.valueType == leftType) {
				Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
				if (r_result.value.valueType == rightType) {
					Value returnValue = valueFunc(t_result.value, l_result.value, r_result.value);
					return new Result(returnValue, r_result.store);
				} else {
					return new Result(new Value(rightType, r_result.value.valueType), r_result.store); //Throw Error
				}
			} else {
				return new Result(new Value(leftType, l_result.value.valueType), l_result.store); //Throw Error
			}
		} else {
			return new Result(new Value(targetType, t_result.value.valueType), t_result.store); //Throw Error
		}
	}

	// SPECIFIC TRIOPERATOR FUNCTIONS

	Value listSublistValue(Value targetValue, Value leftValue, Value rightValue) {
		if (leftValue.vFloat >= 0 && leftValue.vFloat < rightValue.vFloat && rightValue.vFloat <= targetValue.vList.Count) {
			Value returnValue = new Value(5);
			int idx = leftValue.vInt;
			int num = rightValue.vInt - idx;
			returnValue.vList = targetValue.vList.GetRange(idx, num);
			return returnValue;
		} else {
			return indexOutOfRangeError();
		}
	} 

	Value stringSubstringValue(Value targetValue, Value leftValue, Value rightValue) {
		if (leftValue.vFloat >= 0 && leftValue.vFloat < rightValue.vFloat && rightValue.vFloat <= targetValue.vString.Length) {
			Value returnValue = new Value(3);
			int idx = leftValue.vInt;
			int num = rightValue.vInt - idx;
			returnValue.vString = targetValue.vString.Substring(idx, num);
			return returnValue;
		} else {
			return indexOutOfRangeError();
		}
	}

	Result interpIf(
		Expression cond, 
		Expression consq, 
		Expression alter, 
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result cond_result = interpret(cond, env, store, ref tokenEnv, ref cubeEnv);
		if (cond_result.value.valueType == 4) {
			if (cond_result.value.vBool) {
				return interpret(consq, env, cond_result.store, ref tokenEnv, ref cubeEnv);
			} else {
				return interpret(alter, env, cond_result.store, ref tokenEnv, ref cubeEnv);
			}
		} else {
			return new Result(new Value(4, cond_result.value.valueType), store);
		}
	}

	Result interpLam(
		string param, 
		Expression body, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Value returnValue = new Value();
		returnValue.vFunParam = param;
		returnValue.vFunBody = body;
		returnValue.vFunEnviroment = env;
		return new Result(returnValue, store);
	}

	Result interpApp(
		Expression func, 
		Expression arg, 
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result func_result = interpret(func, env, store, ref tokenEnv, ref cubeEnv);
		if (func_result.value.valueType == 5) {
			Result arg_result = interpret(arg, env, func_result.store, ref tokenEnv, ref cubeEnv);
			string loc = System.Guid.NewGuid().ToString();
			func_result.value.vFunEnviroment.Add(func_result.value.vFunParam, loc);
			arg_result.store.Add(loc, arg_result.value);
			return interpret(func_result.value.vFunBody, func_result.value.vFunEnviroment, arg_result.store, ref tokenEnv, ref cubeEnv); 
		} else {
			return new Result(new Value(5, func_result.value.valueType), store); //Throw Error
		}
	}

	Result interpSet(
		string name, 
		Expression newValue, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		if (env.ContainsKey(name)) {
			string pointer = env[name];
			if (store.ContainsKey(pointer)) {
				Result newValue_result = interpret(newValue, env, store, ref tokenEnv, ref cubeEnv);
				newValue_result.store[pointer] = newValue_result.value;
				return new Result(newValue_result.value, newValue_result.store);
			} else {
				Value errorValue = new Value(0);
				errorValue.errorMessage = "Identifier Pointing To Nothing: " + name;
				return new Result(errorValue, store);
			}
		} else {
			Value errorValue = new Value(0);
			errorValue.errorMessage = "Unbound identifier: " + name;
			return new Result(errorValue, store);
		}
	}

	Result interpDo(
		List<Expression> expressionList, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result last_result = new Result(new Value(), store);
		foreach (Expression expression in expressionList) {
			// If statement in Do handles define statements. Defines are only relevent in Do
			// The change to the env is only relevent across other Do expressions
			// In the same Do statement
			if (expression.expressionType == 12) {
				Result define_result = interpret(expression.eDefineValue, env, last_result.store, ref tokenEnv, ref cubeEnv);
				string loc = System.Guid.NewGuid().ToString();
				env.Add(expression.eDefineName, loc);
				define_result.store.Add(loc, define_result.value);
				last_result = define_result;
			} else {
				last_result = interpret(expression, env, last_result.store, ref tokenEnv, ref cubeEnv);
			}
			
		}
		return last_result;
	}

	Result interpWhile(
		Expression cond, 
		Expression body, 
		Value lastValue, 
		bool useLast, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result cond_result = interpret(cond, env, store, ref tokenEnv, ref cubeEnv);
		if (cond_result.value.valueType == 4) {
			if (cond_result.value.vBool) {
				Result body_result = interpret(body, env, cond_result.store, ref tokenEnv, ref cubeEnv);
				return interpWhile(cond, body, body_result.value, true, env, body_result.store, ref tokenEnv, ref cubeEnv);
			} else if (useLast) {
				return new Result(lastValue, cond_result.store);
			} else {
				return cond_result;
			}
		} else {
			return new Result(new Value(4, cond_result.value.valueType), store); //Throw Error
		}
	}

	Result interpId(
		string name, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		if (env.ContainsKey(name)) {
			if (store.ContainsKey(name)) {
				return new Result(store[env[name]], store);
			} else {
				Value errorValue = new Value(0);
				errorValue.errorMessage = "Identifier Pointing To Nothing: " + name;
				return new Result(errorValue, store); //Throw Error
			}
		} else {
			Value errorValue = new Value(0); 
			errorValue.errorMessage = "Unbound identifier: " + name;
			return new Result(errorValue, store); //Throw Error	
		}
	}

	Result interpIgVariable(
		string name, 
		string variable, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		if (tokenEnv.ContainsKey(name)) {
			Token token = tokenEnv[name];
			if (token.variables.ContainsKey(variable)) {
				switch(token.variables[variable]) {
					case 1:
						Value intValue = new Value(1);
						intValue.vInt = token.intVars[variable];
						return new Result(intValue, store);
					case 2:
						Value floatValue = new Value(2);
						floatValue.vFloat = token.floatVars[variable];
						return new Result(floatValue, store);
					case 3:
						Value stringValue = new Value(3);
						stringValue.vString= token.stringVars[variable];
						return new Result(stringValue, store);
					case 4:
						Value boolValue = new Value(4);
						boolValue.vBool = token.boolVars[variable];
						return new Result(intValue, store);
				}
			} else {
				Value errorValue = new Value(0); 
				errorValue.errorMessage = "Undefined ig Variable: " + variable;
				return new Result(errorValue, store); //Throw Error	
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case 1:
						Value intValue = new Value(1);
						intValue.vInt = cube.intVars[variable];
						return new Result(intValue, store);
					case 2:
						Value floatValue = new Value(2);
						floatValue.vFloat = cube.floatVars[variable];
						return new Result(floatValue, store);
					case 3:
						Value stringValue = new Value(3);
						stringValue.vString= cube.stringVars[variable];
						return new Result(stringValue, store);
					case 4:
						Value boolValue = new Value(4);
						boolValue.vBool = cube.boolVars[variable];
						return new Result(intValue, store);
				}
			} else {
				Value errorValue = new Value(0); 
				errorValue.errorMessage = "Undefined ig Variable: " + variable;
				return new Result(errorValue, store); //Throw Error	
			}
		} else {
			Value errorValue = new Value(0); 
			errorValue.errorMessage = "No ig named: " + name;
			return new Result(errorValue, store); //Throw Error	
		}
	}

	Result interpSetIgVariable(
		string name, 
		string variable, 
		Expression newValue, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		if (tokenEnv.ContainsKey(name)) {
			Token token = tokenEnv[name];
			Result nv_result = interpret(newValue, env, store, tokenEnv, cubeEnv, ref tokenEnv, ref cubeEnv);
			if (token.variables.ContainsKey(variable)) {
				switch(token.variables[variable]) {
					case 1:
						if (nv_result.value.returnType == 1) {
							token.intVars[variable] = nv_result.value.vInt;
							return nv_result;
						} else {
							return new Result(new Value(1, nv_result.value.returnType), nv_result.store);
						}
					case 2:
						if (nv_result.value.returnType == 2) {
							token.intVars[variable] = nv_result.value.vFloat;
							return nv_result;
						} else {
							return new Result(new Value(2, nv_result.value.returnType), nv_result.store);
						}
					case 3:
						if (nv_result.value.returnType == 3) {
							token.intVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return new Result(new Value(3, nv_result.value.returnType), nv_result.store);
						}
					case 4:
						if (nv_result.value.returnType == 4) {
							token.intVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return new Result(new Value(4, nv_result.value.returnType), nv_result.store);
						}
				}
			} else {
				switch(nv_result.value.valueType) {
					case 0:
						return nv_result;
					case 1:
						token.intVars.Add(variable, nv_result.vInt);
						return nv_result;
					case 2:
						token.floatVars.Add(variable, nv_result.vFloat);
						return nv_result;
					case 3:
						token.stringVars.Add(variable, nv_result.vString);
						return nv_result;
					case 4:
						token.boolVars.Add(variable, nv_result.vBool)
						return nv_result;
				}
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			Result nv_result = interpret(newValue, env, store, tokenEnv, cubeEnv, ref tokenEnv, ref cubeEnv);
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case 1:
						if (nv_result.value.returnType == 1) {
							cube.intVars[variable] = nv_result.value.vInt;
							return nv_result;
						} else {
							return new Result(new Value(1, nv_result.value.returnType), nv_result.store);
						}
					case 2:
						if (nv_result.value.returnType == 2) {
							cube.intVars[variable] = nv_result.value.vFloat;
							return nv_result;
						} else {
							return new Result(new Value(2, nv_result.value.returnType), nv_result.store);
						}
					case 3:
						if (nv_result.value.returnType == 3) {
							cube.intVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return new Result(new Value(3, nv_result.value.returnType), nv_result.store);
						}
					case 4:
						if (nv_result.value.returnType == 4) {
							cube.intVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return new Result(new Value(4, nv_result.value.returnType), nv_result.store);
						}
				}
			} else {
				switch(nv_result.value.valueType) {
					case 0:
						return nv_result;
					case 1:
						cube.intVars.Add(variable, nv_result.vInt);
						return nv_result;
					case 2:
						cube.floatVars.Add(variable, nv_result.vFloat);
						return nv_result;
					case 3:
						cube.stringVars.Add(variable, nv_result.vString);
						return nv_result;
					case 4:
						cube.boolVars.Add(variable, nv_result.vBool);
						return nv_result;
				}
			}
		} else {
			Value errorValue = new Value(0); 
			errorValue.errorMessage = "No ig named: " + name;
			return new Result(errorValue, store); //Throw Error	
		}
	}
}

// ====================================================================================================================
// ===================================================== Data Types ===================================================
// ====================================================================================================================

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

	public Value(int vType) {
		valueType = vType;
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

	public Operator eTriOperatorOp; // type=7
	public Expression eTriOperatorTarget;
	public Expression eTriOperatorLeft;
	public Expression eTriOperatorRight;

	public Expression eIfCond; // type=8
	public Expression eIfConsq;
	public Expression eIfAlter;

	public string eLamParam; // type=9
	public Expression eLamBody;

	public Expression eAppFunc; // type=10
	public Expression eAppArg;

	public string eSetName; // type=11
	public Expression eSetValue;

	public List<Expression> eDo; //type=12

	public Expression eWhileCond; //type=13
	public Expression eWhileBody; 

	public string eDefineName; //type=14
	public Expression eDefineValue;

	public string eId; //type=15

	public string eIgName; //type=16
	public string eIgVariable;
	public int eIgVariableType; //1 int, 2 float, 3 string, 4 bool

	public string eSetIgName; //type=17
	public string eSetIgVariable; 
	public int eIgVariableType;
	public Expression eSetIgValue;


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

	// op-float-plus, type=1
	// op-float-minus, type=2
	// op-float-multiply, type=3
	// op-float-divide, type=4
	// op-float-exponent, type=5
	// op-float-modulus, type=6

	// op-list-append, type=7
	// op-str-append, type=8

	// op-str-eq, type=9
	// op-float-eq, type=10
	// op-float-g, type=11
	// op-float-l, type=12
	// op-float-geq, type=13
	// op-float-leq, type=14

	// op-list-index type=15
	// op-string-index type=16	

	// op-list-sublist type=17
	// op-string-substring type=18

	// op-int-plus, type=19
	// op-int-minus, type=20
	// op-int-multiply, type=21
	// op-int-divide, type=22
	// op-int-exponent, type=23
	// op-int-modulus, type=24
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
