using Unity.VisualScripting;
using UnityEngine;


namespace Player
{
    public abstract class Movement : MonoBehaviour
    {
        [Header("Values")]
        public float gridSize = 5f;

        public float vAccel = 1f;
        bool grounded;
        float fallVelocity = 0;

        public float moveTime = 0.5f;
        

         [System.Serializable]
        public class ErpCurve2
        {
            public AnimationCurve x;
            public AnimationCurve y;
        }


        [Header("Interpolation")]
        
        public AnimationCurve moveInterpolation;
        public AnimationCurve rotationInterpolation;
        protected float lastMoveTime = 0f;


        
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
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask envioronmentMask;
        [SerializeField] private float stunFallHeight = 3f;
        [SerializeField] private float stunDuration;
        protected float currentStun;
        float lastGroundedHeight;

        

        [Space]

        [SerializeField] private string ElevatorTag = "Elevator";

        protected Vector2 targetPosition;
        protected Vector2 lastPosition;
        protected float targetRotation;
        protected float lastRotation;


        public bool IsFalling()
        {
            return !grounded;
        }

        public bool IsStunned(out float stunTimeRemaining)
        {
            stunTimeRemaining = currentStun;
            return currentStun > 0;
        }


        protected virtual void OnEnable()
        {
            InputManager.OnPlayerMove += OnMovePressed;
            InputManager.OnPlayerRotate += OnRotatePressed;
        }

        protected virtual void OnDisable()
        {
            InputManager.OnPlayerMove -= OnMovePressed;
            InputManager.OnPlayerRotate -= OnRotatePressed;
        }

        void Start()
        {
            lastMoveTime = -moveTime;
        }

        protected virtual float GetMoveFactor()
        {
            return (Time.time - lastMoveTime) / moveTime;
        }


        protected InputValue lastMove;
        protected virtual void Update()
        {
            bool wasgrounded = grounded;
            //// Check if Player is Grounded and snap put on Ground
            float castDistance = playerHeight + (grounded ? slopeLenience : 0f);
            grounded = Physics.SphereCast(transform.position + new Vector3(0, playerHeight, 0), groundOverlapRadius, Vector3.down,
                        out RaycastHit hit, castDistance, groundMask);

            if (grounded)
            {
                // Just touched the ground
                if (!wasgrounded)
                    if (lastGroundedHeight - hit.point.y > stunFallHeight)
                        currentStun = stunDuration;

                fallVelocity = 0;
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                lastGroundedHeight = transform.position.y;
            }
            else
            {
                fallVelocity += vAccel * Time.deltaTime;
                transform.position += new Vector3(0, -fallVelocity, 0);
            }


            //move player to target position using interpolation curves

            float factor = GetMoveFactor();
            
            bool lastMoveWasRotation = lastMove is RotateInput;

            if (lastMoveWasRotation) // last move was rotation
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

            if (factor < 1)
            {
                ViewBob(factor);
                hudBobber.Bob(factor, lastMoveWasRotation, xBobDir);//lastMoveWasRotation ? moveDirection : xBobDir);
            }

            currentStun = Mathf.Max(0, currentStun - Time.deltaTime);

        }

        
        

        protected short xBobDir = 1;
        protected void ViewBob(float fac)
        {
            viewBobber.localPosition =
            new Vector2((viewBobCurves.x.Evaluate(fac) - 0.5f) * 2 * xBobDir * viewBobMult.x, viewBobCurves.y.Evaluate(fac) * viewBobMult.y);
        }

        protected abstract void OnMovePressed(MoveInput moveInput);

        protected abstract void OnRotatePressed(RotateInput rotateInput);

        protected void Move(MoveInput moveInput)
        {
            if (!CanMove(moveInput, GetNextMoveTime())) return;

            //rotate input by direction being faced
            float theta = Mathf.Deg2Rad * -transform.rotation.eulerAngles.y;
            Vector2 dir = moveInput.GetValue();
            float x = dir.x * Mathf.Cos(theta) - dir.y * Mathf.Sin(theta);
            float y = dir.x * Mathf.Sin(theta) + dir.y * Mathf.Cos(theta);
            dir = new Vector2(x, y);

            RaycastHit hit;
            if (Physics.SphereCast(moveCastPoint.position, moveCastRadius, SwizzleFromXY(dir), out hit, gridSize + playerRadius, envioronmentMask))
            {
                if (!hit.collider.CompareTag(ElevatorTag))
                    return;
            }
            //ignore y component
            lastPosition = targetPosition;
            targetPosition += dir * gridSize;
            //Debug

            //snap to grid 
            //(mitigates floating point presition)
            targetPosition = new Vector2(Mathf.Round(targetPosition.x / gridSize)* gridSize, Mathf.Round(targetPosition.y / gridSize)* gridSize);

            lastMoveTime = Time.time;
            xBobDir *= -1;
            lastMove = moveInput;
            //moveDirection = dir;

        }

        protected virtual void Rotate(RotateInput rotateInput)
        {
            if (!CanMove(rotateInput, GetNextMoveTime())) return;

            lastRotation = targetRotation;
            targetRotation = targetRotation + rotateInput.GetValue() * 90;

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
            lastMove = rotateInput;
        }

        protected virtual bool CanMove(InputValue input, float nextMoveTime)
        {
            
            if (Time.time < nextMoveTime)
            {
                return false;
            }
            if (currentStun > 0)
                return false;
            if (!grounded) return false;

            if (moveCastPoint == null)
            {
                Debug.LogError("Move Cast Point Undefined");
                return false;
            }
            return true;
        }

        public float GetNextMoveTime()
        {
            return lastMoveTime + moveTime + (currentStun > 0 ? stunDuration : 0);
        }

        
        protected void MoveOrRotate(InputValue inputValue)
        {
            if(inputValue is MoveInput)
            {
                Move(inputValue as MoveInput);
            }
            else
            {
                Rotate(inputValue as RotateInput);
            }
        }

        public struct Targets
        {
            public Vector3 targetPos;
            public Vector3 oldPos;

            public float targetRot;
            
            public float oldRot;
        }

        public void SetTargets(Targets targets)
        {
            targetPosition = targets.targetPos;
            lastPosition = targets.oldPos;
            targetRotation = targets.targetRot;
            lastRotation = targets.oldRot;
        }

        public Targets GetTargets()
        {
            Targets targets = new();
            
            targets.targetPos = targetPosition;
            targets.oldPos = lastPosition;
            targets.targetRot = targetRotation;
            targets.oldRot = lastRotation;

            return targets;
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
