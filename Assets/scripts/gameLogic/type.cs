using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class type {
	public string name;

	public type parent;
	public bool originator;

	public type (string n, type p) {
		name = n;
		parent = p;
		originator = false;
	}

	public type (string n) {
		name = n;
		parent = null;
		originator = true;
	}
}
