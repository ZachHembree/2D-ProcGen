using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MapDimensions
{
    public int width, height;
}

[System.Serializable]
public struct ProcGenSettings
{
    [Range(0, 100)]
    public int randomFill;

    [Range(1,8)]
    public int resolution;

    [Range(0, 8)]
    public int smoothing;
    public bool randomSeed;
    public string seed;
}

[System.Serializable]
public struct MeshSettings
{
    public bool drawContours, drawSimple, uniqueVertcies;
}

public class MapGeneration : MonoBehaviour
{
    public MapDimensions mapDimensions;
    public ProcGenSettings proceduralGeneration;
    public MeshSettings meshSettings;

    private int randomFillPercent;        
    private int resolution = 1, smoothingPasses, width, height;
    private string seed;
    private bool useRandomSeed;
    private bool[,] map;
    private Mesh mesh;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        GenerateMap();
    }

    void Update()
    {
        if (Input.GetButton("Fire1") && useRandomSeed) GenerateMap();
    }

    void GenerateMap()
    {
        randomFillPercent = proceduralGeneration.randomFill;
        resolution = proceduralGeneration.resolution;
        smoothingPasses = proceduralGeneration.smoothing;
        seed = proceduralGeneration.seed;
        useRandomSeed = proceduralGeneration.randomSeed;

        width = mapDimensions.width * resolution;
        height = mapDimensions.height * resolution;

        if ((width % 2) == 1) width++;
        if ((height & 2) == 1) height++;

        map = new bool[width, height];
        RandomFillMap();

        for (int n = 0; n < smoothingPasses; n++)
        {
           SmoothMap();
        }

        ContouredMesh MeshGenerator = new ContouredMesh(mesh, map, 1 / (float)resolution);

        MeshGenerator.drawContours = meshSettings.drawContours;
        MeshGenerator.drawSimple = meshSettings.drawSimple;
        MeshGenerator.uniqueVertcies = meshSettings.uniqueVertcies;

        MeshGenerator.GetMesh();
    }

    void RandomFillMap()
    {
        if (useRandomSeed) GetSeed();
        System.Random rand = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = true;
                else
                {
                    if (rand.Next(0, 100) < randomFillPercent)
                        map[x, y] = true;
                    else
                        map[x, y] = false;
                }
            }
        }
    }

    void GetSeed()
    {
        char[] trimSeed;
        seed = Time.unscaledTime.ToString();

        trimSeed = new char[seed.Length - 3];
        for (int n = 3; n < seed.Length; n++) trimSeed[n - 3] = seed[n];

        seed = "";
        for (int n = 0; n < trimSeed.Length; n++) seed += trimSeed[n];
    }

    void SmoothMap()
    {
        int wallCount;

        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                wallCount = GetSurroundingWalls(x, y);

                if (wallCount > 4) map[x, y] = true;
                else map[x, y] = false;
            }
        }
    }

    int GetSurroundingWalls(int x, int y)
    {
        int wallCount = 0;

        for (int nX = x - 1; nX <= x + 1; nX++)
        {
            for (int nY = y - 1; nY <= y + 1; nY++)
            {
                if (nX >= 0 && nX < width && nY >= 0 && nY < height)
                {
                    if (map[nX, nY]) wallCount++;
                }
            }
        }

        return wallCount;
    }
}
