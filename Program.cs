using System;
using System.Globalization;

class Program
{
    static void Main()
    {
        // Generate a seed using the current date and time
        long seed = GenerateSeed();
        Console.WriteLine($"Generated Seed: {seed}");

        // Example usage of decisionMaker
        int randomInt = decisionMaker<int>(seed, 100.0, 4);
        //double randomDouble = decisionMaker<double>(seed, 50.0, 6);
        //char randomChar = decisionMaker<char>(seed, 26.0, 2);

        Console.WriteLine($"Random Int: {randomInt}");
        //Console.WriteLine($"Random Double: {randomDouble}");
        //Console.WriteLine($"Random Char: {randomChar}");
        
        // Example usage of GenerateSeedFromText
        string inputText = "HelloWorld";
        int seedFromWord = GenerateSeedFromText(inputText);
        Console.WriteLine($"Generated Seed: {seedFromWord}");

        string NewString = inputText.Insert(5, "T");
        Console.WriteLine(NewString); // Output: HelloTWorld
        
        
    }

    static long GenerateSeed()
    {
        char[] dtg = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture).ToCharArray();
        int i = 0;

        do
        {
            int cursor1 = int.Parse(dtg[0 + i].ToString());
            Console.Out.WriteLine($"cursor1: {cursor1}");

            int cursor2 = int.Parse(dtg[4 + i].ToString());
            Console.Out.WriteLine($"cursor2: {cursor2}");

            int cursor3 = (cursor1 + cursor2) % 10;
            dtg[8 + i] = (char)('0' + cursor3);
            Console.Out.WriteLine($"cursor3: {cursor3}");

            i++; 
        } while (i < 32); 
        return long.Parse(new string(dtg));
    }

    static int GenerateSeedFromText(string text)
    {
        unchecked
        {
            int hash = 0;
            foreach (char c in text)
            {
                hash = (hash * 31) + c; // Multiply by prime number and add char value
            }
            return hash & 0x7FFFFFFF; // Ensure it's a positive 32-bit integer
        }
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
