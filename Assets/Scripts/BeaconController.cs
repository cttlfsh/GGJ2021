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
  // Start is called before the first frame update
  void Start()
  {
    isOwned = false;
    isPlayed = false;
    isMuted = false;
    volume = 0f;  
    maxAngle = 360f;
    maxDistance = 400f;
    minDistance = 200f;
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
    return (distance/(maxDistance+minDistance)) - ((angle/maxAngle)*(1/8));
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
  // Update is called once per frame
  void Update()
  {
    if (isOwned)
    {
      if (!isPlayed)
      {
        isPlayed = true;
        beep.Play();
      }
      if (!isMuted)
      {
        var distance = getDistance(owner, otherPlayer);
        if (distance >= maxDistance)
        {
          volume = 0.1f;
        } else if (distance <= minDistance)
        {
          volume = 1f;
        } else
        {
          var angle = getAngle(owner, otherPlayer);
          volume = getVolume(distance, angle);
          beep.volume = volume;
        }
      }

    }
  }
}
