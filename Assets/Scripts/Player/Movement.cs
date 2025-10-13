using System;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    public class Movement : MonoBehaviour
    {
        public float gridSize = 5f;

        public float vAccel = 9.8f;
        bool grounded;
        float fallVelocity = 0;

        public float moveDelay = 0.5f;
        public AnimationCurve MoveInterpolation;
        float lastMoveTime = 0f;
        [SerializeField] private float inputBufferTime = 0.05f;
        short inputBuffered;

        [Space]
        [SerializeField] private float playerRadius = 1.5f;
        [SerializeField] private float playerHeight = 4.0f;
        [SerializeField] private float slopeLenience = 0.2f;

        [Space]
        [SerializeField] private Transform moveCastPoint;
        public float moveCastRadius = 1f;
        public float groundOverlapRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;

        [Space]
        [SerializeField] private string ElevatorTag = "Elevator";

        Vector2 targetPosition;
        Vector2 lastPosition;


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

        void Start()
        {
            //targetPosition = transform.position;
            //last
        }

        private void Update()
        {

            //// Check if Player is Grounded and snap put on Ground
            
            grounded = Physics.SphereCast(transform.position + new Vector3(0, playerHeight, 0), groundOverlapRadius, Vector3.down,
             out RaycastHit hit, playerHeight + (grounded ? slopeLenience : 0f), groundLayer);

            if (grounded)
            {
                fallVelocity = 0;
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            }
            else
            {
                fallVelocity += vAccel * Time.deltaTime;
                transform.position += new Vector3(0, -fallVelocity, 0);
            }

            //move at the end if input buffered
            if ( inputBuffered != 0 && Time.time - moveDelay > lastMoveTime)
                    OnMove(inputBuffered);

            //move player to target position

            Vector2 erp = targetPosition - lastPosition;
            erp *= MoveInterpolation.Evaluate((Time.time - lastMoveTime) / moveDelay);
            transform.position = SwizzleFromXY(erp + lastPosition, transform.position.y);
        }

        public void OnMove(short dir)
        {

            if (Time.time - moveDelay < lastMoveTime)
            {
                if (Time.time - moveDelay - inputBufferTime > lastMoveTime)
                    inputBuffered = dir;
                return;
            }
            ;
            if (!grounded) return;

            if (moveCastPoint == null)
            {
                Debug.LogError("Move Cast Point Undefined");
                return;
            }

            RaycastHit hit;
            if (Physics.SphereCast(moveCastPoint.position, moveCastRadius, transform.forward * dir, out hit, gridSize + playerRadius))
            {
                if (!hit.collider.CompareTag(ElevatorTag))
                    return;
            }
            //ignore y component
            lastPosition = targetPosition;
            targetPosition += SwizzleToXZ(transform.forward) * dir * gridSize;
            lastMoveTime = Time.time;
            inputBuffered = 0;

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
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(moveCastPoint.position, transform.position);
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.3f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, playerRadius, 0), playerRadius);
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, playerHeight - playerRadius, 0), playerRadius);

            Gizmos.color = Color.greenYellow;
            Gizmos.DrawSphere(new Vector3(targetPosition.x, transform.position.y, targetPosition.y), 1f);
        }

        private Vector2 SwizzleToXZ(Vector3 vec3)
        {
            return new Vector2(vec3.x, vec3.z);
        }

        private Vector3 SwizzleFromXY(Vector2 vec2, float yVal = 0)
        {
            return new Vector3(vec2.x, yVal, vec2.y);
        }
    }
}
