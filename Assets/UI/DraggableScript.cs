﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DraggableScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	public Transform target;
	private bool isMouseDown = false;
	private Vector3 startMousePosition;
	private Vector3 startPosition;
	public bool shouldReturn;

	public void OnPointerDown(PointerEventData dt) {
		isMouseDown = true;
		startPosition = target.localPosition;
		startMousePosition = Input.mousePosition;
	}

	public void OnPointerUp(PointerEventData dt) {
		isMouseDown = false;
		if (shouldReturn) {
			target.localPosition = startPosition;
		}
	}

	// Update is called once per frame
	void Update () {
		if (isMouseDown) {
			Vector3 currentPosition = Input.mousePosition;
			Vector3 diff = currentPosition - startMousePosition;
			Vector3 pos = startPosition + diff;
			target.localPosition = pos;
		}
	}
}