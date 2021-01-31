using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconController : MonoBehaviour
{
  public AudioSource beep;
  private float volume;
  public bool isOwned;
  private bool isPlayed;
  private bool isMuted;
  private GameObject owner;
  public GameObject otherPlayer;

  private float maxDistance;
  private float minDistance;
  private float maxAngle;
  private float maxVolume;
  // Start is called before the first frame update
  void Start()
  {
    isOwned = false;
    isPlayed = false;
    isMuted = false;
    volume = 0.01f;  
    maxAngle = 360f;
    maxDistance = 400f;
    minDistance = 200f;
    maxVolume = 0.2f;
    beep = GetComponent<AudioSource>();
  }

  float getDistance(GameObject object1, GameObject object2)
  {

    return Vector3.Distance (object1.transform.position, object2.transform.position);
  }

  float getAngle(GameObject object1, GameObject object2)
  {
    Vector2 targetDir = object2.transform.position - object1.transform.position;
    return Vector2.Angle(targetDir, object1.transform.forward);
  }
  float getVolume(float distance, float angle)
  {
    if (distance >= maxDistance)
      return maxVolume;
    if (distance <= maxDistance && distance >= 350)
      return maxVolume - 0.17f;
    if (distance < 350 && distance >= 300)
      return maxVolume - 0.15f;
    if (distance < 300 && distance >= 250)
      return maxVolume - 0.1f;
    if (distance < 250 && distance >= 200 )
      return maxVolume - 0.05f;
    // if (distance < 200)
    return 0.01f;
    // var vol = (distance/(maxDistance+minDistance)) - ((angle/maxAngle)*(1/8));
    // if (vol > maxVolume)
    // {
    //   vol = maxVolume;
    // }
    // return vol;
  }

  void mute()
  {
    // savedVolume = volume;
    volume = 0f;
    isMuted = true;
  }
  void unMute()
  {
    isMuted = false;
  }
  public void SetOwned()
  {
    isOwned = true;
  }
  public void SetOwner(GameObject player)
  {
    owner = player;
  }
  // Update is called once per frame
  void Update()
  {
    Debug.Log("update");
    if (isOwned)
    {
      Debug.Log("owned");
      if (!isPlayed)
      {
        isPlayed = true;
        beep.Play(0);
        Debug.Log("Playing!");
      } 
      if (!isMuted)
      {
        var distance = getDistance(owner, otherPlayer);
        Debug.Log("Distance");
        Debug.Log(distance);
        if (distance >= maxDistance)
        {
          volume = 0.001f;
          beep.volume = volume;

        } else if (distance <= minDistance)
        {
          volume = maxVolume;
          beep.volume = volume;

        } else
        {
          var angle = getAngle(owner, otherPlayer);
          volume = getVolume(distance, angle);
          beep.volume = volume;
          Debug.Log(volume);
        }
      }
    }
  }
}
