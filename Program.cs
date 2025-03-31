using System;
using System.Collections.Generic;
using System.Globalization;
using SkiaSharp; // Mac OS
// // Windows
// using System.Drawing;
// using System.Drawing.Imaging;
// using System.Runtime.InteropServices;



//~~~~~~~~~~~~~~~~~~~~~~~
// Questions to consider:
// What is the purpose of this code?
// What are the key components of the code?
// problem to solve?
// Solutions we came up with?
// Problems we had to overcome?
// takeaways?
//
//~~~~~~~~~~~~~~~~~~~~~~~

class Program
{
    static void Main()
    {
        Console.Clear();
        Console.WriteLine("\nWelcome to");
        Console.WriteLine("\n\t【T】【E】【R】【R】【A】【N】【O】【I】【S】【E】\n");
        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        Console.WriteLine("Press enter to generate a seed from the current date and time.");
        Console.ReadLine();

        // Generate a seed using the current date and time
        string seed = GenerateSeed();
        Console.WriteLine($"\n\nGenerated Seed (from Date): {seed}");

        // call decisionMaker 3 times to get 3 random numbers
        List<int> randomInts = decisionMaker<int>(seed, 100.0, 4);
        // Access the three numbers individually
        int xValueDate = randomInts[0];
        int yValueDate = randomInts[1];
        int zValueDate = randomInts[2];

        Console.WriteLine("\n~Random Ints~");
        Console.WriteLine($"x: {xValueDate}");
        Console.WriteLine($"y: {yValueDate}");
        Console.WriteLine($"z: {zValueDate}\n");
        
        // Transfer the seed to the Perlin noise generator
        PerlinNoise dateToNoiseGenerator = new PerlinNoise(int.Parse(seed.Substring(0, Math.Min(9, seed.Length))));

        // Set parameters for Perlin noise generation
        // float noiseScale = 0.08f; // 0.15f for more detail and 0.05f for less detail
        float perlinPictureScaleDate = 0.004f + (zValueDate / 99f) * (0.020f - 0.004f); // around 0.005f; 
        int width = 80;
        int height = 40;

        // Generate and display a small 2D Perlin noise map
        GenerateAndDisplayPerlinNoise(dateToNoiseGenerator, width, height, perlinPictureScaleDate * 8f); // 8f gives good scale
        Console.WriteLine("Perlin noise picture scale: " + perlinPictureScaleDate * 8f);
        // Generate and save a Perlin noise picture bitmap
        // GeneratePerlinNoiseBitmap(dateToNoiseGenerator, 800, 800, perlinPictureScaleDate, "perlinNoiseDate.png");

//~~~~~~~~~~~~~~~~~~~~~~ GenerateSeedFromText ~~~~~~~~~~~~~~~~~~~~~~~
        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        Console.Write("Please enter a seed: ");
        string inputText = Console.ReadLine();
        // inputText and the perlin file name
        string fileName = inputText + "_perlinNoise.png";

        string seedFromWord = GenerateSeedFromText(inputText); // Lots of '%' in 1234567890)(*&^%$#@! AND spaces in 10293847565647382910
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
        
        // Generate and display Perlin noise using the seed from text
        PerlinNoise noiseGenerator = new PerlinNoise(int.Parse(seedFromWord.Substring(0, Math.Min(9, seedFromWord.Length))));
        
        // creates a random scale for the perlin noise picture based on the zValueWord
        float perlinPictureScale = 0.004f + (zValueWord / 99f) * (0.020f - 0.004f); // around 0.005f; 

        // Generate and display a small 2D Perlin noise map
        GenerateAndDisplayPerlinNoise(noiseGenerator, width, height, perlinPictureScale * 12.5f);
        Console.WriteLine("Perlin noise picture scale: " + perlinPictureScale * 12.5f);


        // Generate and save a Perlin noise picture bitmap from WORD input
        GeneratePerlinNoiseBitmap(noiseGenerator, 800, 800, perlinPictureScale, fileName);

        Console.WriteLine("Perlin noise bitmap has been generated and saved as " + fileName + "\n");
    
    }

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Noise picture generator ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

// // Generate a Perlin noise bitmap for WINDOWS
// // Must download System.Drawing.Common NuGet package
// // You will have to dig to find the dll file unless you set a specific path

//     static void GeneratePerlinNoiseBitmap(PerlinNoise noiseGenerator, int width, int height, float scale, string filename)
//     {
//         // Create a new bitmap
//         using (Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb))
//         {
//             // Lock the bitmap's bits
//             BitmapData bmpData = bitmap.LockBits(
//                 new Rectangle(0, 0, width, height), 
//                 ImageLockMode.WriteOnly, 
//                 bitmap.PixelFormat
//             );

//             // Create an array to hold the pixel data
//             int bytesPerPixel = 3; // 24bpp = 3 bytes per pixel (RGB)
//             int byteCount = bmpData.Stride * height;
//             byte[] rgbValues = new byte[byteCount];

//             // Generate noise and convert to grayscale
//             for (int y = 0; y < height; y++)
//             {
//                 for (int x = 0; x < width; x++)
//                 {
//                     // Generate noise value
//                     float noiseValue = noiseGenerator.Noise(x * scale, y * scale);
                    
//                     // Map noise from [-1, 1] to [0, 255]
//                     byte grayValue = (byte)((noiseValue + 1) * 127.5);
                    
//                     // Calculate the index in the byte array
//                     int index = y * bmpData.Stride + x * bytesPerPixel;
                    
//                     // Set RGB values to create grayscale
//                     rgbValues[index] = grayValue;         // Blue
//                     rgbValues[index + 1] = grayValue;     // Green
//                     rgbValues[index + 2] = grayValue;     // Red
//                 }
//             }

//             // Copy the RGB values back to the bitmap
//             Marshal.Copy(rgbValues, 0, bmpData.Scan0, byteCount);
            
//             // Unlock the bits
//             bitmap.UnlockBits(bmpData);

//             // Save the bitmap
//             bitmap.Save(filename, ImageFormat.Png);
//         }
//     }

// Generate a Perlin noise bitmap using SkiaSharp for MACBOOK
    static void GeneratePerlinNoiseBitmap(PerlinNoise noiseGenerator, int width, int height, float scale, string filename)
    {
        // Create a new SKBitmap
        using (SKBitmap bitmap = new SKBitmap(width, height))
        {
            // Create a canvas to draw on the bitmap
            using (SKCanvas canvas = new SKCanvas(bitmap))
            {
                // Fill the canvas with a white background
                canvas.Clear(SKColors.White);

                // Create a paint for drawing pixels
                using (SKPaint paint = new SKPaint())
                {
                    // Generate noise and draw pixels
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // Generate noise value
                            float noiseValue = noiseGenerator.Noise(x * scale, y * scale);
                            
                            // Map noise from [-1, 1] to [0, 255]
                            byte grayValue = (byte)((noiseValue + 1) * 127.5);
                            byte alphaValue = 255; // opacity value
                            byte blueValue = 255; // blue value
                            byte greenValue = 255; // green value
                            byte redValue = 255; // red value

                            
                            // Create a color with the grayscale value
                            // values are (R, G, B, A)
                            paint.Color = new SKColor(grayValue, grayValue, grayValue,alphaValue);
                            // paint.Color = new SKColor(redValue, greenValue, blueValue, alphaValue);
                            
                            // Draw a single pixel
                            canvas.DrawPoint(x, y, paint);
                        }
                    }
                }
            }

            // Save the bitmap to a file
            using (var image = SKImage.FromBitmap(bitmap))
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                // Save to file
                using (var stream = System.IO.File.OpenWrite(filename))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }


// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    // Generate and display a 2D Perlin noise map using ASCII characters
    static void GenerateAndDisplayPerlinNoise(PerlinNoise noiseGenerator, int width, int height, float scale)
    {
        // Characters from dark to light for ASCII representation
        char[] asciiChars = { ' ', '_', '.', ':', '=', '*', '%', '#', '@', '^' }; //good test seeds: 10293847565647382910 and 1234567890)(*&^%$#@!

        // reverse the array to get light to dark
        // char[] asciiChars = { '@', '%', '#', '*', '+', '=', '-', ':', '.', ' ' };
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
