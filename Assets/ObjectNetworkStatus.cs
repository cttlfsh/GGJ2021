using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;

public class ObjectNetworkStatus : ObjectNetworkStatusBehavior
{


    public override void updatePhone(RpcArgs args)
    {
        byte[] image = args.GetNext<byte[]>();
        string name = args.GetNext<string>();
        GameObject.Find(name).GetComponent<SmartphoneController>().receivePic(image);
        Debug.Log(name);
        
    }

    public void UpdatePhoneStatus(byte[] bytes, string phoneName){

        networkObject.SendRpc(RPC_UPDATE_PHONE, Receivers.AllBuffered, bytes, phoneName);
    }

}
