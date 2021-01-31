using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerStatusCheck{
    public class ServerCheck : MonoBehaviour
    {
        // Start is called before the first frame update
        public bool isServer;
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

