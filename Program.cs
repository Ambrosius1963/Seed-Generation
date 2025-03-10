using System;
using System.Globalization;

class Program
{
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
        string inputText = "Chicken Nuggets";
        string seedFromWord = GenerateSeedFromText(inputText);
        Console.WriteLine($"Generated Seed from text: {seedFromWord}"); 
        int randomInt2 = decisionMaker<int>(seedFromWord, 100.0, 4);
        Console.WriteLine($"Random Int: {randomInt2}");
        
                                                                        
        string NewString = inputText.Insert(5, "T");
        Console.WriteLine(NewString); // Output: HelloTWorld
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

    static T decisionMaker<T>(string seed, double maxValue, int rangeLength)
    {
        string seedString = seed;
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
