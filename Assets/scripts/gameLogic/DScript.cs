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

		return parseDo(atomList, 0, false).expression;
		 
	}

	ParseResult parseDo(List<Atom> atomList, int pos, bool bookended) {
		Expression doExpression = new Expression("e-do");
		doExpression.eDo = new List<Expression>();

		if (bookended && atomEquals(atomList[pos], "punctuation", "{")) {
			pos += 1;
		} else if (bookended) {
			// throw error
		}

		while (
			pos < atomList.Count 
			&& !(bookended && atomEquals(atomList[pos], "punctuation", "}"))
			) 
		{
			ParseResult singleResult = parseSingle(atomList, pos, false);
			doExpression.eDo.Add(singleResult.expression);
			pos = singleResult.position;
		}

		return new ParseResult(doExpression, pos);
	}

	ParseResult parseSingle(List<Atom> atomList, int pos, bool bookended) {
		Expression lastExpression = new Expression("Error");
		bool seenExpression = false;

		if (bookended && atomEquals(atomList[pos], "punctuation", "(")) {
			pos += 1;
		} else if (bookended) {
			// throw error
		}

		while (
			pos < atomList.Count 
			&& !(bookended && atomEquals(atomList[pos], "punctuation", ")")) 
			&& !(!bookended && atomEquals(atomList[pos], "punctuation", ";"))
			) 
		{
			if (seenExpression) {
				ParseResult firstResult = parseSingleFirst(atomList, pos);
				lastExpression = firstResult.expression;
				pos = firstResult.position;
			} else {
				ParseResult secondResult = parseSingleSecond(atomList, pos, lastExpression);
				lastExpression = secondResult.epression;
				pos = firstResult.position;
			}
			pos += 1;
		}

		return new ParseResult(lastExpression, pos);
	}

	ParseResult parseSingleFirst(List<Atom> atomList, int pos) {
		Atom curAtom = atomList[pos];
		Expression returnExpression = new Expression("error");

		switch(curAtom.atomType) {
			case "string":
				Expression stringExpression = new Expression("e-string");
				stringExpression.eString = curAtom.value; 
				returnExpression = stringExpression;
				break;
			case "number":
				if (curAtom.value.Contains('.')) {
					Expression floatExpression = new Expression("e-float");
					floatExpression.eFloat = float.Parse(curAtom.value);
					returnExpression = floatExpression;
				} else {
					Expression intExpression = new Expression("e-int");
					intExpression.eInt = Int32.Parse(curAtom.value);
					returnExpression = intExpression;
				}
				break;
			case "bool":
				Expression boolExpression = new Expression("e-bool");
				boolExpression.eBool = bool.Parse(curAtom.value);
				returnExpression = boolExpression;
				break;
			case "identifier":
				ParseResult identifierResult = parseIdentifier(atomList, pos);
				returnExpression = identifierResult.expression;
				pos = identifierResult.position;
				break;
			case "keyword":
				switch(curAtom.value) {
					case "if":
						ParseResult ifResult = parseIf(atomList, pos + 1);
						returnExpression = ifResult.expression;
						pos = ifResult.position;
						break;
					case "lambda":
						ParseResult lamResult = parseLambda(atomList, pos + 1);
						returnExpression = lamResult.expression;
						pos = lamResult.position;
						break;
					case "var":
					case "let":
						ParseResult letResult = parseLet(atomList, pos + 1);
						returnExpression = letResult.expression;
						pos = letResult.position;
						break;
					case "while":
						ParseResult whileResult = parseWhile(atomList, pos + 1);
						returnExpression = whileResult.expression;
						pos = whileResult.position;
						break;
					case "function":
						ParseResult funcResult = parseFunction(atomList, pos + 1);
						returnExpression = funcResult.expression;
						pos = funcResult.position;
					default:
						// throw an error
						break;
				}
				break;
			case "operator":
				if (curAtom.value == "!") {
					ParseResult notResult = parseNot(atomList, pos + 1);
					returnExpression = notResult.expression;
					pos = notResult.position;
				} else {
					// thrower an error
				}
				break;
			case "punctuation":
				switch(curAtom.value) {
					case "(":
						ParseResult singleResult = parseSingle(atomList, pos, true);
						returnExpression = singleResult.expression;
						pos = singleResult.position;
						break;
					case "[":
						ParseResult listResult = parseList(atomList, pos + 1);
						returnExpression = listResult.expression;
						pos = listResult.position;
						break;
					case "$":
						ParseResult igResult = parseIg(atomList, pos + 1);
						returnExpression = igResult.expression;
						pos = igResult.position;
						break;
					default:
						// throw error
						break;
				}
				break;
			default:
				// throw an error
				break;
		}
		return new ParseResult(returnExpression, pos);
	}

	ParseResult parseSingleSecond(List<Atom> atomList, int pos, Expression lastExpression) {
		Atom curAtom = atomList[pos];

		switch(curAtom.atomType) {
			case "operator":
				Expression opExression = new Expression("e-op");
				opExpression.eOperatorLeft = lastExpression;
				switch(curAtom.value) {
					case "+":
						opExpression.eOperatorOp = "+";
						break;
					case "**":
						opExpression.eOperatorOp = "**";
						break;
					case "*":
						opExpression.eOperatorOp = "*";
						break;
					case "/":
						opExpression.eOperatorOp = "/";
						break;
					case "==":
						opExpression.eOperatorOp = "==";
						break;
					case ">=":
						opExpression.eOperatorOp = ">=";
						break;
					case "<=":
						opExpression.eOperatorOp = "<=";
						break;
					case "<":
						opExpression.eOperatorOp = "<";
						break;
					case ">":
						opExpression.eOperatorOp = ">";
						break;
					default:
						// throw error
						break;
				}
				pos += 1;

				ParseResult thirdResult = paseSingle(atomList, pos, false);
				opExpression.oPeratorRight = thirdResult.expression;
				pos = thirdResult.position;
				
				return new ParseResult(opExpression, pos);
			case "punctuation":
				switch(curAtom.value) {
					case "[":
					default:
						return parseIndex(atomList, pos + 1, lastExpression, lastExpression);
				}
				break;
			default:
				// throw error
				break;
		}
	}

	ParseResult parseIndex(List<Atom> atomList, int pos, Expression lastExpression) {
		ParseResult firstResult = parseSingle(atomList, pos);
		pos = firstResult.position + 1;

		if (atomEquals(atomList[pos], "punctuation", ":")) {
			pos += 1;

			Expression subExpression = new Expression("e-triOp"); 
			subExpression.eTriOperatorOp = "[:]";
			subExpression.eTriOperatorTarget = lastExpression;
			subExpression.eTriOperatorLeft = firstResult.expression;

			ParseResult secondResult = parseSingle(atomList, pos);
			subExpression.eTriOperatorRight = secondResult.expression;
			pos = secondResult.position + 1;

			if (atomEquals(atomList[pos], "punctuation", "]")) {
				return new ParseResult(subExpression, pos);
			} else {
				// throw error
			}
		} else if (atomEquals(atomList[pos], "punctuation", "]")) {
			Expression indexExpression = new Expression("e-op");
			indexExpression.eOperatorOp = "[]";
			indexExpression.eOperatorLeft = lastExpression;
			indexExpression.eOperatorRight = firstResult.expression;

			return new ParseResult(indexExpression, pos);
		} else {
			// throw error
		}
	}	

	ParseResult parseFunction(List<Atom> atomList, int pos) {
		Expression funcExpression = new Expression("e-func");

		if (atomList[pos].atomType == "identifier") {
			funcExpression.eFuncId = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "punctuation", "(")) {
				pos += 1;

				List<string> argumentList = new List<string>();
				while (!atomEquals(atomList[pos], "punctuation", ")")) {
					if (atomList[pos].atomType == "identifier") {
						argumentList.(atomList[pos].value);
						pos += 1;

						if (atomEquals(atomList[pos], "punctuation", ",")) {
							pos += 1;
						} else if (atomEquals(atomList[pos], "punctuation", ")")) {
							break;
						} else {
							// throw error
						}
					}
				}
				funcExpression.eFuncArguments = argumentList;
				pos += 1;

				ParseResult bodyResult = parseDo(atomList, pos);
				funcExpression.eFuncBody = bodyResult.expression;
				pos = bodyResult.position;

				return new ParseResult(funcExpression, pos);
			} else {
				// throw error
			}
		} else {
			// throw error
		}
	}



	ParseResult parseNot(List<Atom> atomList, int pos) {
		Expression notExpression = new Expression("e-op");

		ParseResult notResult = parseSingle(atomList, pos, true);
		notExpression.eOperatorLeft = notResult.expression;
		notExpression.eOperatorRight = new Expression("e-bool");
		notExpression.eOperatorOp = new Operator("bool-not");
	}

	ParseResult parseIf(List<Atom> atomList, int pos) {
		Expression ifExpression = new Expression("e-if");


		ParseResult condResult = parseSingle(atomList, pos, true);
		ifExpression.eIfCond = condResult.expression;
		pos = condResult.position + 1;

		ParseResult consqResult = parseDo(atomList, pos, true);
		ifExpression.eIfConsq = consqResult.expression;
		pos = consqResult.position + 1;

		if (atomEquals(atomList[pos], "punctuation", "else")) {
			pos += 1;

			ParseResult alterResult = parseDo(atomList, pos, true);
			ifExpression.eIfAlter = alterResult.expression;
			pos = alterResult.position;

			return new ParseResult(ifExpression, pos);
		} else {
			// throw error
		}
	}

	ParseResult parseLambda(List<Atom> atomList, int pos) {
		Expression lamExpression = new Expression("e-lam");

		if (atomEquals(atomList[pos], "punctuation", "(")) {
			pos += 1;

			List<string> argumentList = new List<string>();
			while (!atomEquals(atomList[pos], "punctuation", ")")) {
				if (atomList[pos].atomType == "identifier") {
					argumentList.Add(atomList[pos].value);
					pos += 1;

					if (atomEquals(atomList[pos], "punctuation", ",")) {
						pos += 1;
					} else if (atomEquals(atomList[pos], "punctuation", ")")) {
						break;
					} else {
						// throw error
					}
				}
			}
			lamExpression.eLamParams = argumentList;
			pos += 1;

			ParseResult bodyResult = parseDo(atomList, pos, true);
			lamExpression.eLamBody = bodyResult.expression;
			pos = bodyResult.position;

			return new ParseResult(lamExpression, pos);
		} else {
			// throw error
		}
	}

	ParseResult parseLet(List<Atom> atomList, int pos) {
		Expression letExpression = new Expression("e-let");

		if (atomList[pos].atomType == "identifier") {
			letExpression.eLetName = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "operator", "=")) {
				pos += 1;

				ParseResult letValueResult = parseSingle(atomList, pos, false);
				letExpression.eLetValue = letValueResult.expression;
				pos = letValueResult.position;
				
				return new ParseResult(letExpression, pos);
			} else {
				// throw error
			}
		} else {
			// throw error
		}
	}

	ParseResult parseWhile(List<Atom> atomList, int pos) {
		Expression whileExpression = new Expression("e-while");

		ParseResult condResult = parseSingle(atomList, pos, true);
		whileExpression.eWhileCond = condResult.expression;
		pos = condResult.position + 1;

		ParseResult bodyResult = parseDo(atomList, pos, true);
		whileExpression.eLamBody = bodyResult.expression;
		pos = bodyResult.position;

		return new ParseResult(whileExpression, pos);
	}

	ParseResult parseIdentifier(List<Atom> atomList, int pos) {
		string identifierName;

		if (atomList[pos].atomType == "identifier") {
			identifierName = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "operator", "=")) { 
				pos += 1;

				Expression setExpression = new Expression("e-set");
				ParseResult valueResult = parseSingle(atomList, pos, false);
				setExpression.eSetName = identifierName;
				setExpression.eSetValue = valueResult.expression;
				pos = valueResult.position;

				return new ParseResult(igVarExpression, pos);
			} else if (atomEquals(atomList[pos], "punctuation", "(")) {
				pos += 1

				Expression appExpression = new Expression("e-app");
				appExpression.eAppFunc = identifierName;
				
				List<Expression> argumentList = new List<Expression>();
				while (!atomEquals(atomList[pos], "punctuation", ")")) {

					ParseResult argumentResult = parseSingle
					argumentList.Add(argumentResult.expression);
					pos = argumentResult.position += 1;

					if (atomEquals(atomList[pos], "punctuation", ",")) {
						pos += 1;
					} else if (atomEquals(atomList[pos], "punctuation", ")")) {
						break;
					} else {
						// throw error
					}
				}
				appExpresion.eAppArguments = argumentList;

				return new ParseResult(appExpression, pos);
			} else {
				Expression idExpression = new Expression("e-id");
				idExpression.eId = identifierName;
				return new ParseResult(idExpression, pos - 1)
			}
		} else {
			// throw error
		}
	}

	ParseResult ParseLet(List<Atom> atomList, int pos) {
		xpression setExpression = new Expression("e-let");

		if (atomList[pos].atomType == "identifier") {
			setExpression.eLetName = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "operator", "=")) { 
				pos += 1;

				Expression setExpression = new Expression("e-set");
				ParseResult valueResult = parseSingle(atomList, pos, false);
				setExpression.eSetName = identifierName;
				setExpression.eSetValue = valueResult.expression;
				pos = valueResult.position;

				return new ParseResult(igVarExpression, pos);
			} else {
				// throw error
			}
		} else {
			// throw error
		}
	}

	ParseResult parseIg(List<Atom> atomList, int pos) {
		string igName;
		string igVariable;

		if (atomList[pos].atomType == "identifier") {
			igName = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "punctuation", ".")) {
				pos += 1;

				if (atomList[pos].atomType == "identifier") {
					igVariable = atomList[pos].value;
					pos += 1;

					if (atomEquals(atomList[pos], "operator", "=")) {
						pos += 1;

						Expression igVarExpression = new Expression("e-set-ig-var");
						ParseResult valueResult = parseSingle(atomList, pos, false);
						igVarExpression.eSetIgName = igName;
						igVarExpression.eSetIgVariable = igVariable;
						igVarExpression.eSetIgValue = valueResult.expression;
						pos = valueResult.position;

						return new ParseResult(igVarExpression, pos);
					} else {
						Expression igVarExpression = new Expression("e-ig-var");
						igVarExpression.eIgName = igName;
						igVarExpression.eIgVariable = igVariable;

						return new ParseResult(igVarExpression, pos - 1);
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

	// ================================= Tokenizer Functions ================================= 

	List<Atom> tokenize(string input) {
		List<Atom> atomList = new List<Atom>();

		bool buildingAtom = false;
		bool buildingString = false;
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
								case "var":
								case "while":
								case "function":
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
					case "operator":
						switch(curAtom.value) {
							case "=":
							case ">":
							case "<":
								if (curChar == '=') {
									curAtom.value += curChar.ToString();
								} else {
									atomList.Add(curAtom);
									buildingAtom = false;
								}
								break;
							case "*":
								if (curChar == '*') {
									curAtom.value += curChar.ToString();
								} else {
									atomList.Add(curAtom);
									buildingAtom = false;
								}
								break;
							case "|":
								if (curChar == '|') {
									curAtom.value += curChar.ToString();
								} else {
									// Throw Error
								}
								break;
							case "&":
								if (curChar == '&') {
									curAtom.value += curChar.ToString();
								} else {
									// Throw Error
								}
								break;
							default:
								atomList.Add(curAtom);
								buildingAtom = false;
								break;
						}
						break;
					default:
						// throw error
						break;
				}
			}

			if (!buildingAtom && !buildingString) {
				if (isWhiteSpace(curChar)) {
					
				} else if (curChar == '\'') {
					curAtom = new Atom();
					curAtom.atomType = "string";
					buildingAtom = true;
					buildingString = true;
					singleQuote = true;
				} else if (curChar == '\"') {
					curAtom = new Atom();
					curAtom.atomType = "string";
					buildingAtom = true;
					buildingString = true;
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
					buildingAtom = true;
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
			} else if (!buildingAtom && buildingString) {
				buildingString = false;
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
			case '$':
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
				return interpLam(expression.eLamParams, expression.eLamBody, env, store, ref tokenEnv, ref cubeEnv);
			case "e-app": //e-app
				return interpApp(expression.eAppFunc, expression.eAppArguments, env, store, ref tokenEnv, ref cubeEnv);
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
		String op, 
		Expression left, 
		Expression right, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (op) {
			case "+":
				return interpOpPlus(left, right, env, store, ref tokenEnv, ref cubeEnv);
			case "-":
				return interpOpMath(left, right, floatSubtract, intSubtract, env, store, ref tokenEnv, ref cubeEnv);
			case "*":
				return interpOpMath(left, right, floatMultiply, intMultiply, env, store, ref tokenEnv, ref cubeEnv);
			case "/":
				return interpOpMath(left, right, floatDivide, intDivide, env, store, ref tokenEnv, ref cubeEnv);
			case "**":
				return interpOpMath(left, right, floatExponent, intExponent, env, store, ref tokenEnv, ref cubeEnv);
			case "%":
				return interpOpMath(left, right, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "==":
				return interpOpPlus(left, right, env, store, ref tokenEnv, ref cubeEnv);
			case ">":
				return interpOpCompare(left, right, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "<":
				return interpOpCompare(left, right, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case ">=":
				return interpOpCompare(left, right, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "<=":
				return interpOpCompare(left, right, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "&&":
				return interpOpLogic(boolNot, left, right, env, store, ref tokenEnv, ref cubeEnv);
			case "||":
				return interpOpLogic(boolNot, left, right, env, store, ref tokenEnv, ref cubeEnv);
			case "!":
				return interpOpLogic(boolNot, left, right, env, store, ref tokenEnv, ref cubeEnv);
			case "[]": // Index
				return interpOpIndex(listIndexValue, left, right, env, store, ref tokenEnv, ref cubeEnv);
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Unknown operator type: " + op.operatorType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	Result interpOpMath(
		Expression left, 
		Expression right,
		Func<float, float, float> floatFunc,
		Func<int, int, int> intFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("float"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("float");
						if (left.value.valueType == "float" && right.value.valueType == "float") {
							returnValue.vFloat = floatFunc(l_result.value.vFloat, r_rseult.value.vFloat);
						} else if (left.value.valueType == "float" && right.value.valueType == "int") {
							returnValue.vFloat = floatFunc(l_result.value.vFloat, (float)r_rseult.value.vInt);
						} else if (left.value.valueType == "int" && right.value.valueType == "float") {
							returnValue.vFloat = floatFunc((float)l_result.value.vInt, r_rseult.value.vFloat);
						} else {
							returnValue.valuteType = "int";
							returnValue.vInt = intFunc(l_result.value.vInt, r_rseult.value.vInt);
						}					
						return new Result(returnValue, r_result.store);
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot add int or float with: " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
				break;
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect int or float, not: " + l_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	float floatSubtract(float left, float right) {
		return left - right;
	}

	int intSubtract(int left, int right) {
		return left - right;
	}

	float floatMultiply(float left, float right) {
		return left * right;
	}

	int intMultiply(int left, int right) {
		return left * right;
	}

	float floatDivide(float left, float right) {
		if (right == 0) {
			if (left > 0) {
				return float.PositiveInfinity;
			} else if (left < 0) {
				return float.NegativeInfinity;
			} else {
				return (float)1;
			}
		} else {
			return left / right;
		}
	}

	int intDivide(int left, int right) {
		if (right == 0) {
			if (left > 0) {
				return int.MaxValue;
			} else if (left < 0) {
				return int.MinValue;
			} else {
				return 1;
			}
		} else {
			return left / right;
		}
	}

	float floatExponent(float left, float right) {
		return Math.Pow(left, right);
	}

	int intExponent(int left, int right) {
		return Math.Pow(left, right);
	}

	float floatModulo(float left, float right) {
		return left % right;
	}

	int intModulo(int left, int right) {
		return left % right;
	}

	Result interpOpPlus(
		Expression left, 
		Expression right,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("float"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("float");
						if (left.value.valueType == "float" && right.value.valueType == "float") {
							returnValue.vFloat = l_result.value.vFloat + r_rseult.value.vFloat;
						} else if (left.value.valueType == "float" && right.value.valueType == "int") {
							returnValue.vFloat = l_result.value.vFloat + (float)r_rseult.value.vInt;
						} else if (left.value.valueType == "int" && right.value.valueType == "float") {
							returnValue.vFloat = (float)l_result.value.vInt + r_rseult.value.vFloat;
						} else {
							returnValue.valuteType = "int";
							returnValue.vInt = l_result.value.vInt + r_rseult.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot add int or float with " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
				break;
			case("string"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("string"):
						Value returnValue = new Value("string");
						returnValue.vString = l_result.value.vString + r_result.value.vString;
						return new Result(returnValue, r_result.store);	
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot concatenate string with " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			case("list"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("vList"):
						Value returnValue = new Value("list");
						returnValue.vList = l_result.value.vList.Concat(r_result.value.vList).ToList();
						return new Result(returnValue, r_result.store);
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot concatenate list with " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect string, float, list, or int. Not: " + l_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	Result interpOpEqual(
		Expression left, 
		Expression right,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("float"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("bool");
						if (left.value.valueType == "float" && right.value.valueType == "float") {
							returnValue.vBool = l_result.value.vFloat == r_rseult.value.vFloat;
						} else if (left.value.valueType == "float" && right.value.valueType == "int") {
							returnValue.vBool = l_result.value.vFloat == (float)r_rseult.value.vInt;
						} else if (left.value.valueType == "int" && right.value.valueType == "float") {
							returnValue.vBool = (float)l_result.value.vInt == r_rseult.value.vFloat;
						} else {
							returnValue.vBool = l_result.value.vInt == r_rseult.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot compare int or float with " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
				break;
			case("string"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("string"):
						Value returnValue = new Value("bool");
						returnValue.vBool = l_result.value.vString == r_result.value.vString;
						return new Result(returnValue, r_result.store);	
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot compare string with " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect string, float, or int. Not: " + l_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	Result interpOpCompare(
		Expression left, 
		Expression right,
		Func<bool, float, float> floatFunc,
		Func<bool, int, int> intFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("float"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("bool");
						if (left.value.valueType == "float" && right.value.valueType == "float") {
							returnValue.vBool = floatFunc(l_result.value.vFloat, r_rseult.value.vFloat);
						} else if (left.value.valueType == "float" && right.value.valueType == "int") {
							returnValue.vBool = floatFunc(l_result.value.vFloat, (float)r_rseult.value.vInt);
						} else if (left.value.valueType == "int" && right.value.valueType == "float") {
							returnValue.vBool = floatFunc((float)l_result.value.vInt, r_rseult.value.vFloat);
						} else {
							returnValue.vBool = intFunc(l_result.value.vInt, r_rseult.value.vInt);
						}					
						return new Result(returnValue, r_result.store);
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot compare int or float with: " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
				break;
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect int or float for comparison, not: " + l_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	bool floatGT(float left, float right){
		return left > right;
	}

	bool intGT(int left, int right){
		return left > right;
	}

	bool floatLT(float left, float right){
		return left < right;
	}

	bool intLT(int left, int right){
		return left < right;
	}

	bool floatGEQ(float left, float right){
		return left >= right;
	}

	bool intGEQ(int left, int right){
		return left >= right;
	}

	bool floatLEQ(float left, float right){
		return left <= right;
	}

	bool intLEQ(int left, int right){
		return left <= right;
	}

	Result interpOpLogic(
		Func<Value, Value, Value> valueFunc, 
		Expression left, 
		Expression right, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		if (l_result.value.valueType == "bool") {
			Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
			if (r_result.value.valueType == "bool") {
				Value returnValue = valueFunc(l_result.value, r_result.value);
				return new Result(returnValue, r_result.store);
			} else {
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect bool for logic, not: " + r_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
			}
		} else {
			Value errorValue = new Value("error"); 
			errorValue.errorMessage = "Expect bool for logic, not: " + l_result.value.valueType.ToString();
			return new Result(errorValue, store); //Throw Error	
		}
	}

	Value boolNot(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = !leftValue.vBool;
		return returnValue;
	}

	Value boolAnd(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vBool && rightValue.vBool;
		return returnValue;
	}

	Value boolOr(Value leftValue, Value rightValue) {
		Value returnValue = new Value("bool");
		returnValue.vBool = leftValue.vBool || rightValue.vBool;
		return returnValue;
	}

	Result interpOpIndex(
		Expression left, 
		Expression right, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("string"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
						if (Math.Abs(r_result.value.vFloat) < l_result.value.vString.Length) {
							Value returnValue = new Value("string");
							returnValue.vString = leftValue.vString[rightValue.vInt].ToString();
							return new Result(returnValue, r_result.store);
						} else {
							Value errorValue = new Value("error");
							errorValue.errorMessage = "Index out of range";
							return new Result(errorValue, store); //Throw Error	
						}
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot index into string with: " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			case("list"):
				Result r_result = interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
						if (Math.Abs(r_result.value.vFloat) < l_result.value.vList.Count) {
							return new Result(leftValue.vList[rightValue.vInt], r_result.store);
						} else {
							Value errorValue = new Value("error");
							errorValue.errorMessage = "Index out of range";
							return new Result(errorValue, store); //Throw Error	
						}
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Cannot index into list with: " + l_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect string or list to index into. Not: " + l_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
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
			case("[:]"):
				return interpTriOperatorHelper(listSublistValue, target, left, right, "list", "int", "int", env, store, ref tokenEnv, ref cubeEnv);
			case("string-substring"):
				return interpTriOperatorHelper(stringSubstringValue, target, left, right, "string", "int", "int", env, store, ref tokenEnv, ref cubeEnv);
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Unknown operator type: " + op.operatorType.ToString();
				return new Result(errorValue, store); //Throw Error	

		}
	}

	Result interpOpSub(
		Func<Value, Value, Value, Value> valueFunc, 
		Expression target, 
		Expression left, 
		Expression right, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		)
	{
		Result l_result =  interpret(left, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case "int":
				Result r_result =  interpret(right, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case "int":
							Result t_result = interpret(target, env, r_result.store, ref tokenEnv, ref cubeEnv);
							switch(t_result.value.valueType) {
								case "string":
									Value returnValue = Value("string");
									int idx = l_result.value.vInt;
									int num = r_result.value.vInt - idx;
									returnValue.vString = t_result.value.vString.Substring(idx, num);
									return new Result(returnValue, t_result.store);
								case "list":	
									Value returnValue = Value("list");
									int idx = l_result.value.vInt;
									int num = r_result.value.vInt - idx;
									returnValue.vList = t_result.value.vList.GetRange(idx, num);
									return new Result(returnValue, t_result.store);
								default:
									Value errorValue = new Value("error"); 
									errorValue.errorMessage = "Expect string or list to index into. Not: " + t_result.value.valueType.ToString();
									return new Result(errorValue, t_result.store); //Throw Error	
							}				
					default:
						Value errorValue = new Value("error"); 
						errorValue.errorMessage = "Expect int for indexing. Not: " + r_result.value.valueType.ToString();
						return new Result(errorValue, store); //Throw Error	
				}
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Expect int for indexing. Not: " + l_result.value.valueType.ToString();
				return new Result(errorValue, store); //Throw Error	
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
		List<string> params, 
		Expression body, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Value returnValue = new Value();
		returnValue.vFunParams = params;
		returnValue.vFunBody = body;
		returnValue.vFunEnviroment = env;
		return new Result(returnValue, store);
	}

	Result interpApp(
		Expression func, 
		List<Expression> args, 
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result func_result = interpret(func, env, store, ref tokenEnv, ref cubeEnv);
		if (func_result.value.valueType == "function") {
			if (args.Count != func_result.value.vFunParams.Count) {
				Value errorValue = new Value("error");
				errorValue.errorMessage = "Functions expects " + func_result.value.vFunParams.Count.ToString() + ", got " + args.Count.ToString();
				return new Result(errorValue, store);
			}

			Dictionary<string, Value> appStore = func_result.store;
			for (int i; i < args.Count; i++) {
				Expression arg = args[i];
				string param = func_result.value.vFunParams[i];
				Result arg_result = interpret(arg, env, appStore, ref tokenEnv, ref cubeEnv);
				string loc = System.Guid.NewGuid().ToString();
				func_result.value.vFunEnviroment.Add(param, loc);
				appStore.Add(loc, arg_result.value);
			}
			return interpret(func_result.value.vFunBody, func_result.value.vFunEnviroment, appStore, ref tokenEnv, ref cubeEnv); 
		} else {
			return new Result(new Value("error", func_result.value.valueType), store); //Throw Error
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

	public List<string> vFunParams; // type=5
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

	public String eOperatorOp; // type=6
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
	public List<string> eLamParams;
	public Expression eLamBody;

	public Expression eAppFunc; // type=10
	public Expression eAppArg;
	public List<Expression> eAppArguments;

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

	public string eLetName;

	public 

	//Sugar

	public List<Expression> eForIter; //type=102
	public string eForVariable;
	public Expression eForBody;

	public Expression eNot; //type=103

	public string eFuncId;
	public string eFuncArgument;
	public List<string> eFuncArguments;
	public Expression eFuncBody;

	public Expression(string etype) {
		expressionType = etype;
	}
}

public class Operator {
	public string operatorType;

	public Operator(string t) {
		operatorType = t;
	}
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

	// op-bool-not
	// op-bool-and
	// op-bool-or
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

	public Atom(string at, string v) {
		atomType = at;
		value = v;
	}

	public Atom() {

	}
}

public class ParseResult {
	public int position;
	public Expression expression;

	public ParseResult(Expression e, int p) {
		position = p;
		expression = e;
	}
}
