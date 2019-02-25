using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	Type type;

	public List<string> conditions;

	public List<string> effects;

	public List<Check> checks;

	string displaceX;
	string displaceY;
	string displaceZ;

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

	public List<string> effects;

	public List<Check> checks;

	string DisplaceAway;
	string DisplaceSide;
	string DisplaceUp;

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
  
	public int maxRange;
	public int minRange;

	public List<string> conditions; 

	public List<string> effects;

	public List<Check> checks;

	bool displaceTo; 

	string DisplaceAway;
	string DisplaceSide;
	string DisplaceUp;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;

	public List<Action> followupActions;
	public List<Taction> followupTactions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;

	public List<Action> followupTargetActions;
	public List<Raction> followupTargetRactions;
}

// Maybe check isn't necessary at all
public class Check {
	public Type type;

	public List<string> conditions;

	// Not necessary, can be handled by other actions
	public List<string> effects;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;
	// Not necessary
	
	public List<Action> followupActions;
	public List<Raction> followupRactions;
	public List<Taction> followupTactions;

	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;

	public List<Action> followupTargetActions;
	public List<Raction> followupTargetRactions;
}
