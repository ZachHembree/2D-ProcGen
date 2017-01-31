using System.Collections.Generic;
using UnityEngine;

public struct MeshData
{
    public List<Vector3> verticies;
    public List<int> triangles;
}

public class Node
{
    public Vector3 position;
    public bool active, isControlNode, vPos, hPos;
    public float size, xDelta = 0, zDelta = 0;
    public int vertexIndex;

    public Node(bool v, bool h, Vector3 p, float s, bool cNode = false, bool a = false)
    {
        size = s;
        vPos = v;
        hPos = h; 
        isControlNode = cNode;
        active = a;

        // Set offset based on position in the square
        if (isControlNode)
            ControlNodePos();
        else
            MidPointNodePos(); 

        p.x += xDelta;
        p.y = 0;
        p.z += zDelta;

        position = p;
    }

    private void ControlNodePos()
    {
        if (hPos)
        {
            zDelta = -(size / 2);

            if (vPos) xDelta = (size / 2);
            else xDelta = -(size / 2);
        }
        else
        {
            zDelta = (size / 2);

            if (vPos) xDelta = (size / 2);
            else xDelta = -(size / 2);
        }
    }

    private void MidPointNodePos()
    {
        if (vPos)
        {
            if (hPos)
                xDelta = -(size / 2);
            else
                zDelta = -(size / 2);
        }
        else
        {
            if (hPos)
                xDelta = (size / 2);
            else
                zDelta = (size / 2);
        }
    }
}
