using Unity.VisualScripting;
using UnityEngine;


namespace Player
{
    public class TapMovement : Movement
    {
        [Space]
        [Header("Tap Movement")]
        [SerializeField] private float inputBufferTime = 0.05f;
        InputValue inputBuffered;



        protected override void Update()
        {
            
            base.Update();


            //move at the end if input buffered
            if (inputBuffered != null && Time.time > lastMoveTime + moveTime && currentStun <= 0)
            {
                //print((inputBuffered != Vector2Int.zero) + " : " + (Time.time > lastMoveTime + moveTime) + " : " + (currentStun));
                if (inputBuffered is RotateInput)
                    Rotate(inputBuffered as RotateInput);
                else
                    Move(inputBuffered as MoveInput);

                inputBuffered = null;
            }

        }

        protected override void OnMovePressed(MoveInput input)
        {
            Move(input);
        }

        protected override void OnRotatePressed(RotateInput input)
        {
            Rotate(input);
        }

        

        protected override bool CanMove(InputValue input, float nextMoveTime)
        {
            
            // input buffering
            if (Time.time < nextMoveTime)
            {
                if (Time.time + inputBufferTime > nextMoveTime)
                {
                    inputBuffered = input;
                }
            }

            return base.CanMove(input, nextMoveTime);
        }


    }
}
