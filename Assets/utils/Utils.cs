using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {

    static public bool indexEqual(Index ind1, Index ind2) {
		return (ind1.x == ind2.x && ind1.y == ind2.y && ind1.z == ind2.z);
	}

    static public int distance(Index point_a, Index point_b) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_z = Math.Abs(point_a.z - point_b.z);

        int max_xy = Math.Max(dist_x, dist_y);
        int min_xy = Math.Min(dist_x, dist_y);
        int dist_xy;
        if (min_xy % 2 == 0) {
            dist_xy = max_xy - min_xy + 3 * (min_xy / 2);
        } else {
            dist_xy = max_xy - min_xy + 3 * ((min_xy - 1) / 2) + 1;
        }
        
        int max_xyz = Math.Max(dist_z, dist_xy);
        int min_xyz = Math.Min(dist_z, dist_xy);
        int dist;
        if (min_xyz % 2 == 0) {
            dist = max_xyz - min_xyz + 3 * (min_xyz / 2);
        } else {
            dist = max_xyz - min_xyz + 3 * ((min_xyz - 1) / 2) + 1;
        }

        return dist;
    }

    static public int distance_xy(Index point_a, Index point_b) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_z = Math.Abs(point_a.z - point_b.z);

        int max_xy = Math.Max(dist_x, dist_y);
        int min_xy = Math.Min(dist_x, dist_y);
        if (min_xy % 2 == 0) {
            return max_xy - min_xy + 3 * (min_xy / 2);
        } else {
            return max_xy - min_xy + 3 * ((min_xy - 1) / 2) + 1;
        }
    }

    static public Dictionary<int, Index> line(Index point_a, Index point_b, int length) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_z = Math.Abs(point_a.z - point_b.z);
        int dist_max = Math.Max(dist_x, Math.Max(dist_y, dist_z));

        Dictionary<int, Index> line = new Dictionary<int, Index>();
        double stepFrequency_x = dist_x / dist_max;
        double stepFrequency_y = dist_y / dist_max;
        double stepFrequency_z = dist_z / dist_max;
        for (int i = -length; i <= length; i++) {
            int pos_x;
            int pos_y;
            int pos_z;
            if (point_a.x < point_b.x) {
                pos_x = point_a.x + (int)(i * stepFrequency_x);
            } else {
                pos_x = point_a.x - (int)(i * stepFrequency_x);
            }
            
            if (point_a.y < point_b.y) {
                pos_y = point_a.y + (int)(i * stepFrequency_y);
            } else {
                pos_y = point_a.y - (int)(i * stepFrequency_y);
            }

            if (point_a.z <= point_b.z) {
                pos_z = point_a.z + (int)(i * stepFrequency_z);
            } else {
                pos_z = point_a.z - (int)(i * stepFrequency_z);
            }
            Index index_i = new Index(pos_x, pos_y, pos_z);
            int dist_i = distance(point_a, index_i);
            line[dist_i] = index_i;
        }
        return line;
    }

    static public Dictionary<int, Index> line_xy(Index point_a, Index point_b, int length) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_max = Math.Max(dist_x, dist_y);

        Dictionary<int, Index> line = new Dictionary<int, Index>();
        double stepFrequency_x = dist_x / dist_max;
        double stepFrequency_y = dist_y / dist_max;
        for (int i = -length; i <= length; i++) {
            int pos_x;
            int pos_y;
            if (point_a.x <= point_b.x) {
                pos_x = point_a.x + (int)(i * stepFrequency_x);
            } else {
                pos_x = point_a.x - (int)(i * stepFrequency_x);
            }
            
            if (point_a.y <= point_b.y) {
                pos_y = point_a.y + (int)(i * stepFrequency_y);
            } else {
                pos_y = point_a.y - (int)(i * stepFrequency_y);
            }
            Index index_i = new Index(pos_x, pos_y, point_a.z);
            int dist_i = distance_xy(point_a, index_i);
            line.Add(dist_i, index_i);
        }
        return line;
    }

    static public Dictionary<int, Index> linePerp_xy(Index point_a, Index point_b, int length) {
        int dist_x = Math.Abs(point_a.x - point_b.x);
        int dist_y = Math.Abs(point_a.y - point_b.y);
        int dist_max = Math.Max(dist_x, dist_y);

        Dictionary<int, Index> line = new Dictionary<int, Index>();
        double stepFrequency_x = dist_x / dist_max;
        double stepFrequency_y = dist_y / dist_max;
        for (int i = -length; i <= length; i++) {
            int pos_x;
            int pos_y;
            if (point_a.x <= point_b.x) {
                pos_y = point_a.y - (int)(i * stepFrequency_y);
            } else {
                pos_y = point_a.y + (int)(i * stepFrequency_y);
            }
            
            if (point_a.y <= point_b.y) {
                pos_x = point_a.x + (int)(i * stepFrequency_x);
            } else {
                pos_x = point_a.x - (int)(i * stepFrequency_x);
            }
            Index index_i = new Index(pos_x, pos_y, point_a.z);
            int dist_i = distance_xy(point_a, index_i);
            line.Add(dist_i, index_i);
        }
        return line;
    }

    static public Vector3 polarToCartesian(float radius, float piRadian) {
        float x = (float)(radius * Math.Cos(Math.PI * piRadian));
        float y = (float)(radius * Math.Sin(Math.PI * piRadian));
        return new Vector3(x, y, 0);
    }
}