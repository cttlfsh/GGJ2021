using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float xOffset = 0;
    public float yOffset = 1;
    public float zOffset = -5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = new Vector3(target.position.x + xOffset, target.position.y + yOffset, target.position.z + zOffset);
        transform.position = newPosition;
    }
}
