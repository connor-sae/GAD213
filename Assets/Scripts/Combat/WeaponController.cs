using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{


    bool fireHeld;
    bool firing;

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

        do
        {
            firing = true;
            int burstCount = 0;

            while (burstCount < _data.burstSize) // fire all bullets in a burst
            {
                burstCount++;
                OnFireSingle();

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

    
    private void OnFireSingle()
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
