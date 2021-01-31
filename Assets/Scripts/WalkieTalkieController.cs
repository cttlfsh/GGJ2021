using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkieTalkieController : MonoBehaviour
{
    public AudioSource[] sounds;

    private bool changedArea;
    private bool isPlaying;
    private bool isActive;

    // Start is called before the first frame update
    void Start()
    {
        changedArea = false;
        isPlaying = false;
        isActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActive)
        {
            if (changedArea)
            {
                //play something
            }
        }
    }
}
