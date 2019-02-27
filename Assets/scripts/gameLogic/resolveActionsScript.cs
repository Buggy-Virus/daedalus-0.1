using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour
{
	Random _random = new Random();

	public Dictionary<string, Token> tokenEnv;
    public Dictionary<string, Cube> cubeEnv;

	void Start()
    {
        daedScript = GameObject.Find("GameLogic").GetComponent<daedScript>();
        igListScript = GameObject.Find("GameLogic").GetComponent<IgListScript>();

        tokenEnv = igListScript.tokenDict;
		cubeEnv = igListScript.cubeDict;
    }

	public void resolveAction(ref Token self, Action action) {
		if (resolveConditions(ref self, action.conditions)) {
			resolveEffects(ref self, action.effects);
			resolveAoe(self.index, action.aoe, action.aoeRactions, action.aoeTactions);

			resolveActions(ref self, action.followupActions);
			resolveTargetedRactions(ref self, action.followupRactions);
			resolveTargetedTactions(ref self, action.followupTactions);
		}	
	}
	
	public void resolveActions(ref Token self, List<Action> actions) {
		for (int i = 0; i < actions.Count; i++) {
			resolveAction(ref self, actions[i]);
		}
	}
	
	public void resolveRaction(ref Token self, ref Token target, Raction raction) {
		resolveEffects(ref self, raction.selfEffects);
		resolveEffects(ref target, raction.targetEffects);
		resolveReffects(ref self, ref target, raction.reffects)
		resolveChecks(ref self, ref target, raction.rchecks);
		resolveAoe(target.index, raction.aoe, raction.aoeRactions, raction.aoeTactions);
		// resolveActions(ref self, raction.followupActions);
		// resolveRaction(ref self, ref target, raction.followupRactions);
		// resolveActions(ref target, raction.followupTargetActions);
		// resolveRaction(ref target, ref self, raction.followupTargetRactions);
		resolveTargetedRactions(ref self, raction.followupRactions);
		resolveTargetedTactions(ref self, raction.followupTactions);
	}
	
	public void resolveRactions(ref Token self, ref Token target, List<Raction> ractions) {
		for (int i = 0; i < ractions.Count; i++) {
			resolveRaction(ref self, ref target, ractions[i]);
		}
	}
	
	public void resolveTaction(ref Token self, ref Cube target, Taction taction) {
		resolveEffects
	}
	
	public void resolveTactions(ref Token self, ref Cube target, List<Taction> tactions) {
		for (int i = 0; i < tactions.Count; i++) {
			resolveTaction(ref self, target, tactions[i]);
		}
	}
	
	public void resolveTargetedRaction(ref Token self, Raction raction) {
	
	}
	
	public void resolveTargetedRaction(ref Token self, List<Raction> ractions) {
		for (int i = 0; i < ractions.Count; i++) {
			resolveTargetedRaction(ref self, ractions[i]);
		}
	}
	
	public void resolveTargetedTaction(ref Token self, Taction taction) {
		
	}
	
	public void resolveTargetedTactions(ref Token self, List<Taction> tactions) {
		for (int i = 0; i < tactions.Count; i++) {
			resolveTargetedTaction(ref self, tactions[i]);
		}
	}
	
	public bool resolveConditions(ref Token self, List<string> conditions) {
		foreach (string condition in conditions) {
			Value evaluateValue = daedScript.evaluate(condition, tokenEnv, cubeEnv);

			if (evaluateValue.valueType == "bool") {
				if (!evaluateValue) {
					return false;
				} 
			} else {
				// print an error
				return false;
			}
		}
		return true;
	}
	
	public void resolveAoe(ref Token self, int radius, List<Raction> ractions, List<Taction> tactions) {
		List<Token> nearbyTokens = detectTokens(self.index, radius);
		nearbyTokens.Remove(self);
		for (int i = 0; i < nearbyTokens.Count; i++) {
			// resolveRactions(ref self, ref nearbyMobs[i], ractions);
		}
		
		List<Cube> nearbyCubes = detectCubes(self.index, radius);
		for (int i = 0; i < nearbyCubes.Count; i++) {
			// resolveTactions(ref self, ref nearbyCubes[i], tactions);
		}
	}
	
	public List<Token> detectTokens(Index center, int radius) {
	
	}
	
	public List<Cube> detectCubes(Index center, int radius) {
	
	}
	
	public void resolveEffect(ref Token self, Effect effect) {
		
	}
	
	public void resolveEffects(ref Token self, List<Effect> effects) {
		for (int i = 0; i < effects.Count; i++) {
			resolveEffect(ref self, effects[i]);
		}
	}
	
	public void resolveReffect(ref Token self, Reffect reffect) {
	
	}
	
	public void resolveReffects(ref Token self, List<Reffect> reffects) {
		for (int i = 0; i < reffects.Count; i++) {
			resolveReffect(ref self, reffects[i]);
		}
	}
	
	public void resolveTeffect(ref Token self, Teffect teffect) {
	
	}
	
	public void resolveTeffects(ref Token self, List<Teffect> teffects) {
		for (int i = 0; i < teffects.Count; i++) {
			resolveTeffect(ref self, teffects[i]);
		}
	}

	public void displaceToken(ref Token token) {

	}

	public void displaceCube(ref Cube cube) {

	}

	public void procEffect(ref Token token, Effect effect) {
		
	}
}
