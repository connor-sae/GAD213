using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    public class Movement : MonoBehaviour
    {
        public float gridSize = 5f;
        [SerializeField] private float playerRadius = 1.5f;

        [Space]
        [SerializeField] private Transform moveCastPoint;
        public float moveCastRadius = 1f;
        public float groundOverlapRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;

        [Space]
        [SerializeField] private string ElevatorTag = "Elevator";


        private void OnEnable()
        {
            InputManager.OnPlayerMove += OnMove;
            InputManager.OnPlayerRotate += OnRotate;
        }

        private void OnDisable()
        {
            InputManager.OnPlayerMove -= OnMove;
            InputManager.OnPlayerRotate -= OnRotate;
        }

        bool grounded;
        private void Update()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, groundOverlapRadius, groundLayer);
            Debug.Log(hits.Length);
            grounded = (hits.Length > 0);

        }


        public void OnMove(short dir)
        {
            RaycastHit hit;
            if (!grounded) return;
            if(moveCastPoint == null)
            {
                Debug.LogError("Move Cast Point Undefined");
                return;
            }
            if (Physics.SphereCast(moveCastPoint.position, moveCastRadius, transform.forward * dir, out hit, gridSize + playerRadius))
            {
                if (!hit.collider.CompareTag(ElevatorTag))
                    return;
            }

            transform.position += transform.forward * dir * gridSize;
        }

        private void OnRotate(short dir)
        {
            float angle = transform.rotation.eulerAngles.y + dir * 90;

            transform.rotation = Quaternion.Euler(0, angle, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(moveCastPoint.position, transform.forward * (gridSize + playerRadius));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(moveCastPoint.position + transform.forward * (gridSize + playerRadius), moveCastRadius);
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(moveCastPoint.position, transform.forward * -1 * (gridSize + playerRadius));
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(moveCastPoint.position + transform.forward * -1 * (gridSize + playerRadius), moveCastRadius);
            Gizmos.DrawSphere(transform.position, 0.3f);
        }

        public enum Direction
        {
            FORWARD,
            BACKWARD,
            RIGHT,
            LEFT,
        }

    }
}
