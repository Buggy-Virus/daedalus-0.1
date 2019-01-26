using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Thing {

    public ThingDT thingDT;
    public string thingName;
    public bool rendered;

	public Thing(ThingDT tDT) {
        thingDT = tDT;
        System.DateTime timeBegin = new System.DateTime(2018, 6, 8);
        System.DateTime now = System.DateTime.Now;
        string elapsedTicks = (timeBegin.Ticks - now.Ticks).ToString();
        thingName = tDT.thingString + " " + elapsedTicks;
        rendered = false;
    }
}
