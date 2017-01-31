using System.Collections.Generic;
using UnityEngine;

public partial class MarchingSquare
{
    int[] activeIndices;
    List<int> triangles;
    List<int> activeIndexList;

    // Assemble the positions of all enabled nodes into a list in clockwise order
    public MeshData GetPoints(int vertexIndexStart)
    {
        GetMidPoints();

        MeshData meshData = new MeshData();
        meshData.verticies = new List<Vector3>();
        activeIndexList = new List<int>();

        for (int n = 0; n < 8; n++)
        {
            if (nodes[n].active)
            {
                meshData.verticies.Add(nodes[n].position);
                nodes[n].vertexIndex = meshData.verticies.Count + (vertexIndexStart - 1);

                activeIndexList.Add(nodes[n].vertexIndex);
            }
        }

        GetTriangles();
        meshData.triangles = triangles;

        return meshData;
    }

    private void GetTriangles()
    {
        activeIndices = activeIndexList.ToArray();
        triangles = new List<int>();

        for (int n = 0; n < 4; n++)
        {
           if (activeIndices.Length > 2 + n)
            {
                GetTriangle(activeIndices[0], activeIndices[n + 1], activeIndices[n + 2]);
            }
        }
    }

    private void GetTriangle(int a, int b, int c)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }
}
