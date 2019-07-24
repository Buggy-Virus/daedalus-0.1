using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	Type type;

	public List<string> conditions;

	public List<Effect> effects;

	public int aoe;

	public List<Raction> aoe_ractions;
	public List<Taction> aoe_tactions;

	public List<Action> followup_actions;
	
	public List<Raction> targeted_followup_ractions;
	public List<Taction> targeted_followup_tactions;
}

public class Raction {
	public Type type;

	public List<Type> targetTypes;

	public int maxRange;
	public int minRange;

	public List<string> conditions; 

	public List<Reffect> reffects;

	public int aoe;

	public List<Raction> aoe_ractions;
	public List<Taction> aoe_tactions;
	
	public List<Action> followup_actions;
	public List<Raction> followup_ractions;
	public List<Raction> targeted_followup_ractions;
	public List<Taction> targeted_followup_tactions;

	public List<Action> followup_target_actions;
	public List<Raction> targeted_followup_target_ractions;
	public List<Taction> targeted_followup_target_tactions;
}

public class Taction {
	Type type;

	public List<Type> targetTypes;
  
	public int maxRange;
	public int minRange;

	public List<string> conditions; 

	public List<Teffect> teffects;

	public int aoe;

	public List<Raction> aoe_ractions;
	public List<Taction> aoe_tactions;

	public List<Action> followup_actions;
	public List<Taction> followup_tactions;
	
	public List<Raction> targeted_followup_ractions;
	public List<Taction> targeted_followup_tactions;
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

	public List<Action> followup_actions;
	
	public List<Raction> targeted_followup_ractions;
	public List<Taction> targeted_followup_tactions;
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

	public string target_displace_perp;
	public string target_displace_para;
	public string target_displace_alt;

	public string self_displace_perp;
	public string self_displace_para;
	public string self_displace_alt;

	public List<Action> followup_actions;
	public List<Raction> followup_ractions;

	public List<Action> target_followup_actions;

	public List<Raction> targeted_followup_ractions;
	public List<Taction> targeted_followup_tactions;
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

	public string target_displace_perp;
	public string target_displace_para;
	public string target_displace_alt;

	public string self_displace_perp;
	public string self_displace_para;
	public string self_displace_alt;

	public List<Action> followup_actions;
	public List<Taction> followup_Tactions;
	
	public List<Raction> targeted_followup_ractions;
	public List<Taction> targeted_followup_tactions;
}
