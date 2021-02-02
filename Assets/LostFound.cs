using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LostFound : MonoBehaviour
{
    public float timeLeft = 300.0f;
    public GameObject player1;
    public GameObject player2;

    private Transform position_p1, position_p2;

    void Start()
    {
        position_p1 = player1.transform;
        position_p2= player2.transform;
        timeLeft = Vector3.Distance (player1.transform.position, player2.transform.position)*10;
    }

    void Update()
    {   
        timeLeft -= Time.deltaTime;
        if ( timeLeft < 0 )
        {
            SceneManager.LoadScene(3);
        }
        float dist = Vector3.Distance(position_p1.position, position_p2.position);
        if (dist<2)
        {
            //do something about game end conditions
            Debug.Log("Where the fuck have you been, MATE?");
            SceneManager.LoadScene(4);
        }
        
    }
}