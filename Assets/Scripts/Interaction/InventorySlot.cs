using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InventorySlot : MonoBehaviour
{
    public Transform storePoint;
    public SlotType slotType;
    public float scaleFactor;

    [HideInInspector] 
    public InventoryItem storedItem;

    private Vector3 _oldScale;
    public bool TryStore(InventoryItem item)
    {
        if (!CanStore(item))
            return false;

        _oldScale = item.transform.localScale;
        item.transform.localScale = item.transform.localScale * scaleFactor;

        OnStoreItem(item);
        
        storedItem = item;

        return true;

    }

    protected virtual void OnStoreItem(InventoryItem item)
    {
        
        item.Store(this, item.storageAnchorPoint);
    }

    protected virtual bool CanStore(InventoryItem item)
    {
        return storedItem == null && item.slotType == slotType && !item.stored;
    }

    public InventoryItem RetrieveItem()
    {
        InventoryItem _item = storedItem;
        storedItem = null;

        OnReleaseItem(_item);

        _item.transform.localScale = _oldScale;

        return _item;
    }

    protected virtual void OnReleaseItem(InventoryItem item)
    {
        item.Release();
    }


}



public enum SlotType
{
    SMALL = 1,
    LARGE = 2,
}
