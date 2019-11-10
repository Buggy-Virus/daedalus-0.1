using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	public string name;	
	public int actionType;
	public bool relational;
	public bool selfTargetable;
	public bool targeted;
	public bool wallTargeted;

	public int maxRange;
	public int minRange;

	public List<string> show_conditions;
	public List<string> available_conditions;
	public List<string> call_conditions;

	public List<string> conditions;

	public List<Effect> effects;

	public bool aoe;
	public int aoe_radius;
	public List<Action> aoe_relational_actions;
	public List<Action> aoe_targeted_actions;

	public bool repeat;
	public List<Action> followup_actions;
	public List<Action> target_followup_actions;
	
	public bool targeted_repeat;
	public List<Action> targeted_followup_actions;
	public List<Action> target_targeted_followup_actions;

	public List<Effect> conditional_effects;

	public bool conditional_aoe;
	public int conditional_aoe_radius;

	public List<Action> conditional_aoe_relational_actions;
	public List<Action> conditional_aoe_targeted_actions;

	public bool conditional_repeat;
	public List<Action> conditional_followup_actions;
	public List<Action> conditional_target_followup_actions;
	
	public bool conditional_targeted_repeat;
	public List<Action> conditional_targeted_followup_actions;
	public List<Action> conditional_target_targeted_followup_actions;

	public Action() { }

	public Action(
		string n,
		int a_t,
		List<string> s_c,
		List<string> a_c,
		List<string> c,
		List<Effect> e,
		bool a,
		int a_r,
		List<Action> a_r_a,
		List<Action> a_t_a,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = false;
		name = n;
		actionType = a_t;
		show_conditions = s_c;
		available_conditions = a_c;
		conditions = c;
		effects = e;
		aoe = a;
		aoe_radius = a_r;
		aoe_relational_actions = a_r_a;
		aoe_targeted_actions = a_t_a;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}

	public Action( // relational
		string n,
		int a_t,
		List<string> s_c,
		List<string> a_c,
		List<string> c_c,
		List<string> c,
		List<Effect> e,
		bool a,
		int a_r,
		List<Action> a_r_a,
		List<Action> a_t_a,
		List<Action> f_a,
		List<Action> t_f_a,
		List<Action> tt_f_a,
		List<Action> tt_t_f_a
	) {
		relational = true;
		targeted = false;
		name = n;
		actionType = a_t;
		show_conditions = s_c;
		available_conditions = a_c;
		call_conditions = c_c;
		conditions = c;
		effects = e;
		aoe = a;
		aoe_radius = a_r;
		aoe_relational_actions = a_r_a;
		aoe_targeted_actions = a_t_a;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
		target_followup_actions = tt_f_a;
		target_targeted_followup_actions = tt_t_f_a;
	}

	public Action( // targeted
		string n,
		int a_t,
		List<string> s_c,
		List<string> a_c,
		List<string> c_c,
		List<string> c,
		List<Effect> e,
		bool a,
		int a_r,
		List<Action> a_r_a,
		List<Action> a_t_a,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = true;
		name = n;
		actionType = a_t;
		show_conditions = s_c;
		available_conditions = a_c;
		call_conditions = c_c;
		conditions = c;
		effects = e;
		aoe = a;
		aoe_radius = a_r;
		aoe_relational_actions = a_r_a;
		aoe_targeted_actions = a_t_a;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}
}

public class Effect {
	public string name;
	public string givenName;
	public GameObject relative;

	public bool relational;
	public bool targeted;
	public bool wallTargeted;	
	public bool stacks;


	public bool instant;
	public List<string> procConditions;
	public List<string> endConditions;
	public int frequency;
	public int timeLeft;

	public List<string> scripts;
	
	public string target_displace;
	public string target_displace_perp;
	public string target_displace_para;
	public string target_displace_alt;

	public string self_displace;
	public string self_displace_perp;
	public string self_displace_para;
	public string self_displace_alt;

	public List<Action> followup_actions;
	public List<Action> target_followup_actions;
	
	public List<Action> targeted_followup_actions;
	public List<Action> target_targeted_followup_actions;

	public List<string> conditions;
	public List<string> conditional_scripts;
	
	public string conditional_target_displace;
	public string conditional_target_displace_perp;
	public string conditional_target_displace_para;
	public string conditional_target_displace_alt;

	public string conditional_self_displace;
	public string conditional_self_displace_perp;
	public string conditional_self_displace_para;
	public string conditional_self_displace_alt;

	public List<Action> conditional_followup_actions;
	public List<Action> conditional_target_followup_actions;
	
	public List<Action> conditional_targeted_followup_actions;
	public List<Action> conditional_target_targeted_followup_actions;

	public Effect () { }

	public Effect(
		string n,
		List<string> c,
		List<string> e_c,
		int f,
		List<string> s,
		string d,
		string d_x,
		string d_y,
		string d_z,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = false;
		name = n;
		instant = false;
		procConditions = c;
		endConditions = e_c;
		frequency = f;
		scripts = s;
		self_displace = d;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}

	public Effect(
		string n,
		List<string> c,
		List<string> s,
		string d,
		string d_x,
		string d_y,
		string d_z,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = false;
		name = n;
		instant = true;
		procConditions = c;
		scripts = s;
		self_displace = d;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}

	public Effect( // relational
		string n,
		List<string> c,
		List<string> e_c,
		int f,
		List<string> s,
		string s_d,
		string s_d_perp,
		string s_d_para,
		string s_d_alt,
		string t_d,
		string t_d_perp,
		string t_d_para,
		string t_d_alt,
		List<Action> f_a,
		List<Action> t_f_a,
		List<Action> tt_f_a,
		List<Action> tt_t_f_a
	) {
		relational = true;
		targeted = false;
		name = n;
		instant = false;
		procConditions = c;
		endConditions = e_c;
		frequency = f;
		scripts = s;
		self_displace = s_d;
		self_displace_para = s_d_para;
		self_displace_perp = s_d_perp;
		self_displace_alt = s_d_alt;
		target_displace = t_d;
		target_displace_para = t_d_para;
		target_displace_perp = t_d_perp;
		target_displace_alt = t_d_alt;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
		target_followup_actions = tt_f_a;
		target_targeted_followup_actions = tt_t_f_a;
	}

	public Effect( // relational
		string n,
		List<string> c,
		List<string> s,
		string s_d,
		string s_d_perp,
		string s_d_para,
		string s_d_alt,
		string t_d,
		string t_d_perp,
		string t_d_para,
		string t_d_alt,
		List<Action> f_a,
		List<Action> t_f_a,
		List<Action> tt_f_a,
		List<Action> tt_t_f_a
	) {
		relational = true;
		targeted = false;
		name = n;
		instant = true;
		procConditions = c;
		scripts = s;
		self_displace = s_d;
		self_displace_para = s_d_para;
		self_displace_perp = s_d_perp;
		self_displace_alt = s_d_alt;
		target_displace = t_d;
		target_displace_para = t_d_para;
		target_displace_perp = t_d_perp;
		target_displace_alt = t_d_alt;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
		target_followup_actions = tt_f_a;
		target_targeted_followup_actions = tt_t_f_a;
	}

	public Effect( // targeted
		string n,
		List<string> c,
		List<string> e_c,
		int f,
		List<string> s,
		string s_d,
		string s_d_perp,
		string s_d_para,
		string s_d_alt,
		string t_d,
		string t_d_perp,
		string t_d_para,
		string t_d_alt,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = true;
		name = n;
		instant = false;
		procConditions = c;
		endConditions = e_c;
		frequency = f;
		scripts = s;
		self_displace = s_d;
		self_displace_para = s_d_para;
		self_displace_perp = s_d_perp;
		self_displace_alt = s_d_alt;
		target_displace = t_d;
		target_displace_para = t_d_para;
		target_displace_perp = t_d_perp;
		target_displace_alt = t_d_alt;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}

	public Effect( // targeted
		string n,
		List<string> c,
		List<string> s,
		string s_d,
		string s_d_perp,
		string s_d_para,
		string s_d_alt,
		string t_d,
		string t_d_perp,
		string t_d_para,
		string t_d_alt,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = true;
		name = n;
		instant = true;
		procConditions = c;
		scripts = s;
		self_displace = s_d;
		self_displace_para = s_d_para;
		self_displace_perp = s_d_perp;
		self_displace_alt = s_d_alt;
		target_displace = t_d;
		target_displace_para = t_d_para;
		target_displace_perp = t_d_perp;
		target_displace_alt = t_d_alt;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}
}