using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InventoryItem : MonoBehaviour
{
    public SlotType slotType;
    public bool stored { get; private set; }


    public void Store(InventorySlot slot)
    {
        stored = true;
        transform.parent = slot.storePoint;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public void Release()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

        Debug.Log("Released " + name);
        stored = false;

        transform.parent = null;
    }


}
