using UnityEngine;

public class WeaponSlot : InventorySlot
{
    [SerializeField] private Animator weaponAnimator;
    //public WeaponDataSO data;

    protected override void OnStoreItem(InventoryItem item)
    {

        item.Store(this, (item as WeaponItem).weaponHoldPoint);

        if(weaponAnimator != null)
            weaponAnimator.SetBool("stored", true);
    }

    protected override bool CanStore(InventoryItem item)
    {
        return base.CanStore(item) && item is WeaponItem;
    }

    protected override void OnReleaseItem(InventoryItem item)
    {
        base.OnReleaseItem(item);

        if (weaponAnimator != null)
            weaponAnimator.SetBool("stored", false);
    }
}
