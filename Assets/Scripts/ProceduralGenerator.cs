using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour {

    public int rows;
    public int columns;

    public GameObject[] terrains;
    public GameObject island;
    public GameObject beach;
    public GameObject mold;
    public GameObject parentObject;
    private float nextXMoldPosition;
    private float nextZMoldPosition;

    void Awake() {
        // Basta la X dato che la tile e' quadrata
        float tileSize = mold.GetComponent<Renderer>().bounds.size.x;

        float sizeX = tileSize * rows;
        float sizeZ = tileSize * columns;

        float centerX = (tileSize * (rows - 1)) / 2;
        float centerZ = (tileSize * (columns - 1)) / 2;

        // set island size
        island.SetActive(true);
        island.transform.position = new Vector3(centerX, 0, centerZ);
        float islandSize = island.GetComponent<Renderer>().bounds.size.x;
        float scaleFactor = (islandSize / tileSize)  * 12f;
        island.transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor);
        // set beach size
        beach.transform.position = new Vector3(centerX, -1.8f, centerZ);
        beach.transform.Rotate(0, 60, 0); 
        beach.transform.localScale = new Vector3(scaleFactor / 1.5f, 1, scaleFactor / 1.5f);

        // spawn tiles
        int seed = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {   
                nextXMoldPosition = i*mold.GetComponent<Renderer>().bounds.size.x;
                nextZMoldPosition = j*mold.GetComponent<Renderer>().bounds.size.z;

                // int r = Random.Range(0, terrains.Length);
                GameObject currentTerrain = terrains[seed];

                seed++;

                var singleTile = Instantiate(currentTerrain, new Vector3(nextXMoldPosition, 0, nextZMoldPosition), Quaternion.identity);
                singleTile.transform.parent = parentObject.transform;
            }
        }
    }
}
