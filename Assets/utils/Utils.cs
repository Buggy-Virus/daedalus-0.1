using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    static public bool indexEqual(Index ind1, Index ind2) {
		return (ind1.x == ind2.x && ind1.y == ind2.y && ind1.z == ind2.z);
	}

    static public int distance(Index point_a, Index point_b) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_z = Math.Abs(point_a.z - point_b.z);

        int max_xz = Math.Max(dist_x, dist_z);
        int min_xz = Math.Min(dist_x, dist_z);
        int dist_xz;
        if (min_xz % 2 == 0) {
            dist_xz = max_xz - min_xz + 3 * (min_xz / 2);
        } else {
            dist_xz = max_xz - min_xz + 3 * ((min_xz - 1) / 2) + 1;
        }
        
        int max_xyz = Math.Max(dist_y, dist_xz);
        int min_xyz = Math.Min(dist_y, dist_xz);
        int dist;
        if (min_xyz % 2 == 0) {
            dist = max_xyz - min_xyz + 3 * (min_xyz / 2);
        } else {
            dist = max_xyz - min_xyz + 3 * ((min_xyz - 1) / 2) + 1;
        }

        return dist;
    }

    static public int distance_xy(Index point_a, Index point_b) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);

        int max_xy = Math.Max(dist_x, dist_y);
        int min_xy = Math.Min(dist_x, dist_y);
        if (min_xy % 2 == 0) {
            return max_xy - min_xy + 3 * (min_xy / 2);
        } else {
            return max_xy - min_xy + 3 * ((min_xy - 1) / 2) + 1;
        }
    }

    static public int distance_xz(Index point_a, Index point_b) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_z = Math.Abs(point_a.z - point_b.z);

        int max_xz = Math.Max(dist_x, dist_z);
        int min_xz = Math.Min(dist_x, dist_z);
        if (min_xz % 2 == 0) {
            return max_xz - min_xz + 3 * (min_xz / 2);
        } else {
            return max_xz - min_xz + 3 * ((min_xz - 1) / 2) + 1;
        }
    }

    static public Dictionary<int, Index> line(Index point_a, Index point_b, int length) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_z = Math.Abs(point_a.z - point_b.z);
        int dist_max = Math.Max(dist_x, Math.Max(dist_y, dist_z));

        Dictionary<int, Index> line = new Dictionary<int, Index>();
        double stepFrequency_x = dist_x / dist_max;
        double stepFrequency_y = dist_y / dist_max;
        double stepFrequency_z = dist_z / dist_max;
        for (int i = -length; i <= length; i++) {
            int pos_x;
            int pos_y;
            int pos_z;
            if (point_a.x < point_b.x) {
                pos_x = point_a.x + (int)(i * stepFrequency_x);
            } else {
                pos_x = point_a.x - (int)(i * stepFrequency_x);
            }
            
            if (point_a.y < point_b.y) {
                pos_y = point_a.y + (int)(i * stepFrequency_y);
            } else {
                pos_y = point_a.y - (int)(i * stepFrequency_y);
            }

            if (point_a.z <= point_b.z) {
                pos_z = point_a.z + (int)(i * stepFrequency_z);
            } else {
                pos_z = point_a.z - (int)(i * stepFrequency_z);
            }
            Index index_i = new Index(pos_x, pos_y, pos_z);
            int dist_i = distance(point_a, index_i);
            line[dist_i] = index_i;
        }
        return line;
    }

    static public Dictionary<int, Index> line_xz(Index point_a, Index point_b, int length) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_z = Math.Abs(point_a.z - point_b.z);
        int dist_max = Math.Max(dist_x, dist_z);

        Dictionary<int, Index> line = new Dictionary<int, Index>();
        double stepFrequency_x = dist_x / dist_max;
        double stepFrequency_z = dist_z / dist_max;
        for (int i = -length; i <= length; i++) {
            int pos_x;
            int pos_z;
            if (point_a.x <= point_b.x) {
                pos_x = point_a.x + (int)(i * stepFrequency_x);
            } else {
                pos_x = point_a.x - (int)(i * stepFrequency_x);
            }
            
            if (point_a.y <= point_b.y) {
                pos_z = point_a.y + (int)(i * stepFrequency_z);
            } else {
                pos_z = point_a.y - (int)(i * stepFrequency_z);
            }
            Index index_i = new Index(pos_x, point_a.y, pos_z);
            int dist_i = distance_xy(point_a, index_i);
            line.Add(dist_i, index_i);
        }
        return line;
    }

    static public Dictionary<int, Index> linePerp_xz(Index point_a, Index point_b, int length) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_z = Math.Abs(point_a.z - point_b.z);
        int dist_max = Math.Max(dist_x, dist_z);

        Dictionary<int, Index> line = new Dictionary<int, Index>();
        double stepFrequency_x = dist_x / dist_max;
        double stepFrequency_z = dist_z / dist_max;
        for (int i = -length; i <= length; i++) {
            int pos_x;
            int pos_z;
            if (point_a.x <= point_b.x) {
                pos_z = point_a.y - (int)(i * stepFrequency_z);
            } else {
                pos_z = point_a.y + (int)(i * stepFrequency_z);
            }
            
            if (point_a.y <= point_b.y) {
                pos_x = point_a.x + (int)(i * stepFrequency_x);
            } else {
                pos_x = point_a.x - (int)(i * stepFrequency_x);
            }
            Index index_i = new Index(pos_x, point_a.y, pos_z);
            int dist_i = distance_xy(point_a, index_i);
            line.Add(dist_i, index_i);
        }
        return line;
    }

    static public Vector3 polarToCartesian(float radius, float piRadian) {
        float x = (float)(radius * Math.Cos(Math.PI * piRadian));
        float y = (float)(radius * Math.Sin(Math.PI * piRadian));
        return new Vector3(x, y, 0);
    }

    static public bool gameCoordExists(int x, int y, int z, GameCoord[,,] gameBoard) {
        return (x < gameBoard.GetLength(0) && y < gameBoard.GetLength(1) && z < gameBoard.GetLength(2));
    }

    static public void printStore(Dictionary<string, Value> store) {
        Debug.Log("Printing Store");
        foreach(KeyValuePair<string, Value> valuePair in store) {
            Debug.Log("(" + valuePair.Key + ":" + valuePair.Value.valueType + ")");
        }
    }

    static public void printEnv(Dictionary<string, string> env) {
        Debug.Log("Printing Env");
        foreach(KeyValuePair<string, string> valuePair in env) {
            Debug.Log("(" + valuePair.Key + ":" + valuePair.Value + ")");
        }
    }

    static public void printEnvStore(Dictionary<string, string> env, Dictionary<string, Value> store) {
        Debug.Log("Printing Env and Store");
        foreach(KeyValuePair<string, string> valuePair in env) {
            string valueType = store[valuePair.Value].valueType;
            Debug.Log("(" + valuePair.Key + ":" + valuePair.Value + ":" + valueType + ")");
        }
    }

    static public string readTextFile(string file_path) {
        StreamReader reader = new StreamReader(file_path);

        string text = "";

        while(!reader.EndOfStream) {
            text += reader.ReadLine( ) + "\n";
        }

        reader.Close( );  
        return text;
    }

    static public Action createAction(
        Dictionary<string, string> strings, 
        Dictionary<string, bool> bools,
        Dictionary<string, int> ints,
        Dictionary<string, List<string>> string_lists,
        Dictionary<string, List<Action>> action_Lists,
        Dictionary<string, List<Effect>> effect_lists) 
    {
        Action return_action = new Action();

        if (strings != null) {
            foreach(KeyValuePair<string, string> string_arg in strings) {
                switch(string_arg.Key) {
                    case "name":
                        return_action.name = string_arg.Value;
                        break; 
                }
            }
        }
        
        if (bools != null) {
            foreach(KeyValuePair<string, bool> bool_arg in bools) {
                switch(bool_arg.Key) {
                    case "relational":
                        return_action.relational = bool_arg.Value;
                        break; 
                    case "selfTargetable":
                        return_action.selfTargetable = bool_arg.Value;
                        break; 
                    case "targeted":
                        return_action.targeted = bool_arg.Value;
                        break; 
                    case "targeted_repeat":
                        return_action.targeted_repeat = bool_arg.Value;
                        break; 
                    case "conditional_repeat":
                        return_action.conditional_repeat = bool_arg.Value;
                        break; 
                    case "conditional_targeted_repeat":
                        return_action.conditional_targeted_repeat = bool_arg.Value;
                        break; 
                }
            }
        }
        
        if (ints != null) {
            foreach(KeyValuePair<string, int> int_arg in ints) {
                switch(int_arg.Key) {
                    case "actionType":
                        return_action.actionType = int_arg.Value;
                        break; 
                        case "maxRange":
                        return_action.maxRange = int_arg.Value;
                        break; 
                        case "minRange":
                        return_action.minRange = int_arg.Value;
                        break; 
                        case "aoe":
                        return_action.aoe = int_arg.Value;
                        break; 
                        case "conditional_aoe":
                        return_action.conditional_aoe = int_arg.Value;
                        break; 
                }
            }
        }

        if (string_lists != null) {
            foreach(KeyValuePair<string, List<string>> string_list_arg in string_lists) {
                switch(string_list_arg.Key) {
                    case "show_conditions":
                        return_action.show_conditions = string_list_arg.Value;
                        break; 
                    case "available_conditions":
                        return_action.available_conditions = string_list_arg.Value;
                        break; 
                    case "call_conditions":
                        return_action.call_conditions = string_list_arg.Value;
                        break; 
                    case "conditions":
                        return_action.conditions = string_list_arg.Value;
                        break; 
                }
            }
        }
        
        if (action_Lists != null) {
            foreach(KeyValuePair<string, List<Action>> action_list_arg in action_Lists) {
                switch(action_list_arg.Key) {
                    case "aoe_relational_actions":
                        return_action.aoe_relational_actions = action_list_arg.Value;
                        break; 
                    case "aoe_targeted_actions":
                        return_action.aoe_targeted_actions = action_list_arg.Value;
                        break; 
                    case "followup_actions":
                        return_action.followup_actions = action_list_arg.Value;
                        break; 
                    case "target_followup_actions":
                        return_action.target_followup_actions = action_list_arg.Value;
                        break; 
                    case "targeted_followup_actions":
                        return_action.targeted_followup_actions = action_list_arg.Value;
                        break; 
                    case "target_targeted_followup_actions":
                        return_action.target_targeted_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_aoe_relational_actions":
                        return_action.conditional_aoe_relational_actions = action_list_arg.Value;
                        break; 
                    case "conditional_aoe_targeted_actions":
                        return_action.conditional_aoe_targeted_actions = action_list_arg.Value;
                        break; 
                    case "conditional_followup_actions":
                        return_action.conditional_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_target_followup_actions":
                        return_action.conditional_target_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_targeted_followup_actions":
                        return_action.conditional_targeted_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_target_targeted_followup_actions":
                        return_action.conditional_target_targeted_followup_actions = action_list_arg.Value;
                        break; 
                }
            }
        }

        if (effect_lists != null) {
            foreach(KeyValuePair<string, List<Effect>> effect_list_arg in effect_lists) {
                switch(effect_list_arg.Key) {
                    case "effects":
                        return_action.effects = effect_list_arg.Value;
                        break; 
                    case "conditional_effects":
                        return_action.conditional_effects = effect_list_arg.Value;
                        break;
                }
            }
        }

        return return_action;
    }

    static public Effect createEffect(
        Dictionary<string, string> strings, 
        Dictionary<string, bool> bools,
        Dictionary<string, int> ints,
        Dictionary<string, List<string>> string_lists,
        Dictionary<string, List<Action>> action_Lists) 
    {
        Effect return_effect = new Effect();

        if (strings != null) {
            foreach(KeyValuePair<string, string> string_arg in strings) {
                switch(string_arg.Key) {
                    case "name":
                        return_effect.name = string_arg.Value;
                        break; 
                    case "target_displace":
                        return_effect.target_displace = string_arg.Value;
                        break; 
                    case "target_displace_perp":
                        return_effect.target_displace_perp = string_arg.Value;
                        break; 
                    case "target_displace_para":
                        return_effect.target_displace_para = string_arg.Value;
                        break; 
                    case "target_displace_alt":
                        return_effect.target_displace_alt = string_arg.Value;
                        break; 
                    case "self_displace":
                        return_effect.self_displace = string_arg.Value;
                        break; 
                    case "self_displace_perp":
                        return_effect.self_displace_perp = string_arg.Value;
                        break; 
                    case "self_displace_para":
                        return_effect.self_displace_para = string_arg.Value;
                        break; 
                    case "self_displace_alt":
                        return_effect.self_displace_alt = string_arg.Value;
                        break; 
                    case "conditional_target_displace":
                        return_effect.conditional_target_displace = string_arg.Value;
                        break; 
                    case "conditional_target_displace_perp":
                        return_effect.conditional_target_displace_perp = string_arg.Value;
                        break; 
                    case "conditional_target_displace_para":
                        return_effect.conditional_target_displace_para = string_arg.Value;
                        break; 
                    case "conditional_target_displace_alt":
                        return_effect.conditional_target_displace_alt = string_arg.Value;
                        break; 
                    case "conditional_self_displace":
                        return_effect.conditional_self_displace = string_arg.Value;
                        break; 
                    case "conditional_self_displace_perp":
                        return_effect.conditional_self_displace_perp = string_arg.Value;
                        break; 
                    case "conditional_self_displace_para":
                        return_effect.conditional_self_displace_para = string_arg.Value;
                        break; 
                    case "conditional_self_displace_alt":
                        return_effect.conditional_self_displace_alt = string_arg.Value;
                        break; 
                }
            }
        }
        
        if (bools != null) {
            foreach(KeyValuePair<string, bool> bool_arg in bools) {
                switch(bool_arg.Key) {
                    case "relational":
                        return_effect.relational = bool_arg.Value;
                        break; 
                    case "targeted":
                        return_effect.targeted = bool_arg.Value;
                        break; 
                    case "stacks":
                        return_effect.stacks = bool_arg.Value;
                        break; 
                    case "instant":
                        return_effect.instant = bool_arg.Value;
                        break; 
                }
            }
        }
        
        if (ints != null) {
            foreach(KeyValuePair<string, int> int_arg in ints) {
                switch(int_arg.Key) {
                    case "frequency":
                        return_effect.frequency = int_arg.Value;
                        break; 
                }
            }
        }

        if (string_lists != null) {
            foreach(KeyValuePair<string, List<string>> string_list_arg in string_lists) {
                switch(string_list_arg.Key) {
                    case "conditions":
                        return_effect.conditions = string_list_arg.Value;
                        break; 
                    case "endConditions":
                        return_effect.endConditions = string_list_arg.Value;
                        break; 
                    case "scripts":
                        return_effect.scripts = string_list_arg.Value;
                        break; 
                    case "conditional_scripts":
                        return_effect.conditional_scripts = string_list_arg.Value;
                        break; 
                }
            }
        }
        
        if (action_Lists != null) {
            foreach(KeyValuePair<string, List<Action>> action_list_arg in action_Lists) {
                switch(action_list_arg.Key) {
                    case "followup_actions":
                        return_effect.followup_actions = action_list_arg.Value;
                        break; 
                    case "target_followup_actions":
                        return_effect.target_followup_actions = action_list_arg.Value;
                        break; 
                    case "targeted_followup_actions":
                        return_effect.targeted_followup_actions = action_list_arg.Value;
                        break; 
                    case "target_targeted_followup_actions":
                        return_effect.target_targeted_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_followup_actions":
                        return_effect.conditional_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_target_followup_actions":
                        return_effect.conditional_target_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_targeted_followup_actions":
                        return_effect.conditional_targeted_followup_actions = action_list_arg.Value;
                        break; 
                    case "conditional_target_targeted_followup_actions":
                        return_effect.conditional_target_targeted_followup_actions = action_list_arg.Value;
                        break; 
                }
            }
        }

        return return_effect;
    }

    public static void printIgVariables(GameObject ig) {
        TokenScript tokenScript = ig.GetComponent<TokenScript>();
        ShapeScript shapeScript = ig.GetComponent<ShapeScript>();
        if (tokenScript != null) {
            foreach (KeyValuePair<string, Value> var in tokenScript.variables) {
                string value = "";
                switch(var.Value.valueType) {
                    case "int":
                        value = var.Value.vInt.ToString();
                        break;
                    case "double":
                        value = var.Value.vDouble.ToString();
                        break;
                    case "bool":
                        value = var.Value.vBool.ToString();
                        break;
                    case "string":
                        value = var.Value.vString;
                        break;
                }
                Debug.Log(var.Key + " : " + value);
            }
        } else if (shapeScript != null) {

        }
    }

    public static string ValueToString(Value value) {
        switch(value.valueType) {
            case "string":
                return value.vString;
            case "int":
                return value.vInt.ToString();
            case "double":
                return value.vDouble.ToString();
            case "ig":
                return String.Concat("<ig $", value.vIg.name, ">");
            case "function":
                return String.Concat("<function>");
            case "List":
                string listString = "[";
                foreach (Value val in value.vList) {
                    listString += ValueToString(val) + ", ";
                }
                if (listString != "[") {
                    listString = listString.Substring(0, listString.Length - 2);
                }
                listString += "]";
                return listString;
            default:
                return "<Error: Got an unknown value Type>";
        }
    }
}