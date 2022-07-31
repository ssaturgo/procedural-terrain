using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainTextureGenerator
{
    public static Texture2D TextureFromColorMap(Color[] colorMap, int terrainWidth, int terrainHeight) {
        Texture2D texture = new Texture2D(terrainWidth, terrainHeight);
        texture.filterMode = FilterMode.Trilinear;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }
    
    public static Texture2D TextureFromHeightMap( float[,] heightMap){
        int terrainWidth = heightMap.GetLength(0);
        int terrainHeight = heightMap.GetLength(1);

        Color[] colorMap = new Color[terrainWidth * terrainHeight];
        
        for(int y = 0; y < terrainHeight; y++) {
            for(int x = 0; x < terrainWidth; x++) {
                colorMap[y * terrainWidth + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]);
            }
        }
        return TextureFromColorMap(colorMap, terrainWidth, terrainHeight);
    }
}
