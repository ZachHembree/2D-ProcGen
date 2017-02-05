using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata 
{
    private int fillPercent, resolution, smoothingPasses, width, height;
    private string seed;
    private bool useRandom;
    private bool[,] map;

    public CellularAutomata(int _fillPercent, int _resolution, int _smoothingPasses, int _width, int _height, string _seed, bool _useRandom)
    {
        fillPercent = _fillPercent;
        resolution = _resolution;
        smoothingPasses = _smoothingPasses;
        width = _width;
        height = _height;
        seed = _seed;
        useRandom = _useRandom;
        map = new bool[width, height];
    }

    public bool[,] GenerateMap()
    {
        width *= resolution;
        height *= resolution;

        if ((width % 2) == 1) width++;
        if ((height & 2) == 1) height++;

        map = new bool[width, height];
        RandomFillMap();

        for (int n = 0; n < smoothingPasses; n++)
        {
            SmoothMap();
        }

        return map;
    }

    private void RandomFillMap()
    {
        if (useRandom) GetSeed();
        System.Random rand = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    map[x, y] = true;
                else
                {
                    if (rand.Next(0, 100) < fillPercent)
                        map[x, y] = true;
                    else
                        map[x, y] = false;
                }
            }
        }
    }

    private void GetSeed()
    {
        char[] trimSeed;
        seed = Time.unscaledTime.ToString();

        trimSeed = new char[seed.Length - 3];
        for (int n = 3; n < seed.Length; n++) trimSeed[n - 3] = seed[n];

        seed = "";
        for (int n = 0; n < trimSeed.Length; n++) seed += trimSeed[n];
    }

    private void SmoothMap()
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

    private int GetSurroundingWalls(int x, int y)
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
