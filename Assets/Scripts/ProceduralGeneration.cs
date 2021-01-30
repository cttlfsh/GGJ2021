using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public int rows;
    public int cols;

    public GameObject[] terrains;
    public GameObject mold;

    private float nextXMoldPosition;
    private float nextZMoldPosition;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < rows; i++)
        {
            Debug.Log("NextX: " + nextXMoldPosition);
            Debug.Log("NextY: " + nextZMoldPosition);
            for (int j = 0; j < cols; j++)
            {   
                nextXMoldPosition = i*mold.GetComponent<Renderer>().bounds.size.x;
                nextZMoldPosition = j*mold.GetComponent<Renderer>().bounds.size.z;
                Debug.Log("ROW: " + i + " COL: " + j + " NextX: " + nextXMoldPosition);
                Debug.Log("ROW: " + i + " COL: " + j + "NextY: " + nextZMoldPosition);
                Debug.Log("------------------------------");
                int r = Random.Range(0, terrains.Length);
                GameObject currentTerrain = terrains[r];

                Debug.Log("MoldX: " + mold.GetComponent<Renderer>().bounds.size.x);
                Debug.Log("MoldY: " + mold.GetComponent<Renderer>().bounds.size.z);
                Instantiate(currentTerrain, new Vector3(nextXMoldPosition, 0, nextZMoldPosition), Quaternion.identity);
                
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}