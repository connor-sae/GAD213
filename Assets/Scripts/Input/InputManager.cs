using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>, InputActions.IPlayerActions//, InputActions.IUIActions
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
    public static event Action<Vector2> OnLookChanged;
    public static event Action OnInteractStart;
    public static event Action OnInteractEnd;
    public static event Action OnInventoryToggle;
    private Vector2 _mousePosition = Vector2.zero;
    public static Vector2 mousePosition
    {
        get
        {
            return instance._mousePosition;
        }
    }

    private void OnDisable()
    {
        IA.Player.RemoveCallbacks(this);
    }



    public void OnLook(InputAction.CallbackContext context)
    {
        _mousePosition = context.ReadValue<Vector2>();
        OnLookChanged?.Invoke(_mousePosition);
    }



    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnInteractStart?.Invoke();
        else
            OnInteractEnd?.Invoke();
    }

    
    public void OnADS(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.performed) OnInventoryToggle?.Invoke();
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
