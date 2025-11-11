using UnityEngine;

public class Inventory : MonoBehaviour
{

    private bool open;

    [SerializeField] private Animator animator;

    #region Enable / Disable
    void OnEnable()
    {
        InputManager.OnInventoryToggle += ToggleInventory;
    }

    void OnDisable()
    {
        InputManager.OnInventoryToggle -= ToggleInventory;
    }
    #endregion
    
    void ToggleInventory()
    {
        open = !open;
        animator.SetBool("Open", open);
    }
}
