using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassController : MonoBehaviour
{
    public GameObject reference;
    private GameObject owner;
    public bool isOwned;
    // Start is called before the first frame update
    void Start()
    {
        isOwned = false;
    }

    public void owned()
    {
        isOwned = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (isOwned)
        {
            // owner = gameObject;
            // Vector2 targetDir = owner.transform.position - reference.transform.position;
            // float angle = Vector2.Angle(targetDir, owner.transform.forward);
            transform.LookAt(reference.transform);
            transform.gameObject.layer = 9;
            transform.GetChild(0).gameObject.layer = 9;
        }
    }
}
