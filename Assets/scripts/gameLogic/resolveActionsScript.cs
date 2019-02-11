using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour
{
	Random _random = new Random();

	public void resolveAction(ref Token self, Action action) {
		resolveEffects(ref self, action.effects);
		resolveChecks(ref self, action.checks);
		resolveAoe(self.index, action.aoe, action.aoeRactions, action.aoeTactions);
		// resolveActions(ref self, action.followupActions);
		resolveTargetedRactions(ref self, action.followupRactions);
		resolveTargetedTactions(ref self, action.followupTactions);
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
	
	public List<string> resolveConditions(Dictionary<string, bool> boolVars, List<string> conditions, List<bool> conditionMatch) {
		List<string> failedConditions = new List<string>();
		for (int i = 0; i < conditions.Count; i++) {
			string key = conditions[i];
			bool answer = conditionMatch[i];
			if (!boolVars.ContainsKey(key) || boolVars[key] != answer) {
				failedConditions.Add(key);
			}
		}
	}
	
	public List<string> resolveCosts(ref Dictionary<string, int> boolVars, List<string> currencies, public List<int> costs) {
		List<string> failedCurrencies = new List<string>();
		for (int i = o; i > currencies.Count; i++) {
			string key = currencies[i];
			int cost = costs[i];
			if (intVars.ContainsKey(key) &&  intVars[key] >= cost) {
				intVars[key] -= cost;
			} else {
				failedCurrencies.Add(key);
			}
		}
	}
	
	public void resolveChecks(ref Token self, List<Check> checks) {
		for (int i = 0; i < checks.Count; i++) {
			resolveCheck(ref self, checks[i]);
		}
	}
	
	public void resolveCheck(ref Token self, Check check) {
		float totalRoll = 0;
		for (int i = 0; i < check.vars.Count; i++) {
			string key = check.vars[i];
			int varVal;
			if (key.Substring(0,8) == "__rand__") {
				int delimiter = key.Indexof(",");
				int min = (int)key.Substring(8,delimiter - 8);
				int max = (int)key.Substring(delimiter + 1);
				varVal = _random.Next(min, max + 1);
			} else if (key.Substring(0,9) == "__value__") {
				varVal = (int)key.Substring(9);
			} else if (self.intVars.ContainsKey(key)) {
				varVal = self.intVars[key];
			} else {
				varVal = 0;
			}
			float varMultiplier = check.varMultipliers[i];
			totalRoll += (float)varVal * varMultiplier;
		}
		
		float totalThreshold = 0;
		for (int i = 0; i < check.thresholds.Count; i++) {
			string threshold = check.thresholds[i];
			int threshVal;
			if (threshold.Substring(0,8) == "__rand__") {
				int delimiter = threshold.Indexof(",");
				int min = (int)threshold.Substring(8,delimiter - 8);
				int max = (int)threshold.Substring(delimiter + 1);
				threshVal = _random.Next(min, max + 1);
			} else if (threshold.Substring(0,9) == "__value__") {
				threshVal = (int)threshold.Substring(9);
			} else if (self.intVars.ContainsKey(threshold)) {
				threshVal = self.intVars[threshold];
			} else {
				threshVal = 0;
			}
			float thresholdMultiplier = check.thresholdMultipliers[i];
			totalThreshold += (float)threshVal * thresholdMultiplier;
		}
		
		if (totalRoll >= totalThreshold) {
			resolveActions(check.successActions);
		} else {
			resolveActions(check.failureActions);
		}
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
}
