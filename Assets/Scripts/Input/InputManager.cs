using System;
using Unity.Collections;
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


    public static event Action<MoveInput> OnPlayerMove;
    public static event Action<RotateInput> OnPlayerRotate;

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
            OnPlayerMove?.Invoke(new MoveInput(0, 1));
    }

    public void OnMoveDown(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerMove?.Invoke(new MoveInput(0, -1));
    }

    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerMove?.Invoke(new MoveInput(1, 0));
    }

    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerMove?.Invoke(new MoveInput(-1, 0));
    }
    public void OnRotateRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerRotate?.Invoke(new RotateInput(1));
    }

    public void OnRotateLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            OnPlayerRotate?.Invoke(new RotateInput(-1));
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
public class InputValue
{
    protected object value;
  
}

public class MoveInput : InputValue
{
    public MoveInput(int x, int y)
    {
        this.value = new Vector2Int(x, y);
    }

    public Vector2Int GetValue()
    {
        return (Vector2Int)value;
    }

}    

public class RotateInput : InputValue
{
    public RotateInput(short value)
    {
        this.value = value;
    }

    public int GetValue()
    {
        return (short)value;
    }
}   
