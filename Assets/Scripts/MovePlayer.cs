using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;

public class MovePlayer : MovePlayerBehavior
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!networkObject.IsServer){
            transform.position = networkObject.position;
            return;
        }

        // transform.position += new Vector3(
        //     Input.GetAxis("Horizontal"),
        //     Input.GetAxis("Vertical"),
        //     0
        // ) * Time.deltaTime * 5.0f;

        networkObject.position = transform.position;
    }
}
