using System;
using UnityEngine;
using UnityEngine.Events;

namespace Grappling
{
    public class Pickup : MonoBehaviour
    {
        
        public Transform destination;
        
        public UnityEvent ReachedDestination;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == destination)
            {
                ReachedDestination.Invoke();
                Destroy(destination.gameObject);
                Destroy(gameObject);
            }
        }
    }
}