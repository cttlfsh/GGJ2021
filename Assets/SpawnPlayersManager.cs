using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

using ServerStatusCheck;

public class SpawnPlayersManager : PlayerSpawnBehavior
{
    public Vector3 myManPos = new Vector3(-1000, 0, 0);
    public Vector3 myGalPos;

    public GameObject myMan;
    public GameObject myGal;
    private bool playerSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(networkObject.IsServer) {
            if (myManPos.x != -1000) {
                networkObject.SendRpc(RPC_SEND_INITIAL_POSITIONS, Receivers.AllBuffered, myManPos, myGalPos);
            }
        }
    }

    public override void SendInitialPositions(RpcArgs args){
            if (!networkObject.IsServer && !playerSpawned){
                Vector3 myManInitialPosition = args.GetNext<Vector3>();
                Vector3 myGalInitialPosition = args.GetNext<Vector3>();
                Debug.Log(myGalInitialPosition);
                Debug.Log(myManInitialPosition);   
                myMan.transform.position = myManInitialPosition;
                myGal.transform.position = myGalInitialPosition;
                myGalPos = myGalInitialPosition;
                myManPos = myManInitialPosition;         
                playerSpawned = true;
            }
        }
}
