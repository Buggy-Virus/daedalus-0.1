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

	Value interpret(Expression expression, Dictionary<string, string> enviroment, Dictionary<string, Value> store) {
		switch (returnValue.valueType) {
            case 1:
            	Value returnValue = new Value;
                returnValue.valueType = 1;
                returnValue.vInt = expression.eInt;
                break;
            case 2:
            	Value returnValue = new Value;
                returnValue.valueType = 2;
                returnValue.vFloat = expression.eFloat;
                break;
            case 3:
            	Value returnValue = new Value;
            	returnValue.valueType = 3;
            	returnValue.vString = expression.eString;
            	break;
            case 4:
            	Value returnValue = new Value;
            	returnValue.valueType = 4;
            	returnValue.vBool = expression.eBool;
            	break;
            case 5:
            	returnValue = interpOperatpr(expression.eOperatorOp, expression.eOperatorLeft, expression.eOperatorRight, enviroment, store);
            	break;
            case 6:
            	returnValue = interpIf(expression.eIfCond, expression.eIfConsq, expression.eIfAlter, enviroment, store);
            	break;
            case 7:
            	returnValue = interpLam(expression.eLamParam, expression.eLamBody, enviroment, store);
            	break;
            case 8:
            	returnValue = interpApp(expression.eAppFun, expression.eAppArg, enviroment, store);
            	break;
            case 9:
            	returnValue = interpSet(expression.eSetNAme, expression.eSetValue, enviroment, store);
            	break;
            case 10:
            	returnValue = interpDo(expression.eDo, enviroment, store);
            	break;	
            case 11:
            	returnValue = interpDo(expression.eWhileCond, expression.eWhileBody, enviroment, store);
            	break;	
        }
        return returnValue;
	}

	Value interpOperator(Operator op, Expression left, Expression right) {
		Value returnValue = new Value;
		switch (op.operatorType) {
			case 1:
				Value l_val = interpret(left);
				if (l_val.valueType == 2) {
						Value r_val = interpret(right);
						if (r_val.valueType == 2) {
								returnValue.valueType = 2;
								returnValue.vFloat = l_val + r_val;
							} else {
								//Throw Error
							}
					} else {
						//Throw Error
					}
			case 2:
				
				break;
			case 3:
				
				break;
			case 4:
				
				break;
		}
	} 
}

public class Value {
	int valueType; 

	int vInt; // type=1
	fliat vFloat; // type=2
	string vString; // type=3
	bool vBool; // type=4

	string vFunParam; // type=5
	Expression vFunExpression;
	Dictionary<String, String> vFunEnviroment;	
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

	stinrg eLamParam; // type=7
	Expression eLamBody;

	Expression eAppFunc; // type=8
	Expression eAppArg;

	String eSetName; // type=9
	Expression eSetValue;

	List<Expression> eDo; //type=10

	Expression eWhileCond; //type=11
	Expression eWhileBody; 

	//Sugar

	Expression eAndLeft; //type=20
	Expression eAndRight;

	Expression eOrLeft; //type=21
	Expression eOrRight;

	List<Expression> eForIter; //type=22
	string eForVariable;
	Expression eForBody;
}

public class Operator {
	int operatorType;

	// Change these

	// op-plus, type=1
	// op-str-append, type=2
	// op-str-eq, type=3;
	// op-num-eq, type=4
}
