using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type {
	public string name;

	public Type parent;
	public bool originator;

	public Type (string n, Type p) {
		name = n;
		parent = p;
		originator = false;
	}

	public Type (string n) {
		name = n;
		parent = null;
		originator = true;
	}
}
