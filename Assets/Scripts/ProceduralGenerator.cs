using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ServerStatusCheck;

public class ProceduralGenerator : MonoBehaviour {

    public int rows;
    public int columns;
    public GameObject[] terrains;
    public GameObject island;
    public GameObject beach;
    public GameObject mold;
    public GameObject parentObject;
    public GameObject myMan;
    public GameObject myGal;

    private float nextXMoldPosition;
    private float nextZMoldPosition;
    private List<GameObject> instantiatedTiles = new List<GameObject>();

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


                GameObject ithTile = Instantiate(currentTerrain, new Vector3(nextXMoldPosition, 0, nextZMoldPosition), Quaternion.identity);
                if (ithTile.tag != "Dungeon") {
                    instantiatedTiles.Add(ithTile);
                }
                seed++;
                

                var singleTile = Instantiate(currentTerrain, new Vector3(nextXMoldPosition, 0, nextZMoldPosition), Quaternion.identity);
                singleTile.transform.parent = parentObject.transform;
            }
        }
        // FINISHED GENERATING MAP
        GameObject serverStatus = GameObject.Find("ServerStatus");
        Debug.Log("SERVER STATUS OBJECT: " + serverStatus + ". IS_SERVER: " + serverStatus.GetComponent<ServerCheck>().isServer);
        myMan.SetActive(true);
        myGal.SetActive(true);
        int r = Random.Range(0, terrains.Length);
        GameObject spawnTerrain = instantiatedTiles[r];
        Debug.Log(spawnTerrain);
        if (serverStatus.GetComponent<ServerCheck>().isServer){
            // Spawning MyMan
            string[] myManSpawnPoints = {"SpawnMyMan1", "SpawnMyMan2"}; 
            myMan.transform.position = spawnTerrain.transform.position + spawnTerrain.transform.Find(myManSpawnPoints[Random.Range(0, 2)]).localPosition;
            // Spawning MyGal
            string[] myGalSpawnPoints = {"SpawnMyGal1", "SpawnMyGal2"}; 
            myGal.transform.position = spawnTerrain.transform.position + spawnTerrain.transform.Find(myGalSpawnPoints[Random.Range(0, 2)]).localPosition;
            GetComponent<SpawnPlayersManager>().myManPos = myMan.transform.position;
            GetComponent<SpawnPlayersManager>().myGalPos = myGal.transform.position;
        }

    }

    void Start()
    {
    }

    private void SpawnPlayer(GameObject player, GameObject terrain, string[] spawnPointNames){
        player.transform.position = terrain.transform.position + terrain.transform.Find(spawnPointNames[Random.Range(0, 2)]).localPosition;
        
    }
}
