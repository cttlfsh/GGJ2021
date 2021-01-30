using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
  private bool isPickedUpSmartphone;
  private bool isPickedUpCompass;
  private bool isPickedUpBeacon;
  private bool isPickedUpFlairGun;
  private bool isPickedUpMap;
  private bool isPickedUpWalkieTalkie;
  public GameObject smartphone;
  
  public Collider coll;

  // Start is called before the first frame update
  void Start()
  {
    coll = GetComponent<Collider>();
  }
  
  void pickUp()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (coll.Raycast(ray, out hit, 100.0f))
    {
      if (hit.collider.tag == "object")
      {
        switch (hit.collider.name)
        {
          case "smartphone":
            isPickedUpSmartphone = true;
            break;
          case "minimap":
            isPickedUpMap = true;
            useObject("map");
            break;
          case "flairGun":
            isPickedUpFlairGun = true;
            break;
          case "compass":
            isPickedUpCompass = true;
            useObject("compass");
            break;
          case "walkieTalkie":
            isPickedUpWalkieTalkie = true;
            break;
          case "beacon":
            isPickedUpBeacon = true;
            break;
          default:
          break;
        }

      }
    }

  }

  void takePic()
  {
    // use ScreenCapture CaptureScreenshotAsTexture or CaptureScreenshotAsRenderTexture
  }
  
  void enableCompass()
  {
    // activate compass
  }
  
  void useBeacon()
  {
    // produce a sound which become stronger the more you go in the right direction
    // angle and distance are important
  }

  void enableMap()
  {
    // bottom left corner mini map activate camera
  }

  void shootFlair()
  {
    // instantiate and force a bullet in air making a lot of red light
  }

  void listen()
  {
    // if the other walkie talkie is picked up, listen to the world sounds
  }

  void useObject(string object)
  {
    switch (object)
    {
      case "smartphone":
        if (isPickedUpSmartphone)
        {
          takePic();        
        }
        break;
      case "compass":
        if (isPickedUpCompass)
        {
          enableCompass();
        }

        break;
      case "beacon":
        if (isPickedUpBeacon)
        {
          useBeacon();
        }
        break;
      case "flairGun":
        if (isPickedUpFlairGun)
        {
          shootFlair();
        }
          
        break;
      case "map":
        if (isPickedUpMap)
        {
          enableMap();
        }

        break;
      case "walkieTalkie":
        if (isPickedUpWalkieTalkie)
        {
          listen();
        }
        break;
      default:
        break;
    }
  }

  void switchItem()
  {
    if (Input.GetKey(KeyCode.Alpha1)){
      useObject("smartphone");
    }
    if (Input.GetKey(KeyCode.Alpha2)){
      useObject("walkieTalkie");
    } 
    if (Input.GetKey(KeyCode.Alpha3)){
      useObject("flairGun");
    }
  }
  // Update is called once per frame
  void Update()
  {
    if (Input.GetKey(KeyCode.F)) 
    {
      pickUp();
    }
    
    
  }
  
}
