using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour {

    public GameObject[] clouds;
    public GameObject parentObject;

    private GameObject island;
    private int maxClouds;
    
    // Start is called before the first frame update
    void Start() {
        island = GameObject.FindWithTag("Island");

        maxClouds = Random.Range(500, 1000);
        generateClouds();
    }

    private void generateClouds() {

        float halfWorldDimension = (island.GetComponent<Renderer>().bounds.size.x) / 2;

        for(int i = 0; i < maxClouds; i++) {
            float x = Random.Range(-halfWorldDimension, halfWorldDimension);
            float y = Random.Range(20, 40);
            float z = Random.Range(-halfWorldDimension, halfWorldDimension);

            int cloudType = Random.Range(0, clouds.Length);

            GameObject cloud = Instantiate(clouds[cloudType], new Vector3(x, y, z), Quaternion.identity);
            int scaleFactor = Random.Range(0,2);
            cloud.transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor);
            cloud.transform.parent = parentObject.transform;
        }
    }

}
