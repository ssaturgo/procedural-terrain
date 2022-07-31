using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainFalloffGenerator
{
    public static float[,] GenerateFalloffMap(int terrainWidth, int terrainHeight, Vector2 falloff) {

        float[,] falloffMap = new float[terrainWidth, terrainHeight];

        for (int y = 0; y < terrainHeight; y++) {
            for (int x = 0; x < terrainWidth; x++) {
                Vector2 position = new Vector2(
                    x/(float)terrainWidth * 2 - 1,
                    y/(float)terrainHeight * 2 - 1
                );

                // find which value is closer to the edge
                float value = Mathf.Max(Mathf.Abs(position.x), Mathf.Abs(position.y));
                
                if(value < falloff.x) {
                    falloffMap[x,y] = 1;
                } else if(value > falloff.y) {
                    falloffMap[x,y] = 0;
                } else {
                    falloffMap[x,y] = Mathf.SmoothStep(1,0, Mathf.InverseLerp(falloff.x,falloff.y, value));
                }
            }
        }
        return falloffMap;
    }
}