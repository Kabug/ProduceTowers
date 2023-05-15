using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject waterPrefab;
    public GameObject rockPrefab;
    public GameObject treePrefab;

    public Texture pathTexture;
    public Texture bridgeTexture;
    public Texture rockRoadTexture;
    public Texture startTexture;
    public Texture endTexture;

    public int xSize = 20;
    public int zSize = 20;

    private float seed;
    public int C = 10;
    public float scale = 0.5f;

    public PathNode[,] grid;
    public List<PathNode> FinalPath;

    // Should make this an enum state
    public bool calculatePath = true;
    public bool gridGenerating = false;
    public bool pathGenerating = false;
    public bool deleting = false;

    public int groundCost = 1;
    public int rockCost = 5;
    public int waterCost = 10; //DON'T YOU EVER STEP ON WATER REEE

    public Vector2Int startCoords;
    public Vector2Int endCoords;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateGrid());
    }

    public IEnumerator CreateGrid()
    {
        gridGenerating = true;
        grid = new PathNode[zSize, xSize];
        //seed = UnityEngine.Random.Range(0f, 0.2f);
        // We can eventually convert this into converting text into a number between 0 and 1 for easier to remember and reproducible seeds
        seed = UnityEngine.Random.Range(0f, 1f);
        List<Vector2Int> possibleStartPos = new List<Vector2Int>();
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                float y = Mathf.PerlinNoise((x + seed * C) * scale, (z * +seed * C) * scale);
                int middlecost = (int)(Mathf.Pow(2, 1 / Mathf.Clamp(Mathf.Abs((z) - (x)), 1, 100)) + UnityEngine.Random.Range(1, 5));
                if (y >= 0.65f)
                {
                    GenerateNode(x, z, 1f, middlecost);
                }
                else if (y <= 0.2f)
                {
                    GenerateNode(x, z, 0f, middlecost);
                }
                else
                {
                    GenerateNode(x, z, 0.5f, middlecost);
                }

                if (x == 0 || z == 0 || x == xSize - 1 || z == zSize - 1)
                {
                    possibleStartPos.Add(new Vector2Int(x, z));
                }
            }
            if (z % 2 == 0)
            {
                yield return new WaitForSeconds(0.0001f);
            }
        }

        int randomIndex = UnityEngine.Random.Range(0, possibleStartPos.Count - 1);
        startCoords = possibleStartPos[randomIndex];

        float endDistance = (xSize + zSize) * 0.6f;
        List<Vector2Int> possibleEndPos = new List<Vector2Int>();

        for (int i = 0; i < possibleStartPos.Count; i++)
        {
            if (GetManhattenDistance(startCoords, possibleStartPos[i]) >= endDistance)
            {
                possibleEndPos.Add(possibleStartPos[i]);
            }
        }

        randomIndex = UnityEngine.Random.Range(0, possibleEndPos.Count - 1);
        endCoords = possibleEndPos[randomIndex];

        grid[startCoords.x, startCoords.y].obj.tag = "Start";
        grid[endCoords.x, endCoords.y].obj.tag = "End";
        gridGenerating = false;
        //yield return new WaitForSeconds(0.0001f);
    }

    private void GenerateNode(int x, int z, float y, int middlecost)
    {
        GameObject prefab;
        int cost;
        NodeTypes type;

        if (y == 0)
        {
            prefab = waterPrefab;
            cost = waterCost;
            type = NodeTypes.WATER;
        }
        else if (y == 1)
        {
            prefab = rockPrefab;
            cost = rockCost;
            type = NodeTypes.ROCK;
        }
        else
        {
            prefab = blockPrefab;
            cost = groundCost;
            type = NodeTypes.GROUND;
        }

        var nodeObject = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity);
        nodeObject.transform.parent = transform;
        grid[z, x] = new PathNode(new Vector3(x, y, z), x, z, cost + middlecost, nodeObject, type);
    }


    int GetManhattenDistance(Vector2Int StartPos, Vector2Int EndPos)
    {
        int ix = Mathf.Abs(StartPos.x - EndPos.x);
        int iz = Mathf.Abs(StartPos.y - EndPos.y);

        return ix + iz;
    }

    public List<PathNode> GetNeighboringNodes(PathNode CurrentNode)
    {
        List<PathNode> NeighboringNodes = new List<PathNode>();

        int xCheck;
        int zCheck;

        //Right Side
        xCheck = CurrentNode.gridx + 1;
        zCheck = CurrentNode.gridz;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck, xCheck]);
            }
        }

        //Left Side
        xCheck = CurrentNode.gridx - 1;
        zCheck = CurrentNode.gridz;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck, xCheck]);
            }
        }

        //Top Side
        xCheck = CurrentNode.gridx;
        zCheck = CurrentNode.gridz + 1;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck, xCheck]);
            }
        }

        //Bottom Side
        xCheck = CurrentNode.gridx;
        zCheck = CurrentNode.gridz - 1;
        if (xCheck >= 0 && xCheck < xSize)
        {
            if (zCheck >= 0 && zCheck < zSize)
            {
                NeighboringNodes.Add(grid[zCheck, xCheck]);
            }
        }

        return NeighboringNodes;
    }

    public IEnumerator HighlightPath()
    {
        pathGenerating = true;

        grid[startCoords.x, startCoords.y].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", startTexture);
        grid[endCoords.x, endCoords.y].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", endTexture);
        for (int i = 1; i < FinalPath.Count-1; i++)
        {
            // For outline Color/Texture etc you'll need to change these variable names in the SimpleOutlines pbr graph thingy properties settings
            // https://i.imgur.com/SPO1IiJ.png

            PathNode node = FinalPath[i];

            if (grid[node.gridz, node.gridx].obj.transform.position.y == 0)
            {
                grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", bridgeTexture);
                node.nodeType = NodeTypes.PATH_BRIDGE;
            }
            else if (grid[node.gridz, node.gridx].obj.transform.position.y == 1f)
            {
                grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", rockRoadTexture);
                grid[node.gridz, node.gridx].obj.transform.position = grid[node.gridz, node.gridx].obj.transform.position - new Vector3(0, 0.25f, 0);
                node.nodeType = NodeTypes.PATH_TUNNEL;
            }
            else
            {
                grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", pathTexture);
                grid[node.gridz, node.gridx].obj.transform.position = grid[node.gridz, node.gridx].obj.transform.position - new Vector3(0, 0.25f, 0);
                node.nodeType = NodeTypes.PATH;
                // In case outline colour needs to be changed
                //grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetColor("OutlineColor", new Color(242f / 255f, 166f / 255f, 94f / 255f, 1));
            }
            yield return new WaitForSeconds(0.0001f);
        }
        pathGenerating = false;
        StartCoroutine(GenerateTrees());
    }

    public bool IsValidTreeSpot(int z, int x)
    {
        return grid[z - 1, x + 1].nodeType == NodeTypes.GROUND
            && grid[z, x + 1].nodeType == NodeTypes.GROUND
            && grid[z + 1, x + 1].nodeType == NodeTypes.GROUND
            && grid[z - 1, x].nodeType == NodeTypes.GROUND
            && grid[z, x].nodeType == NodeTypes.GROUND
            && grid[z + 1, x].nodeType == NodeTypes.GROUND
            && grid[z - 1, x - 1].nodeType == NodeTypes.GROUND
            && grid[z, x - 1].nodeType == NodeTypes.GROUND
            && grid[z + 1, x - 1].nodeType == NodeTypes.GROUND;
    }
    public IEnumerator GenerateTrees()
    {
        if (xSize > 3 || zSize > 3)
        {
            for (int x = 1; x < xSize - 1; x++)
            {
                for (int z = 1; z < zSize - 1; z++)
                {
                    if (IsValidTreeSpot(z, x) && UnityEngine.Random.Range(0f, 1f) > 0.25f)
                    {
                        var treeObject = Instantiate(treePrefab, new Vector3(x, 1, z), Quaternion.identity);
                        treeObject.transform.parent = transform;
                        yield return new WaitForSeconds(0.0001f);
                    }
                }
            }
        }
    }

    public void DeleteGrid()
    {
        if (null != grid && !calculatePath && !gridGenerating && !pathGenerating && !deleting)
        {
            deleting = true;
            foreach (var node in grid)
            {
                Destroy(node.obj);
            }
            grid = null;
            deleting = false;
            calculatePath = true;
        }

        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    // Might want to add something here to prevent it from being called twice
    public void RegenerateGrid()
    {
        if (!gridGenerating && !pathGenerating && !deleting)
        {
            DeleteGrid();
            StartCoroutine(CreateGrid());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
