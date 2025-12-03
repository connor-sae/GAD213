using Player;
using UnityEngine;

public class MovementSwapper : MonoBehaviour
{
    public bool usingTapMovement;
    [SerializeField] private TapMovement tapMovement;
    [SerializeField] private HoldMovement holdMovement;
    bool tapActive = true;
    void Update()
    {
        if(tapActive != usingTapMovement) // swap
        {
            tapActive = usingTapMovement;
            
            if(tapActive)
            {
                tapMovement.SetTargets(holdMovement.GetTargets());
                tapMovement.enabled = true;
                holdMovement.enabled = false;
            }
            else
            {
                holdMovement.SetTargets(tapMovement.GetTargets());
                tapMovement.enabled = false;
                holdMovement.enabled = true;
            }
            
        }
    }


    public Movement GetActiveMovement()
    {
        return tapActive ? tapMovement : holdMovement;
    }

}
