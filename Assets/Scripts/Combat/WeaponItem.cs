using UnityEngine;

public class WeaponItem : InventoryItem
{
    public Animator weaponAnimator;

    public Transform weaponHoldPoint;
    public Transform bulletShootPoint;
    public WeaponDataSO data;

    [HideInInspector] public int currentAmmo;

    private void Start()
    {
        if(data != null)
            currentAmmo = data.maxAmmo;
    }

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
