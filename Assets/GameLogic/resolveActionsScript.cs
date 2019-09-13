using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour
{
    static public void resolveAction(ref GameObject self, Action action, ref GameEnv gameEnv)
    {
        if (resolveConditions(self, action.conditions, gameEnv))
        {
            resolveEffects(ref self, action.effects, ref gameEnv);
            if (action.aoe != 0) {
                resolveAoe(ref self, self.GetComponent<TokenScript>().index, action.aoe, action.aoe_relational_actions, action.aoe_targeted_actions, ref gameEnv);
            }

            resolveActions(ref self, action.followup_actions, ref gameEnv);
        }
    }

    static public void resolveRelationalAction(ref GameObject self, ref GameObject target, Action action, ref GameEnv gameEnv) {
        if (resolveRelationalConditions(self, target, action.conditions, gameEnv))
        {
            resolveRelationalEffects(ref self, ref target, action.effects, ref gameEnv);
            if (action.aoe != 0) {
                resolveAoe(ref self, target.GetComponent<TokenScript>().index, action.aoe, action.aoe_relational_actions, action.aoe_targeted_actions, ref gameEnv);
            }

            resolveRelationalActions(ref self, ref target, action.followup_actions, ref gameEnv);
        }
    }

    static public void resolveTargetedAction(ref GameObject self, ref GameObject target, Action action, ref GameEnv gameEnv) {
        if (resolveTargetedConditions(self, target, action.conditions, gameEnv))
        {
            resolveTargetedEffects(ref self, ref target, action.effects, ref gameEnv);
            if (action.aoe != 0) {
                resolveAoe(ref self, target.GetComponent<ShapeScript>().index, action.aoe, action.aoe_relational_actions, action.aoe_targeted_actions, ref gameEnv);
            }

            resolveRelationalActions(ref self, ref target, action.followup_actions, ref gameEnv);
        }
    }

    static public void resolveActions(ref GameObject self, List<Action> actions, ref GameEnv gameEnv)
    {
        foreach (Action action in actions)
        {
            if (!action.relational && !action.targeted) {
                resolveAction(ref self, action, ref gameEnv);
            } else {
                // Print Error
            }
        }
    }

    static public void resolveRelationalActions(ref GameObject self, ref GameObject target, List<Action> actions, ref GameEnv gameEnv) {
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

    static public void resolveTargetedActions(ref GameObject self, ref GameObject target, List<Action> actions, ref GameEnv gameEnv) {
        foreach (Action action in actions)
        {
            if (!action.relational && !action.targeted) {
                resolveAction(ref self, action, ref gameEnv);
            } else if (action.relational) {
                resolveTargetedAction(ref self, ref target, action, ref gameEnv);
            } else {
                // Print Error
            }
        } 
    }

    static public bool resolveConditions(GameObject self, List<string> conditions, GameEnv gameEnv)
    {
        foreach (string condition in conditions)
        {
            Value conditionValue = DaedScript.evaluateSelfToken(condition, ref self, ref gameEnv);
            if (conditionValue.valueType == "bool" && !conditionValue.vBool)
            {
                return false;
            }
            else if (conditionValue.valueType != "bool")
            {
                // print an error
                return false;
            }
        }
        return true;
    }

    static public bool resolveRelationalConditions(GameObject self, GameObject target, List<string> conditions, GameEnv gameEnv)
    {
        foreach (string condition in conditions)
        {
            Value conditionValue = DaedScript.evaluateSelfTokenTargetToken(condition, ref self, ref target, ref gameEnv);
            if (conditionValue.valueType == "bool" && !conditionValue.vBool)
            {
                return false;
            }
            else if (conditionValue.valueType != "bool")
            {
                // print an error
                return false;
            }
        }
        return true;
    }

    static public bool resolveTargetedConditions(GameObject self, GameObject target, List<string> conditions, GameEnv gameEnv)
    {
        foreach (string condition in conditions)
        {
            Value conditionValue = DaedScript.evaluateSelfTokenTargetCube(condition, ref self, ref target, ref gameEnv);
            if (conditionValue.valueType == "bool" && !conditionValue.vBool)
            {
                return false;
            }
            else if (conditionValue.valueType != "bool")
            {
                // print an error
                return false;
            }
        }
        return true;
    }

    static public void resolveAoe(ref GameObject self, Index index, int radius, List<Action> relationalActions, List<Action> targetedActions, ref GameEnv gameEnv)
    {
        List<GameObject> nearbyTokens = detectTokens(self.GetComponent<TokenScript>().index, radius, gameEnv);
        nearbyTokens.Remove(self);
        foreach (GameObject nearbyToken in nearbyTokens)
        {
            GameObject nearbyTokenRef = nearbyToken;
            resolveRelationalActions(ref self, ref nearbyTokenRef, relationalActions, ref gameEnv);
        }

        List<GameObject> nearbyCubes = detectCubes(self.GetComponent<TokenScript>().index, radius, gameEnv);
        foreach (GameObject nearbyCube in nearbyCubes)
        {
            GameObject nearbyCubeRef = nearbyCube;
            resolveTargetedActions(ref self, ref nearbyCubeRef, targetedActions, ref gameEnv);
        }
    }

    static public List<GameObject> detectTokens(Index center, int radius, GameEnv gameEnv)
    {
        List<GameObject> nearbyTokens = new List<GameObject>();
        int z = center.z;

        int start_x = center.x - radius;
        int end_x = center.x + radius;
        int start_y = center.y - radius;
        int end_y = center.y + radius;

        for (int i = start_x; i <= end_x; i++)
        {
            for (int j = start_y; j <= end_y; j++)
            {
                Index cur_index = new Index(i, j, z);
				if (Utils.distance(center, cur_index) <= radius) {
					GameCoord gameCoord = gameEnv.mapScript.gameBoard[cur_index.x, cur_index.y, cur_index.z];
					nearbyTokens.AddRange(gameCoord.tokens);
				}
            }
        }

        return nearbyTokens;
    }

    static public List<GameObject> detectCubes(Index center, int radius, GameEnv gameEnv)
    {
        List<GameObject> nearbyCubes = new List<GameObject>();
        int z = center.z;

        int start_x = center.x - radius;
        int end_x = center.x + radius;
        int start_y = center.y - radius;
        int end_y = center.y + radius;

        for (int i = start_x; i <= end_x; i++)
        {
            for (int j = start_y; j <= end_y; j++)
            {
                Index cur_index = new Index(i, j, z);
				if (Utils.distance(center, cur_index) <= radius) {
					GameCoord gameCoord = gameEnv.mapScript.gameBoard[cur_index.x, cur_index.y, cur_index.z];
					nearbyCubes.Add(gameCoord.shape);
				}
            }
        }

        return nearbyCubes;
    }

    static public void resolveEffect(ref GameObject self, Effect effect, ref GameEnv gameEnv)
    {
        if (effect.instant)
        {
            procEffect(ref self, effect, ref gameEnv);
        }
        else
        {
            effect.timeLeft = effect.frequency;
            if (effect.stacks)
            {
                effect.givenName = effect.name + "_" + System.Guid.NewGuid();
            }
            else
            {
                effect.givenName = effect.name;
            }
            self.GetComponent<TokenScript>().effects[effect.givenName] = effect;
        }
    }

    static public void resolveEffects(ref GameObject self, List<Effect> effects, ref GameEnv gameEnv)
    {
        foreach (Effect effect in effects)
        {
            if (!effect.relational && !effect.targeted) {
                resolveEffect(ref self, effect, ref gameEnv);
            } else {
                // print error
            }
        }
    }

    static public void resolveRelationalEffects(ref GameObject self, ref GameObject target, List<Effect> effects, ref GameEnv gameEnv)
    {
        foreach (Effect effect in effects)
        {
            if (!effect.relational && !effect.targeted) {
                resolveEffect(ref self, effect, ref gameEnv);
            } else if (effect.relational) {
                resolveRelationalEffect(ref self, ref target, effect, ref gameEnv);
            } else {
                // print error
            }
        }
    }

    static public void resolveTargetedEffects(ref GameObject self, ref GameObject target, List<Effect> effects, ref GameEnv gameEnv)
    {
        foreach (Effect effect in effects)
        {
            if (!effect.relational && !effect.targeted) {
                resolveEffect(ref self, effect, ref gameEnv);
            } else if (effect.targeted) {
                resolveTargetedEffect(ref self, ref target, effect, ref gameEnv);
            } else {
                // print error
            }
        }
    }

    static public void resolveRelationalEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv)
    {
		if (effect.instant)
        {
            procRelationalEffect(ref self, ref target, effect, ref gameEnv);
        }
        else
        {
            effect.relative = self;
            effect.timeLeft = effect.frequency;
            if (effect.stacks)
            {
                effect.givenName = effect.name  + "_" + System.Guid.NewGuid();
            }
            else
            {
                effect.givenName = effect.name;
            }
            target.GetComponent<TokenScript>().effects[effect.givenName] = effect;
        }
    }

    

    static public void resolveTargetedEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv)
    {
		if (effect.instant)
        {
            procTargetedEffect(ref self, ref target, effect, ref gameEnv);
        }
        else
        {
            effect.relative = self;
            effect.timeLeft = effect.frequency;
            if (effect.stacks)
            {
                effect.givenName = effect.name  + "_" + System.Guid.NewGuid();
            }
            else
            {
                effect.givenName = effect.name;
            }
            target.GetComponent<ShapeScript>().effects[effect.givenName] = effect;
        }
    }

    static public void procEffect(ref GameObject self, Effect effect, ref GameEnv gameEnv)
    {
        TokenScript selfScript = self.GetComponent<TokenScript>();

        Value conditionValue = DaedScript.evaluateSelfToken(effect.condition, ref self, ref gameEnv);
        if (conditionValue.valueType == "bool" && conditionValue.vBool)
        {
            DaedScript.evaluateSelfToken(effect.script, ref self, ref gameEnv);

            if (effect.displace)
            {
                Value x_value = DaedScript.evaluateSelfToken(effect.displace_x, ref self, ref gameEnv);
                Value y_value = DaedScript.evaluateSelfToken(effect.displace_y, ref self, ref gameEnv);
                Value z_value = DaedScript.evaluateSelfToken(effect.displace_z, ref self, ref gameEnv);
                if (x_value.valueType == "int" && y_value.valueType == "int" && z_value.valueType == "int")
                {
                    Index endIndex = new Index(selfScript.index);
                    endIndex.x += x_value.vInt;
                    endIndex.y += y_value.vInt;
                    endIndex.z += z_value.vInt;
                    selfScript.moveTo(endIndex);
                }
                else
                {
                    // print an error
                }
            }

            resolveActions(ref self, effect.followup_actions, ref gameEnv);
            // resolveTargetedRactions(ref GameObject, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref GameObject, effect.targetedFollowupTactions);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        Value endValue = DaedScript.evaluateSelfToken(effect.endCondition, ref self, ref gameEnv);
        if (endValue.valueType == "bool" && endValue.vBool)
        {
            selfScript.effects.Remove(effect.givenName);
        }
        else if (endValue.valueType != "bool")
        {
            // print an error
        }
    }

	static public void procRelationalEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv)
    {
        TokenScript selfScript = self.GetComponent<TokenScript>();
        TokenScript targetScript = target.GetComponent<TokenScript>();

        Value conditionValue = DaedScript.evaluateSelfTokenTargetToken(effect.condition, ref self, ref target, ref gameEnv);
        if (conditionValue.valueType == "bool" && conditionValue.vBool)
        {
            DaedScript.evaluateSelfTokenTargetToken(effect.script, ref self, ref target, ref gameEnv);

            if (effect.self_displace_para != null)
            {
                Value self_para_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace_para, ref self, ref target, ref gameEnv);
                if (self_para_value.valueType == "int") 
                {
                    int para_dist = self_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(selfScript.index, targetScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        selfScript.moveTo(para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        selfScript.moveTo(para_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.self_displace_perp != null)
            {
                Value self_perp_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace_perp, ref self, ref target, ref gameEnv);
                if (self_perp_value.valueType == "int") 
                {
                    int perp_dist = self_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(selfScript.index, targetScript.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index perp_index = perp_line[perp_dist];
                        selfScript.moveTo(perp_index);
                    }
                    else
                    {
                        Index perp_index = perp_line[perp_dist - 1];
                        selfScript.moveTo(perp_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.self_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(effect.self_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") 
                {
                    Index endIndex = new Index(selfScript.index);
                    endIndex.y += self_alt_value.vInt;
                    selfScript.moveTo(endIndex);
                } 
                else 
                {
                    // print an error
                }
            }
            
            if (effect.target_displace_para != null)
            {
                Value target_para_value = DaedScript.evaluateSelfTokenTargetToken(effect.target_displace_para, ref self, ref target, ref gameEnv);
                if (target_para_value.valueType == "int") 
                {
                    int para_dist = target_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(targetScript.index, selfScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        targetScript.moveTo(para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        targetScript.moveTo(para_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.target_displace_perp != null)
            {
                Value target_perp_value = DaedScript.evaluateSelfTokenTargetToken(effect.target_displace_perp, ref self, ref target, ref gameEnv);
                if (target_perp_value.valueType == "int") 
                {
                    int perp_dist = target_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(target.GetComponent<TokenScript>().index, self.GetComponent<TokenScript>().index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index perp_index = perp_line[perp_dist];
                        targetScript.moveTo(perp_index);
                    }
                    else
                    {
                        Index perp_index = perp_line[perp_dist - 1];
                        targetScript.moveTo(perp_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.target_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(effect.target_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") 
                {
                    Index endIndex = new Index(targetScript.index);
                    endIndex.y += self_alt_value.vInt;
                    targetScript.moveTo(endIndex);
                } 
                else 
                {
                    // print an error
                }
            }

            resolveRelationalActions(ref self, ref target, effect.followup_actions, ref gameEnv);
            resolveRelationalActions(ref target, ref self, effect.target_followup_actions, ref gameEnv);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        if (!effect.instant) {
            Value endValue = DaedScript.evaluateSelfTokenTargetToken(effect.endCondition, ref self, ref target, ref gameEnv);
            if (endValue.valueType == "bool" && endValue.vBool) {
                target.GetComponent<TokenScript>().effects.Remove(effect.givenName);
            }
            else if (endValue.valueType != "bool") {
                // print an error
            }
        }
    }

	static public void procTargetedEffect(ref GameObject self, ref GameObject target, Effect effect, ref GameEnv gameEnv)
    {
        TokenScript selfScript = self.GetComponent<TokenScript>();
        ShapeScript targetScript = target.GetComponent<ShapeScript>();

        Value conditionValue = DaedScript.evaluateSelfToken(effect.condition, ref self, ref gameEnv);
        if (conditionValue.valueType == "bool" && conditionValue.vBool)
        {
            DaedScript.evaluateSelfToken(effect.script, ref self, ref gameEnv);

            if (effect.self_displace_para != null)
            {
                Value self_para_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_para, ref self, ref target, ref gameEnv);
                if (self_para_value.valueType == "int") 
                {
                    int para_dist = self_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(selfScript.index, targetScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        selfScript.moveTo(para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        selfScript.moveTo(para_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.self_displace_perp != null)
            {
                Value self_perp_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_perp, ref self, ref target, ref gameEnv);
                if (self_perp_value.valueType == "int") 
                {
                    int perp_dist = self_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(selfScript.index, targetScript.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index perp_index = perp_line[perp_dist];
                        selfScript.moveTo(perp_index);
                    }
                    else
                    {
                        Index perp_index = perp_line[perp_dist - 1];
                        selfScript.moveTo(perp_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.self_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") 
                {
                    Index endIndex = new Index(selfScript.index);
                    endIndex.y += self_alt_value.vInt;
                    selfScript.moveTo(endIndex);
                } 
                else 
                {
                    // print an error
                }
            }
            
            if (effect.target_displace_para != null)
            {
                Value target_para_value = DaedScript.evaluateSelfTokenTargetCube(effect.target_displace_para, ref self, ref target, ref gameEnv);
                if (target_para_value.valueType == "int") 
                {
                    int para_dist = target_para_value.vInt;
                    Dictionary<int, Index> para_line = Utils.line(targetScript.index, selfScript.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        targetScript.moveTo(para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        targetScript.moveTo(para_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.target_displace_perp != null)
            {
                Value target_perp_value = DaedScript.evaluateSelfTokenTargetCube(effect.target_displace_perp, ref self, ref target, ref gameEnv);
                if (target_perp_value.valueType == "int") 
                {
                    int perp_dist = target_perp_value.vInt;
                    Dictionary<int, Index> perp_line = Utils.line(targetScript.index, selfScript.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index perp_index = perp_line[perp_dist];
                        targetScript.moveTo(perp_index);
                    }
                    else
                    {
                        Index perp_index = perp_line[perp_dist - 1];
                        targetScript.moveTo(perp_index);
                    }
                } 
                else 
                {
                    // print an error
                }
            }

            if (effect.self_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(effect.self_displace_alt, ref self, ref target, ref gameEnv);
                if (self_alt_value.valueType == "int") 
                {
                    Index endIndex = new Index(targetScript.index);
                    endIndex.y += self_alt_value.vInt;
                    targetScript.moveTo(endIndex);
                } 
                else 
                {
                    // print an error
                }
            }

            resolveTargetedActions(ref self, ref target, effect.followup_actions, ref gameEnv);
            resolveTargetedActions(ref target, ref self, effect.target_followup_actions, ref gameEnv);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        if (!effect.instant) {
            Value endValue = DaedScript.evaluateSelfTokenTargetCube(effect.endCondition, ref self, ref target, ref gameEnv);
            if (endValue.valueType == "bool" && endValue.vBool)
            {
                targetScript.effects.Remove(effect.givenName);
            }
            else if (endValue.valueType != "bool")
            {
                // print an error
            }
        }
    }
}
