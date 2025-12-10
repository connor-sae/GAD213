using UnityEngine;

public class CursorSwapper : MonoBehaviour
{
    private bool weaponEnabled;

    [SerializeField] private CursorInteraction interaction;
    [SerializeField] private WeaponController weaponController;

    private void EnableWeapon()
    {
        weaponEnabled = true;

        interaction.enabled = false;
        weaponController.enabled = true;
    }


    private void DisableWeapon()
    {
        weaponEnabled = false;

        interaction.enabled = true;
        interaction.enabled = false;
    }
}
