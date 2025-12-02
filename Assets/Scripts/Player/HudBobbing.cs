using Player;
using UnityEngine;

public class HudBobbing : MonoBehaviour
{

    [SerializeField] private Movement movement;
    [SerializeField] private AnimationCurve xBobEffector;
    [SerializeField] private AnimationCurve yBobEffector;
    [SerializeField] private Vector2 bobMax;
    [SerializeField] private AnimationCurve rotationEffector;
    [SerializeField] private float rotationMax;
    [SerializeField] private Transform pistonTrans;
    [SerializeField] private AnimationCurve pistonMovement;
    [SerializeField] private Animator HudAnimator;
    [SerializeField] private float exitStunEarly;


    private void Update()
    {
        HudAnimator.SetBool("Falling", movement.IsFalling());
        bool stunned = movement.IsStunned(out float stunRemaining);
        //print(stunRemaining);

        HudAnimator.SetBool("Stunned", stunned && stunRemaining > exitStunEarly);
    }

    public void Bob(float factor, bool isRotation, int bobDirection)
     {
        float xPos = xBobEffector.Evaluate(factor) * bobDirection * bobMax.x;
        float yPos = yBobEffector.Evaluate(factor) * bobMax.y;
        //print(moveDirection);

        transform.localPosition = new Vector3(xPos, yPos, 0);

        if(isRotation)
        {
            float yRot = rotationEffector.Evaluate(factor) * bobDirection * rotationMax;

            transform.localRotation = Quaternion.Euler(0, yRot, 0);
        }

        pistonTrans.localPosition = Vector3.forward * pistonMovement.Evaluate(factor);
     }


}
