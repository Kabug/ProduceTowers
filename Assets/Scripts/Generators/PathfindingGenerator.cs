using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGenerator : MonoBehaviour
{

    public static PathfindingGenerator Instance { get; private set; }

    private void Awake()
    {
        // Ensure there is only one instance of PathfindingGenrator
        if (Instance == null)
        {
            // Set the static instance property to this instance
            Instance = this;
        }
        else
        {
            // If an instance already exists, destroy this instance
            Destroy(gameObject);
            return;
        }

        // Ensure the singleton persists across scenes
        DontDestroyOnLoad(gameObject);
    }

    public List<PathNode> FindPath(List<List<PathNode>> grid, PathNode startNode, PathNode endNode)
    {
        List<PathNode> OpenList = new List<PathNode>();
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();
        List<PathNode> FinalPath = new List<PathNode>();

        OpenList.Add(startNode);

        while (OpenList.Count > 0)
        {
            PathNode CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].fCost < CurrentNode.fCost || OpenList[i].fCost == CurrentNode.fCost && OpenList[i].hCost < CurrentNode.hCost)
                {
                    CurrentNode = OpenList[i];
                }
            }

            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode == endNode)
            {
                FinalPath = GetFinalPath(startNode, endNode);
            }

            foreach (PathNode NeighborNode in GetNeighboringNodes(CurrentNode, grid))
            {
                if (ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
                //grid.HighlightSpot(NeighborNode);
                int MoveCost = CurrentNode.gCost + GetManhattenDistance(CurrentNode, NeighborNode) + CurrentNode.tCost;

                if (MoveCost < NeighborNode.gCost || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.gCost = MoveCost;
                    NeighborNode.hCost = GetManhattenDistance(NeighborNode, endNode);
                    NeighborNode.parent = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
                //grid.UnhighlightSpot(NeighborNode);
            }
        }

        return FinalPath;
    }

    public List<PathNode> GetNeighboringNodes(PathNode CurrentNode, List<List<PathNode>> grid)
    {
        List<PathNode> NeighboringNodes = new List<PathNode>();

        int xCheck;
        int zCheck;

        // Get the z size (number of rows)
        int zSize = grid.Count;

        // Get the x size (number of columns)
        int xSize = grid.Count > 0 ? grid[0].Count : 0;

        //Right Side
        xCheck = CurrentNode.gridx + 1;
        zCheck = CurrentNode.gridz;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck][xCheck]);
            }
        }

        //Left Side
        xCheck = CurrentNode.gridx - 1;
        zCheck = CurrentNode.gridz;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck][xCheck]);
            }
        }

        //Top Side
        xCheck = CurrentNode.gridx;
        zCheck = CurrentNode.gridz + 1;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck][xCheck]);
            }
        }

        //Bottom Side
        xCheck = CurrentNode.gridx;
        zCheck = CurrentNode.gridz - 1;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck][xCheck]);
            }
        }

        return NeighboringNodes;
    }

    List<PathNode> GetFinalPath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> FinalPath = new List<PathNode>();
        PathNode CurrentNode = endNode;

        while (CurrentNode != startNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.parent;
        }
        FinalPath.Add(startNode);
        FinalPath.Reverse();
        return FinalPath;
        
    }

    int GetManhattenDistance(PathNode CurrentNode, PathNode NeighborNode)
    {
        int ix = Mathf.Abs(CurrentNode.gridx - NeighborNode.gridx);
        int iz = Mathf.Abs(CurrentNode.gridz - NeighborNode.gridz);

        return ix + iz;
    }
}
