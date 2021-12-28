using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject waterPrefab;
    [SerializeField]
    private GameObject rockPrefab;

    public int xSize = 20;
    public int zSize = 20;

    private float seed;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CreateShape());
    }

    IEnumerator CreateShape()
    {
        //Instantiate(blockPrefab, new Vector3(1, 1, 1), Quaternion.identity);
        seed = UnityEngine.Random.Range(0f, 0.2f);
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = (float)Math.Round(Mathf.PerlinNoise(x * (0.25f + seed), z * (0.25f + seed)) * 2.5f, MidpointRounding.AwayFromZero) / 2;
                //Debug.Log(new Vector3(x, y, z));
                if (0.5 == y)
                {
                    var groundObject = Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
                    groundObject.transform.parent = GameObject.Find("Grid Generator").transform;
                }
                else if ( 1 == y)
                {
                    var rockObject = Instantiate(rockPrefab, new Vector3(x, y, z), Quaternion.identity);
                    rockObject.transform.parent = GameObject.Find("Grid Generator").transform;
                }
                else if (0 == y)
                {
                    var waterObject = Instantiate(waterPrefab, new Vector3(x, y, z), Quaternion.identity);
                    waterObject.transform.parent = GameObject.Find("Grid Generator").transform;
                }
            }
            yield return new WaitForSeconds(0.0001f);
        }

        //yield return new WaitForSeconds(0.0001f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
