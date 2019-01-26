using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerScript : MonoBehaviour {

    int sizeX;
    int sizeZ;

    float cubeSize;

    MapScript mapScript;
    MouseControls controlScript;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;

    // Use this for initialization
    void Start() {
        mapScript = GameObject.Find("Map").GetComponent<MapScript>();
        controlScript = GameObject.Find("Controls").GetComponent<MouseControls>();
        sizeX = mapScript.sizeX;
        sizeZ = mapScript.sizeZ;
        cubeSize = mapScript.cubeSize;

        buildMesh();
    }

    void buildMesh() {
        int numTiles = sizeX * sizeZ;

        int vertSizeX = sizeX + 1;
        int vertSizeZ = sizeZ + 1;
        int numVerts = vertSizeX * vertSizeZ;

        int numTriangles = 2 * numTiles;

        Vector3[] vertices = new Vector3[numVerts];
        int[] triangles = new int[3 * numTriangles];
        Vector3[] normals = new Vector3[numVerts];

        int x, z;
        for (z = 0; z < vertSizeZ; z++) {
            for (x = 0; x < vertSizeX; x++) {
                vertices[z * vertSizeX + x] = new Vector3(x * cubeSize, 0, z * cubeSize);
                normals[z * vertSizeX + x] = Vector3.up;
            }
        }

        for (z = 0; z < sizeZ; z++) {
            for (x = 0; x < sizeX; x++) {
                int squareIndex = z * sizeX + x;
                int triangleIndex = squareIndex * 6;
                int vertOffset = z * vertSizeX + x;

                triangles[triangleIndex + 0] = vertOffset + 0;
                triangles[triangleIndex + 1] = vertOffset + vertSizeX + 0;
                triangles[triangleIndex + 2] = vertOffset + vertSizeX + 1;

                triangles[triangleIndex + 3] = vertOffset + 0;
                triangles[triangleIndex + 4] = vertOffset + vertSizeX + 1;
                triangles[triangleIndex + 5] = vertOffset + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update() {
        if (transform.position.y <= controlScript.activeLayer && !meshRenderer.enabled) {
            meshRenderer.enabled = true;
        } else if (transform.position.y > controlScript.activeLayer && meshRenderer.enabled) {
            meshRenderer.enabled = false;
        }
    }
}
