﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveActionsScript : MonoBehaviour
{
	Random _random = new Random();

    public void resolveAction(ref Mob self, Action action) {
	
		List<string> failedConditions = resolveConditions(self.boolVars, action.conditions, action.conditionMatch);
		if (failedConditions.Count == 0) {
			List<string> failedCurrencies = resolveCosts(ref self.intVars, action.currencies, action.costs);
			if (failedCurrencies.Count == 0) {
				resolveEffects(ref self, action.effects);
				resolveChecks(ref self, action.checks);
				resolveAoe(mob.index, action.aoe, action.aoeRactions);
				resolveActions(ref self, action.followupActions);
				resolveRactions(ref self, action.followupRactions);
				resolveTactions(ref self, action.followupTactions);
			} else {
				Debug.Log("Failed Currencies(s)");
				for (int i = 0; i < failedConditions.Count; i++) {
					Debug.Log(failedConditions[i]);
				}
			}			
		} else {
			Debug.Log("Failed Condition(s)");
			for (int i = 0; i < failedConditions.Count; i++) {
				Debug.Log(failedConditions[i]);
			}
		}
	}
	
	public void resolveActions(ref Mob self, List<Action> actions) {
		for (int i = 0; i < actions.Count; i++) {
			resolveAction(ref self, actions[i]);
		}
	}
	
	public void resolveRaction(ref Mob self, Mob target, Raction raction) {
		
	}
	
	public void resolveTaction(ref Mob self, Cube target, Taction taction) {
		
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
	
	public void resolveChecks(ref Mob self, List<Check> checks) {
		for (int i = 0; i < checks.Count; i++) {
			resolveCheck(ref self, checks[i]);
		}
	}
	
	public void resolveCheck(ref Mob self, Check check) {
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
}