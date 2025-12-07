using UnityEngine;

public class WeaponItem : InventoryItem
{
    [SerializeField] private Animator weaponAnimator;

    public Transform weaponHoldPoint;
    public Transform bulletShootPoint;
    public WeaponDataSO data;

    public int currentAmmo;

    public override void Store(InventorySlot slot, Transform anchorPoint = null)
    {
        base.Store(slot, anchorPoint);


        //weaponAnimator?.SetBool("stored", true);
    }

    public override void Release()
    {
        base.Release();


        //weaponAnimator?.SetBool("stored", false);
    }
}
