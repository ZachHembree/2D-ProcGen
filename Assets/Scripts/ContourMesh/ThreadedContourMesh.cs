using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public struct MapData
{
    public int x, z;
}

public struct MapCells
{
    public int xOffset, zOffset;
    public bool[,] map;
}

public struct MeshData
{
    public List<Vector3> verticies;
    public List<int> triangles;
}

public class ThreadedContourMesh
{
    private MethodQueue methodQueue;
    private Action<MeshData[]> MeshReciever;
    private bool[,] map;
    private MeshData[] meshCells;
    private MapCells[] cells;
    private float squareSize;
    private int cellWidth, cellHeight, widthDelta, heightDelta, completionCount = 0, divX = 1, divZ = 1, threadCount;
    private bool drawContours, drawSimple, uniqueVertcies;

    public ThreadedContourMesh(Action<MeshData[]> _MeshReciever, int _threadCount, bool[,] _map, float _squareSize, bool _drawContours = true, bool _drawSimple = true, bool _uniqueVerticies = true)
    {
        MeshReciever = _MeshReciever;
        threadCount = _threadCount;
        map = _map;
        squareSize = _squareSize;

        GetCellDiv();

        drawContours = _drawContours;
        drawSimple = _drawSimple;
        uniqueVertcies = _uniqueVerticies;

        methodQueue = new MethodQueue();
        meshCells = new MeshData[threadCount];
        cells = new MapCells[threadCount];

        cellWidth = map.GetLength(0) / divX;
        cellHeight = map.GetLength(1) / divZ;

        Debug.Log("Map Dimensions: " + map.GetLength(0) + ", " + map.GetLength(1));
        Debug.Log("Threads: " + threadCount);
    }

    private void GetCellDiv()
    {
        if (threadCount % 2 == 1)
        {
            if (threadCount == 1)
            {
                divX = 1;
                divZ = 1;
            }
            else if (threadCount == 3)
            {
                divX = 3;
                divZ = 1;
            }
        }
        else
        {
            if (threadCount == 2)
            {
                divX = 2;
                divZ = 1;
            }
            else if (threadCount == 4)
            {
                divX = 2;
                divZ = 2;
            }
            else if (threadCount == 6)
            {
                divX = 3;
                divZ = 2;
            }
            else if (threadCount >= 8)
            {
                divX = 4;
                divZ = 2;
            }
        }
    }

    public void StartThreads()
    {
        int cellCount = 0, cellX, cellZ;

        for (int a = 0; a < divX; a++)
        {
            for (int b = 0; b < divZ; b++)
            {
                meshCells[cellCount] = new MeshData();
                cells[cellCount] = new MapCells();
                cells[cellCount].map = new bool[cellWidth, cellHeight];

                cellX = (a * cellWidth);
                cellZ = (b * cellHeight);
                
                cells[cellCount].xOffset = cellX / 2;
                cells[cellCount].zOffset = cellZ / 2;

                for (int c = cellX; c < cellWidth + cellX; c++)
                {
                    for (int d = cellZ; d < cellHeight + cellZ; d++)
                    {
                        cells[cellCount].map[c - cellX, d - cellZ] = map[c, d];
                    }
                }

                cellCount++;
            }
        }

        for (int n = 0; n < threadCount; n++)
        {
            int x = n;
            var thread = new Thread(() => GetMesh(cells[x], ref meshCells[x]));
            thread.Start();
        }
    }

    private void GetMesh(MapCells cell, ref MeshData cellMesh)
    {
        ContourMesh contourMesh = new ContourMesh(cell.map, squareSize, drawContours, drawSimple, uniqueVertcies, cell.xOffset, cell.zOffset);
        cellMesh = contourMesh.GetMeshData();

        methodQueue.Enqueue(() => MeshDataCounter());
    }

    public void MeshQueue()
    {
        if (completionCount < threadCount)
        {
            if (methodQueue.count > 0)
            {
                Action update = methodQueue.Pop();
                update();
            }
        }
    }

    private void MeshDataCounter()
    {
        completionCount++;

        if (completionCount == threadCount)
        {            
            SendMeshData();
        }
    }

    private void SendMeshData()
    {
        Debug.Log("Sending Mesh Data.");
        MeshReciever(meshCells);
    }
}

public class MethodQueue
{
    private Queue actions;
    public int count;

    public MethodQueue()
    {
        actions = new Queue();
        count = 0;
    }

    public void Enqueue(Action method)
    {
        lock (actions)
        {
            count++;
            actions.Enqueue(method);
        }
    }

    public Action Pop()
    {
        lock (actions)
        {
            count--;
            return actions.Dequeue() as Action;
        }
    }
}