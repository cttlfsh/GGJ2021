using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;


public class SmartphoneController : MonoBehaviour
{
  private bool isPicAcquired = false;
  private static Texture2D pic;
  public Texture2D[] privateGallery  = new Texture2D[3];
  public Texture2D[] receivedGallery = new Texture2D[3];
  private int privateIndex;
  private int receivedIndex; 
  public GameObject theOtherSmartphone;
  private int batteryLevel;

  void Start()
  {
    batteryLevel  = 100;
    privateIndex  = 0;
    receivedIndex = 0;
  }

  // use ScreenCapture CaptureScreenshotAsTexture or CaptureScreenshotAsRenderTexture
  public IEnumerator takePic()
  {
    Debug.Log("takePic");
    Debug.Log(privateIndex);
    if (privateIndex < 3)
    {
      yield return new WaitForEndOfFrame();
      Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
      privateGallery[privateIndex] = texture;
      privateIndex++;
      // batteryLevel -= 100/3;
      sendPic();
    }
  }

  void sendPic()
  {
    Debug.Log("Prima if");
    if (theOtherSmartphone != null)
    {
      Debug.Log("Dentro if");
      NativeArray<byte> toSend = privateGallery[privateIndex-1].GetRawTextureData<byte>();
      byte[] bytes = new byte[toSend.Length];
      toSend.CopyTo(bytes);
      //theOtherSmartphone.GetComponent<SmartphoneController>().receivePic(bytes);
      GameObject.Find("Items").GetComponent<ObjectNetworkStatus>().UpdatePhoneStatus(bytes,theOtherSmartphone.name);
      // networkObject.SendRpc(RPC_UPDATEPHONE, Receivers.AllBuffered);
    }
  }
    // public override void prova(RpcArgs args){
      
    // }
    // public override void UpdatePhone(RpcArgs args)
    // {

    // }

  public void receivePic(byte[] pic)
  {
    if (receivedIndex < 3)
    {
      Texture2D convertedPic = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
      convertedPic.LoadRawTextureData(pic);
      convertedPic.Apply();
      receivedGallery[receivedIndex] = convertedPic;
      receivedIndex++;
    }
  }
  
  public bool hasPics()
  {
    // return receivedIndex > 0;
    return receivedIndex > 0;
  }

  public Texture2D showPic()
  {
    Debug.Log("Showing pic!");
    // Debug.Log(privateGallery[privateIndex-1]);
    return receivedGallery[receivedIndex-1];
    
  }

  void Update()
  {

  }
  
  
}
