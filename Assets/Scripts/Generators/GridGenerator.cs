using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject blockPrefab;

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
        seed = UnityEngine.Random.Range(0f, 0.5f);
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = (float)Math.Round(Mathf.PerlinNoise(x * (0.3f + seed), z * (0.3f + seed)) * 2.5f, MidpointRounding.AwayFromZero) / 2;
                //Debug.Log(new Vector3(x, y, z));
                Instantiate(blockPrefab, new Vector3(x, y, z), Quaternion.identity);
                //yield return new WaitForSeconds(0.0001f);
            }
        }
        yield return new WaitForSeconds(0.0001f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
