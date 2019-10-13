using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour {
    static public void resolveAction(ref GameObject self, Action action, ref GameEnv gameEnv) {
        Debug.Log("Resolve Action");
        resolveEffects(ref self, action.effects, ref gameEnv);
        resolveAoe(ref self, self.GetComponent<TokenScript>().index, action.aoe, action.aoe_relational_actions, action.aoe_targeted_actions, ref gameEnv);
        
        if (action.conditions != null && resolveConditions(self, action.conditions, gameEnv)) {
            Debug.Log("Passed Conditions");
            resolveEffects(ref self, action.conditional_effects, ref gameEnv);
            resolveAoe(ref self, self.GetComponent<TokenScript>().index, action.conditional_aoe, action.conditional_aoe_relational_actions, action.conditional_aoe_targeted_actions, ref gameEnv);
            resolveActions(ref self, action.conditional_followup_actions, ref gameEnv);
        }

        resolveActions(ref self, action.followup_actions, ref gameEnv);
    }

    static public void resolveRelationalAction(ref GameObject self, ref GameObject target, Action action, ref GameEnv gameEnv) {
        resolveRelationalEffects(ref self, ref target, action.effects, ref gameEnv);
        resolveAoe(ref self, target.GetComponent<TokenScript>().index, action.aoe, action.aoe_relational_actions, action.aoe_targeted_actions, ref gameEnv);

        if (action.conditions != null && resolveRelationalConditions(self, target, action.conditions, gameEnv)) {
            resolveRelationalEffects(ref self, ref target, action.conditional_effects, ref gameEnv);
            resolveAoe(ref self, target.GetComponent<TokenScript>().index, action.conditional_aoe, action.conditional_aoe_relational_actions, action.conditional_aoe_targeted_actions, ref gameEnv);
            resolveRelationalActions(ref self, ref target, action.conditional_followup_actions, ref gameEnv);
            resolveRelationalActions(ref target, ref self, action.conditional_target_followup_actions, ref gameEnv);
        }

        resolveRelationalActions(ref self, ref target, action.followup_actions, ref gameEnv);
        resolveRelationalActions(ref target, ref self, action.target_followup_actions, ref gameEnv);
    }

    static public void resolveTargetedAction(ref GameObject self, ref GameObject target, Action action, ref GameEnv gameEnv) {
        resolveTargetedEffects(ref self, ref target, action.effects, ref gameEnv);
        resolveAoe(ref self, target.GetComponent<ShapeScript>().index, action.aoe, action.aoe_relational_actions, action.aoe_targeted_actions, ref gameEnv);
        resolveTargetedActions(ref self, ref target, action.followup_actions, ref gameEnv);

        if (action.conditions != null && resolveTargetedConditions(self, target, action.conditions, gameEnv)) {
            resolveTargetedEffects(ref self, ref target, action.conditional_effects, ref gameEnv);
            resolveAoe(ref self, target.GetComponent<ShapeScript>().index, action.conditional_aoe, action.conditional_aoe_relational_actions, action.conditional_aoe_targeted_actions, ref gameEnv);
        }

        resolveTargetedActions(ref self, ref target, action.conditional_followup_actions, ref gameEnv);
    }

    static public void resolveActions(ref GameObject self, List<Action> actions, ref GameEnv gameEnv) {
        if (actions != null) {
            Debug.Log("Resolve Actions");
            foreach (Action action in actions) {
                if (!action.relational && !action.targeted) {
                    resolveAction(ref self, action, ref gameEnv);
                } else {
                    // Print Error
                }
            }
        }
    }

    static public void resolveRelationalActions(ref GameObject self, ref GameObject target, List<Action> actions, ref GameEnv gameEnv) {
        if (actions != null) {
            foreach (Action action in actions)
            {
                if (!action.relational && !action.targeted) {
                    resolveAction(ref self, action, ref gameEnv);
                } else if (action.relational) {
                    resolveRelationalAction(ref self, ref target, action, ref gameEnv);
                } else {
                    // Print Error
                }
            } 
        }
    }

    static public void resolveTargetedActions(ref GameObject self, ref GameObject target, List<Action> actions, ref GameEnv gameEnv) {
        if (actions != null) {
            foreach (Action action in actions) {
                if (!action.relational && !action.targeted) {
                    resolveAction(ref self, action, ref gameEnv);
                } else if (action.relational) {
                    resolveTargetedAction(ref self, ref target, action, ref gameEnv);
                } else {
                    // Print Error
                }
            } 
        }
    }

    static public bool resolveConditions(GameObject self, List<string> conditions, GameEnv gameEnv) {
        if (conditions != null) {
        foreach (string condition in conditions) {
                Value conditionValue = DaedScript.evaluateSelfToken(condition, ref self, ref gameEnv);
                if (conditionValue.valueType == "bool" && !conditionValue.vBool) {
                    return false;
                }
                else if (conditionValue.valueType != "bool") {
                    // print an error
                    return false;
                }
            }
        }
        return true;
    }

    static public bool resolveRelationalConditions(GameObject self, GameObject target, List<string> conditions, GameEnv gameEnv)
    {
        if (conditions != null) {
            foreach (string condition in conditions) {
                Value conditionValue = DaedScript.evaluateSelfTokenTargetToken(condition, ref self, ref target, ref gameEnv);
                if (conditionValue.valueType == "bool" && !conditionValue.vBool) {
                    return false;
                }
                else if (conditionValue.valueType != "bool") {
                    // print an error
                    return false;
                }
            }
        }
        return true;
    }

    static public bool resolveTargetedConditions(GameObject self, GameObject target, List<string> conditions, GameEnv gameEnv) {
        if (conditions != null) {
            foreach (string condition in conditions) {
                Value conditionValue = DaedScript.evaluateSelfTokenTargetCube(condition, ref self, ref target, ref gameEnv);
                if (conditionValue.valueType == "bool" && !conditionValue.vBool) {
                    return false;
                }
                else if (conditionValue.valueType != "bool") {
                    // print an error
                    return false;
                }
            }
        }
        return true;
    }

    static public void resolveAoe(ref GameObject self, Index index, int radius, List<Action> relationalActions, List<Action> targetedActions, ref GameEnv gameEnv) {
        if (radius != -1) {
            Debug.Log("Resolve Aoe");
            List<GameObject> nearbyTokens = detectTokens(self.GetComponent<TokenScript>().index, radius, gameEnv);
            nearbyTokens.Remove(self);
            foreach (GameObject nearbyToken in nearbyTokens) {
                GameObject nearbyTokenRef = nearbyToken;
                resolveRelationalActions(ref self, ref nearbyTokenRef, relationalActions, ref gameEnv);
            }

            List<GameObject> nearbyCubes = detectCubes(self.GetComponent<TokenScript>().index, radius, gameEnv);
            foreach (GameObject nearbyCube in nearbyCubes) {
                GameObject nearbyCubeRef = nearbyCube;
                resolveTargetedActions(ref self, ref nearbyCubeRef, targetedActions, ref gameEnv);
            }
        }
    }

    static public List<GameObject> detectTokens(Index center, int radius, GameEnv gameEnv) {
        List<GameObject> nearbyTokens = new List<GameObject>();
        int z = center.z;

        int start_x = center.x - radius;
        int end_x = center.x + radius;
        int start_y = center.y - radius;
        int end_y = center.y + radius;

        for (int i = start_x; i <= end_x; i++) {
            for (int j = start_y; j <= end_y; j++) {
                Index cur_index = new Index(i, j, z);
				if (Utils.distance(center, cur_index) <= radius) {
					GameCoord gameCoord = gameEnv.mapScript.gameBoard[cur_index.x, cur_index.y, cur_index.z];
					nearbyTokens.AddRange(gameCoord.tokens);
				}
            }
        }
        return nearbyTokens;
    }

    static public List<GameObject> detectCubes(Index center, int radius, GameEnv gameEnv) {
        List<GameObject> nearbyCubes = new List<GameObject>();
        int z = center.z;

        int start_x = center.x - radius;
        int end_x = center.x + radius;
        int start_y = center.y - radius;
        int end_y = center.y + radius;

        for (int i = start_x; i <= end_x; i++) {
            for (int j = start_y; j <= end_y; j++) {
                Index cur_index = new Index(i, j, z);
				if (Utils.distance(center, cur_index) <= radius) {
					GameCoord gameCoord = gameEnv.mapScript.gameBoard[cur_index.x, cur_index.y, cur_index.z];
					nearbyCubes.Add(gameCoord.shape);
				}
            }
        }
        return nearbyCubes;
    }

    static public void resolveEffect(ref GameObject self, Effect effect, ref GameEnv gameEnv) {
        Debug.Log("Resolve Effect");
        if (effect.instant) {
            procEffect(ref self, effect, ref gameEnv);
        } else {
            effect.timeLeft = effect.frequency;
            if (effect.stacks) {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
            } else {
                effect.givenName = effect.name;
            }
            self.GetComponent<TokenScript>().effects[effect.givenName] = effect;
        }
    }

    static public void resolveEffects(ref GameObject self, List<Effect> effects, ref GameEnv gameEnv) {
        if (effects != null) {
            Debug.Log("Resolve Effects");
            foreach (Effect effect in effects) {
                if (!effect.relational && !effect.targeted) {
                    resolveEffect(ref self, effect, ref gameEnv);
                } else {
                    // print error
                }
            }
        }
    }

    static public void resolveRelationalEffects(ref GameObject self, ref GameObject target, List<Effect> effects, ref GameEnv gameEnv) {
        if (effects != null) {
            foreach (Effect effect in effects) {
                if (!effect.relational && !effect.targeted) {
                    resolveEffect(ref self, effect, ref gameEnv);
                } else if (effect.relational) {
                    resolveRelationalEffect(ref self, ref target, effect, ref gameEnv);
                } else {
                    // print error
                }
            }
        }  
    }

    static public void resolveTargetedEffects(ref GameObject self, ref GameObject target, List<Effect> effects, ref GameEnv gameEnv) {
        if (effects != null) {
            foreach (Effect effect in effects) {
                if (!effect.relational && !effect.targeted) {
                    resolveEffect(ref self, effect, ref gameEnv);
                } else if (effect.targeted) {
                    resolveTargetedEffect(ref self, ref target, effect, ref gameEnv);
                } else {
                    // print error
                }
            }
        }
    }

    static public void resolveRelationalEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv) {
		if (effect.instant) {
            procRelationalEffect(ref self, ref target, effect, ref gameEnv);
        } else {
            effect.relative = self;
            effect.timeLeft = effect.frequency;
            if (effect.stacks) {
                effect.givenName = effect.name  + "_" + System.Guid.NewGuid();
            } else {
                effect.givenName = effect.name;
            }
            target.GetComponent<TokenScript>().effects[effect.givenName] = effect;
        }
    }

    

    static public void resolveTargetedEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv) {
		if (effect.instant) {
            procTargetedEffect(ref self, ref target, effect, ref gameEnv);
        } else {
            effect.relative = self;
            effect.timeLeft = effect.frequency;
            if (effect.stacks) {
                effect.givenName = effect.name  + "_" + System.Guid.NewGuid();
            } else {
                effect.givenName = effect.name;
            }
            target.GetComponent<ShapeScript>().effects[effect.givenName] = effect;
        }
    }

    static public void procEffect(ref GameObject self, Effect effect, ref GameEnv gameEnv) {
        Debug.Log("Proc Effect");
        TokenScript selfScript = self.GetComponent<TokenScript>();

        if (effect.scripts != null) {
            foreach (String script in effect.scripts) {
                Debug.Log(script);
                Utils.printEnvStore(gameEnv.env, gameEnv.store);
                Value scriptValue = DaedScript.evaluateSelfToken(script, ref self, ref gameEnv);
                if (scriptValue.errorMessage != null) {
                    Debug.Log(scriptValue.errorMessage);
                }
            }
        }
        

        if (effect.self_displace != null && effect.self_displace != "") {
            Value index_value = DaedScript.evaluateSelfToken(effect.self_displace, ref self, ref gameEnv);
            if (index_value.valueType == "list") {
                if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                    selfScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                } else {
                    // print an error
                }
            } else {
                // print an error
            }
        }

        if (effect.conditions != null && resolveConditions(self, effect.conditions, gameEnv)) {
            Debug.Log("Passed Conditions");
            if (effect.conditional_scripts != null) {
                foreach (String script in effect.conditional_scripts) {
                    Debug.Log(script);
                    Utils.printEnvStore(gameEnv.env, gameEnv.store);
                    Value scriptValue = DaedScript.evaluateSelfToken(script, ref self, ref gameEnv);
                    if (scriptValue.errorMessage != null) {
                        Debug.Log(scriptValue.errorMessage);
                    }
                }
            }

            if (effect.conditional_self_displace != null && effect.conditional_self_displace != "") {
                Value index_value = DaedScript.evaluateSelfToken(effect.conditional_self_displace, ref self, ref gameEnv);
                if (index_value.valueType == "list") {
                    if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                        selfScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                    } else {
                        // print an error
                    }
                } else {
                    // print an error
                }
            }

            resolveActions(ref self, effect.conditional_followup_actions, ref gameEnv);
            // resolveTargetedRactions(ref GameObject, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref GameObject, effect.targetedFollowupTactions);
        }

        resolveActions(ref self, effect.followup_actions, ref gameEnv);
        // resolveTargetedRactions(ref GameObject, effect.targetedFollowupRactions);
        // resolveTargetedTactions(ref GameObject, effect.targetedFollowupTactions);
    }

	static public void procRelationalEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv) {
        TokenScript selfScript = self.GetComponent<TokenScript>();
        TokenScript targetScript = target.GetComponent<TokenScript>();

        if (effect.scripts != null) {
            foreach (string script in effect.scripts) {
                DaedScript.evaluateSelfTokenTargetToken(script, ref self, ref target, ref gameEnv);
            }

            if (effect.self_displace != null && effect.self_displace != "") {
                Value index_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace, ref self, ref target, ref gameEnv);
                if (index_value.valueType == "list") {
                    if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                        selfScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                    } else {
                        // print an error
                    }
                } else {
                    // print an error
                }
            }
        } 

        if (effect.self_displace_para != null && effect.self_displace_para != "") {
            Value self_para_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace_para, ref self, ref target, ref gameEnv);
            if (self_para_value.valueType == "int") {
                int para_dist = self_para_value.vInt;
                Dictionary<int, Index> para_line = Utils.line(selfScript.index, targetScript.index, para_dist);
                if (para_line.ContainsKey(para_dist)) {
                    Index para_index = para_line[para_dist];
                    selfScript.moveTo(para_index);
                } else {
                    Index para_index = para_line[para_dist - 1];
                    selfScript.moveTo(para_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.self_displace_perp != null && effect.self_displace_perp != "") {
            Value self_perp_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace_perp, ref self, ref target, ref gameEnv);
            if (self_perp_value.valueType == "int") {
                int perp_dist = self_perp_value.vInt;
                Dictionary<int, Index> perp_line = Utils.line(selfScript.index, targetScript.index, perp_dist);
                if (perp_line.ContainsKey(perp_dist)) {
                    Index perp_index = perp_line[perp_dist];
                    selfScript.moveTo(perp_index);
                } else {
                    Index perp_index = perp_line[perp_dist - 1];
                    selfScript.moveTo(perp_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.self_displace_alt != null && effect.self_displace_alt != "") {
            Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace_alt, ref self, ref target, ref gameEnv);
            if (self_alt_value.valueType == "int") {
                Index endIndex = new Index(selfScript.index);
                endIndex.y += self_alt_value.vInt;
                selfScript.moveTo(endIndex);
            } else {
                // print an error
            }
        }

        if (effect.target_displace != null && effect.target_displace != "") {
            Value index_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace, ref self, ref target, ref gameEnv);
            if (index_value.valueType == "list") {
                if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                    targetScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                } else {
                    // print an error
                }
            } else {
                // print an error
            }
        }
        
        if (effect.target_displace_para != null && effect.target_displace_para != "") {
            Value target_para_value = DaedScript.evaluateSelfTokenTargetToken(effect.target_displace_para, ref self, ref target, ref gameEnv);
            if (target_para_value.valueType == "int") {
                int para_dist = target_para_value.vInt;
                Dictionary<int, Index> para_line = Utils.line(targetScript.index, selfScript.index, para_dist);
                if (para_line.ContainsKey(para_dist)) {
                    Index para_index = para_line[para_dist];
                    targetScript.moveTo(para_index);
                } else {
                    Index para_index = para_line[para_dist - 1];
                    targetScript.moveTo(para_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.target_displace_perp != null && effect.target_displace_perp != "") {
            Value target_perp_value = DaedScript.evaluateSelfTokenTargetToken(effect.target_displace_perp, ref self, ref target, ref gameEnv);
            if (target_perp_value.valueType == "int") {
                int perp_dist = target_perp_value.vInt;
                Dictionary<int, Index> perp_line = Utils.line(target.GetComponent<TokenScript>().index, self.GetComponent<TokenScript>().index, perp_dist);
                if (perp_line.ContainsKey(perp_dist)) {
                    Index perp_index = perp_line[perp_dist];
                    targetScript.moveTo(perp_index);
                } else {
                    Index perp_index = perp_line[perp_dist - 1];
                    targetScript.moveTo(perp_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.target_displace_alt != null && effect.target_displace_alt != "") {
            Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(effect.target_displace_alt, ref self, ref target, ref gameEnv);
            if (self_alt_value.valueType == "int") {
                Index endIndex = new Index(targetScript.index);
                endIndex.y += self_alt_value.vInt;
                targetScript.moveTo(endIndex);
            } else {
                // print an error
            }
        }

        if (effect.conditions != null && resolveRelationalConditions(self, target, effect.conditions, gameEnv)) {
            if (effect.conditional_scripts != null) {
                foreach (string script in effect.conditional_scripts) {
                    DaedScript.evaluateSelfTokenTargetToken(script, ref self, ref target, ref gameEnv);
                }

                if (effect.conditional_self_displace != null && effect.conditional_self_displace != "") {
                    Value index_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_self_displace, ref self, ref target, ref gameEnv);
                    if (index_value.valueType == "list") {
                        if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                            selfScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                        } else {
                            // print an error
                        }
                    } else {
                        // print an error
                    }
                }
            } 

            if (effect.conditional_self_displace_para != null && effect.conditional_self_displace_para != "") {
                Value self_para_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_self_displace_para, ref self, ref target, ref gameEnv);
                if (self_para_value.valueType == "int") {
                    int para_dist = self_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(selfScript.index, targetScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) {
                        Index para_index = para_line[para_dist];
                        selfScript.moveTo(para_index);
                    } else {
                        Index para_index = para_line[para_dist - 1];
                        selfScript.moveTo(para_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_self_displace_perp != null && effect.conditional_self_displace_perp != "") {
                Value self_perp_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_self_displace_perp, ref self, ref target, ref gameEnv);
                if (self_perp_value.valueType == "int") {
                    int perp_dist = self_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(selfScript.index, targetScript.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) {
                        Index perp_index = perp_line[perp_dist];
                        selfScript.moveTo(perp_index);
                    } else {
                        Index perp_index = perp_line[perp_dist - 1];
                        selfScript.moveTo(perp_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_self_displace_alt != null && effect.conditional_self_displace_alt != "") {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_self_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") {
                    Index endIndex = new Index(selfScript.index);
                    endIndex.y += self_alt_value.vInt;
                    selfScript.moveTo(endIndex);
                } else {
                    // print an error
                }
            }

            if (effect.conditional_target_displace != null && effect.conditional_target_displace != "") {
                Value index_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_self_displace, ref self, ref target, ref gameEnv);
                if (index_value.valueType == "list") {
                    if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                        targetScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                    } else {
                        // print an error
                    }
                } else {
                    // print an error
                }
            }
            
            if (effect.conditional_target_displace_para != null && effect.conditional_target_displace_para != "") {
                Value target_para_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_target_displace_para, ref self, ref target, ref gameEnv);
                if (target_para_value.valueType == "int") {
                    int para_dist = target_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(targetScript.index, selfScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) {
                        Index para_index = para_line[para_dist];
                        targetScript.moveTo(para_index);
                    } else {
                        Index para_index = para_line[para_dist - 1];
                        targetScript.moveTo(para_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_target_displace_perp != null && effect.conditional_target_displace_perp != "") {
                Value target_perp_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_target_displace_perp, ref self, ref target, ref gameEnv);
                if (target_perp_value.valueType == "int") {
                    int perp_dist = target_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(target.GetComponent<TokenScript>().index, self.GetComponent<TokenScript>().index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) {
                        Index perp_index = perp_line[perp_dist];
                        targetScript.moveTo(perp_index);
                    } else {
                        Index perp_index = perp_line[perp_dist - 1];
                        targetScript.moveTo(perp_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_target_displace_alt != null && effect.conditional_target_displace_alt != "") {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(effect.conditional_target_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") {
                    Index endIndex = new Index(targetScript.index);
                    endIndex.y += self_alt_value.vInt;
                    targetScript.moveTo(endIndex);
                } else {
                    // print an error
                }
            }

            resolveRelationalActions(ref self, ref target, effect.conditional_followup_actions, ref gameEnv);
            resolveRelationalActions(ref target, ref self, effect.conditional_target_followup_actions, ref gameEnv);
        }

        resolveRelationalActions(ref self, ref target, effect.followup_actions, ref gameEnv);
        resolveRelationalActions(ref target, ref self, effect.target_followup_actions, ref gameEnv);
    }

	static public void procTargetedEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv)
    {
        TokenScript selfScript = self.GetComponent<TokenScript>();
        ShapeScript targetScript = target.GetComponent<ShapeScript>();

        if (effect.scripts != null) {
            foreach (string script in effect.scripts) {
                    DaedScript.evaluateSelfTokenTargetCube(script, ref self, ref target, ref gameEnv);
            }
        }

        if (effect.self_displace != null && effect.self_displace != "") {
            Value index_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace, ref self, ref target, ref gameEnv);
            if (index_value.valueType == "list") {
                if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                    selfScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                } else {
                    // print an error
                }
            } else {
                // print an error
            }
        }

        if (effect.self_displace_para != null && effect.self_displace_para != "") {
            Value self_para_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_para, ref self, ref target, ref gameEnv);
            if (self_para_value.valueType == "int") {
                int para_dist = self_para_value.vInt;
                Dictionary<int, Index> para_line = Utils.line(selfScript.index, targetScript.index, para_dist);
                if (para_line.ContainsKey(para_dist)) {
                    Index para_index = para_line[para_dist];
                    selfScript.moveTo(para_index);
                } else {
                    Index para_index = para_line[para_dist - 1];
                    selfScript.moveTo(para_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.self_displace_perp != null && effect.self_displace_perp != "") {
            Value self_perp_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_perp, ref self, ref target, ref gameEnv);
            if (self_perp_value.valueType == "int") {
                int perp_dist = self_perp_value.vInt;
                Dictionary<int, Index> perp_line = Utils.line(selfScript.index, targetScript.index, perp_dist);
                if (perp_line.ContainsKey(perp_dist)) {
                    Index perp_index = perp_line[perp_dist];
                    selfScript.moveTo(perp_index);
                } else {
                    Index perp_index = perp_line[perp_dist - 1];
                    selfScript.moveTo(perp_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.self_displace_alt != null && effect.self_displace_alt != "") {
            Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_alt, ref self, ref target, ref gameEnv);
            if (self_alt_value.valueType == "int") {
                Index endIndex = new Index(selfScript.index);
                endIndex.y += self_alt_value.vInt;
                selfScript.moveTo(endIndex);
            } else {
                // print an error
            }
        }

        if (effect.target_displace != null && effect.target_displace != "") {
            Value index_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace, ref self, ref target, ref gameEnv);
            if (index_value.valueType == "list") {
                if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                    targetScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                } else {
                    // print an error
                }
            } else {
                // print an error
            }
        }
        
        if (effect.target_displace_para != null && effect.target_displace_para != "") {
            Value target_para_value = DaedScript.evaluateSelfTokenTargetCube(effect.target_displace_para, ref self, ref target, ref gameEnv);
            if (target_para_value.valueType == "int") {
                int para_dist = target_para_value.vInt;
                Dictionary<int, Index> para_line = Utils.line(targetScript.index, selfScript.index, para_dist);
                if (para_line.ContainsKey(para_dist)) {
                    Index para_index = para_line[para_dist];
                    targetScript.moveTo(para_index);
                } else {
                    Index para_index = para_line[para_dist - 1];
                    targetScript.moveTo(para_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.target_displace_perp != null && effect.target_displace_perp != "") {
            Value target_perp_value = DaedScript.evaluateSelfTokenTargetCube(effect.target_displace_perp, ref self, ref target, ref gameEnv);
            if (target_perp_value.valueType == "int") {
                int perp_dist = target_perp_value.vInt;
                Dictionary<int, Index> perp_line = Utils.line(targetScript.index, selfScript.index, perp_dist);
                if (perp_line.ContainsKey(perp_dist)) {
                    Index perp_index = perp_line[perp_dist];
                    targetScript.moveTo(perp_index);
                } else {
                    Index perp_index = perp_line[perp_dist - 1];
                    targetScript.moveTo(perp_index);
                }
            } else {
                // print an error
            }
        }

        if (effect.self_displace_alt != null && effect.self_displace_alt != "") {
            Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_alt, ref self, ref target, ref gameEnv);
            if (self_alt_value.valueType == "int") {
                Index endIndex = new Index(targetScript.index);
                endIndex.y += self_alt_value.vInt;
                targetScript.moveTo(endIndex);
            } else {
                // print an error
            }
        }

        if (effect.conditions != null && resolveTargetedConditions(self, target, effect.conditions, gameEnv)) {
            if (effect.conditional_scripts != null) {
                foreach (string script in effect.conditional_scripts) {
                        DaedScript.evaluateSelfTokenTargetCube(script, ref self, ref target, ref gameEnv);
                }
            }


            if (effect.conditional_self_displace != null && effect.conditional_self_displace != "") {
                Value index_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_self_displace, ref self, ref target, ref gameEnv);
                if (index_value.valueType == "list") {
                    if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                        selfScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                    } else {
                        // print an error
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_self_displace_para != null && effect.conditional_self_displace_para != "") {
                Value self_para_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_self_displace_para, ref self, ref target, ref gameEnv);
                if (self_para_value.valueType == "int") {
                    int para_dist = self_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(selfScript.index, targetScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) {
                        Index para_index = para_line[para_dist];
                        selfScript.moveTo(para_index);
                    } else {
                        Index para_index = para_line[para_dist - 1];
                        selfScript.moveTo(para_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_self_displace_perp != null && effect.conditional_self_displace_perp != "") {
                Value self_perp_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_self_displace_perp, ref self, ref target, ref gameEnv);
                if (self_perp_value.valueType == "int") {
                    int perp_dist = self_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(selfScript.index, targetScript.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) {
                        Index perp_index = perp_line[perp_dist];
                        selfScript.moveTo(perp_index);
                    } else {
                        Index perp_index = perp_line[perp_dist - 1];
                        selfScript.moveTo(perp_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_self_displace_alt != null && effect.conditional_self_displace_alt != "") {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_self_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") {
                    Index endIndex = new Index(selfScript.index);
                    endIndex.y += self_alt_value.vInt;
                    selfScript.moveTo(endIndex);
                } else {
                    // print an error
                }
            }

            if (effect.conditional_target_displace != null && effect.conditional_target_displace != "") {
                Value index_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_self_displace, ref self, ref target, ref gameEnv);
                if (index_value.valueType == "list") {
                    if (index_value.vList.Count == 3 && index_value.vList[0].valueType == "int" && index_value.vList[1].valueType == "int" && index_value.vList[2].valueType == "int") {
                        targetScript.moveTo(new Index(index_value.vList[0].vInt, index_value.vList[1].vInt, index_value.vList[2].vInt));
                    } else {
                        // print an error
                    }
                } else {
                    // print an error
                }
            }
            
            if (effect.conditional_target_displace_para != null && effect.conditional_target_displace_para != "") {
                Value target_para_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_target_displace_para, ref self, ref target, ref gameEnv);
                if (target_para_value.valueType == "int") {
                    int para_dist = target_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(targetScript.index, selfScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) {
                        Index para_index = para_line[para_dist];
                        targetScript.moveTo(para_index);
                    } else {
                        Index para_index = para_line[para_dist - 1];
                        targetScript.moveTo(para_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_target_displace_perp != null && effect.conditional_target_displace_perp != "") {
                Value target_perp_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_target_displace_perp, ref self, ref target, ref gameEnv);
                if (target_perp_value.valueType == "int") {
                    int perp_dist = target_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(targetScript.index, selfScript.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) {
                        Index perp_index = perp_line[perp_dist];
                        targetScript.moveTo(perp_index);
                    } else {
                        Index perp_index = perp_line[perp_dist - 1];
                        targetScript.moveTo(perp_index);
                    }
                } else {
                    // print an error
                }
            }

            if (effect.conditional_self_displace_alt != null && effect.conditional_self_displace_alt != "") {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(effect.conditional_self_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") {
                    Index endIndex = new Index(targetScript.index);
                    endIndex.y += self_alt_value.vInt;
                    targetScript.moveTo(endIndex);
                } else {
                    // print an error
                }
            }

            resolveTargetedActions(ref self, ref target, effect.conditional_followup_actions, ref gameEnv);
            resolveTargetedActions(ref target, ref self, effect.conditional_target_followup_actions, ref gameEnv);
        }

        resolveTargetedActions(ref self, ref target, effect.followup_actions, ref gameEnv);
        resolveTargetedActions(ref target, ref self, effect.target_followup_actions, ref gameEnv);
    }
}