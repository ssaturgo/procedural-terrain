using System.Collections;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int terrainWidth, int terrainHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[terrainWidth,terrainHeight];

        System.Random psuedoRandomNumberGenerator = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        for(int i = 0; i < octaves; i++) {
            float offSetX = psuedoRandomNumberGenerator.Next(-100000, 100000) + offset.x;
            float offSetY = psuedoRandomNumberGenerator.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offSetX, offSetY);
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfTerrainWidth = terrainWidth / 2f;
        float halfTerrainHeight = terrainHeight / 2f;

        for (int y = 0; y < terrainHeight; y++) {
            for(int x = 0; x < terrainWidth; x++) {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++) {
                    float sampleX = (x - halfTerrainWidth) / scale * frequency + octaveOffset[i].x * frequency;
                    float sampleY = (y - halfTerrainHeight) / scale * frequency - octaveOffset[i].y * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) *2 -1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                
                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                } else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }
                noiseMap[x,y] = noiseHeight;
            }
        }
        
        for(int y = 0; y < terrainHeight; y++) {
            for(int x = 0; x < terrainWidth; x++) {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
