using System;
using System.Collections;
using UnityEngine;

namespace Grappling
{
    public class GrapplingHook : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] private float detectionRange;
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private float movementDuration;
        [SerializeField] private bool initOnStart = true;
        
        [Header("References")]
        [SerializeField] private Transform target;
        

        [SerializeField]
        private Pickup heldPickup;
        private Coroutine currentMovement = null;
        private Collider[] hitColliders = new Collider[5];

        private Vector3 start;
        private Vector3 end;

        private void Start()
        {
            if (initOnStart)
            {
                Setup();
            }
        }

        private void Setup()
        {
            if (!TryGetComponent(out Collider _))
            {
                gameObject.AddComponent<SphereCollider>();
            }
        }

        private void Update()
        {
            if (currentMovement == null)
            {
                int hitCount = Physics.OverlapSphereNonAlloc(transform.position, detectionRange, hitColliders, layerMask.value);
                for (int i = 0; i < hitCount; i++)
                {
                    Transform hitTransform = hitColliders[i].transform;
                    if (hitTransform.TryGetComponent<Pickup>(out _))
                    {
                        currentMovement = StartCoroutine(Movement(target.position, hitTransform.position));
                    }
                }
            }

            if (heldPickup != null)
            {
                heldPickup.transform.position = transform.position;
            }
        }

        private IEnumerator Movement(Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;
            float timer = 0.0f;
            Vector3 hopMaxHeight = (start - end).magnitude * Vector3.up;
            while (timer <= movementDuration)
            {
                float completion = timer / movementDuration;
                target.position = Vector3.Lerp(start, end, completion) + hopMaxHeight * Mathf.Sin(completion * Mathf.PI);
                timer += Time.deltaTime;
                yield return null;
            }

            target.position = end;
        }

        private void OnTriggerEnter(Collider coll)
        {
            if (coll.transform.TryGetComponent(out Pickup pickup))
            {
                heldPickup = pickup;
                heldPickup.ReachedDestination.AddListener(DropPickup);
                
                if (currentMovement != null)
                {
                    StopCoroutine(currentMovement);
                }

                currentMovement = StartCoroutine(Movement(target.position, heldPickup.destination.position));
            }
        }

        private void DropPickup()
        {
            heldPickup = null;
            if (currentMovement != null)
            {
                StopCoroutine(currentMovement);
                currentMovement = null;
            }
        }

        private void OnDrawGizmos()
        {
            int kSeg = 20;
            Vector3 hopMaxHeight = (start - end).magnitude * Vector3.up;
            for (int i = 0; i < kSeg; i++)
            {
                float comp1 = i / (float)kSeg;
                float comp2 = (i + 1) / (float)kSeg;
                Vector3 pos1 = Vector3.Lerp(start, end, comp1) + hopMaxHeight * Mathf.Sin((comp1 * Mathf.PI));
                Vector3 pos2 = Vector3.Lerp(start, end, comp2) + hopMaxHeight * Mathf.Sin((comp2 * Mathf.PI));
                
                Debug.DrawLine(pos1, pos2, Color.magenta);
            }
        }
    }
}