using System.Collections;
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

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;
	
	public List<Action> followupActions;
	public List<Raction> followupRactions;
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;

	public List<Action> followupTargetActions;
	public List<Raction> targetedFollowupTargetRactions;
	public List<Taction> targetedFollowupTargetTactions;
}

public class Taction {
	Type type;

	public List<Type> targetTypes;
  
	public int maxRange;
	public int minRange;

	public List<string> conditions; 

	public List<Teffect> teffects;

	public int aoe;

	public List<Raction> aoeRactions;
	public List<Taction> aoeTactions;

	public List<Action> followupActions;
	public List<Taction> followupTactions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Effect {
	public string name;
	public string givenName;
	public Type type;
	
	public bool stacks;

	public bool instant;
	public string condition;
	public string endCondition;
	public int frequency;
	public int timeLeft;

	public string script;
	
	public bool displace;
	public string displace_x;
	public string displace_y;
	public string displace_z;

	public List<Action> followupActions;
	
	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Reffect {
	public string name;
	public string givenName;
	public Type type;

	public bool stacks;

	public bool instant;
	public string condition;
	public string endCondition;
	public int frequency;
	public int timeLeft;

	public string script;

	public bool displaceGo;
	public bool displaceCome; 

	public string target_displace_perp;
	public string target_displace_para;
	public string target_displace_alt;

	public string self_displace_perp;
	public string self_displace_para;
	public string self_displace_alt;

	public List<Action> followupActions;
	public List<Raction> followupRactions;

	public List<Raction> targetedFollowupRactions;
	public List<Taction> targetedFollowupTactions;
}

public class Teffect {
	public string name;
	public string givenName;
	public Type type;

	public bool stacks;

	public bool instant;
	public string condition;
	public string endCondition;
	public int frequency;
	public int timeLeft;
	
	public string script;

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
