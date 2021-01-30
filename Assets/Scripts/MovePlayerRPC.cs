using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using UnityEngine;

public class MovePlayerRPC : GuyBehavior
{
    public bool amIServer;
    public override void MoveGuy(RpcArgs args)
    {
        transform.position += Vector3.up;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(amIServer == networkObject.IsServer){
            if(Input.GetKeyDown(KeyCode.Space)){
                networkObject.SendRpc(RPC_MOVE_GUY, Receivers.AllBuffered);
            }
        }
    }
}
