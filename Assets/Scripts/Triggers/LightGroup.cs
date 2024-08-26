using System;
using UnityEngine;

namespace Triggers
{
    public class LightGroup : MonoBehaviour
    {
        public LightTrigger[] lights;
        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Player"))
                TurnOnLights();
            
            if(other.gameObject.CompareTag("Demon"))
                TurnOffLights();
        }

        private void TurnOnLights()
        {
            foreach (var l in lights)
            {
                l.TurnOnLight();
            }
        }
        private void TurnOffLights()
        {
            foreach (var l in lights)
            {
                l.TurnOffLight();
            }
        }
        
        
    }
}
