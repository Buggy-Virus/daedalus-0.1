using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event {
    string name;
    bool gameTime;
    string condition;
    string endCondition;

    List<Popup> popups;

    Transition transition; 

    // List<Token, Action> Actions;
}