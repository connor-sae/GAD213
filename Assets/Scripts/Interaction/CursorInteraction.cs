using UnityEngine;

public class CursorInteraction : MonoBehaviour
{
    private Vector2 _mousePos;

    [SerializeField] private Transform cursorAnchor;

    [SerializeField] private float maxGrabDistance = 5f;
    [SerializeField] private float targetHoldDistance = 2f;
    [SerializeField] private float holdDistanceLerpSpeed = 5f;
    [SerializeField] private float inventoryInteractUpscale = 1.2f;
    private Grabable _heldItem;
    private Grabable _hoverItem;
    private InventorySlot _hoverSlot;
    private Vector3 _hoverPos;

    #region Enable / Disable
    void OnEnable()
    {
        InputManager.OnLookChanged += OnLookChanged;
        InputManager.OnInteractStart += OnGrab;
        InputManager.OnInteractEnd += OnRelease;
    }
    void OnDisable()
    {
        InputManager.OnLookChanged -= OnLookChanged;
        InputManager.OnInteractStart -= OnGrab;
        InputManager.OnInteractEnd -= OnRelease;
    }
    #endregion

    private void OnLookChanged(Vector2 newMousePos)
    {
        _mousePos = newMousePos;
    }

    private void OnGrab()
    {
        if (_hoverItem != null)
            Grab(_hoverItem, _hoverPos);

        if (_hoverSlot != null)
        {
            storingItem = true;
            _heldItem = _hoverSlot.storedItem.GetComponent<Grabable>();
            _heldItem.transform.localScale *= inventoryInteractUpscale;
        }    
    }
    
    private void Grab(Grabable item, Vector3 _grabPos)
    {
        _currentHoldDistance = Vector3.Distance(Camera.main.transform.position, _hoverPos);

        _heldItem = item;
        _heldItem.OnGrabbed(cursorAnchor, _grabPos);
    }


    private void OnRelease()
    {
        _heldItem?.OnReleased();
        if (storingItem)
        {
            storingItem = false;

            _heldItem.transform.localScale /= inventoryInteractUpscale;
            _hoverItem = null;
        }
        
        _heldItem = null;
    }

    private float _currentHoldDistance;
    private bool storingItem;
    private InventorySlot _oldSlot;

    void Update()
    {
        //smooth Hold distance
        _currentHoldDistance = Mathf.Lerp(_currentHoldDistance, targetHoldDistance, holdDistanceLerpSpeed * Time.deltaTime);


        Ray mouseRay = Camera.main.ScreenPointToRay(_mousePos);
        Vector3 newCursorAnchorPos = mouseRay.GetPoint(_currentHoldDistance);

        cursorAnchor.position = newCursorAnchorPos;

        Vector3 _rayOrigin = Camera.main.transform.position;
        Vector3 _rayDirection = cursorAnchor.position - _rayOrigin;
        _rayDirection.Normalize();

        _hoverItem = null;
        _oldSlot = _hoverSlot;
        _hoverSlot = null;

        if (Physics.Raycast(Camera.main.transform.position, _rayDirection, out RaycastHit hit, maxGrabDistance))
        {
            if (hit.collider.TryGetComponent(out Grabable grabable)) // if hit grabable
            {
                _hoverItem = grabable;
                _hoverPos = hit.point;
            }

            if (hit.collider.TryGetComponent(out InventorySlot slot)) // if hit inventory Slot
            {
                _hoverSlot = slot;
                    
                if (!storingItem || _oldSlot != _hoverSlot) // just started storing
                {

                    if (_heldItem != null && _heldItem.TryGetComponent(out InventoryItem item)) // is storable
                    {

                        if (slot.TryStore(item)) // if successfully stored
                        {
                            _heldItem?.OnReleased();
                            storingItem = true;

                            item.transform.localScale *= inventoryInteractUpscale;
                        }
                    }
                }

            }
        }
        if ( _hoverSlot == null && storingItem)
        {
            UnStoreHeldItem();
        }
    }
    
    private void UnStoreHeldItem()
    {
        storingItem = false;

        //_hoverPos = _heldItem.transform.position;
        InventoryItem item = _oldSlot.RetrieveItem();

        float grabHeight = item.GetComponent<Collider>().bounds.extents.z  * item.transform.localScale.y   * item.transform.localScale.y ; // grab slightly higher        
        item.transform.position = cursorAnchor.position - Vector3.up * grabHeight;
        _hoverPos = item.transform.position;
        Grab(item.GetComponent<Grabable>(), item.transform.position + Vector3.up * grabHeight);

        _hoverSlot = null;
    }
}
