using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{


    bool fireHeld;
    bool firing;
    Vector2 _mousePos;

    [SerializeField] Transform rayOrigin;
    [SerializeField] LayerMask targetableMask;


    #region Enable / Disable
    void OnEnable()
    {
        InputManager.OnLookChanged += OnLookChanged;
    }
    void OnDisable()
    {
        InputManager.OnLookChanged -= OnLookChanged;
    }
    #endregion

    public void OnLookChanged(Vector2 newMousePos)
    {
        _mousePos = newMousePos;
    }

    public WeaponItem ActiveWeapon { 
        get
        {
            if (_slot == null)
                return null;
            return _slot.storedItem as WeaponItem;
        }
    }

    private WeaponSlot _slot;

    void Awake()
    {
        _slot = GetComponent<WeaponSlot>();
    }


    private IEnumerator FireRoutine()
    {

        WeaponDataSO _data = ActiveWeapon.data;

        yield return StartCoroutine(SpoolingUp());
        if(!fireHeld && _data.automatic)
        {
            firing = false;
            yield break;
        }


        do
        {
            firing = true;
            int burstCount = 0;

            while (burstCount < _data.burstSize) // fire all bullets in a burst
            {
                burstCount++;

                if(_data.isRaycast)
                    OnFireSingleRay();
                else
                    OnFireSingleObject();

                if(_data.fireRate <= 0) // catch if firerate will throw error
                {
                    Debug.LogWarning($"FireRate for {_data.name} must be a posotive value!!");
                    _data.fireRate = 0.1f;
                }

                yield return new WaitForSeconds(1/_data.fireRate);
            }

            yield return new WaitForSeconds(_data.burstDelay);


        } while (fireHeld);

        firing = false;
    }

    private IEnumerator SpoolingUp()
    {
        float startTime = Time.time;
        while(Time.time - startTime <= ActiveWeapon.data.spoolupTime)
        {
            if(!fireHeld)
                yield break;
        }
        
    }

    
    private void OnFireSingleObject()
    {
        GameObject _bullet = ActiveWeapon.data.bulletBrefab;

        if (_bullet == null)
        {
            Debug.LogError($"Weapon data: {ActiveWeapon.data.name} does not have a bullet prefab assigned");
            return;
        }

        if(ActiveWeapon.currentAmmo <= 0)
        {
            Debug.Log("Weapon is Empty");
            //play click sound?
            return;
        }

        float _spread = ActiveWeapon.data.spreadAngle;

        float _xAngle = Random.Range(-_spread, _spread);
        float _yAngle = 0;
        float _zAngle = Random.Range(-_spread, _spread);

        Transform _shotPoint = ActiveWeapon.bulletShootPoint;

        Quaternion _newRotation = Quaternion.Euler(_xAngle, _yAngle, _zAngle);

        
        Instantiate(_bullet, _shotPoint.position, _newRotation);
    }

    private void OnFireSingleRay()
    {
        Debug.Log("fire Ray");
    }

    public void FireStart()
    {
        fireHeld = ActiveWeapon.data.automatic;

        if(!firing) // do not fire if still firing
            if(ActiveWeapon.currentAmmo <= 0)
            {
                //out of ammo
                //play click sound / prompt reload
            }else
                StartCoroutine(FireRoutine());
    }

    public void FireEnd()
    {
        fireHeld = false;
    }
}
