using System.Collections;
using UnityEngine;

public class TerrainAnimator : MonoBehaviour
{
    public Terrain terrain;
    public float animationSpeed = 0.5f; // Adjust for speed control
    public float animationDuration = 5f; // Duration of the animation
    public float maxHeight = 0.05f;

    private int terrainWidth;
    private int terrainHeight;
    private float[,] initialHeights;
    private float[,] targetHeights;

    void Start()
    {
        terrainWidth = terrain.terrainData.heightmapResolution;
        terrainHeight = terrain.terrainData.heightmapResolution;

        // Store initial flat height
        initialHeights = terrain.terrainData.GetHeights(0, 0, terrainWidth, terrainHeight);
        
        // Generate target heights using Perlin noise
        targetHeights = GeneratePerlinHeights();

        // Uncomment to start animation automatically
        StartTerrainAnimation();
    }

public void StartTerrainAnimation(bool resetToFlat = false)
{
    Debug.Log("Starting Terrain Animation...");
    StopAllCoroutines(); // Stop any ongoing animation

    if (resetToFlat)
    {
        targetHeights = new float[terrainWidth, terrainHeight]; // Flat terrain (all zeros)
    }
    else
    {
        targetHeights = GeneratePerlinHeights(); // Generate noise-based terrain
    }

    StartCoroutine(AnimateTerrain());
}


    IEnumerator AnimateTerrain()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime; // Ensures smooth animation
            float t = elapsedTime / animationDuration; // Normalized time (0 to 1)

            float[,] currentHeights = new float[terrainWidth, terrainHeight];

            for (int x = 0; x < terrainWidth; x++)
            {
                for (int y = 0; y < terrainHeight; y++)
                {
                    currentHeights[x, y] = Mathf.Lerp(initialHeights[x, y], targetHeights[x, y], t);
                }
            }

            Debug.Log($"Animating... Progress: {t * 100}%");

            terrain.terrainData.SetHeights(0, 0, currentHeights);

            yield return null; // Allow frame update
        }

        terrain.terrainData.SetHeights(0, 0, targetHeights); // Ensure final state
        Debug.Log("Terrain Animation Complete!");
    }

    private float[,] GeneratePerlinHeights()
    {
        float[,] heights = new float[terrainWidth, terrainHeight];
        float scale = 0.001f;

        for (int x = 0; x < terrainWidth; x++)
        {
            for (int y = 0; y < terrainHeight; y++)
            {
                float perlinValue = Mathf.PerlinNoise(x * scale, y * scale);
                perlinValue = Mathf.Lerp(perlinValue, 0.5f, 0.2f); // Blend with a neutral value to smooth

            }
        }

        Debug.Log("Perlin Heights Generated!");
        return heights;
    }
}
