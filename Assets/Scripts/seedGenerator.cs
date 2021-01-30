using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seedGenerator : MonoBehaviour
{

    public int x = 3;
    public int z = 3;
    public GameObject sandTile;
    public GameObject waterTile;

    int waterThickness = 5;

    // Start is called before the first frame update
    void Start() {

        int finalX = x + waterThickness * 2;
        int finalZ = z + waterThickness * 2;

        for(int i = 0; i < finalX; i++) {
            for(int j = 0; j < finalZ; j++) {
                if(i < waterThickness || j < waterThickness || i > finalX - waterThickness || j > finalZ - waterThickness) {
                    Instantiate(waterTile, new Vector3(i * 30.0F, 0, j * 30.0F), Quaternion.identity);
                } else {
                    Instantiate(sandTile, new Vector3(i * 30.0F, 0, j * 30.0F), Quaternion.identity); 
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
