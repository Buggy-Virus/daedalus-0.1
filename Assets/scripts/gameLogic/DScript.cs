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

	public Value evaluate(
		string input, 
		Dictionary<string, Token> tokenEnv, 
		Dictionary<string, Cube> cubeEnv
	) 
	{
		return interpret(desugar(parse(input)));
	}

	public Value evaluateSelfToken(
		string input, 
		Token self,
		Dictionary<string, Token> tokenEnv, 
		Dictionary<string, Cube> cubeEnv
	) 
	{
		tokenEnv.Add("self", self);

		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			tokenEnv,
			cubeEnv
		);
	}

	public Value evaluateSelfCube(
		string input, 
		Cube self,
		Dictionary<string, Token> tokenEnv, 
		Dictionary<string, Cube> cubeEnv
	) 
	{
		cubeEnv.Add("self", self);

		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			tokenEnv,
			cubeEnv
		);
	}

	public Value evaluateSelfTokenTargetToken(
		string input, 
		Token self, 
		Token target,
		Dictionary<string, Token> tokenEnv, 
		Dictionary<string, Cube> cubeEnv
	) 
	{
		tokenEnv.Add("self", self);
		tokenEnv.Add("target", target);

		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			tokenEnv,
			cubeEnv
		);
	}

	public Value evaluateSelfTokenTargetCube(
		string input, 
		Token self, 
		Cube cube,
		Dictionary<string, Token> tokenEnv, 
		Dictionary<string, Cube> cubeEnv
	) 
	{
		tokenEnv.Add("self", self);
		cubeEnv.Add("target", target);
		
		return interpret(
			desugar(parse(input)), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			tokenEnv,
			cubeEnv
		);
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
		Expression error = new Expression("Error", atomList[pos].line, atomList[pos].character);
		error.eErrorMessage = message + ", got: (" + atomList[pos].atomType + ",\"" + atomList[pos].value + "\")";
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
		Expression lastExpression;
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
					Expression floatExpression = new Expression("e-float", curAtom.line, curAtom.character);
					floatExpression.eFloat = float.Parse(curAtom.value);
					returnExpression = floatExpression;
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

	ParseResult parseFunction(List<Atom> atomList, int pos) {
		Expression funcExpression = new Expression("e-func", atomList[pos].line, atomList[pos].character - 1);

		if (atomList[pos].atomType == "identifier", atomList[pos].line, atomList[pos].character) {
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
				funcExpression.eFuncArguments = argumentList;
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

				Expression appExpression = new Expression("e-app", atomList[pos].line, atomList[pos].character - 1);
				appExpression.eAppFunc = identifierName;
				
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
				appExpression.eAppArguments = argumentList;

				return new ParseResult(appExpression, pos);
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
				return interpInt(expression, env, store);
			case "e-float": //e-float
				return interpFloat(expression, env, store);
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
			case "e-let": //e-let
				return interpretLet(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-id": // e-id
				return interpId(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-ig-var": //e-ig-variable
				return interpIgVariable(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-set-ig-var": //e-set-ig-variable
				return interpSetIgVariable(expression, env, store, ref tokenEnv, ref cubeEnv);
			case "e-return":
				return interpretReturn(expression, env, store, ref tokenEnv, ref cubeEnv);
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
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + message + ", from: ("
		switch(error.valueType) {
			case "int": //e-int
				errorValue.errorMessage += error.ValueType + ",\"" + error.vInt.ToString() + "\")";
				break;
			case "float": //e-float
				errorValue.errorMessage += error.ValueType + ",\"" + error.vFloat.ToString() + "\")";
				break;
			case "string": //e-string
				errorValue.errorMessage += error.ValueType + ",\"" + error.vString + "\")";
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
		return new Result(errorValue, errorResult.store); //Throw Error	
	}

	Result expressionError(Expression error, Dictionary<string, Value> store, string message) {
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + expected + ", from: ("
		switch(error.valueType) {
			case "int": //e-int
				errorValue.errorMessage += error.ValueType + ",\"" + error.vInt.ToString() + "\")";
				break;
			case "float": //e-float
				errorValue.errorMessage += error.ValueType + ",\"" + error.vFloat.ToString() + "\")";
				break;
			case "string": //e-string
				errorValue.errorMessage += error.ValueType + ",\"" + error.vString + "\")";
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

	Result interpFloat(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("float", expression.line, expression.character);
		returnValue.vFloat = expression.eFloat;
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
		// Result last_result = new Result(new Value(), store);
		foreach (Expression expression in expression.eList) {
			Result last_result = interpret(expression, env, last_result.store, ref tokenEnv, ref cubeEnv);
			returnValue.vList.Add(last_result.value);
		}
		return new Result(returnValue, last_result.store);
	}

	// ================================= Interpret Operator Functions ==================================================
	// ================================= Interpret Operator Functions ==================================================
	// ================================= Interpret Operator Functions ==================================================
	
	Result interpOperator(
		Expression expression
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
				return interpOpMath(expression, floatSubtract, intSubtract, env, store, ref tokenEnv, ref cubeEnv);
			case "*":
				return interpOpMath(expression, floatMultiply, intMultiply, env, store, ref tokenEnv, ref cubeEnv);
			case "/":
				return interpOpMath(expression, floatDivide, intDivide, env, store, ref tokenEnv, ref cubeEnv);
			case "**":
				return interpOpMath(expression, floatExponent, intExponent, env, store, ref tokenEnv, ref cubeEnv);
			case "%":
				return interpOpMath(expression, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "==":
				return interpOpEqual(expression, env, store, ref tokenEnv, ref cubeEnv);
			case ">":
				return interpOpCompare(expression, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "<":
				return interpOpCompare(expression, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case ">=":
				return interpOpCompare(expression, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "<=":
				return interpOpCompare(expression, floatModulo, intModulo, env, store, ref tokenEnv, ref cubeEnv);
			case "&&":
				return interpOpLogic(expression, boolAnd, env, store, ref tokenEnv, ref cubeEnv);
			case "||":
				return interpOpLogic(expression, boolOr, env, store, ref tokenEnv, ref cubeEnv);
			case "!":
				return interpOpLogic(expression, boolNot, env, store, ref tokenEnv, ref cubeEnv);
			case "[]": // Index
				return interpOpIndex(expression, listIndexValue, env, store, ref tokenEnv, ref cubeEnv);
			default:
				return expressionError(expression, store, "Unknown operator") //Throw Error	
		}
	}

	Result interpOpMath(
		Expression expression,
		Func<float, float, float> floatFunc,
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
			case("float"):
				Result r_result = interpret(expression.eOperatorReft, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("float", expression.line, expression.character);
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
						return resultError(r_result, "Expected int or float") //Throw Error	
				}
				break;
			default:
				return resultError(l_result, "Expected int or float") //Throw Error	
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
			case("float"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("float", expression.line, expression.character);
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
						return resultError(r_result, "Expected int or float") //Throw Error	
				}
				break;
			case("string"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("string"):
						Value returnValue = new Value("string", expression.line, expression.character);
						returnValue.vString = l_result.value.vString + r_result.value.vString;
						return new Result(returnValue, r_result.store);	
					default:
						return resultError(r_result, "Expected string") //Throw Error
				}
			case("list"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("vList"):
						Value returnValue = new Value("list", expression.line, expression.character);
						returnValue.vList = l_result.value.vList.Concat(r_result.value.vList).ToList();
						return new Result(returnValue, r_result.store);
					default:
						return resultError(r_result, "list") //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int, float, string, or list") //Throw Error	
		}
	}

	Result interpOpEqual(
		Expression expression
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
			case("float"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("bool", expression.line, expression.character);
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
						return resultError(r_result, "Expected int or float") //Throw Error	
				}
				break;
			case("string"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("string"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vString == r_result.value.vString;
						return new Result(returnValue, r_result.store);	
					default:
						return resultError(r_result, "Expected string") //Throw Error
				}
			case("bool"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("bool"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vBool == r_result.value.vBool;
						return new Result(returnValue, r_result.store);	
					default:
						return resultError(r_result, "Expected bool") //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int, float, string, or bool") //Throw Error
		}
	}

	Result interpOpCompare(
		Expression expression,
		Func<bool, float, float> floatFunc,
		Func<bool, int, int> intFunc,
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
			case("float"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("float"):
						Value returnValue = new Value("bool", expression.line, expression.character);
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
						return resultError(r_result, "Expected int or float") //Throw Error
				}
				break;
			default:
				return resultError(r_result, "Expected int or float") //Throw Error
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
				return resultError(r_result, "Expected bool") //Throw Error
			}
		} else {
			return resultError(r_result, "Expected bool") //Throw Error
		}
	}

	Value boolNot(bool left, bool right) {
		return !left;
	}

	Value boolAnd(bool left, bool right) {
		return left && right;
	}

	Value boolOr(bool left, bool right) {
		return left || right;
	}

	Result interpOpIndex(
		Expression expression 
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
						if (Math.Abs(r_result.value.vFloat) < l_result.value.vString.Length) {
							Value returnValue = new Value("string", expression.line, expression.character);
							returnValue.vString = leftValue.vString[rightValue.vInt].ToString();
							return new Result(returnValue, r_result.store);
						} else {
							return resultError(r_result, "Index out of range") //Throw Error
						}
					default:
						return resultError(r_result, "Expected int") //Throw Error
				}
			case("list"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case("int"):
						if (Math.Abs(r_result.value.vFloat) < l_result.value.vList.Count) {
							return new Result(leftValue.vList[rightValue.vInt], r_result.store);
						} else {
							return resultError(r_result, "Index out of range") //Throw Error	
						}
					default:
						return resultError(r_result, "Expected int") //Throw Error
				}
			default:
				return resultError(r_result, "Expected list or string") //Throw Error	
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
				return interpOpSub(expression, "list", "int", "int", env, store, ref tokenEnv, ref cubeEnv);
			default:
				return expressionError(expression, store, "Unknown operator") //Throw Error	

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
		Result l_result =  interpret((expression.eTriOperatorLeft, env, store, ref tokenEnv, ref cubeEnv);
		switch(l_result.value.valueType) {
			case "int":
				Result r_result =  interpret((expression.eTriOperatorRight, env, l_result.store, ref tokenEnv, ref cubeEnv);
				switch(r_result.value.valueType) {
					case "int":
							Result t_result = interpret((expression.eTriOperatorTarget, env, r_result.store, ref tokenEnv, ref cubeEnv);
							switch(t_result.value.valueType) {
								case "string":
									Value returnValue = Value("string", expression.line, expression.character);
									int idx = l_result.value.vInt;
									int num = r_result.value.vInt - idx;
									returnValue.vString = t_result.value.vString.Substring(idx, num);
									return new Result(returnValue, t_result.store);
								case "list":	
									Value returnValue = Value("list", expression.line, expression.character);
									int idx = l_result.value.vInt;
									int num = r_result.value.vInt - idx;
									returnValue.vList = t_result.value.vList.GetRange(idx, num);
									return new Result(returnValue, t_result.store);
								default:
									return resultError(r_result, "Expected list or string") //Throw Error	
							}				
					default:
						return resultError(r_result, "Expected int") //Throw Error	
				}
			default:
				return resultError(r_result, "Expected int") //Throw Error
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
			return resultError(cond_result, "Expected bool") //Throw Error
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
				return expressionError(expression, store, expression.eAppFunc + " expects " + func_result.value.vFunParams.Count.ToString() + " arguments, got " + args.Count.ToString()) //Throw Error		
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
			return resultError(func_result, "Expected function") //Throw Error
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
		if (env.ContainsKey()) {
			string pointer = env[name];
			if (store.ContainsKey(pointer)) {
				Result newValue_result = interpret(expression.eSetValue, env, store, ref tokenEnv, ref cubeEnv);
				newValue_result.store[pointer] = newValue_result.value;
				return new Result(newValue_result.value, newValue_result.store);
			} else {
				return expressionError(expression, store, "Unset variable" + name) //Throw Error
			}
		} else {
			return expressionError(expression, store, "Unbound variable" + name) //Throw Error
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
				return interpWhile(expression.eWhileCond, expression.eWhileBody, body_result.value, true, env, body_result.store, ref tokenEnv, ref cubeEnv);
			} else if (useLast) {
				return new Result(lastValue, cond_result.store);
			} else {
				return cond_result;
			}
		} else {
			return resultError(func_result, "Expected bool") //Throw Error
		}
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
				return expressionError(expression, store, "Unset variable" + name) //Throw Error
			}
		} else {
			return expressionError(expression, store, "Unbound variable" + name) //Throw Error
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
					case "float":
						Value floatValue = new Value("float", expression.line, expression.character);
						floatValue.vFloat = token.floatVars[variable];
						return new Result(floatValue, store);
					case "string":
						Value stringValue = new Value("string", expression.line, expression.character);
						stringValue.vString= token.stringVars[variable];
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool", expression.line, expression.character);
						boolValue.vBool = token.boolVars[variable];
						return new Result(boolValue, store);
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\"") //Throw Error
				}
			} else {
				return expressionError(expression, store, "ig \"" + name + "\" has no variable \"" + variable + "\"") //Throw Error
			}
		} else if (cubeEnv.ContainsKey(name)) {
			Cube cube = cubeEnv[name];
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case "int":
						Value intValue = new Value("int", expression.line, expression.character);
						intValue.vInt = cube.intVars[variable];
						return new Result(intValue, store);
					case "float":
						Value floatValue = new Value("float", expression.line, expression.character);
						floatValue.vFloat = cube.floatVars[variable];
						return new Result(floatValue, store);
					case "string":
						Value stringValue = new Value("string", expression.line, expression.character);
						stringValue.vString= cube.stringVars[variable];
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool", expression.line, expression.character);
						boolValue.vBool = cube.boolVars[variable];
						return new Result(boolValue, store);
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\"") //Throw Error
				}
			} else {
				return expressionError(expression, store, "ig \"" + name + "\" has no variable \"" + variable + "\"") //Throw Error
			}
		} else {
			return expressionError(expression, store, "ig \"" + name + "\" does not exist in the current context") //Throw Error	
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
							return resultError(nv_result, "Expected int") //Throw Error
						}
					case "float":
						if (nv_result.value.valueType == "float") {
							token.floatVars[variable] = nv_result.value.vFloat;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected float") //Throw Error
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							token.stringVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected string") //Throw Error
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							token.boolVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected bool") //Throw Error
						}
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\"") //Throw Error
				}
			} else {
				switch(nv_result.value.valueType) {
					case "error":
						return resultError(nv_result, "Expected bool") //Throw Error
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
						return resultError(nv_result, "Expected int, float, string or bool") //Throw Error
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
							return resultError(nv_result, "Expected int") //Throw Error
						}
					case "float":
						if (nv_result.value.valueType == "float") {
							cube.floatVars[variable] = nv_result.value.vFloat;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected float") //Throw Error
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							cube.stringVars[variable] = nv_result.value.vString;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected string") //Throw Error
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							cube.boolVars[variable] = nv_result.value.vBool;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected bool") //Throw Error
						}
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\"") //Throw Error	
				}
			} else {
				switch(nv_result.value.valueType) {
					case "error":
						return resultError(nv_result, "Expected bool") //Throw Error
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
						return resultError(nv_result, "Expected int, float, string or bool") //Throw Error
				}
			}
		} else {
			return expressionError(expression, store, "ig \"" + name + "\" does not exist in the current context") //Throw Error	
		}
	}

	Result interpLet(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	) {
		return expressionError(expression, store, "Unexpected let expression outside of programming block") //Throw Error
	}

	Result interpReturn(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref Dictionary<string, Token> tokenEnv, 
		ref Dictionary<string, Cube> cubeEnv
	)  {
		return expressionError(expression, store, "Unexpected return expression outside of programming block") //Throw Error
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

	public Value(string vType, l, c) {
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
	public float eFloat; // type=2
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

	public string eAppFunc; // type=10
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

	public Expression eReturn;

	//Sugar

	public List<Expression> eForIter; //type=102
	public string eForVariable;
	public Expression eForBody;

	public string eFuncId;
	public List<string> eFuncArguments;
	public Expression eFuncBody;

	public Expression(string etype) {
		expressionType = etype;
	}

	public Expression(string etype, int l, int c) {
		expressionType = etype;
		line = l;
		character = c;
	}
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
