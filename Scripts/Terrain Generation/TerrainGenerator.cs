using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class TerrainGenerator : MonoBehaviour
{
    public enum DrawMode {NoiseMap, ColorMap, DrawMesh, FalloffMap};
    public enum DrawTextureType {NoiseMap, ColorMap}

    [Header("Terrain Setup")]
    public Vector2 terrainSize = new Vector2(100,100);
    public GameObject textureDisplay;
    public GameObject meshDisplay;

    [Header("Generator Settings")]
    public int seed;
    public Vector2 offset;
    [Range(1f, 50f)] public float terrainHeightMultiplier;
    [Range(0f,100f)] public float noiseScale;
    [Range(1,10)] public int octaves;
    [Range(0f,1f)] public float persistance;
    public float lacunarity;
    public bool useFalloffMap;
    [MinMaxSlider(0f, 1f)] public Vector2 falloff;
    public AnimationCurve terrainHeightCurve;
    public Gradient textureGradient;

    [Header("Others")]
    public bool autoUpdate;
    public DrawMode drawMode;
    public DrawTextureType meshTextureType;
    private void Start() {
        GenerateTerrain();
    }

    public void GenerateTerrain() {
        int terrainWidth = (int)terrainSize.x;
        int terrainHeight = (int)terrainSize.y;
        float[,] noiseMap = Noise.GenerateNoiseMap(terrainWidth, terrainHeight,seed, noiseScale, octaves, persistance, lacunarity, offset);
        float[,] falloffMap = new float[terrainWidth, terrainHeight];
        if(useFalloffMap) {
            falloffMap = TerrainFalloffGenerator.GenerateFalloffMap(terrainWidth, terrainHeight, falloff);
        }

        Color[] colorMap = new Color[terrainWidth * terrainHeight];
        for (int y = 0; y < terrainHeight; y++) {
            for (int x = 0; x < terrainWidth; x++) {
                if(useFalloffMap) {
                    noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] * falloffMap[x,y]);
                }

                // float currentHeight = noiseMap[x,y];
                // for(int i = 0; i < regions.Length; i++) {
                //     if(currentHeight <= regions[i].height) {
                //         // colorMap[y * terrainWidth + x] = regions[i].color;
                //         colorMap[y * terrainWidth + x] = textureGradient.Evaluate(i);
                //         break;
                //     }
                // }
                float currentHeight = noiseMap[x,y];
                colorMap[y * terrainWidth + x] = textureGradient.Evaluate(currentHeight);
            }
        }

        if(drawMode == DrawMode.NoiseMap) {
            DrawTexture(TerrainTextureGenerator.TextureFromHeightMap(noiseMap));
        } else if (drawMode == DrawMode.ColorMap) {
            DrawTexture(TerrainTextureGenerator.TextureFromColorMap(colorMap, terrainWidth, terrainHeight));
        } else if(drawMode == DrawMode.DrawMesh) {
            Texture2D texture = new Texture2D(terrainWidth, terrainHeight);
            if(meshTextureType == DrawTextureType.NoiseMap) {
                texture = TerrainTextureGenerator.TextureFromHeightMap(noiseMap);
            } else if(meshTextureType == DrawTextureType.ColorMap) {
                texture = TerrainTextureGenerator.TextureFromColorMap(colorMap, terrainWidth, terrainHeight);
            }
            DrawMesh(TerrainMeshGenerator.GenerateTerrainMesh(noiseMap, terrainHeightMultiplier, terrainHeightCurve), texture);
        } else if(drawMode == DrawMode.FalloffMap) {
            DrawTexture(TerrainTextureGenerator.TextureFromHeightMap(TerrainFalloffGenerator.GenerateFalloffMap(terrainWidth, terrainHeight, falloff)));
        }
    }

    public void DrawTexture(Texture2D texture) {
        Renderer textureRenderer = textureDisplay.GetComponent<Renderer>();

        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1 ,texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        MeshFilter meshFilter = meshDisplay.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshDisplay.GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }

    private void OnValidate() {
        if(terrainSize.x <= 0) terrainSize.x = 1;
        if(terrainSize.y <= 0) terrainSize.y = 1;
        if(lacunarity < 1) lacunarity = 1;
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
}

[System.Serializable]
public struct MaterialType {
    public string name;
    public Material material;
}