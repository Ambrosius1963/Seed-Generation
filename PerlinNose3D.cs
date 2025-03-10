using System;
using System.Numerics;

public static class PerlinNoise3D
{
    private static readonly int[] PermutationTable;

    static PerlinNoise3D()
    {
        Random rand = new Random();
        PermutationTable = new int[512];
        int[] p = new int[256];

        for (int i = 0; i < 256; i++) p[i] = i;

        for (int i = 255; i > 0; i--)
        {
            int swapIdx = rand.Next(i + 1);
            (p[i], p[swapIdx]) = (p[swapIdx], p[i]);
        }

        for (int i = 0; i < 512; i++)
            PermutationTable[i] = p[i % 256];
    }

    private static Vector3[] Gradients =
    {
        new Vector3(1,1,0), new Vector3(-1,1,0), new Vector3(1,-1,0), new Vector3(-1,-1,0),
        new Vector3(1,0,1), new Vector3(-1,0,1), new Vector3(1,0,-1), new Vector3(-1,0,-1),
        new Vector3(0,1,1), new Vector3(0,-1,1), new Vector3(0,1,-1), new Vector3(0,-1,-1)
    };

    private static int Hash(int x, int y, int z)
    {
        return PermutationTable[PermutationTable[PermutationTable[x & 255] + (y & 255)] + (z & 255)] & 11;
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private static float DotGradient(int hash, float x, float y, float z)
    {
        Vector3 g = Gradients[hash];
        return g.X * x + g.Y * y + g.Z * z;
    }

    public static float Noise(float x, float y, float z)
    {
        int X = (int)MathF.Floor(x) & 255;
        int Y = (int)MathF.Floor(y) & 255;
        int Z = (int)MathF.Floor(z) & 255;


        x -= (float)Math.Floor(x);
        y -= (float)Math.Floor(y);
        z -= (float)Math.Floor(z);


        float u = Fade(x);
        float v = Fade(y);
        float w = Fade(z);

        int g000 = Hash(X, Y, Z);
        int g001 = Hash(X, Y, Z + 1);
        int g010 = Hash(X, Y + 1, Z);
        int g011 = Hash(X, Y + 1, Z + 1);
        int g100 = Hash(X + 1, Y, Z);
        int g101 = Hash(X + 1, Y, Z + 1);
        int g110 = Hash(X + 1, Y + 1, Z);
        int g111 = Hash(X + 1, Y + 1, Z + 1);

        float n000 = DotGradient(g000, x, y, z);
        float n100 = DotGradient(g100, x - 1, y, z);
        float n010 = DotGradient(g010, x, y - 1, z);
        float n110 = DotGradient(g110, x - 1, y - 1, z);
        float n001 = DotGradient(g001, x, y, z - 1);
        float n101 = DotGradient(g101, x - 1, y, z - 1);
        float n011 = DotGradient(g011, x, y - 1, z - 1);
        float n111 = DotGradient(g111, x - 1, y - 1, z - 1);

        float nx00 = Lerp(n000, n100, u);
        float nx01 = Lerp(n001, n101, u);
        float nx10 = Lerp(n010, n110, u);
        float nx11 = Lerp(n011, n111, u);

        float nxy0 = Lerp(nx00, nx10, v);
        float nxy1 = Lerp(nx01, nx11, v);

        return Lerp(nxy0, nxy1, w);
    }
}
