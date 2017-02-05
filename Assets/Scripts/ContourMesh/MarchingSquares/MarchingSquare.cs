using UnityEngine;

public partial class MarchingSquare
{
    private Node[] nodes; 
    private int pointCounter = 0;
    public bool initialized = false;

    // Nodes are indexed in clockwise order relative to their vectors
    // The v and h bools are used to determine positions and cases
    public MarchingSquare(Vector3 position, float size, bool a, bool b, bool c, bool d)
    {
        nodes = new Node[8];
        initialized = true;

        // Control Nodes
        nodes[0] = new Node(false, false, position, size, true, a);
        nodes[2] = new Node(true, false, position, size, true, b);
        nodes[4] = new Node(true, true, position, size, true, c);
        nodes[6] = new Node(false, true, position, size, true, d);

        // Midpoint nodes
        nodes[1] = new Node(false, false, position, size);
        nodes[3] = new Node(false, true, position, size);
        nodes[5] = new Node(true, false, position, size);
        nodes[7] = new Node(true, true, position, size);
        
        SetAllMidpoints(false);
    }

    // Configuration cases; called by GetPoints()
    private void GetMidPoints()
    {
        // Index of relevant ControlNodes
        int a, b;
        int[] activePoints;

        // Count active points/ControlNodes
        for (int n = 0; n < 8; n += 2)
            if (nodes[n].active) pointCounter++;
        
        // Initialize array and store active points in order
        activePoints = new int[pointCounter];
        int activePos = 0;

        for (int n = 0; n < 8; n += 2)
        {
            if (nodes[n].active)
            {
                activePoints[activePos] = n;
                activePos++;
            }
        }

        // Ensure all midpoints are set to false before starting
        SetAllMidpoints(false);

        if (pointCounter == 1)
        {
            // Find adjacent midpoint nodes
            a = activePoints[0];
            SetMidPoints(a);
        }
        else if (pointCounter == 2)
        {
            a = activePoints[0];
            b = activePoints[1];

            SetMidPoints(a, b);
        }
        else if (pointCounter == 3)
        {
            // 3 points are active; find the inactive one
            for (int n = 0; n < 8; n += 2)
            {
                if (!nodes[n].active)
                {
                    SetMidPoints(n);
                    break;
                }
            }
        }
    }

    // One/Three point active
    private void SetMidPoints(int a)
    {
        if (nodes[a].hPos)
            nodes[5].active = true;
        else
            nodes[1].active = true;

        if (nodes[a].vPos)
            nodes[3].active = true;
        else
            nodes[7].active = true;
    }

    // Two points active
    private void SetMidPoints(int a, int b)
    {
        if (nodes[a].vPos != nodes[b].vPos && nodes[a].hPos == nodes[b].hPos) // Half vertical
        {
            nodes[3].active = true;
            nodes[7].active = true;
        }
        else if (nodes[a].vPos == nodes[b].vPos && nodes[a].hPos != nodes[b].hPos) // Half horizontal
        {
            nodes[1].active = true;
            nodes[5].active = true;
        }
        else // Ambiguous; points opposing
        {
            SetAllMidpoints(true);
        }
    }

    // Disable/enable all Midpoint Nodes
    private void SetAllMidpoints(bool v)
    {
        nodes[1].active = v;
        nodes[3].active = v;
        nodes[5].active = v;
        nodes[7].active = v;
    }
}
