﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testDaeDaedScript : MonoBehaviour
{
    GameEnvScript gameEnvScript;
	public Dictionary<string, GameObject> tokenEnv;
    public Dictionary<string, GameObject> cubeEnv;

    public Dictionary<string, string> builtInEnv;
    public Dictionary<string, Value> builtInStore; 
    public bool evaluate;

    void printValue(Value val) {
        string printString = "type: " + val.valueType + ", value: ";
        switch(val.valueType) {
            case "int":
                printString += val.vInt.ToString();
                Debug.Log(printString);
                break;
            case "double":
                printString += val.vDouble.ToString();
                Debug.Log(printString);
                break;
            case "string":
                printString += val.vString;
                Debug.Log(printString);
                break;
            case "bool":
                printString += val.vBool.ToString();
                Debug.Log(printString);
                break;
            case "list":
                Debug.Log(printString);
                foreach (Value v in val.vList) {
                    printValue(v);
                }
                break;
            case "error":
                printString += val.errorMessage.ToString();
                Debug.Log(printString);
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // DaedScript = GameObject.Find("GameLogic").GetComponent<DScript>();
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>();

        tokenEnv = gameEnvScript.gameEnv.tokenDict;
		cubeEnv = gameEnvScript.gameEnv.shapeDict;
    }

    string test_int1 = "100;";
    string test_int2 = "1;";
    string test_int3 = "10;";
    string test_int4 = "2;";
    string test_int5 = "5;";
    string test_int6 = "7;";
    string test_int7 = "0;";
    string test_int8 = "12;";
    string test_int9 = "15;";
    string test_int10 = "13;";
    string test_int11 = "  13;";

    string test_double1 = "13.0;";
    string test_double2 = "1.0;";
    string test_double3 = "0.0;";
    string test_double4 = "1.5;";
    string test_double5 = "105.47;";
    string test_double6 = "18.9238912345;";
    string test_double7 = "500.00000000000;";
    string test_double8 = "984289.5550000000000;";
    string test_double9 = "12.125;";
    string test_double10 = "828472849.124124;";

    string test_bool1 = "true;";
    string test_bool2 = "false;";

    string test_string1 = "\"\";";
    string test_string2 = "\"here\";";
    string test_string3 = "\"there\";";
    string test_string4 = "\"a\";";
    string test_string5 = "\"b\";";
    string test_string6 = "\"here and there\";";
    string test_string7 = "\"up, dup, cup \t, \n here here\";";
    string test_string8 = "\"dummy dummy\";";
    string test_string9 = "\"run away\";";
    string test_string10 = "\"aefoefoin1112292031rjip3fklmaefn3p91p3finelks\";";
    string test_string11 = "\"@@@##$@!$&)%*&$)()[]\";";

    string test_list1 = "[];";
    string test_list2 = "[\"rock\"];";
    string test_list3 = "[\"rock\", \"rock\"];";
    string test_list4 = "[\"rock\", \"rock\", \"rock\"];";
    string test_list5 = "[1, 32, 12556, 2, 0];";
    string test_list6 = "[1.05904, 100.9843, 28.1, 1.0];";
    string test_list7 = "[true, false, true];";
    string test_list8 = "[1, 1.0];";
    string test_list9 = "[\"rock\", 1, 1.0, true];";

    string test_operator1 = "2 + 1;";
    string test_operator2 = "2 - 1;";
    string test_operator3 = "3 * 1;";
    string test_operator4 = "6 / 2;";
    string test_operator5 = "3 ** 2;";
    string test_operator6 = "5 % 2;";
    string test_operator7 = "2 == 2;";
    string test_operator8 = "3 == 2;";
    string test_operator9 = "2 != 2;";
    string test_operator10 = "3 != 2;";
    string test_operator11 = "3 > 2;";
    string test_operator12 = "2 > 2;";
    string test_operator13 = "3 < 2;";
    string test_operator14 = "3 < 4;";
    string test_operator15 = "2 < 2;";
    string test_operator16 = "3 >= 2;";
    string test_operator17 = "1 >= 2;";
    string test_operator18 = "4 >= 4;";
    string test_operator19 = "3 <= 2;";
    string test_operator20 = "1 <= 2;";
    string test_operator21 = "1 <= 1;";
    string test_operator22 = "1.2 + 2.4;";
    string test_operator23 = "8.5 - 20.6;";
    string test_operator24 = "1.5 * 2.7;";
    string test_operator25 = "1.6 / 3.2;";
    string test_operator26 = "6.0 ** 2.5;";
    string test_operator27 = "1.5 % 1.0;";
    string test_operator28 = "1.5 > 0.9;";
    string test_operator29 = "0.5 > 1.5;";
    string test_operator30 = "6.7 < 6.8;";
    string test_operator31 = "1.2 < 1.0;";
    string test_operator32 = "7.0 >= 6.0;";
    string test_operator33 = "5.6 >= 5.6;";
    string test_operator34 = "77.6 >= 100.77;";
    string test_operator35 = "0.0 <= 8.9;";
    string test_operator36 = "7584.1 <= 9000.0;";
    string test_operator37 = "0.5 <= 0.5;";
    string test_operator38 = "1.0 != 1.0;";
    string test_operator39 = "1.1 != 1.0;";
    string test_operator40 = "7.53 == 7.53;";
    string test_operator41 = "true == false;";
    string test_operator42 = "true != false;";
    string test_operator43 = "false != false;";
    string test_operator44 = "true != true;";
    string test_operator45 = "true && true;";
    string test_operator46 = "true && false;";
    string test_operator47 = "false && false;";
    string test_operator48 = "true || true;";
    string test_operator49 = "true || false;";
    string test_operator50 = "false || false;";
    string test_operator51 = "!false;";
    string test_operator52 = "!true;";
    string test_operator53 = "\"here\" + \"there\";";
    string test_operator54 = "\"here\" == \"there\";";
    string test_operator55 = "\"here\" == \"here\";";
    string test_operator56 = "\"012345\"[0];";
    string test_operator57 = "\"012345\"[1];";
    string test_operator58 = "\"012345\"[3];";
    string test_operator59 = "\"012345\"[5];";
    string test_operator60 = "\"012345\"[0:4];";
    string test_operator61 = "\"012345\"[1:4];";
    string test_operator62 = "\"012345\"[0:2];";
    string test_operator63 = "\"012345\"[0:1];";
    string test_operator64 = "\"012345\"[3:4];";
    string test_operator65 = "\"012345\"[3:5];";
    string test_operator66 = "[\"here\"] + [\"there\"];";
    string test_operator67 = "[0, 1, 2, 3, 4, 5][0];";
    string test_operator68 = "[0, 1, 2, 3, 4, 5][2];";
    string test_operator69 = "[0, 1, 2, 3, 4, 5][5];";
    // I don't support negatives yet
    string test_operator70 = "[0, 1, 2, 3, 4, 5][-1];";
    string test_operator71 = "[0, 1, 2, 3, 4, 5][0:1];";
    string test_operator72 = "[0, 1, 2, 3, 4, 5][0:5];";
    string test_operator73 = "[0, 1, 2, 3, 4, 5][0:6];";
    string test_operator74 = "[0, 1, 2, 3, 4, 5][1:4];";
    string test_operator75 = "[0, 1, 2, 3, 4, 5][1:2];";
    string test_operator76 = "[0, 1, 2, 3, 4, 5][3:5];";
    
    string test_nestedOperators0 = "1 + (8 + 9);";
    string test_nestedOperators1 = "(2 + 3) + 1;";
    string test_nestedOperators2 = "(8 + 1) + (0 + 4);";
    string test_nestedOperators3 = "1 - (8 + 9);";
    string test_nestedOperators4 = "(2 + 3) - 1;";
    string test_nestedOperators5 = "(8 + 1) - (0 + 4);";
    string test_nestedOperators6 = "1 * (8 + 9);";
    string test_nestedOperators7 = "(2 + 3) * 1;";
    string test_nestedOperators8 = "(8 + 1) * (0 + 4);";
    string test_nestedOperators9 = "6 / (1 + 1);";
    string test_nestedOperators10 = "(3 + 3) / 2;";
    string test_nestedOperators11 = "6 / (1 + 1);";
    string test_nestedOperators12 = "(3 + 3) / (1 + 1);";
    string test_nestedOperators13 = "(1 + 3) ** 2;";
    string test_nestedOperators14 = "(1 + 2) ** (1 + 1);";
    string test_nestedOperators15 = "3 ** (2 + 0);";
    string test_nestedOperators16 = "(3 + 3) % 5;";
    string test_nestedOperators17 = "(3 + 3) % (2 + 3);";
    string test_nestedOperators18 = "6 % (2 + 3);";
    string test_nestedOperators19 = "(3 + 3) == (2 + 4);";
    string test_nestedOperators20 = "(3 + 3) != (2 + 1);";
    string test_nestedOperators21 = "(3 + 3) == 6;";
    string test_nestedOperators22 = "3 == (2 + 1);";
    string test_nestedOperators23 = "4 != (2 + 1);";
    string test_nestedOperators24 = "(3 + 3) != 7;";
    string test_nestedOperators25 = "(3.0 + 3.4) + (2.78 + 1.1111);";
    string test_nestedOperators26 = "(3.6 + 3.6) - (2.0 + 1.1);";
    string test_nestedOperators27 = "(3.1 + 3.3) * (2.0 + 1.1);";
    string test_nestedOperators28 = "(3.1 + 8.23) / (2.1 + 1.1);";
    string test_nestedOperators29 = "(3.0 + 3.0) % (1.5 + 0.5);";
    string test_nestedOperators30 = "(3.0 + 3.5) == (6.5 + 0.0);";
    string test_nestedOperators31 = "(1.0 + 1.5) > 2;";
    string test_nestedOperators32 = "(1.0 + 1.5) >= 2;";
    string test_nestedOperators33 = "(1.0 + 1.5) >= 2.5;";
    string test_nestedOperators34 = "(1.0 + 1.5) < (2.0 + 2.5);";
    string test_nestedOperators35 = "(1.0 + 1.5) <= (2.0 + 2.5);";
    string test_nestedOperators36 = "(3.0 + 1.5) <= (2.0 + 2.5);";
    string test_nestedOperators37 = "(3.0 + 3.5) != (6.5 + 0.0);";
    string test_nestedOperators38 = "(\"cat\" + \"cat\") + \"cat\";";
    string test_nestedOperators39 = "(\"cat\" + \"cat\") != \"cat\";";
    string test_nestedOperators40 = "(\"cat\" + \"cat\") == \"cat\";";
    string test_nestedOperators41 = "(\"cat\" + \"cat\") == (\"cat\" + \"cat\");";
    string test_nestedOperators42 = "[(\"cat\" + \"cat\")];";
    string test_nestedOperators43 = "[(\"cat\" + \"cat\"), (\"cat\" + \"cat\")];";
    string test_nestedOperators44 = "[1 + 5, 7 + 9];";
    string test_nestedOperators45 = "([1] + [5]) + [5, 6];";
    string test_nestedOperators46 = "(\"0123\" + \"456\")[0];";
    string test_nestedOperators47 = "(\"0123\" + \"456\")[3];";
    string test_nestedOperators48 = "(\"0123\" + \"456\")[5];";
    string test_nestedOperators49 = "(\"0123\" + \"456\")[(0 + 1)];";
    string test_nestedOperators50 = "(\"0123\" + \"456\")[(4 + 1)];";
    string test_nestedOperators51 = "(\"0123\" + \"456\")[0:(3 + 1)];";
    string test_nestedOperators52 = "[0, 1, 2, 3, 4, 5][(0 + 1)];";
    string test_nestedOperators53 = "[0, 1, 2, 3, 4, 5][(4 + 1)];";
    string test_nestedOperators54 = "[0, 1, 2, 3, 4, 5][0:(1 + 1)];";
    string test_nestedOperators55 = "[0, 1, 2, 3, 4, 5][(1 + 1):(1 + 3)];";
    string test_nestedOperators56 = "(true && true) && true;";
    string test_nestedOperators57 = "(true && false) && true;";
    string test_nestedOperators58 = "(true && true) || false;";
    string test_nestedOperators59 = "(true && false) || false;";
    string test_nestedOperators60 = "(1 + 3) > 5;";
    string test_nestedOperators61 = "(5 + 6) > 6;";
    string test_nestedOperators62 = "(4 + 8) < 60;";
    string test_nestedOperators63 = "(4 + 4) < 8;";
    string test_nestedOperators64 = "(6 + 18) <= 70;";
    string test_nestedOperators65 = "(6 + 80) <= 70;";
    string test_nestedOperators66 = "(6 + 64) <= 70;";
    string test_nestedOperators67 = "(9 + 19) >= 4;";
    string test_nestedOperators68 = "(9 + 19) >= (400 + 91);";
    string test_nestedOperators69 = "(9 + 19) >= (8 + 20);";
    string test_nestedOperators70 = "(\"string\" + \"equal\") == \"stringequal\";";

    string test_mixedOperators1 = "1 + 1.0;";
    string test_mixedOperators2 = "1.0 + 1;";
    string test_mixedOperators3 = "2.0 - 1;";
    string test_mixedOperators4 = "2 - 1.0;";
    string test_mixedOperators5 = "1 * 3.0;";
    string test_mixedOperators6 = "3.0 * 1;";
    string test_mixedOperators7 = "4.0 / 2;";
    string test_mixedOperators8 = "4 / 2.0;";
    string test_mixedOperators9 = "2 ** 3.0;";
    string test_mixedOperators10 = "2.0 ** 3;";
    string test_mixedOperators11 = "6 % 5.0;";
    string test_mixedOperators12 = "6.0 % 5;";
    string test_mixedOperators13 = "2.0 == 2;";
    string test_mixedOperators14 = "2 == 2.0;";
    string test_mixedOperators15 = "2 > 3.0;";
    string test_mixedOperators16 = "2.0 > 3;";
    string test_mixedOperators17 = "3.0 < 4;";
    string test_mixedOperators18 = "3 < 4.0;";
    string test_mixedOperators19 = "2.0 >= 3;";
    string test_mixedOperators20 = "2 >= 3.0;";
    string test_mixedOperators21 = "2 <= 3.0;";
    string test_mixedOperators22 = "2.0 <= 3;";

    // Update is called once per frame
    void Update()
    {
        if (evaluate) {
            // Debug.Log("==== Int Tests ====");
            // printValue(DaedScript.evaluate(test_int1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int9, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int10, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_int11, ref tokenEnv, ref cubeEnv));
            // Debug.Log("====End Int Tests ====");

            // Debug.Log("==== Double Tests ====");
            // printValue(DaedScript.evaluate(test_double1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double9, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_double10, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End Double Tests ====");

            // Debug.Log("==== Bool Tests ====");
            // printValue(DaedScript.evaluate(test_bool1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_bool2, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End Bool Tests ====");

            // Debug.Log("==== String Tests ====");
            // printValue(DaedScript.evaluate(test_string1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string9, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string10, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_string11, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End String Tests ====");

            // Debug.Log("==== List Tests ====");
            // printValue(DaedScript.evaluate(test_list1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_list9, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End List Tests ====");

            // Debug.Log("==== Operator Tests ====");
            // printValue(DaedScript.evaluate(test_operator1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator9, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator10, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator11, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator12, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator13, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator14, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator15, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator16, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator17, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator18, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator19, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator20, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator21, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator22, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator23, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator24, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator25, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator26, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator27, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator28, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator29, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator30, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator31, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator32, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator33, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator34, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator35, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator36, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator37, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator38, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator39, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator40, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator41, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator42, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator43, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator44, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator45, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator46, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator47, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator48, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator49, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator50, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator51, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator52, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator53, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator54, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator55, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator56, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator57, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator58, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator59, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator60, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator61, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator62, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator63, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator64, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator65, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator66, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator67, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator68, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator69, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator70, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator71, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator72, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator73, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator74, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator75, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_operator76, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End Operator Tests ====");

            // Debug.Log("==== Nexted Operator Tests ====");
            // printValue(DaedScript.evaluate(test_nestedOperators0, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators9, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators10, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators11, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators12, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators13, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators14, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators15, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators16, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators17, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators18, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators19, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators20, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators21, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators22, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators23, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators24, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators25, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators26, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators27, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators28, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators29, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators30, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators31, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators32, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators33, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators34, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators35, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators36, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators37, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators38, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators39, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators40, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators41, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators42, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators43, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators44, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators45, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators46, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators47, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators48, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators49, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators50, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators51, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators52, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators53, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators54, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators55, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators56, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators57, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators58, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators59, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators60, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators61, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators62, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators63, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators64, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators65, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators66, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators67, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators68, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators69, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_nestedOperators70, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End Nexted Operator Tests ====");

            // Debug.Log("==== Mixed Operator Tests ====");
            // printValue(DaedScript.evaluate(test_mixedOperators1, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators2, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators3, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators4, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators5, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators6, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators7, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators8, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators9, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators10, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators11, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators12, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators13, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators14, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators15, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators16, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators17, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators18, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators19, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators20, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators21, ref tokenEnv, ref cubeEnv));
            // printValue(DaedScript.evaluate(test_mixedOperators22, ref tokenEnv, ref cubeEnv));
            // Debug.Log("==== End Mixed Operator Tests ====");

            evaluate = false;
        }
    }
}
