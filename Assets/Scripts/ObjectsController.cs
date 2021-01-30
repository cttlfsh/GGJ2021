using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsController : MonoBehaviour
{
  private bool isPickedUpSmartphone;
  private bool isPickedUpCompass;
  private bool isPickedUpBeacon;
  private bool isPickedUpFlareGun;
  private bool isPickedUpMap;
  private bool isPickedUpWalkieTalkie;

  private GameObject smartphone;
  private GameObject flareGun;
  private GameObject map;
  private GameObject beacon;
  private GameObject compass;
  private GameObject walkieTalkie;

  public GameObject pickedUpSmartphonePrefab;
  public GameObject pickedUpFlareGunPrefab;
  private bool isPhoneInHand;
  private bool isGunInHand;

  // private Collider coll;

  // Start is called before the first frame update
  void Start()
  {
    // coll = GetComponent<Collider>();
    isPhoneInHand = false;
    isGunInHand = false;
  }
  void pickUpFromScene(ref GameObject element, GameObject sceneObject)
  {
    element = sceneObject;
    sceneObject.SetActive(false);
    // Destroy(sceneObject);
  }
  bool pickUp()
  {
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 1.0f))
    {
      // Debug.Log("Raycast!");
      if (hit.collider.tag == "object")
      {
        // Debug.Log("collider");
        switch (hit.collider.name)
        {
          case "smartphone":
            Debug.Log("Smartphone picked up!");
            isPickedUpSmartphone = true;
            pickUpFromScene(ref smartphone, hit.collider.gameObject);
            break;
          case "minimap":
            isPickedUpMap = true;
            useObject("map");
            pickUpFromScene(ref map, hit.collider.gameObject);
            break;
          case "flareGun":
            isPickedUpFlareGun = true;
            pickUpFromScene(ref flareGun, hit.collider.gameObject);
            break;
          case "compass":
            isPickedUpCompass = true;
            useObject("compass");
            pickUpFromScene(ref compass, hit.collider.gameObject);
            break;
          case "walkieTalkie":
            isPickedUpWalkieTalkie = true;
            useObject("walkieTalkie");  
            pickUpFromScene(ref walkieTalkie, hit.collider.gameObject);
            break;
          case "beacon":
            isPickedUpBeacon = true;
            pickUpFromScene(ref beacon, hit.collider.gameObject);
            break;
          default:
          break;
        }
        return true;
      }
    }
    return false;

  }
  void takeOutPhone()
  {

  }

  IEnumerator takePic()
  {
    // appear phone
    return smartphone.GetComponent<SmartphoneController>().takePic();
    
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

  void takeOutGun()
  {

  }

  void shootFlare()
  {
    // instantiate and force a bullet in air making a lot of red light
    flareGun.GetComponent<FlareGunController>().shoot();
  }

  public void activateSound()
  {
    // if the other walkie talkie is picked up, listen to the world sounds
    
  }

  void useObject(string element)
  {
    switch (element)
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
      case "flareGun":
        if (isPickedUpFlareGun)
        {
          shootFlare();
        }
          
        break;
      case "map":
        if (isPickedUpMap)
        {
          enableMap();
        }

        break;
      // case "walkieTalkie":
      //   if (isPickedUpWalkieTalkie)
      //   {
      //     listen();
      //   }
        // break;
      default:
        break;
    }
  }

  void switchItem()
  {
    if (Input.GetKey(KeyCode.Alpha1)){
      // useObject("smartphone");
      isGunInHand = false;
      isPhoneInHand = true;
    }
    if (Input.GetKey(KeyCode.Alpha2)){
      // useObject("flareGun");
      isGunInHand = true;
      isPhoneInHand = false;
    }
  }

  // Update is called once per frame
  void Update()
  {
    switchItem();
    if (Input.GetKey(KeyCode.F)) 
    {
      // Debug.Log("F pressed");
      pickUp();
      // useObject("walkieTalkie"); 
      useObject("flareGun");
    }
    
    
  }
  public void LateUpdate()
  {
    if  ((Input.GetKey(KeyCode.F)) && (isPhoneInHand))
    {
      StartCoroutine(takePic());
    }
  }
  
}
