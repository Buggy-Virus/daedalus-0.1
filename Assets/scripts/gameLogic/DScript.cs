using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DScript {

	IgListScript igListScript;
	public Dictionary<string, Token> tokenEnv;
    public Dictionary<string, Cube> cubeEnv;

    public Dictionary<string, string> builtInEnv;
    public Dictionary<string, Value> builtInStore; 

	void Start(){
		igListScript = GameObject.Find("GameLogic").GetComponent<IgListScript>();
		tokenEnv = igListScript.tokenDict;
		cubeEnv = igListScript.cubeDict;
	}

	public Value evaluate(string input) {
		return interpret(desugar(parse(input)));
	}

	public Value evaluateSelfToken(string input, Token self) {
		return interpret(desugar(parse(input)));
	}

	public Value evaluateSelfCube(string input, Cube self) {
		return interpret(desugar(parse(input)));
	}

	public Value evaluateSelfTokenTargetToken(string input, Token self, Token target) {
		return interpret(desugar(parse(input)));
	}

	public Value evaluateSelfTokenTargetCube(string input, Token self, Cube cube) {
		return interpret(desugar(parse(input)));
	}

	// ================================= Parse Functions ================================= 

	bool atomEquals(Atom atom, string targetType, string targetValue) {
		return atom.atomType == targetType && atom.value == targetValue;
	}

	Expression parse(string input) {
		List<Atom> atomList = tokenize(input);

		return parseDo(atomList, 0).expression;
		 
	}

	ParseResult parseHelper(List<Atom> atomList, int pos) {
		while (pos < atomList.Count) {
			Atom curAtom = atomList[pos];

			switch(curAtom.atomType) {
				case "string":
					new Expression expr = new Expression("e-string");
					expr.eString = curAtom.value; 
				case "number":
					if (curAtom.Contains('.')) {
						new Expression expr = new Expression("e-float");
						expr.eFloat = (float)curAtom.value;
					} else {
						new Expression expr = new Expression("e-int");
						expr.eInt = (int)curAtom.value;
					}
				case "bool":
					new Expression expr = new Expression("e-bool");
					expr.eBool = (bool)curAtom.value;
				case "identifier":
					new Expression expr = new Expression("e-id");
					expr.eId = curAtom.value;
				case "keyword":
					switch(curAtom.value) {
						case "if":
							parseIf(atomList, pos + 1)
						case "lambda":
							parseLambda(atomList, pos + 1)
						case "let":
							parseLet(atomList, pos + 1)
						case "while":
							parseWhile(atomList, pos + 1)
						default:
							// does something
					}
				case "punctuation":
					switch(curAtom.value) {
						case '(':
							parseHelper(atomList, pos + 1);
						case '{':
						case ';':
						case '[':
						default:
							// does something
					}
				case "operator":
					switch(curAtom.Value) {
						case '+':
						case '=':
						case '-':
						case '/':
						case '*':
						case '>':
						case '<':
						case '!':
					}
				default:
					// does something
			}
		}
	}

	ParseResult parseDo(List<Atom> atomList, int pos) {
		List<Expression> expressionList = new List<Expression>();
		Expression lastExpression;

		if (atomEquals(atomList[pos], "punctuation", "{")) {

		} else {
			// parse Result error
		}
	}

	ParseResult parseIf(List<Atom> atomList, int pos) {
		Expression ifExpression = new Expression("e-if");

		if (atomEquals(atomList[pos], "punctuation", "(")) {
			pos += 1;
			if (atomEquals(atomList[pos], "punctuation", "{")) {
				ParseResult consqResult = parseHelper(atomList, pos);
				ifExpression.eIfConsq = consqResult.expression;
				pos = consqResult.position;

				if (atomEquals(atomList[pos], "punctuation", "else")) {
					pos += 1;
					if (atomEquals(atomList[pos], "punctuation", "{")) {
						ParseResult alterResult = parseHelper(atomList, pos);
						ifExpression.eIfAlter = alterResult.expression;
						pos = alterResult.position;

						return new ParseResult(ifExpression, pos);
					} else {
						// throw error
					}
				} else {
					// throw error
				}
			} else {
				// throw error
			}
			ParseResult condResult = parseHelper(atomList, pos);
			ifExpression.eIfCond = condResult.expression;
			pos = condResult.position;
		} else {
			// throw error
		}
	}

	ParseResult parseLambda(List<Atom> atomList, int pos) {
		Expression lamExpression = new Expression("e-lam");

		if (atomEquals(atomList[pos], "punctuation", "(")) {
			pos += 1;
			if (atomList[pos].atomType == "identifier") {
				lamExpression.eLamParam = atomList[pos].value;
				pos += 1;
				if (atomEquals(atomList[pos], "punctuation", ")")) {
					pos += 1;
					if (atomEquals(atomList[pos], "punctuation", "{")) {
						ParseResult bodyResult = parseHelper(atomList, pos);
						lamExpression.eLamBody = bodyResult.expression;
						pos = bodyResult.position;

						return new ParseResult(lamExpression, pos);
					}				
				} else {
					// throw error
				}
			} else {
				// throw error
			}
		} else {
			// throw error
		}
	}

	ParseResult parseResult(List<Atom> atomList, int pos) {
		Expression letExpression = new Expression("e-let");

		if (atomList[pos].atomType == "identifier") {
			letExpression.eLetName = atomList[pos].value;
			pos += 1;
			if (atomEquals(atomList[pos], "operator", "=")) {
				pos += 1;
				ParseResult letValueResult = parseHelper(atomList, pos);
				letExpression.eLetValue = letValueResult.expression;
				
				return new ParseResult(letExpression, pos);
			} else {
				// throw error
			}
		} else {
			// throw error
		}
	}

	// ================================= Tokenizer Functions ================================= 

	List<Atom> tokenize(string input) {
		List<Atom> atomList = new List<Atom>();

		bool buildingAtom = false;
		bool singleQuote = false;
		Atom curAtom = new Atom();

		for (int i = 0; i < input.Length; i++) {
			char curChar = input[i];

			if (buildingAtom) {
				switch(curAtom.atomType) {
					case "string":
						if ((curChar == '\'' && singleQuote == true) || (curChar == '\"' && singleQuote == false)) {
							atomList.Add(curAtom);
							buildingAtom = false;

							// this is wrong, shouldn't consider the quotation character if we just finished processing the string
						} else {
							curAtom.value += curChar.ToString();
						}
						break;
					case "number":
						if (Char.IsDigit(curChar) || curChar == '.') {
							curAtom.value += curChar.ToString();
						} else {
							atomList.Add(curAtom);
							buildingAtom = false;
						}
						break;
					case "identifier":
						if (Char.IsDigit(curChar) || Char.IsLetter(curChar)) {
							curAtom.value += curChar.ToString();
						} else {
							switch(curAtom.value) {
								case "if":
								case "else":
								case "lambda":
								case "let":
								case "while":
								case "def":
									curAtom.atomType = "keyword";
									break;
								case "true":
								case "false":
									curAtom.atomType = "bool";
									break;
							}
							atomList.Add(curAtom);
							buildingAtom = false;
						}
						break;
				}
			}

			if (!buildingAtom) {
				if (isWhiteSpace(curChar)) {
					
				} else if (curChar == '\'') {
					curAtom = new Atom();
					curAtom.atomType = "string";
					buildingAtom = true;
					singleQuote = true;
				} else if (curChar == '\"') {
					curAtom = new Atom();
					curAtom.atomType = "string";
					buildingAtom = true;
					singleQuote = false;
				} else if (isPunctuation(curChar)) {
					curAtom = new Atom();
					curAtom.atomType = "punc";
					curAtom.value = curChar.ToString();
					atomList.Add(curAtom);
				} else if (isOperator(curChar)) {
					curAtom = new Atom();
					curAtom.atomType = "operator";
					curAtom.value = curChar.ToString();
					atomList.Add(curAtom);
				} else if (Char.IsDigit(curChar)) {
					curAtom = new Atom();
					curAtom.atomType = "number";
					curAtom.value = curChar.ToString();
					buildingAtom = true;
				} else if (Char.IsLetter(curChar)) {
					curAtom = new Atom();
					curAtom.atomType = "identifier";
					curAtom.value = curChar.ToString();
					buildingAtom = true;
				} else {
					curAtom = new Atom();
					curAtom.atomType = "error";
					atomList.Add(curAtom);
					break;
				} 
			}	
		}
		return atomList;
	}

	bool isPunctuation (char c) {
		switch(c) {
			case '(':
			case ')':
			case '{':
			case '}':
			case ';':
			case ',':
			case ']':
			case '[':
			case ':':
				return true;
			default:
				return false;
		}
	}

	bool isWhiteSpace (char c) {
		switch(c) {
			case '\t':
			case '\n':
				return true;
			default:
				return false;
		}
	}

	bool isOperator (char c) {
		switch(c) {
			case '+':
			case '=':
			case '-':
			case '/':
			case '*':
			case '>':
			case '<':
			case '!':
				return true;
			default:
				return false;
		}
	}

	bool isString (char c) {
		switch(c) {
			case '\"':
			case '\'':
				return true;
			default:
				return false;
		}
	}

	// ================================= Interpet Functions ================================= 

	Expression desugar(Expression sugaredExpression) {

	}

	Result interpret(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store,
		ref Dictionary<string, Token> tokenEnv,
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (expression.expressionType) {
			case "e-int": //e-int
				return interpInt(expression.eInt, env, store);
			case "e-float": //e-float
				return interpFloat(expression.eFloat, env, store);
			case "e-string": //e-string
				return interpString(expression.eString, env, store);
			case "e-bool": //e-bool
				return interpBool(expression.eBool, env, store);
			case "e-list": // e-list
				return interpList(expression.eList, env, store, ref tokenEnv, ref cubeEnv);
			case "e-op": //e-op
				return interpOperator(expression.eOperatorOp, expression.eOperatorLeft, expression.eOperatorRight, env, store, ref tokenEnv, ref cubeEnv);
			case "e-triOp": //e-triOp
				return interpTriOperator(expression.eTriOperatorOp, expression.eTriOperatorTarget, expression.eTriOperatorLeft, expression.eTriOperatorRight, env, store, ref tokenEnv, ref cubeEnv);
			case "e-if": //e-if
				return interpIf(expression.eIfCond, expression.eIfConsq, expression.eIfAlter, env, store, ref tokenEnv, ref cubeEnv);
			case "e-lam": //e-lam
				return interpLam(expression.eLamParam, expression.eLamBody, env, store, ref tokenEnv, ref cubeEnv);
			case "e-app": //e-app
				return interpApp(expression.eAppFunc, expression.eAppArg, env, store, ref tokenEnv, ref cubeEnv);
			case "e-set": //e-set
				return interpSet(expression.eSetName, expression.eSetValue, env, store, ref tokenEnv, ref cubeEnv);
			case "e-do": //e-do
				return interpDo(expression.eDo, env, store, ref tokenEnv, ref cubeEnv);	
			case "e-while": //e-while
				return interpWhile(expression.eWhileCond, expression.eWhileBody, new Value(), false, env, store, ref tokenEnv, ref cubeEnv);
			case "e-let": //e-let
				return interpret(expression.eLetValue, env, store, ref tokenEnv, ref cubeEnv);
			case "e-id": // e-id
				return interpId(expression.eId, env, store, ref tokenEnv, ref cubeEnv);
			case "e-ig-var": //e-ig-variable
				return interpIgVariable(expression.eIgName, expression.eIgVariable, env, store, ref tokenEnv, ref cubeEnv);
			case "e-set-ig-var": //e-set-ig-variable
				return interpSetIgVariable(expression.eSetIgName, expression.eSetIgVariable, expression.eSetIgValue, env, store, ref tokenEnv, ref cubeEnv);
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Unknown expression type: " + expression.expressionType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	// ================================= Interpret Values Functions =================================

	Result interpInt(int eInt, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("int");
		returnValue.vInt = eInt;
		return new Result(returnValue, store);
	}

	Result interpFloat(float eFloat, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("float");
		returnValue.vFloat = eFloat;
		return new Result(returnValue, store);
	}

	Result interpString(string eString, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("string");
		returnValue.vString = eString;
		return new Result(returnValue, store);
	}

	Result interpBool(bool eBool, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("bool");
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
		Value returnValue = new Value("list");
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
			case "float-add": //Addition
				return interpOperatorHelper(additionFloatValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-sub": //Subtraction
				return interpOperatorHelper(subtractionFloatValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-mult": //Multiplication
				return interpOperatorHelper(multiplicationFloatValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-div": //Division
				return interpOperatorHelper(divisionFloatValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-exp": //Exponent
				return interpOperatorHelper(exponentFloatValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-mod": //Modulo
				return interpOperatorHelper(moduloFloatValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "list-concat": //List Concat
				return interpOperatorHelper(listConcatValue, left, right, "e-list", "e-list", env, store, ref tokenEnv, ref cubeEnv);
			case "string-concat": // String Concant
				return interpOperatorHelper(stringConcatValue, left, right, "e-string", "e-string", env, store, ref tokenEnv, ref cubeEnv);
			case "string-eql": // string equal
				return interpOperatorHelper(stringEqualValue, left, right, "e-string", "e-string", env, store, ref tokenEnv, ref cubeEnv);
			case "float-eql": // num equal
				return interpOperatorHelper(floatEqualValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-gt": // num greater than
				return interpOperatorHelper(floatGreaterValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-lt": // num less than
				return interpOperatorHelper(floatLesserValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-geq": // num geq
				return interpOperatorHelper(floatGeqValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "float-leq": // num leq
				return interpOperatorHelper(floatLeqValue, left, right, "e-float", "e-float", env, store, ref tokenEnv, ref cubeEnv);
			case "list-ind": // List Index
				return interpOperatorHelper(listIndexValue, left, right, "v-list", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "string-ind": // String Index
				return interpOperatorHelper(stringIndexValue, left, right, "e-string", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-add": //Addition
				return interpOperatorHelper(additionIntValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-sub": //Subtraction
				return interpOperatorHelper(subtractionIntValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-mult": //Multiplication
				return interpOperatorHelper(multiplicationIntValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-div": //Division
				return interpOperatorHelper(divisionIntValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-exp": //Exponent
				return interpOperatorHelper(exponentIntValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-mod": //Modulo
				return interpOperatorHelper(moduloIntValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-eql":
				return interpOperatorHelper(intEqualValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-gt":
				return interpOperatorHelper(intLesserValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-lt":
				return interpOperatorHelper(intGreaterValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-geq":
				return interpOperatorHelper(intGeqValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "int-leq":
				return interpOperatorHelper(intLeqValue, left, right, "e-int", "e-int", env, store, ref tokenEnv, ref cubeEnv);
			case "bool-not":
				return interpOperatorHelper(boolNot, left, right, "e-bool", "e-bool", env, store, ref tokenEnv, ref cubeEnv);
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Unknown operator type: " + op.operatorType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	Result interpOperatorHelper(
		Func<Value, Value, Value> valueFunc, 
		Expression left, 
		Expression right, 
		string leftType, 
		string rightType, 
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
		Value returnValue = new Value("float");
		returnValue.vFloat = leftValue.vFloat + rightValue.vFloat;
		return returnValue;
	}

	Value subtractionFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("float");
		returnValue.vFloat = leftValue.vFloat - rightValue.vFloat;
		return returnValue;
	}

	Value multiplicationFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("float");
		returnValue.vFloat = leftValue.vFloat * rightValue.vFloat;
		return returnValue;
	}

	Value divisionFloatValue(Value leftValue, Value rightValue) {
		if (rightValue.vFloat == 0) {
			Value returnValue = new Value("error");
			returnValue.errorMessage = "Divide by zero error";
			return returnValue;
		} else {
			Value returnValue = new Value("float");
			returnValue.vFloat = leftValue.vFloat / rightValue.vFloat;
			return returnValue;
		}
	}

	Value exponentFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("float");
		returnValue.vFloat = (float)Math.Pow(leftValue.vFloat, rightValue.vFloat);
		return returnValue;
	}

	Value moduloFloatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("float");
		returnValue.vFloat = leftValue.vFloat % rightValue.vFloat;
		return returnValue;
	}

	Value listConcatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("list");
		returnValue.vList = leftValue.vList.Concat(rightValue.vList).ToList();
		return returnValue;
	}

	Value stringConcatValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("string");
		returnValue.vString = leftValue.vString + rightValue.vString;
		return returnValue;
	}

	Value stringEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vString == rightValue.vString;
		return returnValue;
	}

	Value floatEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vFloat == rightValue.vFloat;
		return returnValue;
	}

	Value floatGreaterValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vFloat > rightValue.vFloat;
		return returnValue;
	}

	Value floatLesserValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vFloat < rightValue.vFloat;
		return returnValue;
	}

	Value floatGeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vFloat >= rightValue.vFloat;
		return returnValue;
	}

	Value floatLeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
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
			Value returnValue = new Value("string");
			returnValue.vString = leftValue.vString[rightValue.vInt].ToString();
			return returnValue;
		} else {
			return indexOutOfRangeError();
		}
	}

	Value indexOutOfRangeError() {
		Value returnValue = new Value("error");
		returnValue.errorMessage = "Index out of range";
		return returnValue;
	}

	Value additionIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vInt = leftValue.vInt + rightValue.vInt;
		return returnValue;
	}

	Value subtractionIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vInt = leftValue.vInt - rightValue.vInt;
		return returnValue;
	}

	Value multiplicationIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vInt = leftValue.vInt * rightValue.vInt;
		return returnValue;
	}

	Value divisionIntValue(Value leftValue, Value rightValue) {
		if (rightValue.vInt == 0) {
			Value returnValue = new Value("error");
			returnValue.errorMessage = "Divide by zero error";
			return returnValue;
		} else {
			Value returnValue = new Value("int");
			returnValue.vInt = leftValue.vInt / rightValue.vInt;
			return returnValue;
		}
	}

	Value exponentIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vInt = (int)Math.Pow(leftValue.vInt, rightValue.vInt);
		return returnValue;
	}

	Value moduloIntValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vInt = leftValue.vInt % rightValue.vInt;
		return returnValue;
	}

	Value intEqualValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vBool = leftValue.vInt == rightValue.vInt;
		return returnValue;
	}

	Value intGreaterValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vBool = leftValue.vInt > rightValue.vInt;
		return returnValue;
	}

	Value intLesserValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vBool = leftValue.vInt < rightValue.vInt;
		return returnValue;
	}

	Value intGeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vBool = leftValue.vInt >= rightValue.vInt;
		return returnValue;
	}

	Value intLeqValue(Value leftValue, Value rightValue) {
		Value returnValue = new Value("int");
		returnValue.vBool = leftValue.vInt <= rightValue.vInt;
		return returnValue;
	}

	Value boolNot(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = !leftValue.vBool;
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
			case("list-sublist"):
				return interpTriOperatorHelper(listSublistValue, target, left, right, "list", "int", "int", env, store, ref tokenEnv, ref cubeEnv);
			case("string-substring"):
				return interpTriOperatorHelper(stringSubstringValue, target, left, right, "string", "int", "int", env, store, ref tokenEnv, ref cubeEnv);
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Unknown operator type: " + op.operatorType.ToString();
				return new Result(errorValue, store); //Throw Error	

		}
	}

	Result interpTriOperatorHelper(
		Func<Value, Value, Value, Value> valueFunc, 
		Expression target, 
		Expression left, 
		Expression right, 
		string targetType, 
		string leftType, 
		string rightType, 
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
			Value returnValue = new Value("list");
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
			Value returnValue = new Value("string");
			int idx = leftValue.vInt;
			int num = rightValue.vInt - idx;
			returnValue.vString = targetValue.vString.Substring(idx, num);
			return returnValue;
		} else {
			return indexOutOfRangeError();
		}
	}

	// ================================= End of Operators Functions =================================
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
		if (cond_result.value.valueType == "bool") {
			if (cond_result.value.vBool) {
				return interpret(consq, env, cond_result.store, ref tokenEnv, ref cubeEnv);
			} else {
				return interpret(alter, env, cond_result.store, ref tokenEnv, ref cubeEnv);
			}
		} else {
			return new Result(new Value("bool", cond_result.value.valueType), store);
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
		if (func_result.value.valueType == "function") {
			Result arg_result = interpret(arg, env, func_result.store, ref tokenEnv, ref cubeEnv);
			string loc = System.Guid.NewGuid().ToString();
			func_result.value.vFunEnviroment.Add(func_result.value.vFunParam, loc);
			arg_result.store.Add(loc, arg_result.value);
			return interpret(func_result.value.vFunBody, func_result.value.vFunEnviroment, arg_result.store, ref tokenEnv, ref cubeEnv); 
		} else {
			return new Result(new Value("function", func_result.value.valueType), store); //Throw Error
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
				Value errorValue = new Value("error");
				errorValue.errorMessage = "Identifier Pointing To Nothing: " + name;
				return new Result(errorValue, store);
			}
		} else {
			Value errorValue = new Value("error");
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
			if (expression.expressionType == "e-let") {
				Result define_result = interpret(expression.eLetValue, env, last_result.store, ref tokenEnv, ref cubeEnv);
				string loc = System.Guid.NewGuid().ToString();
				env.Add(expression.eLetName, loc);
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
		if (cond_result.value.valueType == "list") {
			if (cond_result.value.vBool) {
				Result body_result = interpret(body, env, cond_result.store, ref tokenEnv, ref cubeEnv);
				return interpWhile(cond, body, body_result.value, true, env, body_result.store, ref tokenEnv, ref cubeEnv);
			} else if (useLast) {
				return new Result(lastValue, cond_result.store);
			} else {
				return cond_result;
			}
		} else {
			return new Result(new Value("list", cond_result.value.valueType), store); //Throw Error
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
				Value errorValue = new Value("error");
				errorValue.errorMessage = "Identifier Pointing To Nothing: " + name;
				return new Result(errorValue, store); //Throw Error
			}
		} else {
			Value errorValue = new Value("error"); 
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
					case "int":
						Value intValue = new Value("int");
						intValue.vInt = token.intVars[variable];
						return new Result(intValue, store);
					case "float":
						Value floatValue = new Value("float");
						floatValue.vFloat = token.floatVars[variable];
						return new Result(floatValue, store);
					case "string":
						Value stringValue = new Value("string");
						stringValue.vString= token.stringVars[variable];
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool");
						boolValue.vBool = token.boolVars[variable];
						return new Result(boolValue, store);
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Unknown variable type: " + token.variables[variable].ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			} else {
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Undefined ig Variable: " + variable;
				return new Result(errorValue, store); //Throw Error	
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case "int":
						Value intValue = new Value("int");
						intValue.vInt = cube.intVars[variable];
						return new Result(intValue, store);
					case "float":
						Value floatValue = new Value("float");
						floatValue.vFloat = cube.floatVars[variable];
						return new Result(floatValue, store);
					case "string":
						Value stringValue = new Value("string");
						stringValue.vString= cube.stringVars[variable];
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool");
						boolValue.vBool = cube.boolVars[variable];
						return new Result(boolValue, store);
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Unknown variable type: " + cube.variables[variable].ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			} else {
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Undefined ig Variable: " + variable;
				return new Result(errorValue, store); //Throw Error	
			}
		} else {
			Value errorValue = new Value("error"); 
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
			Result nv_result = interpret(newValue, env, store, ref tokenEnv, ref cubeEnv);
			if (token.variables.ContainsKey(variable)) {
				switch(token.variables[variable]) {
					case "int":
						if (nv_result.value.valueType == "int") {
							token.intVars[variable] = nv_result.value.vInt;
							return nv_result;
						} else {
							return new Result(new Value("int", nv_result.value.valueType), nv_result.store);
						}
					case "float":
						if (nv_result.value.valueType == "float") {
							token.floatVars[variable] = nv_result.value.vFloat;
							return nv_result;
						} else {
							return new Result(new Value("float", nv_result.value.valueType), nv_result.store);
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							token.stringVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return new Result(new Value("string", nv_result.value.valueType), nv_result.store);
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							token.boolVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return new Result(new Value("bool", nv_result.value.valueType), nv_result.store);
						}
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Unknown variable type: " + token.variables[variable].ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			} else {
				switch(nv_result.value.valueType) {
					case "error":
						return nv_result;
					case "int":
						token.intVars.Add(variable, nv_result.value.vInt);
						return nv_result;
					case "float":
						token.floatVars.Add(variable, nv_result.value.vFloat);
						return nv_result;
					case "string":
						token.stringVars.Add(variable, nv_result.value.vString);
						return nv_result;
					case "bool":
						token.boolVars.Add(variable, nv_result.value.vBool);
						return nv_result;
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Unknown value type: " + nv_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			Result nv_result = interpret(newValue, env, store, ref tokenEnv, ref cubeEnv);
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case "int":
						if (nv_result.value.valueType == "int") {
							cube.intVars[variable] = nv_result.value.vInt;
							return nv_result;
						} else {
							return new Result(new Value("int", nv_result.value.valueType), nv_result.store);
						}
					case "float":
						if (nv_result.value.valueType == "float") {
							cube.floatVars[variable] = nv_result.value.vFloat;
							return nv_result;
						} else {
							return new Result(new Value("float", nv_result.value.valueType), nv_result.store);
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							cube.stringVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return new Result(new Value("string", nv_result.value.valueType), nv_result.store);
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							cube.boolVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return new Result(new Value("bool", nv_result.value.valueType), nv_result.store);
						}
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Unknown variable type: " + cube.variables[variable].ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			} else {
				switch(nv_result.value.valueType) {
					case "error":
						return nv_result;
					case "int":
						cube.intVars.Add(variable, nv_result.value.vInt);
						return nv_result;
					case "float":
						cube.floatVars.Add(variable, nv_result.value.vFloat);
						return nv_result;
					case "string":
						cube.stringVars.Add(variable, nv_result.value.vString);
						return nv_result;
					case "bool":
						cube.boolVars.Add(variable, nv_result.value.vBool);
						return nv_result;
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Unknown value type: " + nv_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			}
		} else {
			Value errorValue = new Value("error"); 
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

	public string valueType; 

	public string errorExpected; // type=0
	public string errorGot;
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

	public Value(string expected, string got) {
		valueType = "Error";
		errorExpected = expected;
		errorGot = got;
	}

	public Value(string vType) {
		valueType = vType;
	}
}

public class Expression {
	public string expressionType;

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

	public string eLetName; //type=14
	public Expression eLetValue;

	public string eId; //type=15

	public string eIgName; //type=16
	public string eIgVariable;

	public string eSetIgName; //type=17
	public string eSetIgVariable;
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

	public Expression(string etype) {
		expressionType = etype;
	}
}

public class Operator {
	public string operatorType;
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
	// casting between types
}

public class Atom {
	public string atomType;
	public string value;
}

public class ParseResult {
	public int position;
	public Expression expression;

	public ParseResult(Expression e, int p) {
		position = p;
		expression = e;
	}
}
