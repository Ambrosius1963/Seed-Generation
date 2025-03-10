using UnityEngine;

public class Perlin3DTerrain : MonoBehaviour
{
    public int size = 16;  // Adjust size of terrain
    public float scale = 5f;  // Controls noise spread
    public GameObject cubePrefab; // Assign in Inspector
    public int seed = 12345;  // Seed for noise

    void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    float noiseValue = PerlinNoise3D.Noise(x / scale, y / scale, z / scale);
                    
                    if (noiseValue > 0.3f) // Adjust threshold for terrain density
                    {
                        Debug.Log($"Placing block at ({x}, {y}, {z}) with noise {noiseValue}");
                        Instantiate(cubePrefab, new Vector3(x, y, z), Quaternion.identity);
                    }
                }
            }
        }
    }
}
