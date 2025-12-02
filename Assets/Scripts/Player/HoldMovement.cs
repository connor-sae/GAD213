using UnityEngine;


namespace Player
{
    public class HoldMovement : Movement
    {
        [Space]
        [Header("Hold Movement")]
        [Tooltip("If the input held for shorter than this time after the previous movement it will revert that movement")]
        [SerializeField] private float inputAcceptanceThreshold;
        private InputValue activeInput;

        #region Enable / Disable

        protected override void OnEnable()
        {
            base.OnEnable();
            InputManager.OnPlayerMoveReleased += OnMoveReleased;
            InputManager.OnPlayerRotateReleased += OnRotateReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputManager.OnPlayerMoveReleased -= OnMoveReleased;
            InputManager.OnPlayerRotateReleased -= OnRotateReleased;
        }

        #endregion

        private float nextMoveTime;
        bool revertingMove;
        float revertStartTime;
        protected override void Update()
        {
            float moveFac = GetMoveFactor();

            if(moveFac <= 0) //when revertion ends
            {
                revertingMove = false;

                if(lastRevertWasRotation) // reset changes
                    targetRotation = lastRotation;
                else
                    targetPosition = lastPosition;
                
                xBobDir *= -1;

                lastMoveTime = Time.time - moveTime;
            }

            if(moveFac >= 1) // not moving
            {
                if(activeInput != null)
                {
                    MoveOrRotate(activeInput);
                    
                }
            }

            
            base.Update();
        }

        protected override float GetMoveFactor()
        {
            if(revertingMove)
            {
                return ((revertStartTime - lastMoveTime) - (Time.time - revertStartTime)) / moveTime;
            }else
                return base.GetMoveFactor();
        }


        protected override void OnMovePressed(MoveInput moveInput)
        {
            if(activeInput != null)
                OnMoveReleased(activeInput as MoveInput);

            activeInput = moveInput;
        }

        protected override void OnRotatePressed(RotateInput rotateInput)
        {
            if(activeInput != null)
                OnRotateReleased(activeInput as RotateInput);

            activeInput = rotateInput;
        }

        private void OnMoveReleased(MoveInput moveInput)
        {
            if((Vector2Int)lastMove.value == moveInput.GetValue()) // released the current move
            {
                if(Time.time - lastMoveTime <= inputAcceptanceThreshold) // within revert threshold
                    RevertMove(moveInput);

                activeInput = null;
            }
        }

        private void OnRotateReleased(RotateInput rotateInput)
        {
            if((lastMove as RotateInput).GetValue() == rotateInput.GetValue()) // released the current move
            {
                Debug.Log("Released matches");
                if(Time.time - lastMoveTime <= inputAcceptanceThreshold) // within revert threshold
                    RevertRotate(rotateInput);   

                activeInput = null;
            }
        }

        bool lastRevertWasRotation;
        private void RevertMove(MoveInput moveInput)
        {
           
            revertStartTime = Time.time;
            revertingMove = true;
            
            lastRevertWasRotation = false;

        }

        private void RevertRotate(RotateInput rotateInput)
        {
            //invert move time
            revertStartTime = Time.time;
            revertingMove = true;

            lastRevertWasRotation = true;

        }
        
    }
}
