using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject waterPrefab;
    public GameObject rockPrefab;

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
    //private List<List<GameObject>> mapObjectList = new List<List<GameObject>>();


    public PathNode[,] grid;
    public List<PathNode> FinalPath;

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
        //Instantiate(blockPrefab, new Vector3(1, 1, 1), Quaternion.identity);
        //seed = UnityEngine.Random.Range(0f, 0.2f);
        seed = UnityEngine.Random.Range(0f, 1f);
        for (int z = 0; z < zSize; z++)
        {
            //mapObjectList.Add(new List<GameObject>());
            for (int x = 0; x < xSize; x++)
            {
                //float y = (float)Math.Round(Mathf.PerlinNoise(x * (0.25f + seed), z * (0.25f + seed)) * 2.5f, MidpointRounding.AwayFromZero) / 2;
                float y = Mathf.PerlinNoise((x + seed * C) * scale, (z * + seed * C) * scale);
                if (y >= 0.65f)
                {
                    y = 1;
                }
                else if (y <= 0.2f)
                {
                    y = 0;
                }
                else
                {
                    y = 0.5f;
                }
                //Debug.Log(new Vector3(x, y, z));
                // Makes it more costly to travel through the center of the map
                //int middlecost = (int)(Mathf.Pow(2, 2 / Mathf.Clamp(Mathf.Abs((z) - (x)), 1, 100)) + UnityEngine.Random.Range(1, 5));
                int middlecost = (int)(Mathf.Pow(2, 1 / Mathf.Clamp(Mathf.Abs((z) - (x)), 1, 100)) + UnityEngine.Random.Range(1, 5));
                //int middlecost = 0;
                if (0.5 == y)
                {
                    var groundObject = Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
                    //mapObjectList[z].Add(groundObject);
                    groundObject.transform.parent = GameObject.Find("Grid Generator").transform;
                    grid[z, x] = new PathNode(new Vector3(x, y, z), x, z, groundCost + middlecost, groundObject);
                }
                else if (1 == y)
                {
                    var rockObject = Instantiate(rockPrefab, new Vector3(x, y, z), Quaternion.identity);
                    //mapObjectList[z].Add(rockObject);
                    rockObject.transform.parent = GameObject.Find("Grid Generator").transform;
                    grid[z, x] = new PathNode(new Vector3(x, y, z), x, z, rockCost + middlecost, rockObject);
                }
                else if (0 == y)
                {
                    var waterObject = Instantiate(waterPrefab, new Vector3(x, y, z), Quaternion.identity);
                    //mapObjectList[z].Add(waterObject);
                    waterObject.transform.parent = GameObject.Find("Grid Generator").transform;
                    grid[z, x] = new PathNode(new Vector3(x, y, z), x, z, waterCost + middlecost, waterObject);
                }
            }
            if (z % 2 == 0)
            {
                yield return new WaitForSeconds(0.0001f);
            }
        }
        startCoords = new Vector2Int(0,0);
        endCoords = new Vector2Int(zSize - 1, xSize - 1);

        grid[startCoords.x, startCoords.y].obj.tag = "Start";
        grid[endCoords.x, endCoords.y].obj.tag = "End";
        gridGenerating = false;
        //yield return new WaitForSeconds(0.0001f);
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
            }
            else if (grid[node.gridz, node.gridx].obj.transform.position.y == 1f)
            {
                grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", rockRoadTexture);
                grid[node.gridz, node.gridx].obj.transform.position = grid[node.gridz, node.gridx].obj.transform.position - new Vector3(0, 0.25f, 0);
            }
            else
            {
                grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetTexture("DiffuseTexture", pathTexture);
                grid[node.gridz, node.gridx].obj.transform.position = grid[node.gridz, node.gridx].obj.transform.position - new Vector3(0, 0.25f, 0);
                //grid[node.gridz, node.gridx].obj.GetComponent<Renderer>().material.SetColor("OutlineColor", new Color(242f / 255f, 166f / 255f, 94f / 255f, 1));
            }
            yield return new WaitForSeconds(0.0001f);
        }
        pathGenerating = false;
    }

    //public void HighlightSpot(PathNode node)
    //{
    //    mapObjectList[node.gridz][node.gridx].transform.position = mapObjectList[node.gridz][node.gridx].transform.position + new Vector3(0, 1f, 0);
    //    //Destroy(mapObjectList[node.gridz][node.gridx]);
    //}

    //public void UnhighlightSpot(PathNode node)
    //{
    //    mapObjectList[node.gridz][node.gridz].transform.position = mapObjectList[node.gridz][node.gridz].transform.position - new Vector3(0, 0.25f, 0);
    //}

    public void DeleteGrid()
    {
        if (null != grid && !calculatePath && !gridGenerating && !pathGenerating && !deleting)
        {
            deleting = true;
            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    Destroy(grid[z, x].obj);
                }
            }

            grid = null;
            deleting = false;
            calculatePath = true;
        }

        while (GameObject.Find("Grid Generator").transform.childCount != 0)
        {
            DestroyImmediate(GameObject.Find("Grid Generator").transform.GetChild(0).gameObject);
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
