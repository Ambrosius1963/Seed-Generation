using System;
using System.Collections.Generic;
using System.Globalization;

class Program
{
    static void Main()
    {
        // Generate a seed using the current date and time
        string seed = GenerateSeed();
        Console.WriteLine($"\n\nGenerated Seed (from Date): {seed}");

        // call decisionMaker 3 times to get 3 random numbers
        List<int> randomInts = decisionMaker<int>(seed, 100.0, 4);
        // Access the three numbers individually
        int xValueDate = randomInts[0];
        int yValueDate = randomInts[1];
        int ZValueDate = randomInts[2];

        Console.WriteLine("\n~Random Ints from Date~");
        Console.WriteLine($"x: {xValueDate}");
        Console.WriteLine($"y: {yValueDate}");
        Console.WriteLine($"z: {ZValueDate}\n");
        
        // Transfer the seed to the Perlin noise generator
        PerlinNoise dateToNoiseGenerator = new PerlinNoise(int.Parse(seed.Substring(0, Math.Min(9, seed.Length))));

        // Generate and display a small 2D Perlin noise map
        int width = 80;
        int height = 40;
        GenerateAndDisplayPerlinNoise(dateToNoiseGenerator, width, height, 0.08f); // try 0.15f for more detail, and 0.05f for less

        // Example usage of GenerateSeedFromText
        Console.Write("Please enter a seed: ");
        string inputText = Console.ReadLine();

        string seedFromWord = GenerateSeedFromText(inputText);
        Console.WriteLine($"Text: {inputText}");
        Console.WriteLine($"Generated Seed from text: {seedFromWord}"); 

        List<int> randomIntsFromWord = decisionMaker<int>(seedFromWord, 100.0, 4);
        int xValueWord = randomIntsFromWord[0];
        int yValueWord = randomIntsFromWord[1];
        int zValueWord = randomIntsFromWord[2];

        Console.WriteLine("\n~Random Ints from Word~");
        Console.WriteLine($"x: {xValueWord}");
        Console.WriteLine($"y: {yValueWord}");
        Console.WriteLine($"z: {zValueWord}\n");
        
        // Generate and display Perlin noise
        Console.WriteLine("\n~2D Perlin Noise from text~");
        
        // Create a Perlin noise generator using the seed from text
        PerlinNoise noiseGenerator = new PerlinNoise(int.Parse(seedFromWord.Substring(0, Math.Min(9, seedFromWord.Length))));
        
        // Generate and display a small 2D Perlin noise map
        // int width = 40;
        // int height = 20;
        GenerateAndDisplayPerlinNoise(noiseGenerator, width, height, 0.08f); // try 0.15f for more detail, and 0.05f for less
    }
    
    // Generate and display a 2D Perlin noise map using ASCII characters
    static void GenerateAndDisplayPerlinNoise(PerlinNoise noiseGenerator, int width, int height, float scale)
    {
        // Characters from dark to light for ASCII representation
        char[] asciiChars = { ' ', '.', ':', '-', '=', '+', '*', '#', '%', '@' };
        // char[] asciiChars = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        
        Console.WriteLine($"Perlin Noise Map ({width}x{height}):");
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Generate noise value at this position
                float noiseValue = noiseGenerator.Noise(x * scale, y * scale);
                
                // Map the noise value (-1 to 1) to a character index (0 to asciiChars.Length-1)
                int charIndex = (int)Math.Floor((noiseValue + 1) * 0.5f * (asciiChars.Length - 1));
                charIndex = Math.Clamp(charIndex, 0, asciiChars.Length - 1);
                
                // Display the character
                Console.Write(asciiChars[charIndex]);
            }
            Console.WriteLine();
        }
    }
    
    // generate random seed from the Date and Time
    static string GenerateSeed()
    {
        string dtg = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);

        int sfff = int.Parse(dtg.Substring(13, 4));

        long seedValue = long.Parse(dtg) * sfff; 
        // more math here! to get more random

        string stringDTG = seedValue.ToString();
        List<int> seedNumbers = new List<int>();

        // Convert date-time to numbers and mix them deterministically
        foreach (char c in stringDTG)
        {
            if (char.IsDigit(c))
            {
                seedNumbers.Add((c - '0') * 37 % 10); // Multiply by 3 and mod 10 for initial scrambling
            }
        }

        seedNumbers.Reverse();

        // Expand and shuffle
        while (seedNumbers.Count < 32)
        {
            for (int i = 0; i < seedNumbers.Count - 1 && seedNumbers.Count < 32; i++)
            {
                int newNum = (seedNumbers[i] * 89 + seedNumbers[i + 1] * 97 + i * 7) % 10;
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

    static List<T> decisionMaker<T>(string seed, double maxValue, int rangeLength)
    {
        string seedString = seed;
        int startIdx = seedString.Length - rangeLength;
        // Console.Out.WriteLine($"Start Index: {startIdx}"); // Debugging
        if (startIdx < 0) 
            startIdx = 0;

        int numericSeed = int.Parse(seedString.Substring(startIdx, rangeLength));
        Random random = new Random(numericSeed);

        List<T> results = new List<T>();

        for (int i = 0; i < 3; i++)
        {
            object result = typeof(T) switch
            {
                Type t when t == typeof(int) => random.Next((int)maxValue),
                // Type t when t == typeof(double) => random.NextDouble() * maxValue,
                // Type t when t == typeof(char) => (char)('A' + random.Next((int)maxValue)),
                _ => throw new NotSupportedException("Unsupported data type")
            };
            results.Add((T)result);
        }

        return results;
    }
}

// Perlin Noise Generator class
class PerlinNoise
{
    private int[] permutation;
    
    // Constructor with seed
    public PerlinNoise(int seed)
    {
        Random random = new Random(seed);
        permutation = new int[512];
        
        // Initialize permutation array
        int[] p = new int[256];
        for (int i = 0; i < 256; i++)
        {
            p[i] = i;
        }
        
        // Shuffle using the provided seed
        for (int i = 255; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (p[i], p[j]) = (p[j], p[i]);
        }
        
        // Duplicate the permutation array
        for (int i = 0; i < 256; i++)
        {
            permutation[i] = permutation[i + 256] = p[i];
        }
    }
    
    // 2D Perlin noise function
    public float Noise(float x, float y)
    {
        // Find unit grid cell containing the point
        int X = (int)Math.Floor(x) & 255;
        int Y = (int)Math.Floor(y) & 255;
        
        // Get relative position within the cell
        x -= (float)Math.Floor(x);
        y -= (float)Math.Floor(y);
        
        // Compute fade curves
        float u = Fade(x);
        float v = Fade(y);
        
        // Hash coordinates of the 4 corners
        int A = permutation[X] + Y;
        int AA = permutation[A];
        int AB = permutation[A + 1];
        int B = permutation[X + 1] + Y;
        int BA = permutation[B];
        int BB = permutation[B + 1];
        
        // Add blended results from 4 corners
        float result = Lerp(v,
            Lerp(u, Grad(permutation[AA], x, y, 0), Grad(permutation[BA], x - 1, y, 0)),
            Lerp(u, Grad(permutation[AB], x, y - 1, 0), Grad(permutation[BB], x - 1, y - 1, 0))
        );
        
        // Return result in the range [-1, 1]
        return result;
    }
    
    // Fade function for smoother interpolation
    private float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }
    
    // Linear interpolation
    private float Lerp(float t, float a, float b)
    {
        return a + t * (b - a);
    }
    
    // Gradient function
    private float Grad(int hash, float x, float y, float z)
    {
        // Convert lower 4 bits of hash into 12 gradient directions
        int h = hash & 15;
        float u = h < 8 ? x : y;
        float v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}


// v~~~~~~~~~~~~~~~~~~~~~~~~v Previous code Without Perlin noise representations v~~~~~~~~~~~~~~~~~~~~~~~~v 

// using System;
// using System.Globalization;

// class Program
// {
//     static void Main()
//     {
//         // Generate a seed using the current date and time
//         string seed = GenerateSeed();
//         Console.WriteLine($"\n\nGenerated Seed (from Date): {seed}");

//         // Example usage of decisionMaker
//         // int randomInt = decisionMaker<int>(seed, 100.0, 4);
        
//         // call desicionMaker 3 times to get 3 random numbers
//         List<int> randomInts = decisionMaker<int>(seed, 100.0, 4);
//         // Access the three numbers individually
//         int xValueDate = randomInts[0];
//         int yValueDate = randomInts[1];
//         int ZValueDate = randomInts[2];

//         //double randomDouble = decisionMaker<double>(seed, 50.0, 6);
//         //char randomChar = decisionMaker<char>(seed, 26.0, 2);

//         // Console.WriteLine($"Random Int: {randomInts}");
//         Console.WriteLine("\n~Random Ints from Date~");
//         Console.WriteLine($"x: {xValueDate}");
//         Console.WriteLine($"y: {yValueDate}");
//         Console.WriteLine($"z: {ZValueDate}\n");

//         //Console.WriteLine($"Random Double: {randomDouble}");
//         //Console.WriteLine($"Random Char: {randomChar}");
        
//         // Example usage of GenerateSeedFromText
//         // string inputText = "Chicken Nuggets rule the World!!"; // Chicken Nuggets rule the World!!: 45694173826542680987491077817199
//         Console.Write("Please enter a seed: ");
//         string inputText = Console.ReadLine();

//         string seedFromWord = GenerateSeedFromText(inputText);
//         Console.WriteLine($"Text: {inputText}");
//         Console.WriteLine($"Generated Seed from text: {seedFromWord}"); 
//         // int randomInt2 = decisionMaker<int>(seedFromWord, 100.0, 4);

//         List<int> randomIntsFromWord = decisionMaker<int>(seedFromWord, 100.0, 4);
//         // int randomInt2 = randomIntsFromWord[0];
//         int xValueWord = randomIntsFromWord[0];
//         int yValueWord = randomIntsFromWord[1];
//         int zValueWord = randomIntsFromWord[2];

//         // Console.WriteLine($"Random Int: {randomInt2}");
//         Console.WriteLine("\n~Random Ints from Word~");
//         Console.WriteLine($"x: {xValueWord}");
//         Console.WriteLine($"y: {yValueWord}");
//         Console.WriteLine($"z: {zValueWord}\n");
        
                                                                        
//         // string NewString = inputText.Insert(5, "T");
//         // Console.WriteLine(NewString);
//     }
    
//     // generate random seed from the Date and Time
//     static string GenerateSeed()
//     {
//         string dtg = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);

//         int sfff = int.Parse(dtg.Substring(13, 4));

//         long seedValue = long.Parse(dtg) * sfff; 
//         // more math here! to get more random

//         string stringDTG = seedValue.ToString();
//         List<int> seedNumbers = new List<int>();

//         // Convert date-time to numbers and mix them deterministically
//         foreach (char c in stringDTG)
//         {
//             if (char.IsDigit(c))
//             {
//                 seedNumbers.Add((c - '0') * 37 % 10); // Multiply by 3 and mod 10 for initial scrambling
//             }
//         }

//         seedNumbers.Reverse();

//         // Expand and shuffle
//         while (seedNumbers.Count < 32)
//         {
//             for (int i = 0; i < seedNumbers.Count - 1 && seedNumbers.Count < 32; i++)
//             {
//                 int newNum = (seedNumbers[i] * 89 + seedNumbers[i + 1] * 97 + i * 7) % 10;
//                 seedNumbers.Add(newNum);
//             }
//         }
//         // Shuffle
//         seedNumbers = ShuffleList(seedNumbers);

//         // return string
//         return string.Join("", seedNumbers);
//     }

//     // Simple deterministic shuffle (ensures same seed for same input)
//     static List<int> ShuffleList(List<int> list)
//     {
//         List<int> shuffled = new List<int>(list);
//         for (int i = 0; i < shuffled.Count; i++)
//         {
//             int swapIndex = (shuffled[i] * 3 + i * 2) % shuffled.Count;
//             (shuffled[i], shuffled[swapIndex]) = (shuffled[swapIndex], shuffled[i]);
//         }
//         return shuffled;
//     }
    
//     // Generate random seed from text
//     static string GenerateSeedFromText(string text)
//     {
//         List<int> seedNumbers = new List<int>();

//         // Convert text numbers and mix
//         foreach (char c in text)
//         {
//             seedNumbers.Add((c * 7) % 10); // Multiply by 7 and mod 10 for scrambling
//         }
//         // If text is less than 32 characters, expand
//         while (seedNumbers.Count < 32)
//         {
//             for (int i = 0; i < seedNumbers.Count - 1 && seedNumbers.Count < 32; i++)
//             {
//                 int newNum = (seedNumbers[i] * 5 + seedNumbers[i + 1] * 3 + i) % 10;
//                 seedNumbers.Add(newNum);
//             }
//         }
//         // Shuffle
//         seedNumbers = ShuffleList(seedNumbers);

//         // return string
//         return string.Join("", seedNumbers);
//     }

//     static List<T> decisionMaker<T>(string seed, double maxValue, int rangeLength)
//     {
//         string seedString = seed;
//         int startIdx = seedString.Length - rangeLength;
//         // Console.Out.WriteLine($"Start Index: {startIdx}"); // Debugging
//         if (startIdx < 0) 
//             startIdx = 0;

//         int numericSeed = int.Parse(seedString.Substring(startIdx, rangeLength));
//         Random random = new Random(numericSeed);

//         List<T> results = new List<T>();

//         for (int i = 0; i < 3; i++)
//         {
//             object result = typeof(T) switch
//             {
//                 Type t when t == typeof(int) => random.Next((int)maxValue),
//                 // Type t when t == typeof(double) => random.NextDouble() * maxValue,
//                 // Type t when t == typeof(char) => (char)('A' + random.Next((int)maxValue)),
//                 _ => throw new NotSupportedException("Unsupported data type")
//             };
//             results.Add((T)result);
//         }

//         return results;
//     }

// }
