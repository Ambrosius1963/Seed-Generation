using System;
using System.Globalization;

class Program
{
    static void Main()
    static void Main()
    {
        // Generate a seed using the current date and time
        string seed = GenerateSeed();
        Console.WriteLine($"\nGenerated Seed (from Date): {seed}");

        // Example usage of decisionMaker
        int randomInt = decisionMaker<int>(seed, 100.0, 4);
        //double randomDouble = decisionMaker<double>(seed, 50.0, 6);
        //char randomChar = decisionMaker<char>(seed, 26.0, 2);

        Console.WriteLine($"Random Int: {randomInt}");
        //Console.WriteLine($"Random Double: {randomDouble}");
        //Console.WriteLine($"Random Char: {randomChar}");
        
        // Example usage of GenerateSeedFromText
        string inputText = "HelloWorld"; // HelloWorld Output: 05674961687621677140788541440568
        string seedFromWord = GenerateSeedFromText(inputText);
        Console.WriteLine($"Generated Seed from text: {seedFromWord}"); 
                                                                        
        string NewString = inputText.Insert(5, "T");
        Console.WriteLine(NewString); // Output: HelloTWorld
    }
    
    // generate random seed from the Date and Time
    static string GenerateSeed()
    {
        string dtg = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        List<int> seedNumbers = new List<int>();

        // Convert date-time to numbers and mix them randomly
        foreach (char c in dtg)
        {
            seedNumbers.Add((c - '0') * 3 % 10); // Multiply by 3 and mod 10 for initial scrambling
        }

        // Expand and shuffle
        while (seedNumbers.Count < 32)
        {
            for (int i = 0; i < seedNumbers.Count - 1 && seedNumbers.Count < 32; i++)
            {
                int newNum = (seedNumbers[i] * 7 + seedNumbers[i + 1] * 5 + i) % 10; 
                seedNumbers.Add(newNum);
            }
        }
        // Shuffle
        seedNumbers = ShuffleList(seedNumbers);

        // return string
        return string.Join("", seedNumbers);
    }

    // Simple deterministic shuffle (ensures same seed for same input)
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
    // Generate random seed from text
    static string GenerateSeedFromText(string text)
    {
        List<int> seedNumbers = new List<int>();

        // Convert text numbers and mix
        foreach (char c in text)
        {
            seedNumbers.Add((c * 7) % 10); // Multiply by 7 and mod 10 for scrambling
        }
        // If text is less than 32 characters, expand
        while (seedNumbers.Count < 32)
        {
            for (int i = 0; i < seedNumbers.Count - 1 && seedNumbers.Count < 32; i++)
            {
                int newNum = (seedNumbers[i] * 5 + seedNumbers[i + 1] * 3 + i) % 10;
                seedNumbers.Add(newNum);
            }
        }
        // Shuffle
        seedNumbers = ShuffleList(seedNumbers);

        // return string
        return string.Join("", seedNumbers);
    }

    static T decisionMaker<T>(long seed, double maxValue, int rangeLength)
    {
        string seedString = seed.ToString();
        int startIdx = seedString.Length - rangeLength;
        if (startIdx < 0) 
            startIdx = 0;

        int numericSeed = int.Parse(seedString.Substring(startIdx, rangeLength));
        Random random = new Random(numericSeed);

        object result = typeof(T) switch
        {
            Type t when t == typeof(int) => random.Next((int)maxValue),
            //Type t when t == typeof(double) => random.NextDouble() * maxValue,
            //Type t when t == typeof(char) => (char)('A' + random.Next((int)maxValue)),
            _ => throw new NotSupportedException("Unsupported data type")
        };

        return (T)result;
    }
}


// using UnityEngine;

// public class Perlin3DTerrain : MonoBehaviour
// {
//     public int size = 32;
//     public float scale = 10f;
//     public GameObject cubePrefab;
//     public int seed = 12345;

//     void Start()
//     {
//         GenerateTerrain();
//     }

//     void GenerateTerrain()
//     {
//         PerlinNoise perlin = new PerlinNoise(seed);

//         for (int x = 0; x < size; x++)
//         {
//             for (int y = 0; y < size; y++)
//             {
//                 for (int z = 0; z < size; z++)
//                 {
//                     float noiseValue = perlin.Perlin(x / scale, y / scale);
//                     if (noiseValue > 0.5f) // Threshold for terrain
//                     {
//                         Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
//                     }
//                 }
//             }
//         }
//     }
// }



// using System;
// using System.Numerics;

// public class PerlinNoise3D
// {
//     private static readonly int[] PermutationTable;
    
//     static PerlinNoise3D()
//     {
//         Random rand = new Random();
//         PermutationTable = new int[512];
//         int[] p = new int[256];
//         for (int i = 0; i < 256; i++) p[i] = i;
        
//         // Shuffle using Fisher-Yates
//         for (int i = 255; i > 0; i--)
//         {
//             int swapIdx = rand.Next(i + 1);
//             (p[i], p[swapIdx]) = (p[swapIdx], p[i]);
//         }
        
//         for (int i = 0; i < 512; i++)
//             PermutationTable[i] = p[i % 256];
//     }

//     private static Vector3[] Gradients =
//     {
//         new Vector3(1,1,0), new Vector3(-1,1,0), new Vector3(1,-1,0), new Vector3(-1,-1,0),
//         new Vector3(1,0,1), new Vector3(-1,0,1), new Vector3(1,0,-1), new Vector3(-1,0,-1),
//         new Vector3(0,1,1), new Vector3(0,-1,1), new Vector3(0,1,-1), new Vector3(0,-1,-1)
//     };

//     private static int Hash(int x, int y, int z)
//     {
//         return PermutationTable[PermutationTable[PermutationTable[x & 255] + (y & 255)] + (z & 255)] & 11;
//     }

//     private static float Fade(float t)
//     {
//         return t * t * t * (t * (t * 6 - 15) + 10);
//     }

//     private static float Lerp(float a, float b, float t)
//     {
//         return a + t * (b - a);
//     }

//     private static float DotGradient(int hash, float x, float y, float z)
//     {
//         Vector3 g = Gradients[hash];
//         return g.X * x + g.Y * y + g.Z * z;
//     }

//     public static float Noise(float x, float y, float z)
//     {
//         int X = (int)Math.Floor(x) & 255;
//         int Y = (int)Math.Floor(y) & 255;
//         int Z = (int)Math.Floor(z) & 255;
        
//         x -= Math.Floor(x);
//         y -= Math.Floor(y);
//         z -= Math.Floor(z);
        
//         float u = Fade(x);
//         float v = Fade(y);
//         float w = Fade(z);

//         int g000 = Hash(X, Y, Z);
//         int g001 = Hash(X, Y, Z + 1);
//         int g010 = Hash(X, Y + 1, Z);
//         int g011 = Hash(X, Y + 1, Z + 1);
//         int g100 = Hash(X + 1, Y, Z);
//         int g101 = Hash(X + 1, Y, Z + 1);
//         int g110 = Hash(X + 1, Y + 1, Z);
//         int g111 = Hash(X + 1, Y + 1, Z + 1);

//         float n000 = DotGradient(g000, x, y, z);
//         float n100 = DotGradient(g100, x - 1, y, z);
//         float n010 = DotGradient(g010, x, y - 1, z);
//         float n110 = DotGradient(g110, x - 1, y - 1, z);
//         float n001 = DotGradient(g001, x, y, z - 1);
//         float n101 = DotGradient(g101, x - 1, y, z - 1);
//         float n011 = DotGradient(g011, x, y - 1, z - 1);
//         float n111 = DotGradient(g111, x - 1, y - 1, z - 1);
        
//         float nx00 = Lerp(n000, n100, u);
//         float nx01 = Lerp(n001, n101, u);
//         float nx10 = Lerp(n010, n110, u);
//         float nx11 = Lerp(n011, n111, u);
        
//         float nxy0 = Lerp(nx00, nx10, v);
//         float nxy1 = Lerp(nx01, nx11, v);
        
//         return Lerp(nxy0, nxy1, w);lite
//     }
// }
