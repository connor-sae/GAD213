using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, InputActions.IPlayerActions//, InputActions.IUIActions
{
    InputActions IA;

    private void OnEnable()
    {
        if (IA == null)
        {
            IA = new InputActions();
            IA.Player.SetCallbacks(this);


            IA.Player.Enable();
        }
    }


    public static event Action<short> OnPlayerMove;
    public static event Action<short> OnPlayerRotate;

    private void OnDisable()
    {
        IA.Player.RemoveCallbacks(this);
    }



    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }



    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }



    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }






    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Debug.Log("Jump");
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }


    public void OnMoveUp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerMove?.Invoke(1);
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerMove?.Invoke(-1);
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerRotate?.Invoke(1);
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerRotate?.Invoke(-1);
    }



    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new NotImplementedException();
    }
}
