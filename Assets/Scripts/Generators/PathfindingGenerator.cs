using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGenerator : MonoBehaviour
{

    GridGenerator grid;
    public Transform start;
    public Transform end;
    
    void Awake()
    {
        grid = GetComponent<GridGenerator>();
    }

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Start") && GameObject.FindGameObjectWithTag("End") && grid.calculatePath && !grid.gridGenerating && !grid.pathGenerating)
        {
            start = GameObject.FindGameObjectWithTag("Start").transform;
            end = GameObject.FindGameObjectWithTag("End").transform;
            FindPath(start.position, end.position);
            grid.calculatePath = false;
        }
    }

    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        PathNode StartNode = grid.grid[0, 0];
        PathNode EndNode = grid.grid[grid.zSize - 1, grid.xSize - 1];

        List<PathNode> OpenList = new List<PathNode>();
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

        OpenList.Add(StartNode);

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

            if (CurrentNode == EndNode)
            {
                GetFinalPath(StartNode, EndNode);
            }

            foreach (PathNode NeighborNode in grid.GetNeighboringNodes(CurrentNode))
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
                    NeighborNode.hCost = GetManhattenDistance(NeighborNode, EndNode);
                    NeighborNode.parent = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
                //grid.UnhighlightSpot(NeighborNode);
            }
        }
    }

    void GetFinalPath(PathNode startNode, PathNode endNode)
    {
        List<PathNode> FinalPath = new List<PathNode>();
        PathNode CurrentNode = endNode;

        while(CurrentNode != startNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.parent;
        }
        FinalPath.Add(startNode);
        FinalPath.Reverse();
        grid.FinalPath = FinalPath;
        StartCoroutine(grid.HighlightPath());
    }

    int GetManhattenDistance(PathNode CurrentNode, PathNode NeighborNode)
    {
        int ix = Mathf.Abs(CurrentNode.gridx - NeighborNode.gridx);
        int iz = Mathf.Abs(CurrentNode.gridz - NeighborNode.gridz);

        return ix + iz;
    }
}
