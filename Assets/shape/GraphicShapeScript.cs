﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicShapeScript : MonoBehaviour
{
    // ================================================ CONFIGUREABLE VARIABLES
    public float moveSpeed = 10f;

    // ================================================ PUBLIC ATTRIBUTES
	public GameObject shape;
    public ShapeScript shapeScript;
    public Transform graphicObject_transform;
    public Collider graphicObject_collider;

    // ================================================ FETCHED REFERENCES
    PlayControlsScript controlScript;

    // ================================================ GLOBAL VARIABLES
    // movement variables
    public Vector3 goingTo;
    
    // ================================================ Movement
    public void moveTo(Vector3 endPos) {
        goingTo = endPos;
    }

    void move() {
        if (transform.position != goingTo) {
            transform.position = Vector3.MoveTowards(transform.position, goingTo, moveSpeed * Time.deltaTime);
        } 
    }

    // ================================================ START/UPDATE
    void Start() {

    }

    void Update()
    {
        move();
    }
}
