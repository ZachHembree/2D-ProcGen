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

    [Range(1,20)]
    public int resolution;

    [Range(0, 32)]
    public int smoothing;
    public bool randomSeed;
    public string seed;

    [Range(1, 8)]
    public int threads;
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

    private CellularAutomata randomMap;
    private ThreadedContourMesh meshGenerator;
    private MeshFilter meshFilter;
    private CombineInstance[] meshCombine;
    private int fillPercent, resolution, smoothingPasses, width, height, threadCount;
    private float squareSize;
    private string seed;
    private bool useRandom, renderDone = false;
    private bool[,] map;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        GetRandomMap();
    }

    private void Update()
    {
        if (meshGenerator != null)
        {
            if (!renderDone) meshGenerator.MeshQueue();
            else if (Input.GetButton("Fire1") && useRandom) GetRandomMap();
        }
    }

    private void GetRandomMap()
    {
        // Dont run another mesh if another render is still running
        renderDone = false;

        // Map dimensions
        width = mapDimensions.width;
        height = mapDimensions.height;

        // ProcGen settings
        fillPercent = proceduralGeneration.randomFill;
        resolution = proceduralGeneration.resolution;
        smoothingPasses = proceduralGeneration.smoothing;
        seed = proceduralGeneration.seed;
        useRandom = proceduralGeneration.randomSeed;
        threadCount = proceduralGeneration.threads;

        // Inversely proportional to resolution; same size but more complex meshes
        squareSize = 1 / (float)resolution;

        // Generates random 2d array of booleans
        randomMap = new CellularAutomata(fillPercent, resolution, smoothingPasses, width, height, seed, useRandom);
        map = randomMap.GenerateMap();

        // Creates contoured mesh from bool array
        meshGenerator = new ThreadedContourMesh(RecieveMeshData, threadCount, map, squareSize, meshSettings.drawContours, meshSettings.drawSimple, meshSettings.uniqueVertcies);
        meshGenerator.StartThreads();
    }

    public void RecieveMeshData(MeshData[] meshData)
    {
        meshCombine = new CombineInstance[meshData.Length];

        // Create meshes in a CombineInstance array for combining the meshes from each thread
        for (int n = 0; n < meshData.Length; n ++)
        {
            meshCombine[n] = new CombineInstance();
            meshCombine[n].mesh = new Mesh();
            meshCombine[n].mesh.vertices = meshData[n].verticies.ToArray();
            meshCombine[n].mesh.triangles = meshData[n].triangles.ToArray();
            meshCombine[n].transform = meshFilter.transform.localToWorldMatrix;
        }

        SetMesh();
    }

    private void SetMesh()
    {
        // Clear old mesh and generate new one by combining the recieved meshes
        meshFilter.mesh.Clear();
        meshFilter.mesh.CombineMeshes(meshCombine);
        gameObject.GetComponent<MeshCollider>().sharedMesh = meshFilter.mesh;

        Debug.Log("Final Verticies: " + meshFilter.mesh.vertexCount);
        Debug.Log("Final Triangles: " + meshFilter.mesh.triangles.Length / 3);

        meshFilter.mesh.RecalculateNormals();
        renderDone = true;
    }
}
