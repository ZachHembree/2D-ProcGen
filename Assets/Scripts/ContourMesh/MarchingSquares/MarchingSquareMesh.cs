using System.Collections.Generic;
using UnityEngine;

public partial class MarchingSquare
{
    List<int> triangles;
    List<int> activeIndexList;
    MeshData meshData = new MeshData();

    // Assemble the positions of all enabled nodes into a list in clockwise order
    public MeshData GetPoints(int vertexIndexStart)
    {
        GetMidPoints();

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
        triangles = new List<int>();

        for (int n = 0; n < 4; n++)
        {
           if (activeIndexList.Count > 2 + n)
            {
                GetTriangle(activeIndexList[0], activeIndexList[n + 1], activeIndexList[n + 2]);
;            }
        }
    }

    private void GetTriangle(int a, int b, int c)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);

     //   Debug.Log("Triangle: " + meshData.verticies[a].GetString() + " " + meshData.verticies[b].GetString() + " " + meshData.verticies[c].GetString());
    }
}
