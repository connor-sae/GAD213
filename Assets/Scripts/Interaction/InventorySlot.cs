using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InventorySlot : MonoBehaviour
{
    public Transform storePoint;
    public SlotType slotType;
    public float scaleFactor;

    //[HideInInspector] 
    public InventoryItem storedItem;

    private Vector3 _oldScale;
    public bool TryStore(InventoryItem item)
    {
        if (!CanStore(item))
            return false;

        _oldScale = item.transform.localScale;
        item.transform.localScale = item.transform.localScale * scaleFactor;//Vector3.Lerp(item.transform.localScale, Vector3.one * scaleFactor, 0.8f);
        item.Store(this);
        
        storedItem = item;

        return true;

    }

    private bool CanStore(InventoryItem item)
    {
        return storedItem == null && item.slotType == slotType && !item.stored;
    }

    public InventoryItem RetrieveItem()
    {
        InventoryItem _item = storedItem;
        storedItem = null;

        _item.Release();
        _item.transform.localScale = _oldScale;

        return _item;
    }


}



public enum SlotType
{
    SMALL,
    LARGE,
}
