using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGenerator : MonoBehaviour
{

    GridGenerator grid;
    public PathNode startNode;
    public PathNode endNode;

    public Texture pathTexture;
    public Texture bridgeTexture;
    public Texture rockRoadTexture;
    public Texture startTexture;
    public Texture endTexture;

    void Awake()
    {
        grid = GetComponent<GridGenerator>();
    }

    void Update()
    {

    }

    public void FindPath()
    {
        grid.mapState = MapState.CREATING_PATH;
        startNode = grid.grid[grid.startCoords.x, grid.startCoords.y];
        endNode = grid.grid[grid.endCoords.x, grid.endCoords.y];
        List<PathNode> OpenList = new List<PathNode>();
        HashSet<PathNode> ClosedList = new HashSet<PathNode>();

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
                GetFinalPath(startNode, endNode);
            }

            foreach (PathNode NeighborNode in grid.GetNeighboringNodes(CurrentNode))
            {
                if (ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
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
        StartCoroutine(HighlightPath(FinalPath));
    }

    public IEnumerator HighlightPath(List<PathNode> FinalPath)
    {
        grid.grid[grid.startCoords.x, grid.startCoords.y].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", startTexture);
        grid.grid[grid.endCoords.x, grid.endCoords.y].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", endTexture);
        for (int i = 1; i < FinalPath.Count - 1; i++)
        {
            // For outline Color/Texture etc you'll need to change these variable names in the SimpleOutlines pbr graph thingy properties settings
            // https://i.imgur.com/SPO1IiJ.png

            PathNode node = FinalPath[i];

            if (grid.grid[node.gridz, node.gridx].obj.transform.position.y == 0)
            {
                grid.grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", bridgeTexture);
                node.nodeType = NodeTypes.PATH_BRIDGE;
            }
            else if (grid.grid[node.gridz, node.gridx].obj.transform.position.y == 1f)
            {
                grid.grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", rockRoadTexture);
                grid.grid[node.gridz, node.gridx].obj.transform.position = grid.grid[node.gridz, node.gridx].obj.transform.position - new Vector3(0, 0.25f, 0);
                node.nodeType = NodeTypes.PATH_TUNNEL;
            }
            else
            {
                grid.grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", pathTexture);
                grid.grid[node.gridz, node.gridx].obj.transform.position = grid.grid[node.gridz, node.gridx].obj.transform.position - new Vector3(0, 0.25f, 0);
                node.nodeType = NodeTypes.PATH;
                // In case outline colour needs to be changed
                //grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetColor("OutlineColor", new Color(242f / 255f, 166f / 255f, 94f / 255f, 1));
            }
            yield return new WaitForSeconds(0.0001f);
        }
    }

    int GetManhattenDistance(PathNode CurrentNode, PathNode NeighborNode)
    {
        int ix = Mathf.Abs(CurrentNode.gridx - NeighborNode.gridx);
        int iz = Mathf.Abs(CurrentNode.gridz - NeighborNode.gridz);

        return ix + iz;
    }
}
