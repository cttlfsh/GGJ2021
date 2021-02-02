using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlareBulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = gameObject.transform.position;
        if (transform.position.y > 20f)
        {
        Destroy(GetComponent<Rigidbody>());
        }
    }
}
