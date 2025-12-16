using UnityEngine;

public class CursorSwapper : MonoBehaviour
{
    public bool weaponEnabled;

    [SerializeField] private CursorInteraction interaction;
    [SerializeField] private WeaponController weaponController;
    [SerializeField] private Animator cursorSwapAnimator;

    [SerializeField] private float TransitionTime = 0.15f;

    private void OnEnable()
    {
        InputManager.OnADSStart += EnableWeapon;
        InputManager.OnADSEnd += DisableWeapon;

        float animSpeed = TransitionTime == 0 ? Mathf.Infinity : 1 / TransitionTime;
        cursorSwapAnimator.speed = animSpeed;
        DisableWeapon();
    }

    private void OnDisable()
    {
        InputManager.OnADSStart -= EnableWeapon;
        InputManager.OnADSEnd -= DisableWeapon;
    }


    private void EnableWeapon()
    {
        weaponEnabled = true;

        interaction.canPickup = false;
        weaponController.enabled = true;

        cursorSwapAnimator.SetBool("ADS", true);
    }


    private void DisableWeapon()
    {
        weaponEnabled = false;

        interaction.canPickup = true;
        weaponController.enabled = false;

        cursorSwapAnimator.SetBool("ADS", false);
    }
}
