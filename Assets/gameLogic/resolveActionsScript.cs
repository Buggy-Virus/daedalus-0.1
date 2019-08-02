using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour
{
    System.Random _random = new System.Random();
    GameEnvScript gameEnvScript;
    GameEnv gameEnv;

    public Dictionary<string, GameObject> tokenEnv;
    public Dictionary<string, GameObject> cubeEnv;

    void Start()
    {
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>();

        gameEnv = gameEnvScript.gameEnv;
    }

    public void resolveAction(ref GameObject self, Action action)
    {
        if (resolveConditions(self, action.conditions))
        {
            resolveEffects(ref self, action.effects);
            resolveAoe(ref self, self.GetComponent<TokenScript>().index, action.aoe, action.aoe_ractions, action.aoe_tactions);

            resolveActions(ref self, action.followup_actions);
            // resolveTargetedRactions(ref self, action.targetedFollowupRactions);
            // resolveTargetedTactions(ref self, action.targetedFollowupTactions);
        }
    }

    public void resolveActions(ref GameObject self, List<Action> actions)
    {
        foreach (Action action in actions)
        {
            resolveAction(ref self, action);
        }
    }

    public void resolveRaction(ref GameObject self, ref GameObject target, Raction raction)
    {
        if (resolveRconditions(self, target, raction.conditions))
        {
            resolveReffects(ref self, ref target, raction.reffects);
            resolveAoe(ref self, target.GetComponent<TokenScript>().index, raction.aoe, raction.aoe_ractions, raction.aoe_tactions);

            resolveActions(ref self, raction.followup_actions);
            resolveRactions(ref self, ref target, raction.followup_ractions);
            // resolveTargetedRactions(ref self, raction.targetedFollowupRactions);
            // resolveTargetedTactions(ref self, raction.targetedFollowupTactions);

            resolveActions(ref target, raction.followup_target_actions);
            // resolveTargetedRactions(ref target, raction.targetedFollowupTargetRactions);
            // resolveTargetedTactions(ref target, raction.targetedFollowupTargetTactions);
        }
    }

    public void resolveRactions(ref GameObject self, ref GameObject target, List<Raction> ractions)
    {
        foreach (Raction raction in ractions)
        {
            resolveRaction(ref self, ref target, raction);
        }
    }

    public void resolveTaction(ref GameObject self, ref GameObject target, Taction taction)
    {
        if (resolveTconditions(self, target, taction.conditions))
        {
            resolveTeffects(ref self, ref target, taction.teffects);
            resolveAoe(ref self, target.GetComponent<CubeScript>().index, taction.aoe, taction.aoe_ractions, taction.aoe_tactions);

            resolveActions(ref self, taction.followup_actions);
            resolveTactions(ref self, ref target, taction.followup_tactions);
            // resolveTargetedRactions(ref self, taction.targetedFollowupRactions);
            // resolveTargetedTactions(ref self, taction.targetedFollowupTactions);
        }
    }

    public void resolveTactions(ref GameObject self, ref GameObject target, List<Taction> tactions)
    {
        foreach (Taction taction in tactions)
        {
            resolveTaction(ref self, ref target, taction);
        }
    }

    // public void resolveTargetedRaction(ref GameObject self, Raction raction) {

    // }

    // public void resolveTargetedRactions(ref GameObject self, List<Raction> ractions) {
    // 	foreach (Raction raction in ractions) {
    // 		resolveTargetedRaction(ref self, raction);
    // 	}
    // }

    // public void resolveTargetedTaction(ref GameObject self, Taction taction) {

    // }

    // public void resolveTargetedTactions(ref GameObject self, List<Taction> tactions) {
    // 	foreach (Taction taction in tactions) {
    // 		resolveTargetedTaction(ref self, taction);
    // 	}
    // }

    public bool resolveConditions(GameObject self, List<string> conditions)
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

    public bool resolveRconditions(GameObject self, GameObject target, List<string> conditions)
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

    public bool resolveTconditions(GameObject self, GameObject target, List<string> conditions)
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

    public void resolveAoe(ref GameObject self, Index index, int radius, List<Raction> ractions, List<Taction> tactions)
    {
        List<GameObject> nearbyTokens = detectTokens(self.GetComponent<TokenScript>().index, radius, gameEnv);
        nearbyTokens.Remove(self);
        foreach (GameObject nearbyToken in nearbyTokens)
        {
            GameObject nearbyTokenRef = nearbyToken;
            resolveRactions(ref self, ref nearbyTokenRef, ractions);
        }

        List<GameObject> nearbyCubes = detectCubes(self.GetComponent<TokenScript>().index, radius, gameEnv);
        foreach (GameObject nearbyCube in nearbyCubes)
        {
            GameObject nearbyCubeRef = nearbyCube;
            resolveTactions(ref self, ref nearbyCubeRef, tactions);
        }
    }

    public List<GameObject> detectTokens(Index center, int radius, GameEnv gameEnv)
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
					GameCoord gameCoord = gameEnv.gameBoard[cur_index.x, cur_index.y, cur_index.z];
					nearbyTokens.AddRange(gameCoord.tokens);
				}
            }
        }

        return nearbyTokens;
    }

    public List<GameObject> detectCubes(Index center, int radius, GameEnv gameEnv)
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
					GameCoord gameCoord = gameEnv.gameBoard[cur_index.x, cur_index.y, cur_index.z];
					nearbyCubes.Add(gameCoord.cube);
				}
            }
        }

        return nearbyCubes;
    }

    public void resolveEffect(ref GameObject self, Effect effect)
    {
        if (effect.instant)
        {
            procEffect(ref self, effect);
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

    public void resolveEffects(ref GameObject self, List<Effect> effects)
    {
        foreach (Effect effect in effects)
        {
            resolveEffect(ref self, effect);
        }
    }

    public void resolveReffect(ref GameObject self, ref GameObject target, Reffect reffect)
    {
		if (reffect.instant)
        {
            procReffect(ref self, ref target, reffect);
        }
        else
        {
            reffect.timeLeft = reffect.frequency;
            if (reffect.stacks)
            {
                reffect.givenName = reffect.name  + "_" + System.Guid.NewGuid();
            }
            else
            {
                reffect.givenName = reffect.name;
            }
            target.GetComponent<TokenScript>().reffects[reffect.givenName] = reffect;
        }
    }

    public void resolveReffects(ref GameObject self, ref GameObject target, List<Reffect> reffects)
    {
        foreach (Reffect reffect in reffects)
        {
            resolveReffect(ref self, ref target, reffect);
        }
    }

    public void resolveTeffect(ref GameObject self, ref GameObject target, Teffect teffect)
    {
		if (teffect.instant)
        {
            procTeffect(ref self, ref target, teffect);
        }
        else
        {
            teffect.timeLeft = teffect.frequency;
            if (teffect.stacks)
            {
                teffect.givenName = teffect.name  + "_" + System.Guid.NewGuid();
            }
            else
            {
                teffect.givenName = teffect.name;
            }
            target.GetComponent<CubeScript>().teffects[teffect.givenName] = teffect;
        }
    }

    public void resolveTeffects(ref GameObject self, ref GameObject target, List<Teffect> teffects)
    {
        foreach (Teffect teffect in teffects)
        {
            resolveTeffect(ref self, ref target, teffect);
        }
    }

    public void procEffect(ref GameObject self, Effect effect)
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

            resolveActions(ref self, effect.followup_actions);
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

	public void procReffect(ref GameObject self, ref GameObject target, Reffect reffect)
    {
        TokenScript selfScript = self.GetComponent<TokenScript>();
        TokenScript targetScript = target.GetComponent<TokenScript>();

        Value conditionValue = DaedScript.evaluateSelfTokenTargetToken(reffect.condition, ref self, ref target, ref gameEnv);
        if (conditionValue.valueType == "bool" && conditionValue.vBool)
        {
            DaedScript.evaluateSelfTokenTargetToken(reffect.script, ref self, ref target, ref gameEnv);

            if (reffect.self_displace_para != null)
            {
                Value self_para_value = DaedScript.evaluateSelfTokenTargetToken(reffect.self_displace_para, ref self, ref target, ref gameEnv);
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

            if (reffect.self_displace_perp != null)
            {
                Value self_perp_value = DaedScript.evaluateSelfTokenTargetToken(reffect.self_displace_perp, ref self, ref target, ref gameEnv);
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

            if (reffect.self_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(reffect.self_displace_alt, ref self, ref target, ref gameEnv);
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
            
            if (reffect.target_displace_para != null)
            {
                Value target_para_value = DaedScript.evaluateSelfTokenTargetToken(reffect.target_displace_para, ref self, ref target, ref gameEnv);
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

            if (reffect.target_displace_perp != null)
            {
                Value target_perp_value = DaedScript.evaluateSelfTokenTargetToken(reffect.target_displace_perp, ref self, ref target, ref gameEnv);
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

            if (reffect.target_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetToken(reffect.target_displace_alt, ref self, ref target, ref gameEnv);
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

            resolveActions(ref self, reffect.followup_actions);
            resolveActions(ref target, reffect.target_followup_actions);

            resolveRactions(ref self, ref target, reffect.followup_ractions);
            
            // resolveTargetedRactions(ref GameObject, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref GameObject, effect.targetedFollowupTactions);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        Value endValue = DaedScript.evaluateSelfTokenTargetToken(reffect.endCondition, ref self, ref target, ref gameEnv);
        if (endValue.valueType == "bool" && endValue.vBool)
        {
            target.GetComponent<TokenScript>().reffects.Remove(reffect.givenName);
        }
        else if (endValue.valueType != "bool")
        {
            // print an error
        }
    }

	public void procTeffect(ref GameObject self, ref GameObject target, Teffect teffect)
    {
        TokenScript selfScript = self.GetComponent<TokenScript>();
        CubeScript targetScript = target.GetComponent<CubeScript>();

        Value conditionValue = DaedScript.evaluateSelfToken(teffect.condition, ref self, ref gameEnv);
        if (conditionValue.valueType == "bool" && conditionValue.vBool)
        {
            DaedScript.evaluateSelfToken(teffect.script, ref self, ref gameEnv);

            if (teffect.self_displace_para != null)
            {
                Value self_para_value = DaedScript.evaluateSelfTokenTargetCube(teffect.self_displace_para, ref self, ref target, ref gameEnv);
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

            if (teffect.self_displace_perp != null)
            {
                Value self_perp_value = DaedScript.evaluateSelfTokenTargetCube(teffect.self_displace_perp, ref self, ref target, ref gameEnv);
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

            if (teffect.self_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(teffect.self_displace_alt, ref self, ref target, ref gameEnv);
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
            
            if (teffect.target_displace_para != null)
            {
                Value target_para_value = DaedScript.evaluateSelfTokenTargetCube(teffect.target_displace_para, ref self, ref target, ref gameEnv);
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

            if (teffect.target_displace_perp != null)
            {
                Value target_perp_value = DaedScript.evaluateSelfTokenTargetCube(teffect.target_displace_perp, ref self, ref target, ref gameEnv);
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

            if (teffect.self_displace_alt != null)
            {
                Value self_alt_value = DaedScript.evaluateSelfTokenTargetCube(teffect.self_displace_alt, ref self, ref target, ref gameEnv);
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

            resolveActions(ref self, teffect.followup_actions);
            resolveTactions(ref self, ref target, teffect.followup_Tactions);
            // resolveTargetedRactions(ref GameObject, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref GameObject, effect.targetedFollowupTactions);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        Value endValue = DaedScript.evaluateSelfTokenTargetCube(teffect.endCondition, ref self, ref target, ref gameEnv);
        if (endValue.valueType == "bool" && endValue.vBool)
        {
            targetScript.teffects.Remove(teffect.givenName);
        }
        else if (endValue.valueType != "bool")
        {
            // print an error
        }
    }
}
