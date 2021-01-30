using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    public int[] sectionWidth;
    public int[] sectionHeight;
    public GameObject[] terrains;
    private float nextXPosition = 0.0f;
    private float nextYPosition = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < sectionWidth.Length; i++)
        {
            for (int j = 0; j < sectionHeight.Length; j++)
            {
                int r = Random.Range(0,terrains.Length);
                GameObject currentTerrain = terrains[r];
                Debug.Log(currentTerrain.GetComponent<Renderer>().bounds.size.x);
                Instantiate(currentTerrain, new Vector3(i*19.0f, 0, j*19.0f), Quaternion.identity);
                nextXPosition = currentTerrain.GetComponent<Renderer>().bounds.size.x;
                nextYPosition = currentTerrain.GetComponent<Renderer>().bounds.size.y;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}