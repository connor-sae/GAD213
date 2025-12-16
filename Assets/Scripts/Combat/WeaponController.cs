using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponController : MonoBehaviour
{


    bool fireHeld;
    bool firing;
    Vector2 _mousePos;

    [SerializeField] WeaponSlot weaponSlot;
    [SerializeField] LayerMask targetableMask;
    [SerializeField] float targetRaycastDistance = 100f;


    #region Enable / Disable
    void OnEnable()
    {
        InputManager.OnLookChanged += OnLookChanged;
        InputManager.OnInteractStart += FireStart;
        InputManager.OnInteractEnd += FireEnd;
    }
    void OnDisable()
    {
        InputManager.OnLookChanged -= OnLookChanged;
        InputManager.OnInteractStart -= FireStart;
        InputManager.OnInteractEnd -= FireEnd;
    }
    #endregion

    public void OnLookChanged(Vector2 newMousePos)
    {
        _mousePos = newMousePos;
    }

    [SerializeField] WeaponItem defaultWeapon;

    public WeaponItem ActiveWeapon { 
        get
        {
            if (weaponSlot == null)
            {
                Debug.LogWarning("weaponSlot is not Assigned!");
                return defaultWeapon;
            }
            if (weaponSlot.storedItem is WeaponItem)
                return weaponSlot.storedItem as WeaponItem;
            else
            {
                if (defaultWeapon == null)
                {
                    Debug.LogWarning("No active or fallback weapon Data Found, Is any assigned?");
                }
                return defaultWeapon;
            }
        }
    }




    #region Firing

    public void FireStart()
    {
        fireHeld = ActiveWeapon.data.automatic;

        if (!firing) // do not fire if still firing
            if (ActiveWeapon.currentAmmo <= 0)
            {
                //out of ammo
                //play click sound / prompt reload
            }
            else
                StartCoroutine(FireRoutine());
    }

    public void FireEnd()
    {
        fireHeld = false;
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

                FireSingle(_data);

                if(_data.fireRate <= 0) // catch if firerate will throw error
                {
                    Debug.LogWarning($"FireRate for {_data.name} must be a positive value!!");
                    _data.fireRate = 0.1f;
                }

                if (_data.countBurstAmmo)
                    ActiveWeapon.currentAmmo -= _data.ammoConsumption;

                yield return new WaitForSeconds(1/_data.fireRate);
            }

            if (!_data.countBurstAmmo)
                ActiveWeapon.currentAmmo -= _data.ammoConsumption;

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

    
    private void FireSingle(WeaponDataSO _data)
    {
        

        if(ActiveWeapon.currentAmmo <= 0)
        {
            Debug.Log("Weapon is Empty");
            //play click sound?
            return;
        }

        //////////target position
        Vector3 targetPos;

        Ray ray = Camera.main.ScreenPointToRay(_mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, targetRaycastDistance, targetableMask))
        {
            targetPos = hit.point;
        }
        else
            targetPos = ray.GetPoint(targetRaycastDistance);


        ///////////spread Offset
        float _spread = _data.spreadAngle;

        float _xAngle = Random.Range(-_spread, _spread);
        float _yAngle = 0;
        float _zAngle = Random.Range(-_spread, _spread);

        Quaternion spreadRotation = Quaternion.Euler(_xAngle, _yAngle, _zAngle);


        if (_data.isRaycast)
            FireRay(_data, targetPos, spreadRotation);
        else
            FireObject(_data, targetPos, spreadRotation);

        if (ActiveWeapon.weaponAnimator != null)
            ActiveWeapon.weaponAnimator.SetTrigger("fire");


    }

    private void FireRay(WeaponDataSO _data, Vector3 _target, Quaternion spreadRotationOffset)
    {
        Vector3 _origin = Camera.main.transform.position;
        Vector3 _direction = _target - _origin;
        _direction.Normalize();

        _direction = spreadRotationOffset * _direction; // add spread

        RaycastHit[] hits = Physics.SphereCastAll(_origin, _data.castRaduis, _direction, _data.range, targetableMask);

        for (int i = 0; i < _data.passthrough; i++) // hit as many enemies as passthrough allows
        {
            if (i >= hits.Length)
                break;

            OnHit(hits[i].collider, _data.damage);
        }
    }

    private void OnHit(Collider other, float damage)
    {
        Debug.Log("hit: " + other.name);
        if (other.TryGetComponent(out IDamagable damageble))
        {
            damageble.Damage(damage);
        }
    }


    private void FireObject(WeaponDataSO _data, Vector3 target, Quaternion spreadRotationOffset)
    {
        Bullet _bullet = _data.bulletBrefab;
        _bullet.Init(_data, targetableMask);

        if (_bullet == null)
        {
            Debug.LogError($"Weapon data: {ActiveWeapon.data.name} does not have a bullet prefab assigned");
            return;
        }

        Transform _shotPoint = ActiveWeapon.bulletShootPoint;

        Vector3 direction = target - ActiveWeapon.bulletShootPoint.position;
        direction.Normalize();

        Quaternion _newRotation = Quaternion.LookRotation(direction, Vector3.up) * spreadRotationOffset;


        Instantiate(_bullet, _shotPoint.position, _newRotation);
    }

    #endregion

}
