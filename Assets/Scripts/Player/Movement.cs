using UnityEngine;


namespace Player
{
    public class Movement : MonoBehaviour
    {
        [Header("Values")]
        public float gridSize = 5f;

        public float vAccel = 9.8f;
        bool grounded;
        float fallVelocity = 0;

        public float moveTime = 0.5f;
        
        [SerializeField] private float inputBufferTime = 0.05f;
        Vector2Int inputBuffered;

         [System.Serializable]
        public class ErpCurve2
        {
            public AnimationCurve x;
            public AnimationCurve y;
        }


        [Header("Interpolation")]
        
        public AnimationCurve moveInterpolation;
        public AnimationCurve rotationInterpolation;
        float lastMoveTime = 0f;


        
        [SerializeField] private Transform viewBobber;

        public ErpCurve2 viewBobCurves;
        public Vector2 viewBobMult;
        public HudBobbing hudBobber;


        [Space]
        [Header("Bounds")]
        [SerializeField] private float playerRadius = 1.5f;
        [SerializeField] private float playerHeight = 4.0f;
        [SerializeField] private float slopeLenience = 0.2f;

        [Space]
        [Header("Physics")]
        [SerializeField] private Transform moveCastPoint;
        public float moveCastRadius = 1f;
        public float groundOverlapRadius = 0.3f;
        [SerializeField] private LayerMask groundLayer;

        

        [Space]

        [SerializeField] private string ElevatorTag = "Elevator";

        Vector2 targetPosition;
        Vector2 lastPosition;
        float targetRotation;
        float lastRotation;


       short moveDirection;


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
            lastMoveTime = -moveTime;
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
            if (inputBuffered != Vector2Int.zero && Time.time - moveTime > lastMoveTime)
            {
                if (inputBuffered.x != 0)
                    OnRotate((short)inputBuffered.x);
                else
                    OnMove((short)inputBuffered.y);
                    
                inputBuffered = Vector2Int.zero;
            }


            //move player to target position using interpolation curves

                float factor = (Time.time - lastMoveTime) / moveTime;

            if (lastMoveWasRotation)
            {
                float erpRot = targetRotation - lastRotation;



                erpRot *= rotationInterpolation.Evaluate(factor);
                transform.rotation = Quaternion.Euler(0, erpRot + lastRotation, 0);
            }
            else
            {

                Vector2 erpPos = targetPosition - lastPosition;
                
                
                erpPos *= moveInterpolation.Evaluate(factor);
                transform.position = SwizzleFromXY(erpPos + lastPosition, transform.position.y);

            }
            
            if(factor < 1)
            {
                ViewBob(factor);
                hudBobber.Bob(factor, lastMoveWasRotation, lastMoveWasRotation ? moveDirection : xBobDir);
            }
        }

        
        

        short xBobDir = 1;
        private void ViewBob(float fac)
        {
            viewBobber.localPosition =
            new Vector2((viewBobCurves.x.Evaluate(fac) - 0.5f) * 2 * xBobDir * viewBobMult.x, viewBobCurves.y.Evaluate(fac) * viewBobMult.y);
        }

        bool lastMoveWasRotation;
        public void OnMove(short dir)
        {
            if (!CanMove(new Vector2Int(0, dir))) return;

            RaycastHit hit;
            if (Physics.SphereCast(moveCastPoint.position, moveCastRadius, transform.forward * dir, out hit, gridSize + playerRadius))
            {
                if (!hit.collider.CompareTag(ElevatorTag))
                    return;
            }
            //ignore y component
            lastPosition = targetPosition;
            targetPosition += SwizzleToXZ(transform.forward) * dir * gridSize;
            //Debug

            //snap to grid 
            //(mitigates floating point presition)
            targetPosition = new Vector2(Mathf.Round(targetPosition.x / gridSize)* gridSize, Mathf.Round(targetPosition.y / gridSize)* gridSize);

            lastMoveTime = Time.time;
            xBobDir *= -1;
            lastMoveWasRotation = false;
            moveDirection = dir;

        }

        private void OnRotate(short dir)
        {
            if (!CanMove(new Vector2Int(dir, 0))) return;

            lastRotation = targetRotation;
            targetRotation = transform.rotation.eulerAngles.y + dir * 90;

            //snap rotation to 90
            //mitigates floating point precision
            targetRotation = Mathf.Round(targetRotation / 90) * 90;

            //fix rotation mapping
            if (targetRotation - lastRotation > 180) targetRotation -= 360;
            if (lastRotation - targetRotation > 180) lastRotation += 360;
            if (targetRotation >= 360 || lastRotation >= 360)
            {
                targetRotation -= 360;
                lastRotation -= 360;
            }
            if (targetRotation <= -360 || lastRotation <= -360)
            {
                targetRotation += 360;
                lastRotation += 360;
            }

            lastMoveTime = Time.time;
            xBobDir *= -1;
            lastMoveWasRotation = true;
            moveDirection = dir;
        }

        private bool CanMove(Vector2Int dir)
        {
            if (Time.time < lastMoveTime + moveTime)
            {
                if (Time.time + inputBufferTime > lastMoveTime + moveTime)
                {
                    inputBuffered = dir;
                }
                return false;
            }
            if (!grounded) return false;

            if (moveCastPoint == null)
            {
                Debug.LogError("Move Cast Point Undefined");
                return false;
            }
            return true;
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
