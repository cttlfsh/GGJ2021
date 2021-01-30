﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGenerator : MonoBehaviour {
    
    public int rows;
    public int columns;

    int waterThickness = 5;

    public GameObject[] terrains;
    public GameObject[] waters;

    private float nextXPosition = 0.0f;
    private float nextZPosition = 0.0f;

    void Start() {

        int finalX = rows + waterThickness * 2;
        int finalZ = columns + waterThickness * 2;

        for (int i = 0; i < finalX; i++) {
            for (int j = 0; j < finalZ; j++) {

                GameObject currentTerrain;

                if(i < waterThickness || j < waterThickness || i > finalX - waterThickness || j > finalZ - waterThickness) {
                    int w = Random.Range(0, waters.Length);
                    currentTerrain = waters[w];
                    Instantiate(currentTerrain, new Vector3(i*nextXPosition, 0, j*nextZPosition), Quaternion.identity);
                } else {
                    int r = Random.Range(0, terrains.Length);
                    currentTerrain = terrains[r];
                    Debug.Log(currentTerrain.GetComponent<Renderer>().bounds.size.x);
                    Instantiate(currentTerrain, new Vector3(i*nextXPosition, 3.5f, j*nextZPosition), Quaternion.Euler(0, Random.Range(0,4) * 90.0f, 0));
                }
                nextXPosition = currentTerrain.GetComponent<Renderer>().bounds.size.x;
                nextZPosition = currentTerrain.GetComponent<Renderer>().bounds.size.z;
            }
        }
        
    }
}