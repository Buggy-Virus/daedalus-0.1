using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
    static public int distance(Index pointA, Index pointB) {
        int distX = pointA.x - pointB.x;
        int distY = pointA.y - pointB.y;
        int distZ = pointA.z - pointB.z;

        int maxXY = Math.Max(distX, distY);
        int minXY = Math.Min(distX, distY);
        int distXY;
        if (minXY % 2 == 0) {
            distXY = maxXY - minXY + 3 * (minXY / 2);
        } else {
            distXY = maxXY - minXY + 3 * ((minXY - 1) / 2) + 1;
        }
        
        int maxXYZ = Math.Max(distZ, distXY);
        int minXYZ = Math.Min(distZ, distXY);
        int dist;
        if (minXYZ % 2 == 0) {
            dist = maxXYZ - minXYZ + 3 * (minXYZ / 2);
        } else {
            dist = maxXYZ - minXYZ + 3 * ((minXYZ - 1) / 2) + 1;
        }

        return dist;
    }
}