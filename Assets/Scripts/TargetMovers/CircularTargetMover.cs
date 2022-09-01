using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace TargetMovers
{
    public class CircularTargetMover : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private bool isActive;
        [SerializeField] private float frequency;
        [SerializeField] private float radius;
        

        private Coroutine movementCoroutine;

        private void Start()
        {
            movementCoroutine = StartCoroutine(Movement());
        }

        private void Update()
        {
            if (movementCoroutine == null && isActive)
            {
                movementCoroutine = StartCoroutine(Movement());
            }
        }

        private IEnumerator Movement()
        {
            while (isActive)
            {

                Vector3 position = transform.right * radius;
                position = transform.position + Quaternion.AngleAxis(Time.time * 360, transform.up) * position;
                target.position = position;
                yield return null;
            }

            movementCoroutine = null;
        }

        private void OnDrawGizmos()
        {
            Handles.color = Color.magenta;
            Handles.DrawWireDisc(transform.position, transform.up, radius);
        }
    }
}