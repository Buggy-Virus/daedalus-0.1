using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaedScript {

	static Dictionary<string, BuiltinFunction> builtInFunctions = new Dictionary<string, BuiltinFunction>() {
		{"ToString", ToString},
		{"ToInt", ToInt},
		{"ToDouble", ToDouble},
		{"ToBool", ToBool},
		{"Abs", Abs},
		{"Max", Max},
		{"Min", Min},
		{"Floor", Floor},
		{"Ceil", Ceil},
		{"Factorial", Factorial},
		{"Binomial", Binomial},
		{"Log", Log},
		{"Logn", Logn},
		{"Round", Round},
		{"Sum", Sum},
		{"Len", Len},
		{"Range", Range},
		{"Append", Append},
		{"Insert", Insert},
		{"RandInt", RandInt},
		{"Rand", Rand},
		{"Contains", Contains},
		{"IndexOf", IndexOf}
	};

	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================
	// ================================= Evaluate Functions ======================================================================

	public static Value evaluate(
		string input, 
		ref GameEnv gameEnv
	) 
	{
		List<Atom> atomList = tokenize(input);

		// Debug.Log("===== NEW EVALUATION =====");

		// foreach (Atom atom in atomList) {
		// 	Debug.Log(atom.atomType + ", " + atom.value);
		// }		
		
		Expression expression = parse(atomList);

		Expression desugar_expression = desugar(expression);

		// Debug.Log(desugar_expression.eDo.Count);
		// Debug.Log(desugar_expression.eDo[0].eInt);	

		//return new Value();

		// Debug.Log("Expression Info");
		// Debug.Log("expression Count: " + expression.eDo.Count.ToString());
		// Debug.Log(expression.eDo[0].expressionType);	
		// // Debug.Log(expression.eDo[0].eOperatorLeft.eInt);
		// Debug.Log(expression.eDo[0].eOperatorOp);	
		// Debug.Log(expression.eDo[0].eErrorMessage);	
		// // Debug.Log(expression.eDo[0].eOperatorRight.eInt);
		// Debug.Log("End of Expression Info");

		return interpret(
			desugar_expression, 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref gameEnv
		).value;
	}

	public static Value evaluateSelfToken(
		string input, 
		ref GameObject self,
		ref GameEnv gameEnv
	) 
	{
		gameEnv.tokenDict.Add("self", self);

		return interpret(
			desugar(parse(tokenize(input))), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref gameEnv
		).value;
	}

	public static Value evaluateSelfCube(
		string input, 
		ref Cube self,
		ref GameEnv gameEnv
	) 
	{
		gameEnv.cubeDict.Add("self", self);

		return interpret(
			desugar(parse(tokenize(input))), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref gameEnv
		).value;
	}

	public static Value evaluateSelfTokenTargetToken(
		string input, 
		ref GameObject self, 
		ref GameObject target,
		ref GameEnv gameEnv
	) 
	{
		gameEnv.tokenDict.Add("self", self);
		gameEnv.tokenDict.Add("target", target);

		return interpret(
			desugar(parse(tokenize(input))), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref gameEnv
		).value;
	}

	public static Value evaluateSelfTokenTargetCube(
		string input, 
		ref GameObject self, 
		ref Cube target,
		ref GameEnv gameEnv
	) 
	{
		gameEnv.tokenDict.Add("self", self);
		gameEnv.cubeDict.Add("target", target);
		
		return interpret(
			desugar(parse(tokenize(input))), 
			new Dictionary<string, string>(),
			new Dictionary<string, Value>(),
			ref gameEnv
		).value;
	}

	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================
	// ================================= Tokenizer Functions =====================================================================

	static List<Atom> tokenize(string input) {
		// Debug.Log("tokenize called");
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
								} else {
									atomList.Add(curAtom);
									buildingAtom = false;
								}
								break;
							case "!":
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
									atomList.Add(new Atom("error", "expected \"|\", got \"" + curChar.ToString() + "\"", line, character));
									buildingAtom = false;
								}
								break;
							case "&":
								if (curChar == '&') {
									curAtom.value += curChar.ToString();
								} else {
									atomList.Add(new Atom("error", "expected \"&\", got \"" + curChar.ToString() + "\"", line, character));
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
					curAtom = new Atom("punctuation", curChar.ToString(), line, character);
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
					curAtom = new Atom("error", "\"" + curChar.ToString() + "\"", line, character);
					atomList.Add(curAtom);
				} 
			} else if (!buildingAtom && buildingString) {
				buildingString = false;
			}
			character += 1;
		}
		return atomList;
	}

	static bool isPunctuation (char c) {
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

	static bool isWhiteSpace (char c) {
		switch(c) {
			case '\t':
			case '\n':
			case ' ':
				return true;
			default:
				return false;
		}
	}

	static bool isOperator (char c) {
		switch(c) {
			case '+':
			case '=':
			case '-':
			case '/':
			case '*':
			case '>':
			case '<':
			case '!':
			case '%':
			case '|':
			case '&':
				return true;
			default:
				return false;
		}
	}

	static bool isString (char c) {
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
	static bool atomEquals(Atom atom, string targetType, string targetValue) {
		return atom.atomType == targetType && atom.value == targetValue;
	}

	static ParseResult parseError(Atom atom, int pos, string message) {
		Expression error = new Expression("error", atom.line, atom.character);
		error.eErrorMessage = "ParseError:" + message + ", got: (" + atom.atomType + ",\"" + atom.value + "\")";
		return new ParseResult(error, pos);
	}

	// ================================= Actual Parser ===============================================================

	static Expression parse(List<Atom> atomList) {
		// Debug.Log("parse called");

		if (atomList.Count == 0) {
			Expression errorExpression = new Expression("error", 0, 0);
			errorExpression.eErrorMessage = "Got empty atomList";
			return errorExpression;
		}

		return parseDo(atomList, 0, false).expression;
		 
	}

	static ParseResult parseDo(List<Atom> atomList, int pos, bool bookended) {
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

	static ParseResult parseSingle(List<Atom> atomList, int pos, bool bookended) {
		// Debug.Log("parseSingle");
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
				} else if (curAtom.atomType == "error") {
					return parseError(atomList[pos], pos, "AtomError"); // throw error
				} else {
					ParseResult firstResult = parseSingleHelper(atomList, pos);
					lastExpression = firstResult.expression;
					seenExpression = true;
					pos = firstResult.position + 1;
				}
			} else if (atomEquals(atomList[pos], "punctuation", ";")) {
				return new ParseResult(lastExpression, pos);
			} else {
				switch(curAtom.atomType) {
					case "operator":
						Expression opExpression = new Expression("e-op", curAtom.line, curAtom.character);
						opExpression.eOperatorLeft = lastExpression;
						switch(curAtom.value) {
							case "+":
								opExpression.eOperatorOp = "+";
								break;
							case "-":
								opExpression.eOperatorOp = "-";
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
							case "%":
								opExpression.eOperatorOp = "%";
								break;
							case "&&":
								opExpression.eOperatorOp = "&&";
								break;
							case "||":
								opExpression.eOperatorOp = "||";
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
						pos = secondResult.position + 1;
						// Debug.Log("Second Result Position: " + pos.ToString());
						break;
					case "punctuation":
						switch(curAtom.value) {
							case "[":
								ParseResult indexResult = parseIndex(atomList, pos + 1, lastExpression);
								lastExpression = indexResult.expression;
								pos = indexResult.position + 1;
								break;
							case ";":
								if (!bookended) {
									// Debug.Log("out of parseSingle through ;");
									return new ParseResult(lastExpression, pos);
								} else {
									return parseError(atomList[pos], pos, "Expected \")\""); // throw error
								}
							case ")":
								if (bookended) {
									return new ParseResult(lastExpression, pos);
								} else {
									return new ParseResult(lastExpression, pos - 1);
								}
							default:
								if (!bookended) {
									// Debug.Log("out of parseSingle through punc");
									return new ParseResult(lastExpression, pos - 1);
								} else {
									return parseError(atomList[pos], pos, "Expected operator or punctuation"); // throw error
								}
						}
						break;
					case "error":
						return parseError(atomList[pos], pos, "AtomError"); // throw error
					default:
						if (!bookended) {
							// Debug.Log("out of parseSingle through default");
							return new ParseResult(lastExpression, pos - 1);
						} else {
							return parseError(atomList[pos], pos, "Expected operator or punctuation"); // throw error
						}
				}
			}
		}
		if (seenExpression && atomEquals(atomList[pos - 1], "punctuation", ";")) {
			return new ParseResult(lastExpression, pos - 1);
		}
		return parseError(atomList[pos - 1], pos, "No expression returned from parseSingle"); // throw error
	}

	static ParseResult parseSingleHelper(List<Atom> atomList, int pos) {
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
					doubleExpression.eDouble = double.Parse(curAtom.value);
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

	static ParseResult parseIndex(List<Atom> atomList, int pos, Expression lastExpression) {
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

	static ParseResult parseList(List<Atom> atomList, int pos) {
		Expression listExpression = new Expression("e-list", atomList[pos].line, atomList[pos].character - 1);
		listExpression.eList = new List<Expression>();

		while (!atomEquals(atomList[pos], "punctuation", "]")) {

			ParseResult argumentResult = parseSingle(atomList, pos, false);
			listExpression.eList.Add(argumentResult.expression);
			pos = argumentResult.position += 1;

			if (atomEquals(atomList[pos], "punctuation", ",")) {
				pos += 1;
			} else if (atomEquals(atomList[pos], "punctuation", "]")) {
				break;
			} else {
				return parseError(atomList[pos], pos, "Expected \",\" or \"]\""); // throw error
			}

			if (pos == atomList.Count) {
				return parseError(atomList[pos], pos, "Expected \",\" or \"]\""); // throw error
			}
		}

		return new ParseResult(listExpression, pos);
	}

	static ParseResult parseFunction(List<Atom> atomList, int pos) {
		Expression funcExpression = new Expression("e-func", atomList[pos].line, atomList[pos].character - 1);

		if (atomList[pos].atomType == "identifier") {
			funcExpression.eFuncId = atomList[pos].value;
			pos += 1;

			if (atomEquals(atomList[pos], "punctuation", "(")) {
				pos += 1;

				funcExpression.eFuncParams = new List<string>();
				while (!atomEquals(atomList[pos], "punctuation", ")")) {
					if (atomList[pos].atomType == "identifier") {
						funcExpression.eFuncParams.Add(atomList[pos].value);
						pos += 1;

						if (atomEquals(atomList[pos], "punctuation", ",")) {
							pos += 1;
						} else if (atomEquals(atomList[pos], "punctuation", ")")) {
							break;
						} else {
							return parseError(atomList[pos], pos, "Expected \",\" or \")\""); // throw error
						}

						if (pos == atomList.Count) {
							return parseError(atomList[pos], pos, "Expected \",\" or \")\""); // throw error
						}
					}
				}
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

	static ParseResult parseNot(List<Atom> atomList, int pos) {
		Expression notExpression = new Expression("e-op", atomList[pos].line, atomList[pos].character - 1);

		ParseResult notResult = parseSingle(atomList, pos, false);
		notExpression.eOperatorLeft = notResult.expression;
		notExpression.eOperatorRight = new Expression("e-bool", atomList[pos].line, atomList[pos].character);
		notExpression.eOperatorOp = "!";
		pos = notResult.position;

		return new ParseResult(notExpression, pos);
	}

	static ParseResult parseIf(List<Atom> atomList, int pos) {
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

	static ParseResult parseLambda(List<Atom> atomList, int pos) {
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

					if (pos == atomList.Count) {
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

	static ParseResult parseLet(List<Atom> atomList, int pos) {
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

	static ParseResult parseWhile(List<Atom> atomList, int pos) {
		Expression whileExpression = new Expression("e-while", atomList[pos].line, atomList[pos].character - 1);

		ParseResult condResult = parseSingle(atomList, pos, true);
		whileExpression.eWhileCond = condResult.expression;
		pos = condResult.position + 1;

		ParseResult bodyResult = parseDo(atomList, pos, true);
		whileExpression.eLamBody = bodyResult.expression;
		pos = bodyResult.position;

		return new ParseResult(whileExpression, pos);
	}

	static ParseResult parseFor(List<Atom> atomList, int pos) {
		Expression forExpression = new Expression("e-foreach", atomList[pos].line, atomList[pos].character);

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

	static ParseResult parseIdentifier(List<Atom> atomList, int pos) {
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

					if (pos == atomList.Count) {
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

	static ParseResult parseIg(List<Atom> atomList, int pos) {
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

						Expression igVarExpression = new Expression("e-set-var", atomList[pos].line, atomList[pos].character);
						ParseResult valueResult = parseSingle(atomList, pos, false);
						igVarExpression.eSetIgName = igName;
						igVarExpression.eSetIgVariable = igVariable;
						igVarExpression.eSetIgValue = valueResult.expression;
						pos = valueResult.position;

						return new ParseResult(igVarExpression, pos);
					} else {
						Expression igVarExpression = new Expression("e-get-var", atomList[pos].line, atomList[pos].character - 1);
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

	static Expression desugar(Expression expression) {
		// Debug.Log("desugar called");
		switch(expression.expressionType) {
			case "e-list": // e-list
				expression.eList = expression.eList.Select(desugar).ToList();
				return expression;
			case "e-op": //e-op
				expression.eOperatorLeft = desugar(expression.eOperatorLeft);
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
			case "e-set-var": //e-set-variable
				expression.eSetIgValue = desugar(expression.eSetIgValue);
				return expression;
			case "e-return":
				expression.eReturn = desugar(expression.eReturn);
				return expression;
			case "e-for":
				expression.eForIter = desugar(expression.eForIter);
				expression.eForBody = desugar(expression.eForBody);
				return expression;
			case "e-eval":
				return desugarEval(expression);
			case "e-func":
				return desugarFunc(expression);
			default:
				return expression;
		}
	}

	static Expression desugarEval(Expression expression) {
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

	static Expression desugarFunc(Expression expression) {
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

	static Result interpret(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store,
		ref GameEnv gameEnv
		) 
	{
		// Debug.Log("interpret called");
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
				return interpList(expression, env, store, ref gameEnv);
			case "e-op": //e-op
				return interpOperator(expression, env, store, ref gameEnv);
			case "e-triOp": //e-triOp
				return interpTriOperator(expression, env, store, ref gameEnv);
			case "e-if": //e-if
				return interpIf(expression, env, store, ref gameEnv);
			case "e-lam": //e-lam
				return interpLam(expression, env, store, ref gameEnv);
			case "e-app": //e-app
				return interpApp(expression, env, store, ref gameEnv);
			case "e-set": //e-set
				return interpSet(expression, env, store, ref gameEnv);
			case "e-do": //e-do
				return interpDo(expression, env, store, ref gameEnv);	
			case "e-while": //e-while
				return interpWhile(expression, new Value(), false, env, store, ref gameEnv);
			case "e-foreach":
				return interpForeach(expression, env, store, ref gameEnv);
			case "e-let": //e-let
				return interpLet(expression, env, store, ref gameEnv);
			case "e-id": // e-id
				return interpId(expression, env, store, ref gameEnv);
			case "e-get-var": //e-get-variable
				return interpGetGameVar(expression, env, store, ref gameEnv);
			case "e-set-var": //e-set-variables
				return interpSetGameVar(expression, env, store, ref gameEnv);
			case "e-builtin-func": //e-set-variable
				return interpBuiltinFunc(expression, env, store, ref gameEnv);
			case "e-return":
				return interpReturn(expression, env, store, ref gameEnv);
			case "error":
				return interpError(expression, env, store, ref gameEnv);
			default:
				return expressionError(expression, store, "Unknown expression type: " + expression.expressionType);
		}
	}

	// ================================= Interpret Error Functions ==================================

	static Result interpError(
		Expression error,  
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + error.eErrorMessage;
		return new Result(errorValue, store); //Throw Error	
	}

	static Result resultError(Result errorResult, string message) {
		Value error = errorResult.value;
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + message + ", from: (";
		switch(error.valueType) {
			case "int": //e-int
				errorValue.errorMessage += error.valueType + ",\"" + error.vInt.ToString() + "\")";
				break;
			case "double": //e-double
				errorValue.errorMessage += error.valueType + ",\"" + error.vDouble.ToString() + "\")";
				break;
			case "string": //e-string
				errorValue.errorMessage += error.valueType + ",\"" + error.vString + "\")";
				break;
			case "bool": //e-bool
				errorValue.errorMessage += error.valueType + ",\"" + error.vBool.ToString() + "\")";
				break;
			case "list": // e-list
				errorValue.errorMessage += error.valueType + ",<listValueObject>)";
				break;
			case "function": //e-op
				errorValue.errorMessage += error.valueType + ",<functionValueObject>)";
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

	static Result expressionError(Expression error, Dictionary<string, Value> store, string message) {
		Value errorValue = new Value("error", error.line, error.character); 
		errorValue.errorMessage = "Error at (" + error.line + "," + error.character + "): " + message + ", from: (";
		switch(error.expressionType) {
			case "e-int": //e-int
				errorValue.errorMessage += error.expressionType + ",\"" + error.eInt.ToString() + "\")";
				break;
			case "e-double": //e-double
				errorValue.errorMessage += error.expressionType + ",\"" + error.eDouble.ToString() + "\")";
				break;
			case "e-string": //e-string
				errorValue.errorMessage += error.expressionType + ",\"" + error.eString + "\")";
				break;
			case "e-bool": //e-bool
				errorValue.errorMessage += error.expressionType + ",\"" + error.eBool.ToString() + "\")";
				break;
			case "e-list": // e-list
				errorValue.errorMessage += error.expressionType + ",<listExpressionObject>)";
				break;
			case "e-op": //e-op
				errorValue.errorMessage += error.expressionType + ",\"" + error.eOperatorOp + "\")";
				break;
			case "e-triOp": //e-triOp
				errorValue.errorMessage += error.expressionType + ",\"" + error.eTriOperatorOp + "\")";
				break;
			case "e-if": //e-if
				errorValue.errorMessage += error.expressionType + ",<ifExpressionObject>)";
				break;
			case "e-lam": //e-lam
				errorValue.errorMessage += error.expressionType + ",<lambdaExpressionObject>)";
				break;
			case "e-app": //e-app
				errorValue.errorMessage += error.expressionType + ",<appExpressionObject>)";
				break;
			case "e-set": //e-set
				errorValue.errorMessage += error.expressionType + ",\"" + error.eSetName + "\")";
				break;
			case "e-do": //e-do
				errorValue.errorMessage += error.expressionType + ",<doExpressionObject>)";
				break;
			case "e-while": //e-while
				errorValue.errorMessage += error.expressionType + ",<whileExpressionObject>)";
				break;
			case "e-foreach":
				errorValue.errorMessage += error.expressionType + ",<forExpressionObject>)";
				break;
			case "e-let": //e-let
				errorValue.errorMessage += error.expressionType + ",\"" + error.eLetName + "\")";
				break;
			case "e-id": // e-id
				errorValue.errorMessage += error.expressionType + ",\"" + error.eId + "\")";
				break;
			case "e-get-var": //e-get-variable
				errorValue.errorMessage += error.expressionType + ",\"" + error.eIgName + "\")";
				break;
			case "e-set-var": //e-set-variable
				errorValue.errorMessage += error.expressionType + ",\"" + error.eSetIgName + "\")";
				break;
			case "e-builtin-func": //e-set-variable
				errorValue.errorMessage += error.expressionType + ",\"" + error.eBuiltinFuncId + "\")";
				break;
			case "e-return":
				errorValue.errorMessage += error.expressionType + ",<returnExpressionObject>)";
				break;
			case "e-error":
				errorValue.errorMessage += error.expressionType + "," + error.eErrorMessage + ")";
				break;
			default:
				errorValue.errorMessage += error.expressionType + ",unknown expression type)";
				break;
		}
		return new Result(errorValue, store); //Throw Error	
	}

	// ================================= Interpret Values Functions =================================

	static Result interpInt(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("int", expression.line, expression.character);
		returnValue.vInt = expression.eInt;
		return new Result(returnValue, store);
	}

	static Result interpdouble(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("double", expression.line, expression.character);
		returnValue.vDouble = expression.eDouble;
		return new Result(returnValue, store);
	}

	static Result interpString(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("string", expression.line, expression.character);
		returnValue.vString = expression.eString;
		return new Result(returnValue, store);
	}

	static Result interpBool(Expression expression, Dictionary<string, string> env, Dictionary<string, Value> store) {
		Value returnValue = new Value("bool", expression.line, expression.character);
		returnValue.vBool = expression.eBool;
		return new Result(returnValue, store);
	}

	static Result interpList(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
		) 
	{
		Value returnValue = new Value("list", expression.line, expression.character);
		returnValue.vList = new List<Value>();
		Result last_result = new Result(new Value(), store);
		foreach (Expression expr in expression.eList) {
			last_result = interpret(expr, env, last_result.store, ref gameEnv);
			if (last_result.value.valueType == "error") {
				return resultError(last_result, "Passed error to list"); //Throw Error
			}
			returnValue.vList.Add(last_result.value);
		}
		return new Result(returnValue, last_result.store);
	}

	// ================================= Interpret Operator Functions ==================================================
	// ================================= Interpret Operator Functions ==================================================
	// ================================= Interpret Operator Functions ==================================================
	
	static Result interpOperator(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
		) 
	{
		switch (expression.eOperatorOp) {
			case "+":
				return interpOpPlus(expression, env, store, ref gameEnv);
			case "-":
				return interpOpMath(expression, doubleSubtract, intSubtract, env, store, ref gameEnv);
			case "*":
				return interpOpMath(expression, doubleMultiply, intMultiply, env, store, ref gameEnv);
			case "/":
				return interpOpMath(expression, doubleDivide, intDivide, env, store, ref gameEnv);
			case "**":
				return interpOpMath(expression, doubleExponent, intExponent, env, store, ref gameEnv);
			case "%":
				return interpOpMath(expression, doubleModulo, intModulo, env, store, ref gameEnv);
			case "==":
				return interpOpEqual(expression, env, store, ref gameEnv);
			case "!=":
				return interpOpNotEqual(expression, env, store, ref gameEnv);
			case ">":
				return interpOpCompare(expression, doubleGT, intGT, env, store, ref gameEnv);
			case "<":
				return interpOpCompare(expression, doubleLT, intLT, env, store, ref gameEnv);
			case ">=":
				return interpOpCompare(expression, doubleGEQ, intGEQ, env, store, ref gameEnv);
			case "<=":
				return interpOpCompare(expression, doubleLEQ, intLEQ, env, store, ref gameEnv);
			case "&&":
				return interpOpLogic(expression, boolAnd, env, store, ref gameEnv);
			case "||":
				return interpOpLogic(expression, boolOr, env, store, ref gameEnv);
			case "!":
				return interpOpLogic(expression, boolNot, env, store, ref gameEnv);
			case "[]": // Index
				return interpOpIndex(expression, env, store, ref gameEnv);
			default:
				return expressionError(expression, store, "Unknown operator"); //Throw Error	
		}
	}

	static Result interpOpMath(
		Expression expression,
		Func<double, double, double> doubleFunc,
		Func<int, int, int> intFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("double", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vDouble = doubleFunc(l_result.value.vDouble, r_result.value.vDouble);
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vDouble = doubleFunc(l_result.value.vDouble, (double)r_result.value.vInt);
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vDouble = doubleFunc((double)l_result.value.vInt, r_result.value.vDouble);
						} else {
							returnValue.valueType = "int";
							returnValue.vInt = intFunc(l_result.value.vInt, r_result.value.vInt);
						}					
						return new Result(returnValue, r_result.store);
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int or double"); //Throw Error	
		}
	}

	static double doubleSubtract(double left, double right) {
		return left - right;
	}

	static int intSubtract(int left, int right) {
		return left - right;
	}

	static double doubleMultiply(double left, double right) {
		return left * right;
	}

	static int intMultiply(int left, int right) {
		return left * right;
	}

	static double doubleDivide(double left, double right) {
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

	static int intDivide(int left, int right) {
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

	static double doubleExponent(double left, double right) {
		return Math.Pow(left, right);
	}

	static int intExponent(int left, int right) {
		return (int)Math.Round(Math.Pow(left, right));
	}

	static double doubleModulo(double left, double right) {
		return left % right;
	}

	static int intModulo(int left, int right) {
		return left % right;
	}

	static Result interpOpPlus(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("double", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vDouble = l_result.value.vDouble + r_result.value.vDouble;
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vDouble = l_result.value.vDouble + (double)r_result.value.vInt;
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vDouble = (double)l_result.value.vInt + r_result.value.vDouble;
						} else {
							returnValue.valueType = "int";
							returnValue.vInt = l_result.value.vInt + r_result.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
			case("string"):
				Result r_result_string = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result_string.value.valueType) {
					case("string"):
						Value returnValue = new Value("string", expression.line, expression.character);
						returnValue.vString = l_result.value.vString + r_result_string.value.vString;
						return new Result(returnValue, r_result_string.store);	
					default:
						return resultError(r_result_string, "Expected string"); //Throw Error
				}
			case("list"):
				Result r_result_list = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result_list.value.valueType) {
					case("list"):
						Value returnValue = new Value("list", expression.line, expression.character);
						returnValue.vList = l_result.value.vList.Concat(r_result_list.value.vList).ToList();
						return new Result(returnValue, r_result_list.store);
					default:
						return resultError(r_result_list, "Expected list"); //Throw Error	
				}
			default:
				return resultError(l_result, "Expected int, double, string, or list"); //Throw Error	
		}
	}

	static Result interpOpEqual(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		// Result r_result = interpret(right, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vBool = l_result.value.vDouble == r_result.value.vDouble;
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vBool = l_result.value.vDouble == (double)r_result.value.vInt;
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vBool = (double)l_result.value.vInt == r_result.value.vDouble;
						} else {
							returnValue.vBool = l_result.value.vInt == r_result.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
			case("string"):
				Result r_result_string = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result_string.value.valueType) {
					case("string"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vString == r_result_string.value.vString;
						return new Result(returnValue, r_result_string.store);	
					default:
						return resultError(r_result_string, "Expected string"); //Throw Error
				}
			case("bool"):
				Result r_result_bool = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
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

	static Result interpOpNotEqual(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		// Result r_result = interpret(right, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vBool = l_result.value.vDouble != r_result.value.vDouble;
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vBool = l_result.value.vDouble != (double)r_result.value.vInt;
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vBool = (double)l_result.value.vInt != r_result.value.vDouble;
						} else {
							returnValue.vBool = l_result.value.vInt != r_result.value.vInt;
						}	
						return new Result(returnValue, r_result.store);				
					default:
						return resultError(r_result, "Expected int or double"); //Throw Error	
				}
			case("string"):
				Result r_result_string = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result_string.value.valueType) {
					case("string"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						returnValue.vBool = l_result.value.vString != r_result_string.value.vString;
						return new Result(returnValue, r_result_string.store);	
					default:
						return resultError(r_result_string, "Expected string"); //Throw Error
				}
			case("bool"):
				Result r_result_bool = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
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

	static Result interpOpCompare(
		Expression expression,
		Func<double, double, bool> doubleFunc,
		Func<int, int, bool> intFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		// Result r_result = interpret(right, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case("int"):
			case("double"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case("int"):
					case("double"):
						Value returnValue = new Value("bool", expression.line, expression.character);
						if (l_result.value.valueType == "double" && r_result.value.valueType == "double") {
							returnValue.vBool = doubleFunc(l_result.value.vDouble, r_result.value.vDouble);
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vBool = doubleFunc(l_result.value.vDouble, (double)r_result.value.vInt);
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vBool = doubleFunc((double)l_result.value.vInt, r_result.value.vDouble);
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

	static bool doubleGT(double left, double right){
		return left > right;
	}

	static bool intGT(int left, int right){
		return left > right;
	}

	static bool doubleLT(double left, double right){
		return left < right;
	}

	static bool intLT(int left, int right){
		return left < right;
	}

	static bool doubleGEQ(double left, double right){
		return left >= right;
	}

	static bool intGEQ(int left, int right){
		return left >= right;
	}

	static bool doubleLEQ(double left, double right){
		return left <= right;
	}

	static bool intLEQ(int left, int right){
		return left <= right;
	}

	static Result interpOpLogic(
		Expression expression,
		Func<bool, bool, bool> boolFunc,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
		) 
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		if (l_result.value.valueType == "bool") {
			Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
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

	static bool boolNot(bool left, bool right) {
		return !left;
	}

	static bool boolAnd(bool left, bool right) {
		return left && right;
	}

	static bool boolOr(bool left, bool right) {
		return left || right;
	}

	static Result interpOpIndex(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		Result l_result = interpret(expression.eOperatorLeft, env, store, ref gameEnv);
		// Result r_result = interpret(right, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case("string"):
				Result r_result = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case("int"):
						if (Math.Abs(r_result.value.vDouble) < l_result.value.vString.Length) {
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
				Result r_result_list = interpret(expression.eOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result_list.value.valueType) {
					case("int"):
						if (Math.Abs(r_result_list.value.vDouble) < l_result.value.vList.Count) {
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

	static Result interpTriOperator(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
		) 
	{
		switch (expression.eTriOperatorOp) {
			case("[:]"):
				return interpOpSub(expression, env, store, ref gameEnv);
			default:
				return expressionError(expression, store, "Unknown operator"); //Throw Error	

		}
	}

	static Result interpOpSub(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
		)
	{
		Result l_result =  interpret(expression.eTriOperatorLeft, env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case "int":
				Result r_result =  interpret(expression.eTriOperatorRight, env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case "int":
							Result t_result = interpret(expression.eTriOperatorTarget, env, r_result.store, ref gameEnv);
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
	static Result interpIf(
		Expression expression, 
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		Result cond_result = interpret(expression.eIfCond, env, store, ref gameEnv);
		if (cond_result.value.valueType == "bool") {
			if (cond_result.value.vBool) {
				return interpret(expression.eIfConsq, env, cond_result.store, ref gameEnv);
			} else {
				return interpret(expression.eIfAlter, env, cond_result.store, ref gameEnv);
			}
		} else {
			return resultError(cond_result, "Expected bool"); //Throw Error
		}
	}

	static Result interpLam(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		Value returnValue = new Value("function", expression.line, expression.character);
		returnValue.vFunParams = expression.eLamParams;
		returnValue.vFunBody = expression.eLamBody;
		returnValue.vFunEnviroment = env;
		return new Result(returnValue, store);
	}

	static Result interpApp(
		Expression expression,
		Dictionary<string, string> env,
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		Result func_result = interpret(expression.eAppFunc, env, store, ref gameEnv);
		if (func_result.value.valueType == "function") {
			if (expression.eAppArguments.Count != func_result.value.vFunParams.Count) {
				return expressionError(expression, store, expression.eAppFunc + " expects " + func_result.value.vFunParams.Count.ToString() + " arguments, got " + func_result.value.vFunParams.Count.ToString()); //Throw Error		
			}

			Dictionary<string, Value> appStore = func_result.store;
			for (int i = 0; i < expression.eAppArguments.Count; i++) {
				Expression arg = expression.eAppArguments[i];
				string param = func_result.value.vFunParams[i];
				Result arg_result = interpret(arg, env, appStore, ref gameEnv);
				string loc = System.Guid.NewGuid().ToString();
				func_result.value.vFunEnviroment.Add(param, loc);
				appStore.Add(loc, arg_result.value);
			}
			return interpret(func_result.value.vFunBody, func_result.value.vFunEnviroment, appStore, ref gameEnv); 
		} else {
			return resultError(func_result, "Expected function"); //Throw Error
		}
	}

	static Result interpSet(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		string name = expression.eSetName;
		if (env.ContainsKey(name)) {
			string pointer = env[name];
			if (store.ContainsKey(pointer)) {
				Result newValue_result = interpret(expression.eSetValue, env, store, ref gameEnv);
				newValue_result.store[pointer] = newValue_result.value;
				return new Result(newValue_result.value, newValue_result.store);
			} else {
				return expressionError(expression, store, "Unset variable" + name); //Throw Error
			}
		} else {
			return expressionError(expression, store, "Unbound variable" + name); //Throw Error
		}
	}

	static Result interpDo(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		Result last_result = new Result(new Value(), store);
		foreach (Expression expr in expression.eDo) {
			// If statement in Do handles define statements. Defines are only relevent in Do
			// The change to the env is only relevent across other Do expressions
			// In the same Do statement
			if (expr.expressionType == "e-let") {
				Result define_result = interpret(expr.eLetValue, env, last_result.store, ref gameEnv);
				string loc = System.Guid.NewGuid().ToString();
				env.Add(expr.eLetName, loc);
				define_result.store.Add(loc, define_result.value);
				last_result = define_result;
			} else if (expr.expressionType == "e-return") {
				return interpret(expr.eReturn, env, last_result.store, ref gameEnv);
			} else {
				last_result = interpret(expr, env, last_result.store, ref gameEnv);
				if (last_result.value.valueType == "error") {
					return last_result;
				}
			}
			
		}
		return last_result;
	}

	static Result interpWhile(
		Expression expression, 
		Value lastValue, 
		bool useLast, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		Result cond_result = interpret(expression.eWhileCond, env, store, ref gameEnv);
		if (cond_result.value.valueType == "bool") {
			if (cond_result.value.vBool) {
				Result body_result = interpret(expression.eWhileBody, env, cond_result.store, ref gameEnv);
				return interpWhile(expression, body_result.value, true, env, body_result.store, ref gameEnv);
			} else if (useLast) {
				return new Result(lastValue, cond_result.store);
			} else {
				return new Result(new Value("null", expression.line, expression.character), cond_result.store);
			}
		} else {
			return resultError(cond_result, "Expected bool"); //Throw Error
		}
	}

	// This could be syntactic sugar on top of while
	static Result interpForeach(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	)
	{
		string loc = System.Guid.NewGuid().ToString();
		env.Add(expression.eForVariable, loc);
		Result lastResult = new Result(new Value("null", expression.line, expression.character), store);
		foreach (Expression expr in expression.eForIter.eList) {
			Result exprResult = interpret(expr, env, store, ref gameEnv);
			store[loc] = exprResult.value;
			Result bodyResult = interpret(expression.eForBody, env, store, ref gameEnv);
			lastResult = bodyResult;
			store = bodyResult.store;
		}
		return lastResult; 
	}

	static Result interpId(
		Expression expression, 
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
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

	static Result interpGetGameVar(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		string name = expression.eIgName;
		string variable = expression.eIgVariable;
		if (gameEnv.tokenDict.ContainsKey(name)) {
			GameObject token = gameEnv.tokenDict[name];
			TokenScript tokenScript = token.GetComponent<TokenScript>();
			if (tokenScript.variables.ContainsKey(variable)) {
				GameVar gameVar = tokenScript.variables[variable];
				switch(gameVar.type) {
					case "int":
						Value intValue = new Value("int", expression.line, expression.character);
						intValue.vInt = gameVar.intValue;
						return new Result(intValue, store);
					case "double":
						Value doubleValue = new Value("double", expression.line, expression.character);
						doubleValue.vDouble = gameVar.doubleValue;
						return new Result(doubleValue, store);
					case "string":
						Value stringValue = new Value("string", expression.line, expression.character);
						stringValue.vString= gameVar.stringValue;
						return new Result(stringValue, store);
					case "bool":
						Value boolValue = new Value("bool", expression.line, expression.character);
						boolValue.vBool = gameVar.boolValue;
						return new Result(boolValue, store);
					default:
						return expressionError(expression, store, "Unknown type from \"" + name + "." + variable + "\""); //Throw Error
				}
			} else {
				return expressionError(expression, store, "ig \"" + name + "\" has no variable \"" + variable + "\""); //Throw Error
			}
		} else if (gameEnv.cubeDict.ContainsKey(name)) {
			Cube cube = gameEnv.cubeDict[name];
			if (cube.variables.ContainsKey(variable)) {
				switch(cube.variables[variable]) {
					case "int":
						Value intValue = new Value("int", expression.line, expression.character);
						intValue.vInt = cube.intVars[variable];
						return new Result(intValue, store);
					case "double":
						Value doubleValue = new Value("double", expression.line, expression.character);
						doubleValue.vDouble = cube.doubleVars[variable];
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

	static Result interpSetGameVar(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) 
	{
		string name = expression.eSetIgName;
		string variable = expression.eSetIgVariable;
		if (gameEnv.tokenDict.ContainsKey(name)) {
			GameObject token = gameEnv.tokenDict[name];
			TokenScript tokenScript = token.GetComponent<TokenScript>();
			Result nv_result = interpret(expression.eSetIgValue, env, store, ref gameEnv);
			if (tokenScript.variables.ContainsKey(variable)) {
				GameVar gameVar = tokenScript.variables[variable];
				switch(gameVar.type) {
					case "int":
						if (nv_result.value.valueType == "int") {
							gameVar.intValue = nv_result.value.vInt;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected int"); //Throw Error
						}
					case "double":
						if (nv_result.value.valueType == "double") {
							gameVar.doubleValue = nv_result.value.vDouble;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected double"); //Throw Error
						}
					case "string":
						if (nv_result.value.valueType == "string") {
							gameVar.stringValue = nv_result.value.vString;
							return nv_result;
						} else {
							return resultError(nv_result, "Expected string"); //Throw Error
						}
					case "bool":
						if (nv_result.value.valueType == "bool") {
							gameVar.boolValue = nv_result.value.vBool;
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
					case "double":
					case "string":
					case "bool":
						tokenScript.variables.Add(variable, new GameVar(nv_result.value.vInt));
						return nv_result;
					default:
						return resultError(nv_result, "Expected int, double, string or bool"); //Throw Error
				}
			}
		} else if (gameEnv.cubeDict.ContainsKey(name)) {
			Cube cube = gameEnv.cubeDict[name];
			Result nv_result = interpret(expression.eSetIgValue, env, store, ref gameEnv);
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
							cube.doubleVars[variable] = nv_result.value.vDouble;
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
						cube.doubleVars.Add(variable, nv_result.value.vDouble);
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

	static Result interpLet(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		return expressionError(expression, store, "Unexpected let expression outside of programming block"); //Throw Error
	}

	static Result interpReturn(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		return expressionError(expression, store, "Unexpected return expression outside of programming block"); //Throw Error
	}

	static Result interpBuiltinFunc(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		return builtInFunctions[expression.eBuiltinFuncId](expression, env, store, ref gameEnv);
	}

	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================
	// ================================= Builtin Functions =======================================================================

	static Result ToString(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("string", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "bool":
				returnValue.vString = inputResult.value.vBool.ToString();
				break;
			case "int":
				returnValue.vString = inputResult.value.vInt.ToString();
				break;
			case "double":
				returnValue.vString = inputResult.value.vInt.ToString();
				break;
			default:
				return resultError(inputResult, "Expected bool, int or double");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result ToInt(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("int", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "string":
				int outInt;
				bool isNumeric = int.TryParse(inputResult.value.vString, out outInt);
				if (isNumeric) {
					returnValue.vInt = outInt;
				} else {
					return resultError(inputResult, "Unable to convert string to int");
				}
				break;
			case "double":
				returnValue.vInt = (int)Math.Floor(inputResult.value.vDouble);
				break;
			default:
				return resultError(inputResult, "Expected string or double");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result ToDouble(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("double", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "string":
				double outDouble;
				bool isNumeric = double.TryParse(inputResult.value.vString, out outDouble);
				if (isNumeric) {
					returnValue.vDouble = outDouble;
				} else {
					return resultError(inputResult, "Unable to convert string to double");
				}
				break;
			case "int":
				returnValue.vDouble = (double)inputResult.value.vInt;
				break;
			default:
				return resultError(inputResult, "Expected string or int");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result ToBool(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("bool", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "string":
				switch(inputResult.value.vString) {
					case "false":
					case "False":
					case "":
						returnValue.vBool = false;
						break;
					case "true":
					case "True":
					default:
						returnValue.vBool = true;
						break;
				}
				break;
			case "int":
				if (inputResult.value.vInt == 0) {
					returnValue.vBool = false;
				} else {
					returnValue.vBool = true;
				}
				break;
			case "double":
				if (inputResult.value.vDouble == 0) {
					returnValue.vBool = false;
				} else {
					returnValue.vBool = true;
				}
				break;
			default:
				return resultError(inputResult, "Expected string, int or double");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result Abs(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("double", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "double":
				returnValue.vDouble = Math.Abs(inputResult.value.vDouble);
				break;
			case "int":
				returnValue.valueType = "int";
				returnValue.vInt = Math.Abs(inputResult.value.vInt);
				break;
			default:
				return resultError(inputResult, "Expected int or double");	
		}
		return new Result(returnValue, inputResult.store);
	} 

	static Result Max(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count == 1) {
			Result firstResult = interpret(arguments[0], env, store, ref gameEnv);
			if (firstResult.value.valueType == "list") {
				return MaxList(firstResult.value.vList, firstResult.store);
			}
		}

		bool first = true;
		Value maxValue = new Value("null", expression.line, expression.character);

		foreach (Expression expr in expression.eBuiltinFuncArguments) {
			Result latestResult = interpret(expr, env, store, ref gameEnv);
			store = latestResult.store;

			if (first) {
				first = false;
				switch(latestResult.value.valueType) {
					case "int":
					case "double":
						maxValue = latestResult.value;
						break;
					default:
						return resultError(latestResult, "Expected a set of arguments of int or double");
				}
			} else if (maxValue.valueType == latestResult.value.valueType) {
				switch(latestResult.value.valueType) {
					case "int":
						if (latestResult.value.vInt > maxValue.vInt) {
							maxValue = latestResult.value;
						}
						break;
					case "double":
						if (latestResult.value.vDouble > maxValue.vDouble) {
							maxValue = latestResult.value;
						}
						break;
				}
			} else {
				switch(latestResult.value.valueType) {
					case "int":
						if (latestResult.value.vInt > maxValue.vDouble) {
							maxValue = latestResult.value;
						}
						break;
					case "double":
						if (latestResult.value.vDouble > maxValue.vInt) {
							maxValue = latestResult.value;
						}
						break;
					default:
						return resultError(latestResult, "Expected a set of arguments of int or double");
				}
			}
		}
		return new Result(maxValue, store);
	}

	static Result MaxList(List<Value> valueList, Dictionary<string, Value> store) {
		bool first = true;
		Value maxValue = new Value("null");

		foreach (Value val in valueList) {
			if (first) {
				first = false;
				switch(val.valueType) {
					case "int":
					case "double":
						maxValue = val;
						break;
					default:
						return resultError(new Result(val, store), "Expected a set of arguments of all int or double");
				}
			} else if (maxValue.valueType == val.valueType) {
				switch(val.valueType) {
					case "int":
						if (val.vInt < maxValue.vInt) {
							maxValue = val;
						}
						break;
					case "double":
						if (val.vDouble < maxValue.vDouble) {
							maxValue = val;
						}
						break;
				}
			} else {
				switch(val.valueType) {
					case "int":
						if (val.vInt < maxValue.vDouble) {
							maxValue = val;
						}
						break;
					case "double":
						if (val.vDouble < maxValue.vInt) {
							maxValue = val;
						}
						break;
					default:
						return resultError(new Result(val, store), "Expected a set of arguments of int or double");
				}
			}
		}
		return new Result(maxValue, store);
	}

	static Result Min(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count == 1) {
			Result firstResult = interpret(arguments[0], env, store, ref gameEnv);
			if (firstResult.value.valueType == "list") {
				return MinList(firstResult.value.vList, firstResult.store);
			}
		}

		bool first = true;
		Value minValue = new Value("null", expression.line, expression.character);

		foreach (Expression expr in expression.eBuiltinFuncArguments) {
			Result latestResult = interpret(expr, env, store, ref gameEnv);
			store = latestResult.store;

			if (first) {
				first = false;
				switch(latestResult.value.valueType) {
					case "int":
					case "double":
						minValue = latestResult.value;
						break;
					default:
						return resultError(latestResult, "Expected a set of arguments of int or double");
				}
			} else if (minValue.valueType == latestResult.value.valueType) {
				switch(latestResult.value.valueType) {
					case "int":
						if (latestResult.value.vInt < minValue.vInt) {
							minValue = latestResult.value;
						}
						break;
					case "double":
						if (latestResult.value.vDouble < minValue.vDouble) {
							minValue = latestResult.value;
						}
						break;
				}
			} else {
				switch(latestResult.value.valueType) {
					case "int":
						if (latestResult.value.vInt < minValue.vDouble) {
							minValue = latestResult.value;
						}
						break;
					case "double":
						if (latestResult.value.vDouble < minValue.vInt) {
							minValue = latestResult.value;
						}
						break;
					default:
						return resultError(latestResult, "Expected a set of arguments of int or double");
				}
			}
		}
		return new Result(minValue, store);
	}

	static Result MinList(List<Value> valueList, Dictionary<string, Value> store) {
		bool first = true;
		Value minValue = new Value("null");

		foreach (Value val in valueList) {
			if (first) {
				first = false;
				switch(val.valueType) {
					case "int":
					case "double":
						minValue = val;
						break;
					default:
						return resultError(new Result(val, store), "Expected a set of arguments of all int or double");
				}
			} else if (minValue.valueType == val.valueType) {
				switch(val.valueType) {
					case "int":
						if (val.vInt < minValue.vInt) {
							minValue = val;
						}
						break;
					case "double":
						if (val.vDouble < minValue.vDouble) {
							minValue = val;
						}
						break;
				}
			} else {
				switch(val.valueType) {
					case "int":
						if (val.vInt < minValue.vDouble) {
							minValue = val;
						}
						break;
					case "double":
						if (val.vDouble < minValue.vInt) {
							minValue = val;
						}
						break;
					default:
						return resultError(new Result(val, store), "Expected a set of arguments of int or double");
				}
			}
		}
		return new Result(minValue, store);
	}

	static Result Floor(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("double", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "double":
				returnValue.vDouble = Math.Floor(inputResult.value.vDouble);
				break;
			case "int":
				returnValue.valueType = "int";
				returnValue.vInt = inputResult.value.vInt;
				break;
			default:
				return resultError(inputResult, "Expected int or double");	
		}
		return new Result(returnValue, inputResult.store);
	} 

	static Result Ceil(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("double", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "double":
				returnValue.vDouble = Math.Ceiling(inputResult.value.vDouble);
				break;
			case "int":
				returnValue.valueType = "int";
				returnValue.vInt = inputResult.value.vInt;
				break;
			default:
				return resultError(inputResult, "Expected int or double");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result Round(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("double", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "double":
				returnValue.vDouble = Math.Round(inputResult.value.vDouble);
				break;
			case "int":
				returnValue.valueType = "int";
				returnValue.vInt = inputResult.value.vInt;
				break;
			default:
				return resultError(inputResult, "Expected int or double");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result Factorial(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		switch(inputResult.value.valueType) {
			case "int":
				if (inputResult.value.vInt >= 1) {
					Value returnValue = new Value("int", expression.line, expression.character);
					returnValue.vInt = factorialHelper(inputResult.value.vInt);
					return new Result(returnValue, inputResult.store);
				} else {
					return resultError(inputResult, "int must be greater than 1");
				}
			default:
				return resultError(inputResult, "Expected int");	
		}
	}

	static int factorialHelper(int x) {
		int result = 1;
		for (int i = 1; i <= x; i++) {
			result = result * i;
		}
		return result;
	}

	static Result Binomial(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 2) {
			return expressionError(expression, store, "Expected 2 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		if (l_result.value.valueType == "int") {
			Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
			if (r_result.value.valueType == "int") {
				Value returnValue = new Value("int", expression.line, expression.character);
				returnValue.vInt = binomialHelper(l_result.value.vInt, r_result.value.vInt);
				return new Result(returnValue, r_result.store);
			} else {
				return resultError(r_result, "Expected int");	
			}
		} else {
			return resultError(l_result, "Expected int");	
		}
	}	

	static int binomialHelper(int n, int k) {
		int numerator = 1;
		for (int i = k + 1; i <= k; i++) {
			numerator = numerator * i;
		}
		int denominator = factorialHelper(n - k);
		return numerator / denominator;
	}

	static Result Log(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 2) {
			return expressionError(expression, store, "Expected 2 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case "int":
			case "double":
				Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case "int":
					case "double":
						Value returnValue = new Value("double", expression.line, expression.character);
						if (l_result.value.valueType == "int" && r_result.value.valueType == "int") {
							returnValue.vDouble = Math.Log(l_result.value.vInt, r_result.value.vInt);
						} else if (l_result.value.valueType == "double" && r_result.value.valueType == "int") {
							returnValue.vDouble = Math.Log(l_result.value.vDouble, r_result.value.vInt);
						} else if (l_result.value.valueType == "int" && r_result.value.valueType == "double") {
							returnValue.vDouble = Math.Log(l_result.value.vInt, r_result.value.vDouble);
						} else {
							returnValue.vDouble = Math.Log(l_result.value.vDouble, r_result.value.vDouble);
						}
						return new Result(returnValue, r_result.store);
					default:
						return resultError(r_result, "Expected int or double");
				}
			default:
				return resultError(l_result, "Expected int or double");
		}
	}	

	static Result Logn(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("double", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "double":
				returnValue.vDouble = Math.Log(inputResult.value.vDouble);
				break;
			case "int":
				returnValue.vDouble = Math.Log(inputResult.value.vInt);
				break;
			default:
				return resultError(inputResult, "Expected double or int");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result Sum(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count == 1) {
			Result firstResult = interpret(arguments[0], env, store, ref gameEnv);
			if (firstResult.value.valueType == "list") {
				return SumList(firstResult.value.vList, firstResult.store);
			}
		}

		bool first = true;
		Value sumValue = new Value("null", expression.line, expression.character);

		foreach (Expression expr in expression.eBuiltinFuncArguments) {
			Result latestResult = interpret(expr, env, store, ref gameEnv);
			store = latestResult.store;

			if (first) {
				first = false;
				switch(latestResult.value.valueType) {
					case "int":
					case "double":
						sumValue = latestResult.value;
						break;
					default:
						return resultError(latestResult, "Expected a set of arguments of int or double");
				}
			} else if (sumValue.valueType == latestResult.value.valueType) {
				switch(latestResult.value.valueType) {
					case "int":
						sumValue.vInt += latestResult.value.vInt;
						break;
					case "double":
						sumValue.vDouble += latestResult.value.vDouble;
						break;
				}
			} else{
				switch(latestResult.value.valueType) {
					case "int":
						sumValue.vDouble += (double)latestResult.value.vInt;
						break;
					case "double":
						sumValue.valueType = "double";
						sumValue.vDouble = (double)sumValue.vInt + (double)latestResult.value.vInt;
						break;
					default:
						return resultError(latestResult, "Expected a set of arguments of int or double");
				}
			}
		}
		return new Result(sumValue, store);
	}

	static Result SumList(List<Value> valueList, Dictionary<string, Value> store) {
		bool first = true;
		Value sumValue = new Value("null");

		foreach (Value val in valueList) {
			if (first) {
				first = false;
				switch(val.valueType) {
					case "int":
					case "double":
						sumValue = val;
						break;
					default:
						return resultError(new Result(val, store), "Expected a set of arguments of int or double");
				}
			} else if (sumValue.valueType == val.valueType) {
				switch(val.valueType) {
					case "int":
						sumValue.vInt += val.vInt;
						break;
					case "double":
						sumValue.vDouble += val.vDouble;
						break;
				}
			} else{
				switch(val.valueType) {
					case "int":
						sumValue.vDouble += (double)val.vInt;
						break;
					case "double":
						sumValue.valueType = "double";
						sumValue.vDouble = (double)sumValue.vInt + (double)val.vInt;
						break;
					default:
						return resultError(new Result(val, store), "Expected a set of arguments of int or double");
				}
			}
		}
		return new Result(sumValue, store);
	}

	static Result Len(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);

		Value returnValue = new Value("int", expression.line, expression.character);
		switch(inputResult.value.valueType) {
			case "string":
				returnValue.vInt = inputResult.value.vString.Length;
				break;
			case "list":
				returnValue.vInt = inputResult.value.vList.Count;
				break;
			default:
				return resultError(inputResult, "Expected list or string");	
		}
		return new Result(returnValue, inputResult.store);
	}

	static Result Range(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 1) {
			return expressionError(expression, store, "Expected 1 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result inputResult = interpret(arguments[0], env, store, ref gameEnv);
		switch(inputResult.value.valueType) {
			case "int":
				Value returnValue = new Value("list", expression.line, expression.character);
				returnValue.vList = new List<Value>();
				for (int i = 0; i < inputResult.value.vInt; i++) {
					Value rangeValue = new Value("int", expression.line, expression.character);
					rangeValue.vInt = i;
					returnValue.vList.Add(rangeValue);
				}
				return new Result(returnValue, inputResult.store);
			default:
				return resultError(inputResult, "Expected int");	
		}
	}

	static Result Append(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 2) {
			return expressionError(expression, store, "Expected 2 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		if (l_result.value.valueType == "list") {
			Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
			if (r_result.value.valueType == "error") {
				return resultError(r_result, "Passed error to list");
			}

			l_result.value.vList.Add(r_result.value);
			return new Result(l_result.value, r_result.store);
		} else {
			return resultError(l_result, "Expected list");	
		}
	}	

	static Result Insert(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 3) {
			return expressionError(expression, store, "Expected 3 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		if (l_result.value.valueType == "list") {
			Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
			if (r_result.value.valueType == "int") {
				if (r_result.value.vInt < 0 || r_result.value.vInt >= l_result.value.vList.Count) {
					return resultError(l_result, r_result.value.vInt.ToString() + " out of range of list of size " + l_result.value.vList.Count.ToString());
				}

				Result t_result = interpret(arguments[2], env, r_result.store, ref gameEnv);
				if (t_result.value.valueType == "error") {
				return resultError(t_result, "Passed error to list");
				}

				l_result.value.vList.Insert(r_result.value.vInt, t_result.value);
				return new Result(l_result.value, t_result.store);
			} else {
				return resultError(l_result, "Expected int");
			}
		} else {
			return resultError(l_result, "Expected list");	
		}
	}

	static Result RandInt(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 2) {
			return expressionError(expression, store, "Expected 2 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		switch(l_result.value.valueType) {
			case "int":
				Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
				switch(r_result.value.valueType) {
					case "int":
						if (l_result.value.vInt >= r_result.value.vInt) {
							return resultError(r_result, "Second argument passed must be greater than first argument passed");	
						}
						Value returnValue = new Value("int", expression.line, expression.character);
						System.Random rnd = new System.Random();
						returnValue.vInt = rnd.Next(l_result.value.vInt, r_result.value.vInt);
						return new Result(returnValue, r_result.store);
					default:
						return resultError(r_result, "Expected int");
				}
			default:
				return resultError(l_result, "Expected int");
		}
	}	

	static Result Rand(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 0) {
			return expressionError(expression, store, "Expected 0 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Value returnValue = new Value("double", expression.line, expression.character);
		System.Random rnd = new System.Random();
		returnValue.vDouble = rnd.NextDouble();
		return new Result(returnValue, store);
	}	

	static Result Contains(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 2) {
			return expressionError(expression, store, "Expected 2 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		if (l_result.value.valueType == "list") {
			Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
			if (r_result.value.valueType == "error") {
				return resultError(r_result, "Passed error to list");
			}

			Value returnValue = new Value("bool", expression.line, expression.character);
			foreach (Value val in l_result.value.vList) {
				if (valuesEqual(val, r_result.value)) {
					returnValue.vBool = true;
					return new Result(returnValue, r_result.store);	
				}
			}
			returnValue.vBool = false;
			return new Result(returnValue, r_result.store);			
		} else {
			return resultError(l_result, "Expected int");	
		}
	}
	
	static bool valuesEqual(Value left, Value right) {
		if (left.valueType == right.valueType) {
			switch(left.valueType) {
				case "int":
					return left.vInt == right.vInt;
				case "double":
					return left.vDouble == right.vDouble;
				case "string":
					return left.vString == right.vString;
				case "bool":
					return left.vBool == right.vBool;
			}
		}
		return false;
	}	

	static Result IndexOf(
		Expression expression,
		Dictionary<string, string> env, 
		Dictionary<string, Value> store, 
		ref GameEnv gameEnv
	) {
		List<Expression> arguments = expression.eBuiltinFuncArguments;
		if (arguments.Count != 2) {
			return expressionError(expression, store, "Expected 2 argument, got " + arguments.Count.ToString()); //Throw Error	
		} 

		Result l_result = interpret(arguments[0], env, store, ref gameEnv);
		if (l_result.value.valueType == "list") {
			Result r_result = interpret(arguments[1], env, l_result.store, ref gameEnv);
			if (r_result.value.valueType == "error") {
				return resultError(r_result, "Passed error to list");
			}

			Value returnValue = new Value("int", expression.line, expression.character);
			for (int i = 0; i < l_result.value.vList.Count; i++) {
				if (valuesEqual(l_result.value.vList[i], r_result.value)) {
					returnValue.vInt = i;
					return new Result(returnValue, r_result.store);	
				}
			}
			return resultError(r_result, "List does not contain value");		
		} else {
			return resultError(l_result, "Expected int");	
		}
	}
}

// ====================================================================================================================
// ===================================================== Data Types ===================================================
// ====================================================================================================================
// ====================================================================================================================
// ===================================================== Data Types ===================================================
// ====================================================================================================================
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
	public double vDouble; // type=2
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
	public double eDouble; // type=2
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

public delegate Result BuiltinFunction(
	Expression expression,
	Dictionary<string, string> env, 
	Dictionary<string, Value> store, 
	ref GameEnv gameEnv
);