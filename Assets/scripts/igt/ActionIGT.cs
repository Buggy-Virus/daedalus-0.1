using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	Type type;

	public List<string> conditions;
	public List<bool> conditionMatch;

	public List<string> currencies;
	public List<int> costs;

	public List<Effect> effects;

	public List<Check> checks;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;

	public List<Action> followupActions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Raction {
	public Type type;

	public List<Type> targetTypes;

	public List<string> selfConditions;
	public List<bool> selfConditionMatch;

	public List<string> targetConditions;
	public List<bool> targetConditionMatch;

	public List<string> currencies;
	public List<int> costs;

	public int maxRange;
	public int minRange;

	public List<Effect> selfEffects;
	public List<Effect> targetEffects;
	public List<Rffect> reffects;

	public List<Rcheck> rchecks;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;
	
	public List<Action> followupActions;
	public List<Raction> followupRactions;

	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;

	public List<Action> followupTargetActions;
	public List<Raction> followupTargetRactions;
}

public class Taction {
	Type type;

	public List<Type> targetTypes;

	public List<string> selfConditions;
	public List<bool> selfConditionMatch;

	public List<string> targetConditions;
	public List<bool> targetConditionMatch;

	public List<string> currencies;
	public List<int> costs;

	public int maxRange;
	public int minRange;

	public List<Effect> selfEffects;
	public List<Effect> targetEffects;
	public List<Tffect> teffects;

	public List<Tcheck> tchecks;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;

	public List<Action> followupActions;
	public List<Taction> followupTactions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Check {
	public Type type;

	public List<string> vars;
	public List<float> varMultipliers;
	public List<string> thresholds;
	public List<float> thresholdMultipliers;

	public List<Action> successActions;
	public List<Raction> successRactions;
	public List<Taction> successTactions;

	public List<Action> failureActions;
	public List<Raction> failureRactions;
	public List<Taction> failureTactions;
}

public class Rcheck {
	public Type type;

	public List<string> selfVars;
	public List<float> selfMultipliers;
	public List<string> targetVars;
	public List<float> targetMultiplier;

	public List<Action> successActions;
	public List<Raction> successRactions;
	public List<Taction> successTactions;

	public List<Raction> targetSuccessRactions;
	public List<Taction> targetSuccessTactions;

	public List<Action> successTargetActions;
	public List<Raction> successTargetRactions;
	public List<Taction> successTargetTactions; 

	public List<Action> failureActions;
	public List<Raction> failureRactions;
	public List<Taction> failureTactions;

	public List<Raction> targetFailureRactions;
	public List<Taction> targetFailureTactions;

	public List<Action> failureTargetActions;
	public List<Raction> failureTargetRactions;
	public List<Taction> failureTargetTactions;
}

public class Tcheck {
	public Type type;

	public List<string> selfVars;
	public List<float> selfMultipliers;
	public List<string> targetVars;
	public List<float> targetMultipliers;

	public List<Action> successActions;
	public List<Raction> successRactions;
	public List<Taction> successTactions;

	public List<Raction> targetSuccessRactions;
	public List<Taction> targetSuccessTactions;

	public List<Action> successTargetActions;
	public List<Raction> successTargetRactions;
	public List<Taction> successTargetTactions; 

	public List<Action> failureActions;
	public List<Raction> failureRactions;
	public List<Taction> failureTactions;

	public List<Raction> targetFailureRactions;
	public List<Taction> targetFailureTactions;

	public List<Action> failureTargetActions;
	public List<Raction> failureTargetRactions;
	public List<Taction> failureTargetTactions;
}

public class Effect {
	public Type type;

	public int effectType;

	public bool instant;
	public int rounds;
	public int roundFrequency;
	public int epoch;
	public int epochFrequency;

	public List<string> calcVars;
	public List<float> calcMultipliers;

	public List<string> buffVars;
	public List<float> buffMultipliers;

	public List<string> statuses;

	public bool displace;

	public float direction; 
	public int xDisplace;
	public int yDisplace;
	public int zDisplace;
}

public class Reffect {
	public Type type;

	public int effectType;
	
	public bool instant;
	public int rounds;
	public int roundFrequency;
	public int epoch;
	public int epochFrequency;

	public List<string> selfCalcVars;
	public List<float> selfCalcMultipliers;

	public List<string> targetCalcVars;
	public List<float> targetCalcMultipliers;

	public List<string> selfBuffVars;
	public List<float> selfBuffMultipliers;

	public List<string> targetBuffVars;
	public List<float> targetBuffMultipliers;

	public List<string> selfStatuses;
	public List<string> targetStatuses;

	public bool displace;

	public bool selfDisplace;
	public int selfToTargetDisplace;
	public int selfToTargetYDisplace;
	public int selfToSelfDispalce;
	public int selfToSelfYDispalce;

	public bool targetDisplace;
	public int targetToSelfDisplace;
	public int targetToTargetDisplace;
	public int targetToSelfYDisplace;
	public int targetToTargetYDisplace;
}

public class Teffect {
	public Type type;

	public int effectType;
	
	public bool instant;
	public int rounds;
	public int roundFrequency;
	public int epoch;
	public int epochFrequency;

	public List<string> selfCalcVars;
	public List<float> selfCalcMultipliers;

	public List<string> targetCalcVars;
	public List<float> targetCalcMultipliers;

	public List<string> selfBuffVars;
	public List<float> selfBuffMultipliers;

	public List<string> targetBuffVars;
	public List<float> targetBuffMultipliers;

	public List<string> selfStatuses;
	public List<string> targetStatuses;

	public bool displace;

	public bool selfDisplace;
	public int selfToTargetDisplace;
	public int selfToTargetYDisplace;
	public int selfToSelfDispalce;
	public int selfToSelfYDispalce;

	public bool targetDisplace;
	public int targetToSelfDisplace;
	public int targetToTargetDisplace;
	public int targetToSelfYDisplace;
	public int targetToTargetYDisplace;
}
