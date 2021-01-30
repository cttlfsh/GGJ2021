using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class winCondition : MonoBehaviour
{
    
    public GameObject player1;
    public GameObject player2;

    private Transform position_p1, position_p2;

    void Start()
    {
        position_p1 = player1.transform;
        position_p2= player2.transform;
    }

    void Update()
    {
        float dist = Vector3.Distance(position_p1.position, position_p2.position);
        if (dist<2)
        {
            //do something about game end conditions
            print("Where the fuck have you been, MATE?");
        }
        
    }
}
