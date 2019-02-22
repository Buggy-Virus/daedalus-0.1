﻿using System;
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
	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================

	Dictionary<string, string> builtInFunctions = new Dictionary<string, string>()
	{
		{"ToString", ToString},
		{"ToInt", ToInt},
		{"Todouble", Todouble},
		{"ToBool", ToBool},
		{"Abs", Abs},
		{"Max", Max},
		{"Min", Min},
		{"Floor", Floor},
		{"Ceil", Ceil},
		{"Factorial", Factorial},
		{"Round", Round},
		{"Sum", Sum},
		{"Len", Len}
	};

	Result ToString(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref tokenEnv, ref cubeEnv);

		if (!(inputResult.value.valueType == "bool" || inputResult.value.valueType == "int" || inputResult.value.valueType == "double")) {
			return resultError(inputResult, "Expected string");
		}


	}

	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================

	public Value evaluate(
		string input, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref tokenEnv,
			ref cubeEnv
		).value;
	}

	public Value evaluateSelfToken(
		string input, 
		Token self,
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		tokenEnv.Add("self", self);

		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref tokenEnv,
			ref cubeEnv
		).value;
	}

	public Value evaluateSelfCube(
		string input, 
		Cube self,
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		cubeEnv.Add("self", self);

		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref tokenEnv,
			ref cubeEnv
		).value;
	}

	public Value evaluateSelfTokenTargetToken(
		string input, 
		Token self, 
		Token target,
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		tokenEnv.Add("self", self);
		tokenEnv.Add("target", target);

		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref tokenEnv,
			ref cubeEnv
		).value;
	}

	public Value evaluateSelfTokenTargetCube(
		string input, 
		Token self, 
		Cube target,
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		tokenEnv.Add("self", self);
		cubeEnv.Add("target", target);
		
		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref tokenEnv,
			ref cubeEnv
		).value;
	}

	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================

	List<Atom> tokenize(string input) {
		List<Atom> atomList = new List<Atom>();

		bool buildingAtom = false;
		bool buildingString = false;
		bool singleQuote = false;

		Atom curAtom = new Atom();
		int character = 1;
		int line = 1;

		for (int i = 0; i < input.Length; i++) {
			char curChar = input[i];

			if (buildingAtom) {
				switch(curAtom.atomType) {
					case "string":
						if ((curChar == '\'' && singleQuote == true) || (curChar == '\"' && singleQuote == false)) {
							atomList.Add(curAtom);
							buildingAtom = false;

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
						if (Char.IsDigit(curChar) || Char.IsLetter(curChar) || curChar == '_') {
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
								case "for":
								case "each":
									curAtom.atomType = "keyword";
									break;
								case "true":
								case "false":
								case "in":
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
									atomList.Add(curAtom);
									buildingAtom = false;
								} else {
									atomList.Add(curAtom);
									buildingAtom = false;
								}
								break;
							case "!":
								if (curChar == '=') {
									curAtom.value += curChar.ToString();
									atomList.Add(curAtom);
									buildingAtom = false;
								} else {
									atomList.Add(curAtom);
									buildingAtom = false;
								}
								break;
							case "*":
								if (curChar == '*') {
									curAtom.value += curChar.ToString();
									atomList.Add(curAtom);
									buildingAtom = false;
								} else {
									atomList.Add(curAtom);
									buildingAtom = false;
								}
								break;
							case "|":
								if (curChar == '|') {
									curAtom.value += curChar.ToString();
									atomList.Add(curAtom);
									buildingAtom = false;
								} else {
									atomList.Add(new Atom("error", curChar.ToString(), line, character));
									buildingAtom = false;
								}
								break;
							case "&":
								if (curChar == '&') {
									curAtom.value += curChar.ToString();
									atomList.Add(curAtom);
									buildingAtom = false;
								} else {
									atomList.Add(new Atom("error", curChar.ToString(), line, character));
									buildingAtom = false;
								}
								break;
							default:
								atomList.Add(curAtom);
								buildingAtom = false;
								break;
						}
						break;
				}
			}

			if (!buildingAtom && !buildingString) {
				if (isWhiteSpace(curChar)) {
					if (curChar == '\n') {
						line += 1;
						character = 0;
					}
				} else if (curChar == '\'') {
					curAtom = new Atom("string", "", line, character);
					buildingAtom = true;
					buildingString = true;
					singleQuote = true;
				} else if (curChar == '\"') {
					curAtom = new Atom("string", "", line, character);
					buildingAtom = true;
					buildingString = true;
					singleQuote = false;
				} else if (isPunctuation(curChar)) {
					curAtom = new Atom("punc", curChar.ToString(), line, character);
					atomList.Add(curAtom);
				} else if (isOperator(curChar)) {
					curAtom = new Atom("operator", curChar.ToString(), line, character);
					buildingAtom = true;
				} else if (Char.IsDigit(curChar)) {
					curAtom = new Atom("number", curChar.ToString(), line, character);
					buildingAtom = true;
				} else if (Char.IsLetter(curChar)) {
					curAtom = new Atom("identifier", curChar.ToString(), line, character);
					buildingAtom = true;
				} else {
					curAtom = new Atom("error", curChar.ToString(), line, character);
					atomList.Add(curAtom);
				} 
			} else if (!buildingAtom && buildingString) {
				buildingString = false;
			}
			character += 1;
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

	// ================================= Parse Functions =============================================================== 
	// ================================= Parse Functions =============================================================== 
	// ================================= Parse Functions =============================================================== 
	// ================================= Parse Functions =============================================================== 
	// ================================= Parse Functions =============================================================== 
	// ================================= Parse Functions =============================================================== 

	// ================================= Parse Utility Functions =======================================================
	bool atomEquals(Atom atom, string targetType, string targetValue) {
		return atom.atomType == targetType && atom.value == targetValue;
	}

	ParseResult parseError(Atom atom, int pos, string message) {
		Expression error = new Expression("Error", atom.line, atom.character);
		error.eErrorMessage = message + ", got: (" + atom.atomType + ",\"" + atom.value + "\")";
		return new ParseResult(error, pos);
	}

	// ================================= Actual Parser ===============================================================

	Expression parse(string input) {
		List<Atom> atomList = tokenize(input);

		return parseDo(atomList, 0, false).expression;
		 
	}

	ParseResult parseDo(List<Atom> atomList, int pos, bool bookended) {
		Expression doExpression = new Expression("e-do", atomList[pos].line, atomList[pos].character);
		doExpression.eDo = new List<Expression>();

		if (bookended && atomEquals(atomList[pos], "punctuation", "{")) {
			pos += 1;
		} else if (bookended) {
			return parseError(atomList[pos], pos, "Expected \"{\""); // throw error
		}

		while (
			pos < atomList.Count 
			&& !(bookended && atomEquals(atomList[pos], "punctuation", "}"))
			) 
		{
			ParseResult singleResult = parseSingle(atomList, pos, false);
			doExpression.eDo.Add(singleResult.expression);
			pos = singleResult.position + 1;
		}

		return new ParseResult(doExpression, pos);
	}

	ParseResult parseSingle(List<Atom> atomList, int pos, bool bookended) {
		Expression lastExpression = new Expression("error", atomList[pos].line, atomList[pos].character);
		bool seenExpression = false;

		if (bookended && atomEquals(atomList[pos], "punctuation", "(")) {
			pos += 1;
		} else if (bookended) {
			return parseError(atomList[pos], pos, "Expected \"(\""); // throw error
		}

		while (pos < atomList.Count) 
		{
			Atom curAtom = atomList[pos]; 
			if (!seenExpression) {
				if (curAtom.atomType == "keyword") {
					switch(curAtom.value) {
						case "if":
							return parseIf(atomList, pos + 1);
						case "lambda":
							return parseLambda(atomList, pos + 1);
						case "var":
						case "let":
							return parseLet(atomList, pos + 1);
						case "while":
							return parseWhile(atomList, pos + 1);
						case "function":
							return parseFunction(atomList, pos + 1);
						case "for":
							return parseFor(atomList, pos + 1);
						default:
							return parseError(atomList[pos], pos, "Undefined keyword"); // throw error
					}
				} else {
					ParseResult firstResult = parseSingleHelper(atomList, pos);
					lastExpression = firstResult.expression;
					seenExpression = true;
					pos = firstResult.position;
				}
			} else {
				switch(curAtom.atomType) {
					case "operator":
						Expression opExpression = new Expression("e-op", curAtom.line, curAtom.character);
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
							case "!=":
								opExpression.eOperatorOp = "!=";
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
								return parseError(atomList[pos], pos, "Undefined operator"); // throw error
						}
						pos += 1;

						ParseResult secondResult = new ParseResult();
						if (atomEquals(atomList[pos], "punctuation", "(")) {
							secondResult = parseSingle(atomList, pos, true);
						} else {
							secondResult = parseSingleHelper(atomList, pos);
						}
						opExpression.eOperatorRight = secondResult.expression;
						lastExpression = opExpression;
						pos = secondResult.position;
						break;
					case "punctuation":
						switch(curAtom.value) {
							case "[":
								ParseResult indexResult = parseIndex(atomList, pos + 1, lastExpression);
								lastExpression = indexResult.expression;
								pos = indexResult.position;
								break;
							case ";":
								if (!bookended) {
									return new ParseResult(lastExpression, pos);
								} else {
									return parseError(atomList[pos], pos, "Expected \")\""); // throw error
								}
							case ")":
								if (bookended) {
									return new ParseResult(lastExpression, pos);
								} else {
									return parseError(atomList[pos], pos, "Unexpected character"); // throw error
								}
							default:
								return parseError(atomList[pos], pos, "Unexpected punctuation"); // throw error
						}
						break;
					default:
						if (!bookended) {
							return new ParseResult(lastExpression, pos - 1);
						} else {
							return parseError(atomList[pos], pos, "Expected operator or punctuation"); // throw error
						}
				}
			}
		}
		return parseError(atomList[pos], pos, "No expression returned from parseSingle"); // throw error
	}

	ParseResult parseSingleHelper(List<Atom> atomList, int pos) {
		Atom curAtom = atomList[pos];
		Expression returnExpression;

		switch(curAtom.atomType) {
			case "string":
				Expression stringExpression = new Expression("e-string", curAtom.line, curAtom.character);
				stringExpression.eString = curAtom.value; 
				returnExpression = stringExpression;
				break;
			case "number":
				if (curAtom.value.Contains('.')) {
					Expression doubleExpression = new Expression("e-double", curAtom.line, curAtom.character);
					doubleExpression.edouble = double.Parse(curAtom.value);
					returnExpression = doubleExpression;
				} else {
					Expression intExpression = new Expression("e-int", curAtom.line, curAtom.character);
					intExpression.eInt = Int32.Parse(curAtom.value);
					returnExpression = intExpression;
				}
				break;
			case "bool":
				Expression boolExpression = new Expression("e-bool", curAtom.line, curAtom.character);
				boolExpression.eBool = bool.Parse(curAtom.value);
				returnExpression = boolExpression;
				break;
			case "identifier":
				ParseResult identifierResult = parseIdentifier(atomList, pos);
				returnExpression = identifierResult.expression;
				pos = identifierResult.position;
				break;
			case "operator":
				if (curAtom.value == "!") {
					ParseResult notResult = parseNot(atomList, pos + 1);
					returnExpression = notResult.expression;
					pos = notResult.position;
				} else {
					return parseError(atomList[pos], pos, "Unexpected operator"); // throw error
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
						return parseError(atomList[pos], pos, "Unexpected punctuation"); // throw error
				}
				break;
			default:
				return parseError(atomList[pos], pos, "Unexpected atomType"); // throw error
		}
		return new ParseResult(returnExpression, pos);
	}

	ParseResult parseIndex(List<Atom> atomList, int pos, Expression lastExpression) {
		ParseResult firstResult = parseSingle(atomList, pos, false);
		pos = firstResult.position + 1;

		if (atomEquals(atomList[pos], "punctuation", ":")) {
			pos += 1;

			Expression subExpression = new Expression("e-triOp", atomList[pos].line, atomList[pos].character - 2); 
			subExpression.eTriOperatorOp = "[:]";
			subExpression.eTriOperatorTarget = lastExpression;
			subExpression.eTriOperatorLeft = firstResult.expression;

			ParseResult secondResult = parseSingle(atomList, pos, false);
			subExpression.eTriOperatorRight = secondResult.expression;
			pos = secondResult.position + 1;

			if (atomEquals(atomList[pos], "punctuation", "]")) {
				return new ParseResult(subExpression, pos);
			} else {
				return parseError(atomList[pos], pos, "Expected \"]\""); // throw error
			}
		} else if (atomEquals(atomList[pos], "punctuation", "]")) {
			Expression indexExpression = new Expression("e-op", atomList[pos].line, atomList[pos].character - 2);
			indexExpression.eOperatorOp = "[]";
			indexExpression.eOperatorLeft = lastExpression;
			indexExpression.eOperatorRight = firstResult.expression;

			return new ParseResult(indexExpression, pos);
		} else {
			return parseError(atomList[pos], pos, "Expected \"]\""); // throw error
		}
	}	

	ParseResult parseList(List<Atom> atomList, int pos) {
		Expression listExpression = new Expression("e-list", atomList[pos].line, atomList[pos].character - 1);

		List<Expression> expressionList = new List<Expression>();
		while (!atomEquals(atomList[pos], "punctuation", ")")) {

			ParseResult argumentResult = parseSingle(atomList, pos, false);
			expressionList.Add(argumentResult.expression);
			pos = argumentResult.position += 1;

			if (atomEquals(atomList[pos], "punctuation", ",")) {
				pos += 1;
			} else if (atomEquals(atomList[pos], "punctuation", ")")) {
				break;
			} else {
				return parseError(atomList[pos], pos, "Expected \",\" or \")\""); // throw error
			}
		}
		listExpression.eList = expressionList;

		return new ParseResult(listExpression, pos);
	}

	ParseResult parseFunction(List<Atom> atomList, int pos) {
		Expression funcExpression = new Expression("e-func", atomList[pos].line, atomList[pos].character - 1);

		if (atomList[pos].atomType == "identifier") {
			funcExpression.eFuncId = atomList[pos].value;
			pos += 1;

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
							return parseError(atomList[pos], pos, "Expected \",\" or \")\""); // throw error
						}
					}
				}
				funcExpression.eFuncParams = argumentList;
				pos += 1;

				ParseResult bodyResult = parseDo(atomList, pos, true);
				funcExpression.eFuncBody = bodyResult.expression;
				pos = bodyResult.position;

				return new ParseResult(funcExpression, pos);
			} else {
				return parseError(atomList[pos], pos, "Expected \"(\""); // throw error
			}
		} else {
			return parseError(atomList[pos], pos, "Expected identifier"); // throw error
		}
	}



	ParseResult parseNot(List<Atom> atomList, int pos) {
		Expression notExpression = new Expression("e-op", atomList[pos].line, atomList[pos].character - 1);

		ParseResult notResult = parseSingle(atomList, pos, false);
		notExpression.eOperatorLeft = notResult.expression;
		notExpression.eOperatorRight = new Expression("e-bool");
		notExpression.eOperatorOp = "!";
		pos = notResult.position;

		return new ParseResult(notExpression, pos);
	}

	ParseResult parseIf(List<Atom> atomList, int pos) {
		Expression ifExpression = new Expression("e-if", atomList[pos].line, atomList[pos].character - 1);


		ParseResult condResult = parseSingle(atomList, pos, true);
		ifExpression.eIfCond = condResult.expression;
		pos = condResult.position + 1;

		ParseResult consqResult = parseDo(atomList, pos, true);
		ifExpression.eIfConsq = consqResult.expression;
		pos = consqResult.position + 1;

		if (atomEquals(atomList[pos], "keyword", "else")) {
			pos += 1;

			ParseResult alterResult = parseDo(atomList, pos, true);
			ifExpression.eIfAlter = alterResult.expression;
			pos = alterResult.position;

			return new ParseResult(ifExpression, pos);
		} else {
			return parseError(atomList[pos], pos, "Expected \"else\""); // throw error
		}
	}

	ParseResult parseLambda(List<Atom> atomList, int pos) {
		Expression lamExpression = new Expression("e-lam", atomList[pos].line, atomList[pos].character - 1);

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
						return parseError(atomList[pos], pos, "Expected \",\" or \")\""); // throw error
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
			return parseError(atomList[pos], pos, "Expected \"(\""); // throw error
		}
	}

	ParseResult parseLet(List<Atom> atomList, int pos) {
		Expression letExpression = new Expression("e-let", atomList[pos].line, atomList[pos].character - 1);

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
				return parseError(atomList[pos], pos, "Expected \"=\""); // throw error
			}
		} else {
			return parseError(atomList[pos], pos, "Expected identifier"); // throw error
		}
	}

	ParseResult parseWhile(List<Atom> atomList, int pos) {
		Expression whileExpression = new Expression("e-while", atomList[pos].line, atomList[pos].character - 1);

		ParseResult condResult = parseSingle(atomList, pos, true);
		whileExpression.eWhileCond = condResult.expression;
		pos = condResult.position + 1;

		ParseResult bodyResult = parseDo(atomList, pos, true);
		whileExpression.eLamBody = bodyResult.expression;
		pos = bodyResult.position;

		return new ParseResult(whileExpression, pos);
	}

	ParseResult parseFor(List<Atom> atomList, int pos) {
		Expression forExpression = new Expression("e-for-each", atomList[pos].line, atomList[pos].character);

		if (atomEquals(atomList[pos], "keyword", "each")) {
			pos += 1;

			if (atomEquals(atomList[pos], "punctuation", "(")) {
				pos += 1;

				if (atomList[pos].atomType == "identifier") {
					forExpression.eForVariable = atomList[pos].value;
					pos += 1;

					if (atomEquals(atomList[pos], "keyword", "in")) {
						pos += 1;

						ParseResult iterResult = parseSingle(atomList, pos, false);
						forExpression.eForIter = iterResult.expression;
						pos = iterResult.position + 1;

						if (atomEquals(atomList[pos], "punctuation", ")")) {
							pos += 1;

							ParseResult bodyResult = parseDo(atomList, pos, true);
							forExpression.eForBody = bodyResult.expression;
							return new ParseResult(forExpression, pos);	
						} else {
							return parseError(atomList[pos], pos, "Expected \")\""); // throw error
						}
					} else {
						return parseError(atomList[pos], pos, "Expected \"in\""); // throw error
					}
				} else {
					return parseError(atomList[pos], pos, "Expected identifier"); // throw error
				}
			} else {
				return parseError(atomList[pos], pos, "Expected \"(\""); // throw error
			}
		} else {
			return parseError(atomList[pos], pos, "Expected \"each\""); // throw error
		}
	}

	ParseResult parseIdentifier(List<Atom> atomList, int pos) {
		string identifierName;

		if (atomList[pos].atomType == "identifier") {
			identifierName = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "operator", "=")) { 
				pos += 1;

				Expression setExpression = new Expression("e-set", atomList[pos].line, atomList[pos].character);
				ParseResult valueResult = parseSingle(atomList, pos, false);
				setExpression.eSetName = identifierName;
				setExpression.eSetValue = valueResult.expression;
				pos = valueResult.position;

				return new ParseResult(setExpression, pos);
			} else if (atomEquals(atomList[pos], "punctuation", "(")) {
				pos += 1;

				Expression evalExpression = new Expression("e-eval", atomList[pos].line, atomList[pos].character - 1);
				evalExpression.eEvalId = identifierName;
				
				List<Expression> argumentList = new List<Expression>();
				while (!atomEquals(atomList[pos], "punctuation", ")")) {

					ParseResult argumentResult = parseSingle(atomList, pos, false);
					argumentList.Add(argumentResult.expression);
					pos = argumentResult.position += 1;

					if (atomEquals(atomList[pos], "punctuation", ",")) {
						pos += 1;
					} else if (atomEquals(atomList[pos], "punctuation", ")")) {
						break;
					} else {
						return parseError(atomList[pos], pos, "Expected \",\" or \")\""); // throw error
					}
				}
				evalExpression.eEvalArguments = argumentList;

				return new ParseResult(evalExpression, pos);
			} else {
				Expression idExpression = new Expression("e-id", atomList[pos].line, atomList[pos].character - 1);
				idExpression.eId = identifierName;
				return new ParseResult(idExpression, pos - 1);
			}
		} else {
			return parseError(atomList[pos], pos, "Expected identifier"); // throw error
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

						Expression igVarExpression = new Expression("e-set-ig-var", atomList[pos].line, atomList[pos].character);
						ParseResult valueResult = parseSingle(atomList, pos, false);
						igVarExpression.eSetIgName = igName;
						igVarExpression.eSetIgVariable = igVariable;
						igVarExpression.eSetIgValue = valueResult.expression;
						pos = valueResult.position;

						return new ParseResult(igVarExpression, pos);
					} else {
						Expression igVarExpression = new Expression("e-ig-var", atomList[pos].line, atomList[pos].character - 1);
						igVarExpression.eIgName = igName;
						igVarExpression.eIgVariable = igVariable;

						return new ParseResult(igVarExpression, pos - 1);
					}
				} else {
					return parseError(atomList[pos], pos, "Expected identifier"); // throw error
				}
			} else {
				return parseError(atomList[pos], pos, "Expected \".\""); // throw error
			}
		} else {
			return parseError(atomList[pos], pos, "Expected identifier"); // throw error
		}
	}

	// ================================= Interpet Functions =========================================================== 
	// ================================= Interpet Functions =========================================================== 
	// ================================= Interpet Functions =========================================================== 
	// ================================= Interpet Functions =========================================================== 
	// ================================= Interpet Functions =========================================================== 
	// ================================= Interpet Functions =========================================================== 

	// ================================= Desugarer ===========================================================
	// ================================= Desugarer ===========================================================
	// ================================= Desugarer ===========================================================

	Expression desugar(Expression expression) {
		switch(expression.expressionType) {
			case "e-list": // e-list
				expression.eList = expression.eList.Select(desugar).ToList();
				return expression;
			case "e-op": //e-op
				expression.eOperatorRight = desugar(expression.eOperatorLeft);
				expression.eOperatorRight = desugar(expression.eOperatorRight);
				return expression;
			case "e-triOp": //e-triOp
				expression.eTriOperatorLeft = desugar(expression.eTriOperatorLeft);
				expression.eTriOperatorRight = desugar(expression.eTriOperatorRight);
				expression.eTriOperatorTarget = desugar(expression.eTriOperatorTarget);
				return expression;
			case "e-if": //e-if
				expression.eIfCond = desugar(expression.eIfCond);
				expression.eIfConsq = desugar(expression.eIfConsq);
				expression.eIfAlter = desugar(expression.eIfAlter);
				return expression;
			case "e-lam": //e-lam
				expression.eLamBody = desugar(expression.eLamBody);
				return expression;
			case "e-app": //e-app
				expression.eAppArguments = expression.eAppArguments.Select(desugar).ToList();
				expression.eAppFunc = desugar(expression.eAppFunc);
				return expression;
			case "e-set": //e-set
				expression.eSetValue = desugar(expression.eSetValue);
				return expression;
			case "e-do": //e-do
				expression.eDo = expression.eDo.Select(desugar).ToList();
				return expression;
			case "e-while": //e-while
				expression.eWhileCond = desugar(expression.eWhileCond);
				expression.eWhileBody = desugar(expression.eWhileBody);
				return expression;
			case "e-let": //e-let
				expression.eLetValue = desugar(expression.eLetValue);
				return expression;
			case "e-set-ig-var": //e-set-ig-variable
				expression.eSetIgValue = desugar(expression.eSetIgValue);
				return expression;
			case "e-return":
				expression.eReturn = desugar(expression.eReturn);
				return expression;
			case "e-eval":
				return desugarEval(expression);
			case "e-for":
				return desugarFor(expression);
			case "e-func":
				return desugarFunc(expression);
			default:
				return expression;
		}
	}

	Expression desugarEval(Expression expression) {
		if (builtInFunctions.ContainsKey(expression.eEvalId)) {
			Expression bFuncExpression = new Expression("e-builtin-func", expression.line, expression.character);
			bFuncExpression.eBuiltinFuncId = expression.eEvalId;
			bFuncExpression.eBuiltinFuncArguments = expression.eEvalArguments;

			return bFuncExpression;
		} else {
			Expression appExpression = new Expression("e-app", expression.line, expression.character);
			appExpression.eAppArguments = expression.eEvalArguments;

			Expression idExpression = new Expression("e-id", expression.line, expression.character);
			idExpression.eId = expression.eEvalId;

			appExpression.eAppFunc = idExpression;

			return appExpression;
		}	
	}

	Expression desugarFor(Expression expression) {
		Expression doExpression = new Expression("e-do", expression.line, expression.character);

		Expression letExpression = new Expression("e-let", expression.line, expression.character);
		letExpression.eLetName = "[iter variable]";

		Expression zeroExpression = new Expression("e-int", expression.line, expression.character);
		zeroExpression.eInt = 0;

		letExpression.eLetValue = zeroExpression;

		Expression whileExpression = new Expression("e-while", expression.line, expression.character);

		Expression ltExpression = new Expression("e-op", expression.line, expression.character);

		Expression idExpression = new Expression("e-id", expression.line, expression.character);
		idExpression.eId = "[iter variable]";

		Expression maxExpression = new Expression("e-int", expression.line, expression.character);
		maxExpression.eInt = 

		ltExpression
	}

	Expression desugarFunc(Expression expression) {
		Expression letExpression = new Expression("e-let", expression.line, expression.character);
		letExpression.eLetName = expression.eFuncId;

		Expression nnnnn = expression.eFuncBody;

		Expression lamExpression = new Expression("e-lam", expression.eFuncBody.line, expression.eFuncBody.character);
		lamExpression.eLamParams = expression.eFuncParams;
		lamExpression.eLamBody = desugar(expression.eFuncBody);

		letExpression.eLetValue = lamExpression;

		return letExpression;
	}

	// ================================= Interpreter Proper ===========================================================
	// ================================= Interpreter Proper ===========================================================
	// ================================= Interpreter Proper ===========================================================

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
				return interpInt(expression, env, store);
			case "e-double": //e-double
				return interpdouble(expression, env, store);
			case "e-string": //e-string
				return interpString(expression, env, store);
			case "e-bool": //e-bool
				return interpBool(expression, env, store);
			case "e-list": // e-list
				return interpList(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-op": //e-op
				return interpOperator(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-triOp": //e-triOp
				return interpTriOperator(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-if": //e-if
				return interpIf(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-lam": //e-lam
				return interpLam(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-app": //e-app
				return interpApp(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-set": //e-set
				return interpSet(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-do": //e-do
				return interpDo(expression, env, store, ref tokenEnv, ref cubeEnv);	
			case "e-while": //e-while
				return interpWhile(expression, new Value(), false, env, store, ref tokenEnv, ref cubeEnv);
			case "e-for-each":
				return interpForEach(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-let": //e-let
				return interpLet(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-id": // e-id
				return interpId(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-ig-var": //e-ig-variable
				return interpIgVariable(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-set-ig-var": //e-set-ig-variable
				return interpSetIgVariable(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-builtin-func": //e-set-ig-variable
				return interpBuiltinFunc(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-return":
				return interpReturn(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-error":
				return interpError(expression, env, store, ref tokenEnv, ref cubeEnv);
			default:
				Value errorValue = new Value("error"); 
				errorValue.errorMessage = "Unknown expression type: " + expression.expressionType.ToString();
				return new Result(errorValue, store); //Throw Error	
		}
	}

	// ================================= Interpret Error Functions ==================================

	Result interpError(
		Expression error,  
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) {
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + error.eErrorMessage;
		return new Result(errorValue, store); //Throw Error	
	}

	Result resultError(Result errorResult, string message) {
		Value error = errorResult.value;
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + message + ", from: (";
		switch(error.valueType) {
			case "int": //e-int
				errorValue.errorMessage += error.valueType + ",\"" + error.vInt.ToString() + "\")";
				break;
			case "double": //e-double
				errorValue.errorMessage += error.valueType + ",\"" + error.vdouble.ToString() + "\")";
				break;
			case "string": //e-string
				errorValue.errorMessage += error.valueType + ",\"" + error.vString + "\")";
				break;
			case "bool": //e-bool
				errorValue.errorMessage += error.valueType + ",\"" + error.vBool.ToString() + "\")";
				break;
			case "list": // e-list
				errorValue.errorMessage += error.valueType + ",<listObject>)";
				break;
			case "function": //e-op
				errorValue.errorMessage += error.valueType + ",<functionObject>)";
				break;
			case "error":
				errorValue.errorMessage += error.valueType + "," + error.errorMessage + ")";
				break;
			default:
				errorValue.errorMessage += error.valueType + ",unknown value type)";
				break;
		}
		return new Result(errorValue, errorResult.store); //Throw Error	
	}

	Result expressionError(Expression error, Dictionary<string, Value> store, string message) {
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + message + ", from: ("
		switch(error.expressionType) {
			case "int": //e-int
				errorValue.errorMessage += error.ValueType + ",\"" + error.eInt.ToString() + "\")";
				break;
			case "double": //e-double
				errorValue.errorMessage += error.ValueType + ",\"" + error.edouble.ToString() + "\")";
				break;
			case "string": //e-string
				errorValue.errorMessage += error.ValueType + ",\"" + error.eString + "\")";
				break;
			case "bool": //e-bool
				errorValue.errorMessage += error.ValueType + ",\"" + error.vBool.ToString() + "\")";
				break;
			case "list": // e-list
				errorValue.errorMessage += error.ValueType + ",<listObject>)";
				break;
			case "function": //e-op
				errorValue.errorMessage += error.ValueType + ",<functionObject>)";
				break;
			case "error":
				errorValue.errorMessage += error.ValueType + "," + error.errorMessage + ")";
			default:
				errorValue.errorMessage += error.expressionType + ",unknown value type)";
		}
		return new Result(errorValue, store); //Throw Error	
	}

	// ================================= Interpret Values Functions =================================

	Result interpInt(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("int", expression.line, expression.character);
		returnValue.vInt = expression.eInt;
		return new Result(returnValue, store);
	}

	Result interpdouble(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("double", expression.line, expression.character);
		returnValue.vdouble = expression.edouble;
		return new Result(returnValue, store);
	}

	Result interpString(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("string", expression.line, expression.character);
		returnValue.vString = expression.eString;
		return new Result(returnValue, store);
	}

	Result interpBool(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("bool", expression.line, expression.character);
		returnValue.vBool = expression.eBool;
		return new Result(returnValue, store);
	}

	Result interpList(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Value returnValue = new Value("list", expression.line, expression.character);
		returnValue.vList = new List<Value>();
		Result last_result = new Result(new Value(), store);
		foreach (Expression expr in expression.eList) {
			last_result = interpret(expr, env, last_result.store, ref tokenEnv, ref cubeEnv);
			returnValue.vList.Add(last_result.value);
		}
		return new Result(returnValue, last_result.store);
	}

	// ================================= Interpret Operator Functions ==================================================
	// ================================= Interpret Operator Functions ==================================================
	// ================================= Interpret Operator Functions ==================================================
	
	Result interpOperator(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (expression.eOperatorOp) {
			case "+":
				return interpOpPlus(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "-":
				return interpOpMath(expression, doubleSubtract, intSubtract, env, store, ref tokenEnv, ref cubeEnv);
			case "*":
				return interpOpMath(expression, doubleMultiply, intMultiply, env, store, ref tokenEnv, ref cubeEnv);
			case "/":
				return interpOpMath(expression, doubleDivide, intDivide, env, store, ref tokenEnv, ref cubeEnv);
			case "**":
				return interpOpMath(expression, doubleExponent, intExponent, env, store, ref tokenEnv, ref cubeEnv);
			case "%":
				return interpOpMath(expression, doubleModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "==":
				return interpOpEqual(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "!=":
				return interpOpNotEqual(expression, env, store, ref tokenEnv, ref cubeEnv);
			case ">":
				return interpOpCompare(expression, doubleGT, intGT, env, store, ref tokenEnv, ref cubeEnv);
			case "<":
				return interpOpCompare(expression, doubleLT, intLT, env, store, ref tokenEnv, ref cubeEnv);
			case ">=":
				return interpOpCompare(expression, doubleGEQ, intGEQ, env, store, ref tokenEnv, ref cubeEnv);
			case "<=":
				return interpOpCompare(expression, doubleLEQ, intLEQ, env, store, ref tokenEnv, ref cubeEnv);
			case "&&":
				return interpOpLogic(expression, boolAnd, env, store, ref tokenEnv, ref cubeEnv);
			case "||":
				return interpOpLogic(expression, boolOr, env, store, ref tokenEnv, ref cubeEnv);
			case "!":
				return interpOpLogic(expression, boolNot, env, store, ref tokenEnv, ref cubeEnv);
			case "[]": // Index
				return interpOpIndex(expression, env, store, ref tokenEnv, ref cubeEnv);
			default:
				return expressionError(expression, store, "Unknown operator"); //Throw Error	
		}
	}

	Result interpOpMath(
		Expression expression,
		Func<double, double, double> doubleFunc,
		Func<int, int, int> intFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("double", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vdouble = doubleFunc(l_result.value.vdouble, r_result.value.vdouble);
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vdouble = doubleFunc(l_result.value.vdouble, (double)r_result.value.vInt);
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vdouble = doubleFunc((double)l_result.value.vInt, r_result.value.vdouble);
						} else {
							returnValue.valueType = "int";
							returnValue.vInt = intFunc(l_result.value.vInt, r_result.value.vInt);
						}					
						return new Result(returnValue, r_result.store);
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
				break;
			default:
				return resultError(l_result, "Expected int or double"); //Throw Error	
		}
	}

	double doubleSubtract(double left, double right) {
		return left - right;
	}

	int intSubtract(int left, int right) {
		return left - right;
	}

	double doubleMultiply(double left, double right) {
		return left * right;
	}

	int intMultiply(int left, int right) {
		return left * right;
	}

	double doubleDivide(double left, double right) {
		if (right == 0) {
			if (left > 0) {
				return double.PositiveInfinity;
			} else if (left < 0) {
				return double.NegativeInfinity;
			} else {
				return (double)1;
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

	double doubleExponent(double left, double right) {
		return Math.Pow(left, right);
	}

	int intExponent(int left, int right) {
		return (int)Math.Round(Math.Pow(left, right));
	}

	double doubleModulo(double left, double right) {
		return left % right;
	}

	int intModulo(int left, int right) {
		return left % right;
	}

	Result interpOpPlus(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("double", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vdouble = l_result.value.vdouble + r_result.value.vdouble;
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vdouble = l_result.value.vdouble + (double)r_result.value.vInt;
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vdouble = (double)l_result.value.vInt + r_result.value.vdouble;
						} else {
							returnValue.valueType = "int";
							returnValue.vInt = l_result.value.vInt + r_result.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
				break;
			case("string"):
				Result r_result_string = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_string.value.valueType) {
					case("string"):
						Value returnValue = new Value("string", expression.line, expression.character);
						returnValue.vString = l_result.value.vString + r_result_string.value.vString;
						return new Result(returnValue, r_result_string.store);	
					default:
						return resultError(r_result_string, "Expected string"); //Throw Error
				}
			case("list"):
				Result r_result_list = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_list.value.valueType) {
					case("vList"):
						Value returnValue = new Value("list", expression.line, expression.character);
						returnValue.vList = l_result.value.vList.Concat(r_result_list.value.vList).ToList();
						return new Result(returnValue, r_result_list.store);
					default:
						return resultError(r_result_list, "list"); //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int, double, string, or list"); //Throw Error	
		}
	}

	Result interpOpEqual(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vBool = l_result.value.vdouble == r_result.value.vdouble;
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vBool = l_result.value.vdouble == (double)r_result.value.vInt;
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vBool = (double)l_result.value.vInt == r_result.value.vdouble;
						} else {
							returnValue.vBool = l_result.value.vInt == r_result.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
				break;
			case("string"):
				Result r_result_string = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_string.value.valueType) {
					case("string"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vString == r_result_string.value.vString;
						return new Result(returnValue, r_result_string.store);	
					default:
						return resultError(r_result_string, "Expected string"); //Throw Error
				}
			case("bool"):
				Result r_result_bool = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_bool.value.valueType) {
					case("bool"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vBool == r_result_bool.value.vBool;
						return new Result(returnValue, r_result_bool.store);	
					default:
						return resultError(r_result_bool, "Expected bool"); //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int, double, string, or bool"); //Throw Error
		}
	}

	Result interpOpNotEqual(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vBool = l_result.value.vdouble != r_result.value.vdouble;
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vBool = l_result.value.vdouble != (double)r_result.value.vInt;
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vBool = (double)l_result.value.vInt != r_result.value.vdouble;
						} else {
							returnValue.vBool = l_result.value.vInt != r_result.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
				break;
			case("string"):
				Result r_result_string = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_string.value.valueType) {
					case("string"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vString != r_result_string.value.vString;
						return new Result(returnValue, r_result_string.store);	
					default:
						return resultError(r_result_string, "Expected string"); //Throw Error
				}
			case("bool"):
				Result r_result_bool = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_bool.value.valueType) {
					case("bool"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vBool != r_result_bool.value.vBool;
						return new Result(returnValue, r_result_bool.store);	
					default:
						return resultError(r_result_bool, "Expected bool"); //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int, double, string, or bool"); //Throw Error
		}
	}

	Result interpOpCompare(
		Expression expression,
		Func<double, double, bool> doubleFunc,
		Func<int, int, bool> intFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vBool = doubleFunc(l_result.value.vdouble, r_result.value.vdouble);
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vBool = doubleFunc(l_result.value.vdouble, (double)r_result.value.vInt);
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vBool = doubleFunc((double)l_result.value.vInt, r_result.value.vdouble);
						} else {
							returnValue.vBool = intFunc(l_result.value.vInt, r_result.value.vInt);
						}					
						return new Result(returnValue, r_result.store);
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error
				}
			default:
				return resultError(l_result, "Expected int or double"); //Throw Error
		}
	}

	bool doubleGT(double left, double right){
		return left > right;
	}

	bool intGT(int left, int right){
		return left > right;
	}

	bool doubleLT(double left, double right){
		return left < right;
	}

	bool intLT(int left, int right){
		return left < right;
	}

	bool doubleGEQ(double left, double right){
		return left >= right;
	}

	bool intGEQ(int left, int right){
		return left >= right;
	}

	bool doubleLEQ(double left, double right){
		return left <= right;
	}

	bool intLEQ(int left, int right){
		return left <= right;
	}

	Result interpOpLogic(
		Expression expression,
		Func<bool, bool, bool> boolFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		if (l_result.value.valueType == "bool") {
			Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
			if (r_result.value.valueType == "bool") {
				Value returnValue = new Value("bool", expression.line, expression.character);
				returnValue.vBool = boolFunc(l_result.value.vBool, r_result.value.vBool);
				return new Result(returnValue, r_result.store);
			} else {
				return resultError(r_result, "Expected bool"); //Throw Error
			}
		} else {
			return resultError(l_result, "Expected bool"); //Throw Error
		}
	}

	bool boolNot(bool left, bool right) {
		return !left;
	}

	bool boolAnd(bool left, bool right) {
		return left && right;
	}

	bool boolOr(bool left, bool right) {
		return left || right;
	}

	Result interpOpIndex(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		// Result r_result = interpret(right, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case("string"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
						if (Math.Abs(r_result.value.vdouble) < l_result.value.vString.Length) {
							Value returnValue = new Value("string", expression.line, expression.character);
							returnValue.vString = l_result.value.vString[r_result.value.vInt].ToString();
							return new Result(returnValue, r_result.store);
						} else {
							return resultError(r_result, "Index out of range"); //Throw Error
						}
					default:
						return resultError(r_result, "Expected int"); //Throw Error
				}
			case("list"):
				Result r_result_list = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result_list.value.valueType) {
					case("int"):
						if (Math.Abs(r_result_list.value.vdouble) < l_result.value.vList.Count) {
							return new Result(l_result.value.vList[r_result_list.value.vInt], r_result_list.store);
						} else {
							return resultError(r_result_list, "Index out of range"); //Throw Error	
						}
					default:
						return resultError(r_result_list, "Expected int"); //Throw Error
				}
			default:
				return resultError(l_result, "Expected list or string"); //Throw Error	
		}
	}

	// ================================= Interpret TriOperator Functions ==================================================
	// ================================= Interpret TriOperator Functions ==================================================
	// ================================= Interpret TriOperator Functions ==================================================

	Result interpTriOperator(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		) 
	{
		switch (expression.eTriOperatorOp) {
			case("[:]"):
				return interpOpSub(expression, env, store, ref tokenEnv, ref cubeEnv);
			default:
				return expressionError(expression, store, "Unknown operator"); //Throw Error	

		}
	}

	Result interpOpSub(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
		)
	{
		Result l_result =  interpret(expression.eTriOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case "int":
				Result r_result =  interpret(expression.eTriOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case "int":
							Result t_result = interpret(expression.eTriOperatorTarget, env, r_result.store, ref tokenEnv, ref cubeEnv);
							switch(t_result.value.valueType) {
								case "string":
									Value returnValue = new Value("string", expression.line, expression.character);
									int idx = l_result.value.vInt;
									int num = r_result.value.vInt - idx;
									returnValue.vString = t_result.value.vString.Substring(idx, num);
									return new Result(returnValue, t_result.store);
								case "list":	
									Value returnValue_list = new Value("list", expression.line, expression.character);
									int idx_list = l_result.value.vInt;
									int num_list = r_result.value.vInt - idx_list;
									returnValue_list.vList = t_result.value.vList.GetRange(idx_list, num_list);
									return new Result(returnValue_list, t_result.store);
								default:
									return resultError(r_result, "Expected list or string"); //Throw Error	
							}				
					default:
						return resultError(r_result, "Expected int"); //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int"); //Throw Error
		}
			
	}

	// ================================= End of Operators Functions ====================================================
	// ================================= End of Operators Functions ====================================================
	// ================================= End of Operators Functions ====================================================
	Result interpIf(
		Expression expression, 
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		Result cond_result = interpret(expression.eIfCond, env, store, ref tokenEnv, ref cubeEnv);
		if (cond_result.value.valueType == "bool") {
			if (cond_result.value.vBool) {
				return interpret(expression.eIfConsq, env, cond_result.store, ref tokenEnv, ref cubeEnv);
			} else {
				return interpret(expression.eIfAlter, env, cond_result.store, ref tokenEnv, ref cubeEnv);
			}
		} else {
			return resultError(cond_result, "Expected bool"); //Throw Error
		}
	}

	Result interpLam(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		Value returnValue = new Value("function");
		returnValue.vFunParams = expression.eLamParams;
		returnValue.vFunBody = expression.eLamBody;
		returnValue.vFunEnviroment = env;
		return new Result(returnValue, store);
	}

	Result interpApp(
		Expression expression,
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		Result func_result = interpret(expression.eAppFunc, env, store, ref tokenEnv, ref cubeEnv);
		if (func_result.value.valueType == "function") {
			if (expression.eAppArguments.Count != func_result.value.vFunParams.Count) {
				return expressionError(expression, store, expression.eAppFunc + " expects " + func_result.value.vFunParams.Count.ToString() + " arguments, got " + func_result.value.vFunParams.Count.ToString()); //Throw Error		
			}

			Dictionary<string, Value> appStore = func_result.store;
			for (int i = 0; i < expression.eAppArguments.Count; i++) {
				Expression arg = expression.eAppArguments[i];
				string param = func_result.value.vFunParams[i];
				Result arg_result = interpret(arg, env, appStore, ref tokenEnv, ref cubeEnv);
				string loc = System.Guid.NewGuid().ToString();
				func_result.value.vFunEnviroment.Add(param, loc);
				appStore.Add(loc, arg_result.value);
			}
			return interpret(func_result.value.vFunBody, func_result.value.vFunEnviroment, appStore, ref tokenEnv, ref cubeEnv); 
		} else {
			return resultError(func_result, "Expected function"); //Throw Error
		}
	}

	Result interpSet(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		string name = expression.eSetName;
		if (env.ContainsKey(name)) {
			string pointer = env[name];
			if (store.ContainsKey(pointer)) {
				Result newValue_result = interpret(expression.eSetValue, env, store, ref tokenEnv, ref cubeEnv);
				newValue_result.store[pointer] = newValue_result.value;
				return new Result(newValue_result.value, newValue_result.store);
			} else {
				return expressionError(expression, store, "Unset variable" + name); //Throw Error
			}
		} else {
			return expressionError(expression, store, "Unbound variable" + name); //Throw Error
		}
	}

	Result interpDo(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		Result last_result = new Result(new Value(), store);
		foreach (Expression expr in expression.eDo) {
			// If statement in Do handles define statements. Defines are only relevent in Do
			// The change to the env is only relevent across other Do expressions
			// In the same Do statement
			if (expr.expressionType == "e-let") {
				Result define_result = interpret(expr.eLetValue, env, last_result.store, ref tokenEnv, ref cubeEnv);
				string loc = System.Guid.NewGuid().ToString();
				env.Add(expr.eLetName, loc);
				define_result.store.Add(loc, define_result.value);
				last_result = define_result;
			} else if (expr.expressionType == "e-return") {
				return interpret(expr.eReturn, env, last_result.store, ref tokenEnv, ref cubeEnv);
			} else {
				last_result = interpret(expr, env, last_result.store, ref tokenEnv, ref cubeEnv);
			}
			
		}
		return last_result;
	}

	Result interpWhile(
		Expression expression, 
		Value lastValue, 
		bool useLast, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		Result cond_result = interpret(expression.eWhileCond, env, store, ref tokenEnv, ref cubeEnv);
		if (cond_result.value.valueType == "bool") {
			if (cond_result.value.vBool) {
				Result body_result = interpret(expression.eWhileBody, env, cond_result.store, ref tokenEnv, ref cubeEnv);
				return interpWhile(expression, body_result.value, true, env, body_result.store, ref tokenEnv, ref cubeEnv);
			} else if (useLast) {
				return new Result(lastValue, cond_result.store);
			} else {
				return new Result(new Value("null", expression.line, expression.character), cond_result.store);
			}
		} else {
			return resultError(cond_result, "Expected bool"); //Throw Error
		}
	}

	// I really cheat on interp for each, it isn't functional at all, but it's very easy
	// STUDY
	// implement built ins, maybe leave this as sugar
	Result interpForEach(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)
	{
		Dictionary<string, Value> activeStore = store;
		Value returnValue = new Value("null", expression.line, expression.character);
		foreach (Expression expr in expression.eForIter) {
			returnResult = interpret(expr, env, activeStore, ref tokenEnv, ref cubeEnv);
			returnValue = exprResult.value;
			activeStore = exprResult.store;
		}
		return 
	}

	Result interpId(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		string name = expression.eId;
		if (env.ContainsKey(name)) {
			if (store.ContainsKey(name)) {
				return new Result(store[env[name]], store);
			} else {
				return expressionError(expression, store, "Unset variable" + name); //Throw Error
			}
		} else {
			return expressionError(expression, store, "Unbound variable" + name); //Throw Error
		}
	}

	Result interpIgVariable(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		string name = expression.eIgName;
		string variable = expression.eIgVariable;
		if (tokenEnv.ContainsKey(name)) {
			Token token = tokenEnv[name];
			if (token.variables.ContainsKey(variable)) {
				switch(token.variables[variable]) {
					case "int":
						Value intValue = new Value("int", expression.line, expression.character);
						intValue.vInt = token.intVars[variable];
						return new Result(intValue, store);
					case "double":
						Value doubleValue = new Value("double", expression.line, expression.character);
						doubleValue.vdouble = token.doubleVars[variable];
						return new Result(doubleValue, store);
					case "string":
						Value stringValue = new Value("string", expression.line, expression.character);
						stringValue.vString= token.stringVars[variable];
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool", expression.line, expression.character);
						boolValue.vBool = token.boolVars[variable];
						return new Result(boolValue, store);
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\""); //Throw Error
				}
			} else {
				return expressionError(expression, store, "ig \"" + name + "\" has no variable \"" + variable + "\""); //Throw Error
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case "int":
						Value intValue = new Value("int", expression.line, expression.character);
						intValue.vInt = cube.intVars[variable];
						return new Result(intValue, store);
					case "double":
						Value doubleValue = new Value("double", expression.line, expression.character);
						doubleValue.vdouble = cube.doubleVars[variable];
						return new Result(doubleValue, store);
					case "string":
						Value stringValue = new Value("string", expression.line, expression.character);
						stringValue.vString= cube.stringVars[variable];
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool", expression.line, expression.character);
						boolValue.vBool = cube.boolVars[variable];
						return new Result(boolValue, store);
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\""); //Throw Error
				}
			} else {
				return expressionError(expression, store, "ig \"" + name + "\" has no variable \"" + variable + "\""); //Throw Error
			}
		} else {
			return expressionError(expression, store, "ig \"" + name + "\" does not exist in the current context"); //Throw Error	
		}
	}

	Result interpSetIgVariable(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) 
	{
		string name = expression.eSetIgName;
		string variable = expression.eSetIgVariable;
		if (tokenEnv.ContainsKey(name)) {
			Token token = tokenEnv[name];
			Result nv_result = interpret(expression.eSetIgValue, env, store, ref tokenEnv, ref cubeEnv);
			if (token.variables.ContainsKey(variable)) {
				switch(token.variables[variable]) {
					case "int":
						if (nv_result.value.valueType == "int") {
							token.intVars[variable] = nv_result.value.vInt;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected int"); //Throw Error
						}
					case "double":
						if (nv_result.value.valueType == "double") {
							token.doubleVars[variable] = nv_result.value.vdouble;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected double"); //Throw Error
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							token.stringVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected string"); //Throw Error
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							token.boolVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected bool"); //Throw Error
						}
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\""); //Throw Error
				}
			} else {
				switch(nv_result.value.valueType) {
					case "error":
						return resultError(nv_result, "Expected bool"); //Throw Error
					case "int":
						token.intVars.Add(variable, nv_result.value.vInt);
						return nv_result;
					case "double":
						token.doubleVars.Add(variable, nv_result.value.vdouble);
						return nv_result;
					case "string":
						token.stringVars.Add(variable, nv_result.value.vString);
						return nv_result;
					case "bool":
						token.boolVars.Add(variable, nv_result.value.vBool);
						return nv_result;
					default:
						return resultError(nv_result, "Expected int, double, string or bool"); //Throw Error
				}
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			Result nv_result = interpret(expression.eSetIgValue, env, store, ref tokenEnv, ref cubeEnv);
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case "int":
						if (nv_result.value.valueType == "int") {
							cube.intVars[variable] = nv_result.value.vInt;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected int"); //Throw Error
						}
					case "double":
						if (nv_result.value.valueType == "double") {
							cube.doubleVars[variable] = nv_result.value.vdouble;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected double"); //Throw Error
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							cube.stringVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected string"); //Throw Error
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							cube.boolVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected bool"); //Throw Error
						}
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\""); //Throw Error	
				}
			} else {
				switch(nv_result.value.valueType) {
					case "error":
						return resultError(nv_result, "Expected bool"); //Throw Error
					case "int":
						cube.intVars.Add(variable, nv_result.value.vInt);
						return nv_result;
					case "double":
						cube.doubleVars.Add(variable, nv_result.value.vdouble);
						return nv_result;
					case "string":
						cube.stringVars.Add(variable, nv_result.value.vString);
						return nv_result;
					case "bool":
						cube.boolVars.Add(variable, nv_result.value.vBool);
						return nv_result;
					default:
						return resultError(nv_result, "Expected int, double, string or bool"); //Throw Error
				}
			}
		} else {
			return expressionError(expression, store, "ig \"" + name + "\" does not exist in the current context"); //Throw Error	
		}
	}

	Result interpLet(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) {
		return expressionError(expression, store, "Unexpected let expression outside of programming block"); //Throw Error
	}

	Result interpReturn(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) {
		return expressionError(expression, store, "Unexpected return expression outside of programming block"); //Throw Error
	}

	Result interpBuiltinFunc(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) {
		return builtInFunctions[expression.eBuiltinFuncId](expression, env, store, tokenEnv, cubeEnv);
	}
}

// ====================================================================================================================
// ===================================================== Data Types ===================================================
// ====================================================================================================================
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

	public int line;
	public int character;

	public string errorMessage; //type=0

	public int vInt; // type=1
	public double vdouble; // type=2
	public string vString; // type=3
	public bool vBool; // type=4

	public List<string> vFunParams; // type=5
	public Expression vFunBody;
	public Dictionary<String, String> vFunEnviroment;	

	public List<Value> vList; //type=5

	public Value() {
	}

	public Value(string vType) {
		valueType = vType;
	}

	public Value(string vType, int l, int c) {
		valueType = vType;
		line = l;
		character = c;
	}
}

public class Expression {

	public int line;
	public int character;

	public string expressionType;

	public string eErrorMessage; // type=0

	public int eInt; // type=1
	public double edouble; // type=2
	public string eString; // type=3
	public bool eBool; // type=4
	public List<Expression> eList; // type=5

	public String eOperatorOp; // type=6
	public Expression eOperatorLeft;
	public Expression eOperatorRight;

	public string eTriOperatorOp; // type=7
	public Expression eTriOperatorTarget;
	public Expression eTriOperatorLeft;
	public Expression eTriOperatorRight;

	public Expression eIfCond; // type=8
	public Expression eIfConsq;
	public Expression eIfAlter;

	public List<string> eLamParams;
	public Expression eLamBody;

	public Expression eAppFunc; // type=10
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

	public string eBuiltinFuncId;
	public List<Expression> eBuiltinFuncArguments;

	public Expression eReturn;

	//Sugar

	public Expression eForIter; //type=102
	public string eForVariable;
	public Expression eForBody;

	public string eFuncId;
	public List<string> eFuncParams;
	public Expression eFuncBody;

	public string eEvalId;
	public List<Expression> eEvalArguments; 

	// constructors

	public Expression(string etype) {
		expressionType = etype;
	}

	public Expression(string etype, int l, int c) {
		expressionType = etype;
		line = l;
		character = c;
	}
}

public class Atom {
	public string atomType;
	public string value;

	public int line;
	public int character;

	public Atom(int l, int c) {
		line = l;
		character = c;
	}

	public Atom(string at, string v, int l, int c) {
		atomType = at;
		value = v;
		line = l;
		character = c;
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

	public ParseResult() {
	}
}
