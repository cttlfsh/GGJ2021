using System;
using UnityEngine.Events;

namespace UnityEngine.AzureSky
{
    [Serializable]
    public sealed class AzureEventAction
    {
        // Not included in build
        #if UNITY_EDITOR
        public bool isExpanded = true;
        #endif
        
        public UnityEvent eventAction;
        public int hour = 0;
        public int minute = 0;
        public int year = 0;
        public int month = 0;
        public int day = 0;
    }
}