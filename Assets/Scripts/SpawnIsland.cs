using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnIsland : MonoBehaviour {

    public int rows;
    public int columns;

    public GameObject[] terrains;
    public GameObject island;

    private float nextXPosition = 0.0f;
    private float nextZPosition = 0.0f;

    void Awake() {

        float tileSize = terrains[0].GetComponent<Renderer>().bounds.size.x;

        float sizeX = tileSize * rows;
        float sizeZ = tileSize * columns;

        float centerX = (tileSize * (rows - 1)) / 2;
        float centerZ = (tileSize * (columns - 1)) / 2;

        island.SetActive(true);
        island.transform.position = new Vector3(centerX, 0, centerZ);

        // Instantiate(island, new Vector3(centerX, 0, centerZ), Quaternion.identity);
        float islandSize = island.GetComponent<Renderer>().bounds.size.x;

        float scaleFactor = (islandSize / tileSize)  * 4f;

        island.transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor);

        for(int i = 0; i < rows; i++) {
            for(int j = 0; j < columns; j++) {
                int r = Random.Range(0, terrains.Length);
                GameObject currentTerrain = terrains[r];
                Debug.Log(currentTerrain.GetComponent<Renderer>().bounds.size.x);
                Instantiate(currentTerrain, new Vector3(i*nextXPosition, 3f, j*nextZPosition), Quaternion.Euler(0, Random.Range(0,4) * 90.0f, 0));
                nextXPosition = currentTerrain.GetComponent<Renderer>().bounds.size.x;
                nextZPosition = currentTerrain.GetComponent<Renderer>().bounds.size.z;
            }
        }

        // maxClouds = Random.Range(500, 1000);
        // generateClouds();

    }
}
