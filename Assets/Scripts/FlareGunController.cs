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
      remainingBullets--;
      var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
      bullet.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
      // bullet.transform.Rotate(Vector3.left * 90);
      Rigidbody rb;
      rb = bullet.GetComponent<Rigidbody>();
      rb.AddForce(new Vector3(0,2,0)*force);

      Destroy(bullet, flareTime);
    }

  }
  // Update is called once per frame
  void Update()
  {
    
  }
}
