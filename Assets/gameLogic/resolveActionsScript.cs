using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour
{
    System.Random _random = new System.Random();
    GameEnvScript gameEnvScript;
    GameEnv gameEnv;

    public Dictionary<string, Token> tokenEnv;
    public Dictionary<string, Cube> cubeEnv;

    void Start()
    {
        gameEnvScript = GameObject.Find("GameLogic").GetComponent<GameEnvScript>();

        gameEnv = gameEnvScript.gameEnv;
    }

    public void resolveAction(ref Token self, Action action)
    {
        if (resolveConditions(self, action.conditions))
        {
            resolveEffects(ref self, action.effects);
            resolveAoe(ref self, self.index, action.aoe, action.aoe_ractions, action.aoe_tactions);

            resolveActions(ref self, action.followup_actions);
            // resolveTargetedRactions(ref self, action.targetedFollowupRactions);
            // resolveTargetedTactions(ref self, action.targetedFollowupTactions);
        }
    }

    public void resolveActions(ref Token self, List<Action> actions)
    {
        foreach (Action action in actions)
        {
            resolveAction(ref self, action);
        }
    }

    public void resolveRaction(ref Token self, ref Token target, Raction raction)
    {
        if (resolveRconditions(self, target, raction.conditions))
        {
            resolveReffects(ref self, ref target, raction.reffects);
            resolveAoe(ref self, target.index, raction.aoe, raction.aoe_ractions, raction.aoe_tactions);

            resolveActions(ref self, raction.followup_actions);
            resolveRactions(ref self, ref target, raction.followup_ractions);
            // resolveTargetedRactions(ref self, raction.targetedFollowupRactions);
            // resolveTargetedTactions(ref self, raction.targetedFollowupTactions);

            resolveActions(ref target, raction.followup_target_actions);
            // resolveTargetedRactions(ref target, raction.targetedFollowupTargetRactions);
            // resolveTargetedTactions(ref target, raction.targetedFollowupTargetTactions);
        }
    }

    public void resolveRactions(ref Token self, ref Token target, List<Raction> ractions)
    {
        foreach (Raction raction in ractions)
        {
            resolveRaction(ref self, ref target, raction);
        }
    }

    public void resolveTaction(ref Token self, ref Cube target, Taction taction)
    {
        if (resolveTconditions(self, target, taction.conditions))
        {
            resolveTeffects(ref self, ref target, taction.teffects);
            resolveAoe(ref self, target.index, taction.aoe, taction.aoe_ractions, taction.aoe_tactions);

            resolveActions(ref self, taction.followup_actions);
            resolveTactions(ref self, ref target, taction.followup_tactions);
            // resolveTargetedRactions(ref self, taction.targetedFollowupRactions);
            // resolveTargetedTactions(ref self, taction.targetedFollowupTactions);
        }
    }

    public void resolveTactions(ref Token self, ref Cube target, List<Taction> tactions)
    {
        foreach (Taction taction in tactions)
        {
            resolveTaction(ref self, ref target, taction);
        }
    }

    // public void resolveTargetedRaction(ref Token self, Raction raction) {

    // }

    // public void resolveTargetedRactions(ref Token self, List<Raction> ractions) {
    // 	foreach (Raction raction in ractions) {
    // 		resolveTargetedRaction(ref self, raction);
    // 	}
    // }

    // public void resolveTargetedTaction(ref Token self, Taction taction) {

    // }

    // public void resolveTargetedTactions(ref Token self, List<Taction> tactions) {
    // 	foreach (Taction taction in tactions) {
    // 		resolveTargetedTaction(ref self, taction);
    // 	}
    // }

    public bool resolveConditions(Token self, List<string> conditions)
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

    public bool resolveRconditions(Token self, Token target, List<string> conditions)
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

    public bool resolveTconditions(Token self, Cube target, List<string> conditions)
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

    public void resolveAoe(ref Token self, Index index, int radius, List<Raction> ractions, List<Taction> tactions)
    {
        List<Token> nearbyTokens = detectTokens(self.index, radius, gameEnv);
        nearbyTokens.Remove(self);
        foreach (Token nearbyToken in nearbyTokens)
        {
            Token nearbyTokenRef = nearbyToken;
            resolveRactions(ref self, ref nearbyTokenRef, ractions);
        }

        List<Cube> nearbyCubes = detectCubes(self.index, radius, gameEnv);
        foreach (Cube nearbyCube in nearbyCubes)
        {
            Cube nearbyCubeRef = nearbyCube;
            resolveTactions(ref self, ref nearbyCubeRef, tactions);
        }
    }

    public List<Token> detectTokens(Index center, int radius, GameEnv gameEnv)
    {
        List<Token> nearbyTokens = new List<Token>();
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

    public List<Cube> detectCubes(Index center, int radius, GameEnv gameEnv)
    {
        List<Cube> nearbyCubes = new List<Cube>();
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

    public void resolveEffect(ref Token self, Effect effect)
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
                effect.givenName = effect.name + System.Guid.NewGuid();
            }
            else
            {
                effect.givenName = effect.name;
            }
            self.effects[effect.givenName] = effect;
        }
    }

    public void resolveEffects(ref Token self, List<Effect> effects)
    {
        foreach (Effect effect in effects)
        {
            resolveEffect(ref self, effect);
        }
    }

    public void resolveReffect(ref Token self, ref Token target, Reffect reffect)
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
                reffect.givenName = reffect.name + System.Guid.NewGuid();
            }
            else
            {
                reffect.givenName = reffect.name;
            }
            target.reffects[reffect.givenName] = reffect;
        }
    }

    public void resolveReffects(ref Token self, ref Token target, List<Reffect> reffects)
    {
        foreach (Reffect reffect in reffects)
        {
            resolveReffect(ref self, ref target, reffect);
        }
    }

    public void resolveTeffect(ref Token self, ref Cube target, Teffect teffect)
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
                teffect.givenName = teffect.name + System.Guid.NewGuid();
            }
            else
            {
                teffect.givenName = teffect.name;
            }
            target.teffects[teffect.givenName] = teffect;
        }
    }

    public void resolveTeffects(ref Token self, ref Cube target, List<Teffect> teffects)
    {
        foreach (Teffect teffect in teffects)
        {
            resolveTeffect(ref self, ref target, teffect);
        }
    }

    public void displaceToken(ref Token token, int x, int y, int z) {
		int start_x = token.index.x;
		int start_y = token.index.y;
		int start_z = token.index.z;
		int end_x = start_x + x;
		int end_y = start_y + y;
		int end_z = start_z + z;
		gameEnv.gameBoard[start_x,start_y,start_z].tokens.Remove(token);
		gameEnv.gameBoard[end_x,end_y,end_z].tokens.Add(token);
		token.index.x = end_x;
		token.index.y = end_y;
		token.index.z = end_z;

		moveGraphicObject(ref token.graphicObject, end_x, end_y, end_z);
    }

    public void displaceCube(ref Cube cube, int x, int y, int z) {
		int start_x = cube.index.x;
		int start_y = cube.index.y;
		int start_z = cube.index.z;
		int end_x = start_x + x;
		int end_y = start_y + y;
		int end_z = start_z + z;
		gameEnv.gameBoard[start_x,start_y,start_z].cube = null;
		gameEnv.gameBoard[end_x,end_y,end_z].cube = cube;
		cube.index.x = end_x;
		cube.index.y = end_y;
		cube.index.z = end_z;

		moveGraphicObject(ref cube.graphicObject, end_x, end_y, end_z);
    }

	public void moveToken(ref Token token, int x, int y, int z) {
		gameEnv.gameBoard[token.index.x,token.index.y,token.index.z].tokens.Remove(token);
		gameEnv.gameBoard[x,y,z].tokens.Add(token);
		token.index.x = x;
		token.index.y = y;
		token.index.z = z;

		moveGraphicObject(ref token.graphicObject, x, y, z);
	}

    public void moveToken(ref Token token, Index index) {
        int x = index.x;
        int y = index.y;
        int z = index.z;
		gameEnv.gameBoard[token.index.x,token.index.y,token.index.z].tokens.Remove(token);
		gameEnv.gameBoard[x,y,z].tokens.Add(token);
		token.index = new Index(index);

		moveGraphicObject(ref token.graphicObject, x, y, z);
	}

	public void moveCube(ref Cube cube, int x, int y, int z) {
		gameEnv.gameBoard[cube.index.x,cube.index.y,cube.index.z].cube = null;
		gameEnv.gameBoard[x,y,z].cube = cube;
		cube.index.x = x;
		cube.index.y = y;
		cube.index.z = z;

		moveGraphicObject(ref cube.graphicObject, x, y, z);
	}

    public void moveCube(ref Cube cube, Index index) {
        int x = index.x;
        int y = index.y;
        int z = index.z;
		gameEnv.gameBoard[cube.index.x,cube.index.y,cube.index.z].cube = null;
		gameEnv.gameBoard[x,y,z].cube = cube;
		cube.index = new Index(index);

		moveGraphicObject(ref cube.graphicObject, x, y, z);
	}

    public void procEffect(ref Token self, Effect effect)
    {
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
                    displaceToken(ref self, x_value.vInt, y_value.vInt, z_value.vInt);
                }
                else
                {
                    // print an error
                }
            }

            resolveActions(ref self, effect.followup_actions);
            // resolveTargetedRactions(ref Token, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref Token, effect.targetedFollowupTactions);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        Value endValue = DaedScript.evaluateSelfToken(effect.endCondition, ref self, ref gameEnv);
        if (endValue.valueType == "bool" && endValue.vBool)
        {
            self.effects.Remove(effect.givenName);
        }
        else if (endValue.valueType != "bool")
        {
            // print an error
        }
    }

	public void procReffect(ref Token self, ref Token target, Reffect reffect)
    {
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
                    Dictionary<int, Index> para_line = Utils.line(self.index, target.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        moveToken(ref self, para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        moveToken(ref self, para_index);
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
                    Dictionary<int, Index> perp_line = Utils.line(self.index, target.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index para_index = perp_line[perp_dist];
                        moveToken(ref self, para_index);
                    }
                    else
                    {
                        Index para_index = perp_line[perp_dist - 1];
                        moveToken(ref self, para_index);
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
                    displaceToken(ref self, 0, 0, self_alt_value.vInt);
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
                    Dictionary<int, Index> para_line = Utils.line(target.index, self.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        moveToken(ref target, para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        moveToken(ref target, para_index);
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
                    Dictionary<int, Index> perp_line = Utils.line(target.index, self.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index para_index = perp_line[perp_dist];
                        moveToken(ref target, para_index);
                    }
                    else
                    {
                        Index para_index = perp_line[perp_dist - 1];
                        moveToken(ref target, para_index);
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
                    displaceToken(ref target, 0, 0, self_alt_value.vInt);
                } 
                else 
                {
                    // print an error
                }
            }

            resolveActions(ref self, reffect.followup_actions);
            resolveActions(ref target, reffect.target_followup_actions);

            resolveRactions(ref self, ref target, reffect.followup_ractions);
            
            // resolveTargetedRactions(ref Token, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref Token, effect.targetedFollowupTactions);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        Value endValue = DaedScript.evaluateSelfTokenTargetToken(reffect.endCondition, ref self, ref target, ref gameEnv);
        if (endValue.valueType == "bool" && endValue.vBool)
        {
            target.reffects.Remove(reffect.givenName);
        }
        else if (endValue.valueType != "bool")
        {
            // print an error
        }
    }

	public void procTeffect(ref Token self, ref Cube target, Teffect teffect)
    {
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
                    Dictionary<int, Index> para_line = Utils.line(self.index, target.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        moveToken(ref self, para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        moveToken(ref self, para_index);
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
                    Dictionary<int, Index> perp_line = Utils.line(self.index, target.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index para_index = perp_line[perp_dist];
                        moveToken(ref self, para_index);
                    }
                    else
                    {
                        Index para_index = perp_line[perp_dist - 1];
                        moveToken(ref self, para_index);
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
                    displaceToken(ref self, 0, 0, self_alt_value.vInt);
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
                    Dictionary<int, Index> para_line = Utils.line(target.index, self.index, para_dist);
                    if (para_line.ContainsKey(para_dist)) 
                    {
                        Index para_index = para_line[para_dist];
                        moveCube(ref target, para_index);
                    }
                    else
                    {
                        Index para_index = para_line[para_dist - 1];
                        moveCube(ref target, para_index);
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
                    Dictionary<int, Index> perp_line = Utils.line(target.index, self.index, perp_dist);
                    if (perp_line.ContainsKey(perp_dist)) 
                    {
                        Index para_index = perp_line[perp_dist];
                        moveCube(ref target, para_index);
                    }
                    else
                    {
                        Index para_index = perp_line[perp_dist - 1];
                        moveCube(ref target, para_index);
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
                    displaceCube(ref target, 0, 0, self_alt_value.vInt);
                } 
                else 
                {
                    // print an error
                }
            }

            resolveActions(ref self, teffect.followup_actions);
            resolveTactions(ref self, ref target, teffect.followup_Tactions);
            // resolveTargetedRactions(ref Token, effect.targetedFollowupRactions);
            // resolveTargetedTactions(ref Token, effect.targetedFollowupTactions);
        }
        else if (conditionValue.valueType != "bool")
        {
            // print an error
        }

        Value endValue = DaedScript.evaluateSelfTokenTargetCube(teffect.endCondition, ref self, ref target, ref gameEnv);
        if (endValue.valueType == "bool" && endValue.vBool)
        {
            target.teffects.Remove(teffect.givenName);
        }
        else if (endValue.valueType != "bool")
        {
            // print an error
        }
    }

    public void moveGraphicObject(ref GameObject graphicObject, int x, int y, int z) {
        // TODO
    }
}
