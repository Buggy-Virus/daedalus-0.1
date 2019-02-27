﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	Type type;

	public List<string> conditions;

	public List<Effect> effects;

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

	public int maxRange;
	public int minRange;

	public List<string> conditions; 

	public List<Reffect> reffects;

	public List<Check> checks;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;
	
	public List<Action> followupActions;
	public List<Raction> followupRactions;

	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Taction {
	Type type;

	public List<Type> targetTypes;
  
	public int maxRange;
	public int minRange;

	public List<string> conditions; 

	public List<Teffect> teffects;

	public List<Check> checks;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;

	public List<Action> followupActions;
	public List<Taction> followupTactions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Effect {
	public Type type;

	public bool instant;
	public bool perRound;
	public int rounds;
	public int roundFrequency;
	public bool perEpoch;
	public int epoch;
	public int epochFrequency;

	public int timeLeft;
	public int procTime;

	bool addVars;
	public List<string> selfAddVars;
	public List<string> selfAddValues;

	bool setVars;
	public List<string> selfSetVars;
	public List<string> selfSetValues;

	bool displace;
	string displaceX;
	string displaceY;
	string displaceZ;

	public List<Action> followupActions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Reffect {
	public Type type;

	public bool instant;
	public bool perRound;
	public int rounds;
	public int roundFrequency;
	public bool perEpoch;
	public int epoch;
	public int epochFrequency;

	public int timeLeft;
	public int procTime;

	bool addVars;
	public List<string> selfAddVars;
	public List<string> selfAddValues;

	public List<string> targetAddVars;
	public List<string> targetAddValues;

	bool setVars;
	public List<string> selfSetVars;
	public List<string> selfSetValues;

	public List<string> targetSetVars;
	public List<string> targetSetValues;

	bool displaceGo;
	bool displaceCome; 

	string targetDisplacePerp;
	string targetDisplacePara;
	string targetDisplaceAlt;

	string sefDisplacePerp;
	string selfDisplacePara;
	string selfDisplaceAlt;

	public List<Action> followupActions;
	
	public List<Action> followupActions;
	public List<Raction> followupRactions;

	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Teffect {
	public Type type;

	public bool instant;
	public bool perRound;
	public int rounds;
	public int roundFrequency;
	public bool perEpoch;
	public int epoch;
	public int epochFrequency;

	public int timeLeft;
	public int procTime;
	
	bool addVars;
	public List<string> selfAddVars;
	public List<string> selfAddValues;

	public List<string> targetAddVars;
	public List<string> targetAddValues;

	bool setVars;
	public List<string> selfSetVars;
	public List<string> selfSetValues;

	public List<string> targetSetVars;
	public List<string> targetSetValues;

	bool displaceGo;
	bool displaceCome; 

	string targetDisplacePerp;
	string targetDisplacePara;
	string targetDisplaceAlt;

	string sefDisplacePerp;
	string selfDisplacePara;
	string selfDisplaceAlt;

	public List<Action> followupActions;
	public List<Taction> followupTactions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}
