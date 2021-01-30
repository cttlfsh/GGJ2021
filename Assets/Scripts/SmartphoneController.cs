using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class SmartphoneController : MonoBehaviour
{
  private bool isPicAcquired = false;
  private static Texture2D pic;
  private Texture2D[] privateGallery  = new Texture2D[3];
  private Texture2D[] receivedGallery = new Texture2D[3];
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
    if (theOtherSmartphone != null)
    {
      NativeArray<byte> toSend = privateGallery[privateIndex-1].GetRawTextureData<byte>();
      byte[] bytes = new byte[toSend.Length];
      toSend.CopyTo(bytes);
      theOtherSmartphone.GetComponent<SmartphoneController>().receivePic(bytes);
    }
  }

  public void receivePic(byte[] pic)
  {
    if (receivedIndex < 3)
    {
      Texture2D convertedPic = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
      convertedPic.LoadRawTextureData(pic);
      receivedGallery[receivedIndex] = convertedPic;
      receivedIndex++;
    }
  }

  // Texture2D showPic(int i, bool private)
  // {
  //   if ((i >= 0) and (i < 3))
  //   {
  //     if (private and i <= privateIndex)
  //     {
  //       return privateGallery[i];
  //     }
  //     else if (!private and i <= receivedIndex)
  //     {
  //       return receivedGallery[i];
  //     }
  //   }
  // }
  void Update()
  {

  }
  
  
}
