using UnityEngine;

public class CursorInteraction : MonoBehaviour
{
    private Vector2 _mousePos;

    [SerializeField] private Transform cursorAnchor;

    [SerializeField] private float maxGrabDistance = 5f;
    [SerializeField] private float targetHoldDistance = 2f;
    [SerializeField] private float holdDistanceLerpSpeed = 5f;
    [SerializeField] private float inventoryInteractUpscale = 1.2f;
    private Grabable _heldGrabable;
    private Grabable _hoverGrabable;
    private InventorySlot _hoverSlot;
    private Vector3 _hoverPos;

    public Animator cursorAnimator;

    #region Enable / Disable
    void OnEnable()
    {
        InputManager.OnLookChanged += OnLookChanged;
        InputManager.OnInteractStart += OnGrabPressed;
        InputManager.OnInteractEnd += OnRelease;
        Cursor.visible = false;
    }
    void OnDisable()
    {
        InputManager.OnLookChanged -= OnLookChanged;
        InputManager.OnInteractStart -= OnGrabPressed;
        InputManager.OnInteractEnd -= OnRelease;
        Cursor.visible = true;
    }
    #endregion

    private void OnLookChanged(Vector2 newMousePos)
    {
        _mousePos = newMousePos;
    }

    private void OnGrabPressed()
    {
        if (_hoverGrabable != null)
            Grab(_hoverGrabable, _hoverPos);

        if (_hoverSlot != null)
        {
            storingItem = true;
            _heldGrabable = _hoverSlot.storedItem.GetComponent<Grabable>();
            _heldGrabable.transform.localScale *= inventoryInteractUpscale;
            cursorAnimator.SetBool("holding", true);
        }    
    }
    
    private void Grab(Grabable item, Vector3 _grabPos)
    {
        _currentHoldDistance = Vector3.Distance(Camera.main.transform.position, _hoverPos);

        _heldGrabable = item;
        _heldGrabable.OnGrabbed(cursorAnchor, _grabPos);
        cursorAnimator.SetBool("holding", true );
    }


    private void OnRelease()
    {
        _heldGrabable?.OnReleased();
        if (storingItem)
        {
            storingItem = false;

            _heldGrabable.transform.localScale /= inventoryInteractUpscale;
            _hoverGrabable = null;
        }
        
        _heldGrabable = null;

        cursorAnimator.SetBool("holding", false);
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
        cursorAnchor.rotation = transform.rotation;

        Vector3 _rayOrigin = Camera.main.transform.position;
        Vector3 _rayDirection = cursorAnchor.position - _rayOrigin;
        _rayDirection.Normalize();

        _hoverGrabable = null;

        if (Physics.Raycast(Camera.main.transform.position, _rayDirection, out RaycastHit hit, maxGrabDistance))
        {
            if (hit.collider.TryGetComponent(out Grabable grabable)) // if hit grabable
            {
                _hoverGrabable = grabable;
                _hoverPos = hit.point;
            }

            _hoverSlot = hit.collider.GetComponent<InventorySlot>();

            if (_hoverSlot != null) // if hit inventory Slot
            {
                if (_oldSlot != _hoverSlot) // just started storing
                {

                    if (_heldGrabable != null && _heldGrabable.TryGetComponent(out InventoryItem item)) // is storable
                    {
                        if (item.stored)
                            UnStoreItem(_oldSlot);
                        StoreItem(item, _hoverSlot);
                    }
                }

            }
        }
        else
            _hoverSlot = null;

        if ( _hoverSlot == null && storingItem)
        {
            UnStoreItem(_oldSlot);
        }


        _oldSlot = _hoverSlot;
        cursorAnimator.SetBool("hovering", _hoverGrabable != null || (_hoverSlot != null && _hoverSlot.storedItem != null));
    }

    private void StoreItem(InventoryItem item, InventorySlot slot)
    {
        if (slot.TryStore(item)) // if successfully stored
        {
            _heldGrabable?.OnReleased();
            storingItem = true;

            item.transform.localScale *= inventoryInteractUpscale;
        }
    }
    
    private void UnStoreItem(InventorySlot slot)
    {
        storingItem = false;

        //_hoverPos = _heldItem.transform.position;
        InventoryItem item = slot.RetrieveItem();

        float grabHeight = item.GetComponent<Collider>().bounds.extents.z  * item.transform.localScale.y   * item.transform.localScale.y ; // grab slightly higher        
        item.transform.position = cursorAnchor.position - Vector3.up * grabHeight;
        _hoverPos = item.transform.position;
        Grab(item.GetComponent<Grabable>(), item.transform.position + Vector3.up * grabHeight);

        _hoverSlot = null;
    }
}
