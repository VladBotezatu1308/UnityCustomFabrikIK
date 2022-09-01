using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Grappling
{
    public class PickupManager : MonoBehaviour
    {
        [SerializeField] private SphereCollider spawnzone;
        [SerializeField] private Pickup pickupPrefab;
        [SerializeField] private Transform destinationPrefab;

        private void Start()
        {
            Spawn();
        }

        private void Spawn()
        {
            float degreePickup = Random.Range(0, 360.0f);
            float degreeDest = Random.Range(0, 90.0f) + 180.0f + degreePickup;

            Quaternion rotPickup = Quaternion.Euler(0, degreePickup, 0);
            Vector3 posPickup = Vector3.forward * Random.Range(Mathf.Min(1, spawnzone.radius), spawnzone.radius);
            posPickup = transform.position + rotPickup * posPickup;
            
            Quaternion rotDest = Quaternion.Euler(0, degreeDest, 0);
            Vector3 posDest = Vector3.forward * Random.Range(Mathf.Min(1, spawnzone.radius), spawnzone.radius);
            posDest = transform.position + rotDest * posDest;

            Pickup pickup = Instantiate(pickupPrefab, posPickup, rotPickup);
            Transform destination = Instantiate(destinationPrefab, posDest, rotDest);
            
            pickup.destination = destination;
            pickup.ReachedDestination.AddListener(Spawn);
        }
    }
}