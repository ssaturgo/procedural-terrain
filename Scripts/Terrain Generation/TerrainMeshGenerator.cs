using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainMeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve) {
        int terrainWidth = heightMap.GetLength(0);
        int terrainHeight = heightMap.GetLength(1);
        float topLeftX = (terrainWidth-1) / -2f;
        float topLeftZ = (terrainHeight-1) / 2f;

        MeshData meshData = new MeshData(terrainWidth, terrainHeight);
        int vertexIndex = 0;

        for (int y = 0; y < terrainHeight; y++) {
            for (int x = 0; x < terrainWidth; x++) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x,heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)terrainWidth, y/(float)terrainHeight);

                if(x < terrainWidth-1 && y < terrainHeight-1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex+terrainWidth+1, vertexIndex + terrainWidth);
                    meshData.AddTriangle(vertexIndex + terrainWidth +1, vertexIndex, vertexIndex +1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth-1) * (meshHeight -1) * 6];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}