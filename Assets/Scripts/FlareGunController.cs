using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareGunController : MonoBehaviour
{

  // private GameObject[] bullets = new GameObject[3];
  private int remainingBullets;
  public GameObject bulletPrefab;
  public float flareTime;
  public float force;
  // Start is called before the first frame update
  void Start()
  {
    remainingBullets = 3;
  }

  public void shoot()
  {
    if ((remainingBullets > 0) && (remainingBullets <= 3))
    {
      var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
      bullet.transform.Rotate(Vector3.left * 90);
      Rigidbody rb;
      rb = bullet.GetComponent<Rigidbody>();
      rb.AddForce(transform.forward * force);

      Destroy(bullet, flareTime);
    }

  }
  // Update is called once per frame
  void Update()
  {
    
  }
}
