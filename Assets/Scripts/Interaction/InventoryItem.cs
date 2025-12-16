using UnityEngine;

//[RequireComponent(typeof(Collider))]
public class InventoryItem : MonoBehaviour
{
    public SlotType slotType;
    [Tooltip("anchor must be a child of this object")]
    public Transform storageAnchorPoint;
    public bool stored { get; private set; }


    public virtual void Store(InventorySlot slot, Transform anchorPoint = null)
    {
        stored = true;
        transform.parent = slot.storePoint;

        if(anchorPoint == null)
            anchorPoint = storageAnchorPoint;


        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if ((anchorPoint != null && anchorPoint.parent == transform)) // if somthing assigned and a child
        {
            transform.localRotation = Quaternion.Inverse(anchorPoint.localRotation);
            transform.position += transform.parent.position - anchorPoint.position;
        }
        GetComponent<Collider>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    public virtual void Release()
    {
        GetComponent<Collider>().enabled = true;
        GetComponent<Rigidbody>().isKinematic = false;

        stored = false;
        transform.parent = null;
    }


}
