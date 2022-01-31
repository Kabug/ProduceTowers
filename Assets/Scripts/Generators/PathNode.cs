using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int gridx;
    public int gridz;

    public PathNode parent;
    public Vector3 position;
    public GameObject obj;

    public NodeTypes nodeType;

    public int gCost;
    public int hCost;
    public int tCost; // Terrain Cost, higher value means more likely to avoid and lower value means more likey to go to
    public int fCost { get { return gCost + hCost + tCost; } }

    public PathNode(Vector3 position, int gridx, int gridz, int tCost, GameObject obj, NodeTypes nodeType)
    {
        this.gridx = gridx;
        this.gridz = gridz;
        this.position = position;
        this.tCost = tCost;
        this.obj = obj;
        this.nodeType = nodeType;
    }
}
