using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class SeededRandomGenerator : MonoBehaviour
{
    public TMP_InputField userSeedInput;
    public TMP_Text dateSeedOutput;
    public TMP_Text userSeedOutput;
    public TMP_Text randomNumbersDate;
    public TMP_Text randomNumbersUser;
    public Button generateDateSeedButton;
    public Button generateUserSeedButton;
    public Terrain terrain; // Reference to the Unity Terrain
    public GameObject terrainParent;


    void Start()
    {
        generateDateSeedButton.onClick.AddListener(GenerateFromDateSeed);
        generateUserSeedButton.onClick.AddListener(GenerateFromUserSeed);
    }

    void GenerateFromDateSeed()
    {
        string seed = GenerateSeed();
        dateSeedOutput.text = "Date Seed: " + seed;
        List<int> randomNumbers = DecisionMaker<int>(seed, 100.0, 4);
        randomNumbersDate.text = $"x: {randomNumbers[0]} | y: {randomNumbers[1]} | z: {randomNumbers[2]}";

        int numericSeed = ConvertSeedToInt(seed);
        StartCoroutine(GenerateTerrainWithAnimation(numericSeed));
    }

    void GenerateFromUserSeed()
    {
        string inputText = userSeedInput.text;
        if (string.IsNullOrWhiteSpace(inputText)) return;

        string seed = GenerateSeedFromText(inputText);
        userSeedOutput.text = "User Seed: " + seed;
        List<int> randomNumbers = DecisionMaker<int>(seed, 100.0, 4);
        randomNumbersUser.text = $"x: {randomNumbers[0]} | y: {randomNumbers[1]} | z: {randomNumbers[2]}";

        int numericSeed = ConvertSeedToInt(seed);
        StartCoroutine(GenerateTerrainWithAnimation(numericSeed));
    }

    static string GenerateSeed()
    {
        string dtg = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        int sfff = int.Parse(dtg.Substring(13, 4));
        long seedValue = long.Parse(dtg) * sfff;

        string stringDTG = seedValue.ToString();
        List<int> seedNumbers = new List<int>();

        foreach (char c in stringDTG)
        {
            if (char.IsDigit(c))
            {
                seedNumbers.Add((c - '0') * 37 % 10);
            }
        }

        return string.Join("", ShuffleList(seedNumbers));
    }

    static string GenerateSeedFromText(string text)
    {
        List<int> seedNumbers = new List<int>();

        foreach (char c in text)
        {
            seedNumbers.Add((c * 7) % 10);
        }

        return string.Join("", ShuffleList(seedNumbers));
    }

    static List<int> ShuffleList(List<int> list)
    {
        List<int> shuffled = new List<int>(list);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int swapIndex = (shuffled[i] * 3 + i * 2) % shuffled.Count;
            (shuffled[i], shuffled[swapIndex]) = (shuffled[swapIndex], shuffled[i]);
        }
        return shuffled;
    }

    static List<T> DecisionMaker<T>(string seed, double maxValue, int rangeLength)
    {
        int numericSeed = int.Parse(seed.Substring(seed.Length - rangeLength, rangeLength));
        System.Random random = new System.Random(numericSeed);

        List<T> results = new List<T>();

        for (int i = 0; i < 3; i++)
        {
            object result = typeof(T) switch
            {
                Type t when t == typeof(int) => random.Next((int)maxValue),
                _ => throw new NotSupportedException("Unsupported data type")
            };
            results.Add((T)result);
        }

        return results;
    }

    int ConvertSeedToInt(string seed)
    {
        string numericPart = new string(seed.Where(char.IsDigit).ToArray());
        return int.Parse(numericPart.Substring(0, Math.Min(numericPart.Length, 9)));
    }

    // Coroutine to animate terrain growth
    IEnumerator GenerateTerrainWithAnimation(int seed)
    {
        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];

        UnityEngine.Random.InitState(seed);
        float scale = 20f;

        // Initial state: flat terrain
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = 0f; // Flat terrain
            }
        }

        terrainData.SetHeights(0, 0, heights);

        // Animation loop: gradually increase terrain height to its final shape
        float progress = 0f;
        float animationSpeed = 0.1f;  // Control the speed of the animation

        while (progress < 1f)
        {
            progress += animationSpeed * Time.deltaTime;
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float xCoord = (float)x / width * scale + seed * 0.001f;
                    float yCoord = (float)y / height * scale + seed * 0.001f;
                    heights[x, y] = Mathf.Lerp(0f, Mathf.PerlinNoise(xCoord, yCoord), progress);
                }
            }

            terrainData.SetHeights(0, 0, heights);
            yield return null;  // Wait for the next frame
        }

        // Final terrain heights
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale + seed * 0.001f;
                float yCoord = (float)y / height * scale + seed * 0.001f;
                heights[x, y] = Mathf.PerlinNoise(xCoord, yCoord);
            }
        }

        terrainData.SetHeights(0, 0, heights);  // Ensure the final state is set
    }

    public void ResetTerrain()
    {
        if (terrainParent == null)
        {
            Debug.LogError("Terrain parent is not assigned!");
            return;
        }

        Terrain terrain = terrainParent.GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogError("No Terrain component found on the assigned object!");
            return;
        }

        TerrainData terrainData = terrain.terrainData;
        int width = terrainData.heightmapResolution;
        int height = terrainData.heightmapResolution;
        float[,] heights = new float[width, height];

        // Flatten the terrain
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = 0f; // Set height to flat (0)
            }
        }

        terrainData.SetHeights(0, 0, heights);
    }
}
