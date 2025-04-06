using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        string seed = GenerateSeed();

        List<int> randomInts = DecisionMaker<int>(seed, 100.0, 4);
        int xValueDate = randomInts[0];
        int yValueDate = randomInts[1];
        int zValueDate = randomInts[2];

        PerlinNoise dateToNoiseGenerator = new PerlinNoise(int.Parse(seed.Substring(0, Math.Min(9, seed.Length))));

        float noiseScale = 0.02f; // Lowered to smooth terrain
        int width = 80;
        int height = 40;

        GenerateTerrainFeatures(dateToNoiseGenerator, width, height, noiseScale);
    }

    static void GenerateTerrainFeatures(PerlinNoise noiseGenerator, int width, int height, float scale)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float noiseValue = (noiseGenerator.Noise(x * scale, y * scale) + 1) * 0.5f;
                string terrainType = GetTerrainType(noiseValue);


                // Display the terrain feature (replace with rendering logic)
                Console.Write(terrainType[0]); 
            }
            Console.WriteLine();
        }
    }

    static string GetTerrainType(float noiseValue)
    {
        noiseValue = Math.Clamp(noiseValue, -0.5f, 0.5f); // Clamp extreme values

        if (noiseValue < -0.1)
            return "Water";
        else if (noiseValue < 0.2)
            return "Plains";
        else if (noiseValue < 0.35)
            return "Forest";
        else
            return "Mountain";
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
                seedNumbers.Add((c - '0') * 37 % 10);
        }

        seedNumbers.Reverse();
        while (seedNumbers.Count < 32)
        {
            for (int i = 0; i < seedNumbers.Count - 1 && seedNumbers.Count < 32; i++)
            {
                int newNum = (seedNumbers[i] * 89 + seedNumbers[i + 1] * 97 + i * 7) % 10;
                seedNumbers.Add(newNum);
            }
        }

        seedNumbers = ShuffleList(seedNumbers);
        return string.Join("", seedNumbers);
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
        int sliceStart = Math.Max(0, seed.Length - rangeLength);
        int numericSeed = int.Parse(seed.Substring(sliceStart, Math.Min(rangeLength, seed.Length)));

        var random = new Random(numericSeed);
        var results = new List<T>();

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
}

// Perlin Noise Generator
class PerlinNoise
{
    private int[] permutation;

    public PerlinNoise(int seed)
    {
        Random random = new Random(seed);
        permutation = new int[512];

        int[] p = new int[256];
        for (int i = 0; i < 256; i++)
        {
            p[i] = i;
        }

        for (int i = 255; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (p[i], p[j]) = (p[j], p[i]);
        }

        for (int i = 0; i < 256; i++)
        {
            permutation[i] = permutation[i + 256] = p[i];
        }
    }

    public float Noise(float x, float y)
    {
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;

        x -= (float)Math.Floor(x);
        y -= (float)Math.Floor(y);

        float u = Fade(x);
        float v = Fade(y);

        int a = permutation[X] + Y;
        int b = permutation[X + 1] + Y;

        return Lerp(v, Lerp(u, Grad(permutation[a], x, y), Grad(permutation[b], x - 1, y)),
                    Lerp(u, Grad(permutation[a + 1], x, y - 1), Grad(permutation[b + 1], x - 1, y - 1)));
    }

    private float Fade(float t) => t * t * t * (t * (t * 6 - 15) + 10);
    private float Lerp(float t, float a, float b) => a + t * (b - a);

    private float Grad(int hash, float x, float y)
    {
        int h = hash & 15;
        float u = h < 8 ? x : y;
        return ((h & 1) == 0 ? u : -u);
    }
}
