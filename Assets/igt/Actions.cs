﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action {
	public string name;	
	public bool relational;
	public bool selfTargetable;
	public bool targeted;

	public int maxRange;
	public int minRange;

	public List<string> show_conditions;
	public List<string> available_conditions;
	public List<string> call_conditions;

	public List<string> conditions;

	public List<Effect> effects;

	public int aoe;

	public List<Action> aoe_relational_actions;
	public List<Action> aoe_targeted_actions;

	public List<Action> followup_actions;
	public List<Action> target_followup_actions;
	
	public List<Action> targeted_followup_actions;
	public List<Action> target_targeted_followup_actions;

	public Action(
		string n,
		List<string> s_c,
		List<string> a_c,
		List<string> c,
		List<Effect> e,
		int a,
		List<Action> a_r_a,
		List<Action> a_t_a,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		name = n;
		show_conditions = s_c;
		available_conditions = a_c;
		conditions = c;
		effects = e;
		aoe = a;
		aoe_relational_actions = a_r_a;
		aoe_targeted_actions = a_t_a;
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}

	public Action( // relational
		string n,
		List<string> s_c,
		List<string> a_c,
		List<string> c_c,
		List<string> c,
		List<Effect> e,
		int a,
		List<Action> a_r_a,
		List<Action> a_t_a,
		List<Action> f_a,
		List<Action> t_f_a,
		List<Action> tt_f_a,
		List<Action> tt_t_f_a
	) {
		relational = false;
		targeted = false;
		name = n;
		show_conditions = s_c;
		available_conditions = a_c;
		call_conditions = c_c;
		conditions = c;
		effects = e;
		aoe = a;
		aoe_relational_actions = a_r_a;
		aoe_targeted_actions = a_t_a;
		followup_actions = f_a;
		target_targeted_followup_actions = t_f_a;
		target_followup_actions = tt_f_a;
		target_targeted_followup_actions = tt_t_f_a;
	}

	public Action( // targeted
		string n,
		List<string> s_c,
		List<string> a_c,
		List<string> c_c,
		List<string> c,
		List<Effect> e,
		int a,
		List<Action> a_r_a,
		List<Action> a_t_a,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		relational = false;
		targeted = false;
		name = n;
		show_conditions = s_c;
		available_conditions = a_c;
		call_conditions = c_c;
		conditions = c;
		effects = e;
		aoe = a;
		aoe_relational_actions = a_r_a;
		aoe_targeted_actions = a_t_a;
		followup_actions = f_a;
		target_targeted_followup_actions = t_f_a;
	}
}

public class Effect {
	public string name;
	public string givenName;
	public GameObject relative;

	public bool relational;
	public bool targeted;	
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

	public bool target_displace;
	public string target_displace_perp;
	public string target_displace_para;
	public string target_displace_alt;

	public bool self_displace;
	public string self_displace_perp;
	public string self_displace_para;
	public string self_displace_alt;

	public List<Action> followup_actions;
	public List<Action> target_followup_actions;
	
	public List<Action> targeted_followup_actions;
	public List<Action> target_targeted_followup_actions;

	public Effect(
		string n,
		string c,
		bool i,
		string e_c,
		int f,
		string s,
		bool d,
		string d_x,
		string d_y,
		string d_z,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		name = n;
		condition = c;
		instant = i;
		if (!i) {
			endCondition = e_c;
			frequency = f;
		}
		script = s;
		displace = d;
		if (d) {
			displace_x = d_x;
			displace_y = d_y;
			displace_z = d_z;
		}
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}

	public Effect( // relational
		string n,
		string c,
		bool i,
		string e_c,
		int f,
		string s,
		bool s_d,
		string s_d_perp,
		string s_d_para,
		string s_d_alt,
		bool t_d,
		string t_d_perp,
		string t_d_para,
		string t_d_alt,
		List<Action> f_a,
		List<Action> t_f_a,
		List<Action> tt_f_a,
		List<Action> tt_t_f_a
	) {
		name = n;
		condition = c;
		instant = i;
		if (!i) {
			endCondition = e_c;
			frequency = f;
		}
		script = s;
		self_displace = s_d;
		if (s_d) {
			self_displace_para = s_d_para;
			self_displace_perp = s_d_perp;
			self_displace_alt = s_d_alt;
		}
		target_displace = t_d;
		if (t_d) {
			target_displace_para = t_d_para;
			target_displace_perp = t_d_perp;
			target_displace_alt = t_d_alt;
		}
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
		target_followup_actions = tt_f_a;
		target_targeted_followup_actions = tt_t_f_a;
	}

	public Effect( // targeted
		string n,
		string c,
		bool i,
		string e_c,
		int f,
		string s,
		bool s_d,
		string s_d_perp,
		string s_d_para,
		string s_d_alt,
		bool t_d,
		string t_d_perp,
		string t_d_para,
		string t_d_alt,
		List<Action> f_a,
		List<Action> t_f_a
	) {
		name = n;
		condition = c;
		instant = i;
		if (!i) {
			endCondition = e_c;
			frequency = f;
		}
		script = s;
		self_displace = s_d;
		if (s_d) {
			self_displace_para = s_d_para;
			self_displace_perp = s_d_perp;
			self_displace_alt = s_d_alt;
		}
		target_displace = t_d;
		if (t_d) {
			target_displace_para = t_d_para;
			target_displace_perp = t_d_perp;
			target_displace_alt = t_d_alt;
		}
		followup_actions = f_a;
		targeted_followup_actions = t_f_a;
	}
}