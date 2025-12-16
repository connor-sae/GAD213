using UnityEngine;

public class Bullet : MonoBehaviour
{
    private WeaponDataSO data;
    private LayerMask hitableMask;
    private float timeToLive = Mathf.Infinity;

    private int hitsToLive = 1;

    public void Init(WeaponDataSO _data, LayerMask _hitableMask)
    {
        data = _data;
        hitableMask = _hitableMask;
        hitsToLive = _data.passthrough;

        if (data.bulletSpeed == 0)
        {
            Debug.LogWarning("bullet has 0 speed, and will never be destroyed!");

            timeToLive = Mathf.Infinity;
        }
        else
            timeToLive = data.range / data.bulletSpeed;
    }

    private void Update()
    {
        if (data == null) // not initialised
            return;

        if(timeToLive <= 0 || hitsToLive <= 0) // bullet at max range ro hits
        {
            Destroy(gameObject);
            return;
        }

        timeToLive -= Time.deltaTime;

        float moveDistance = data.bulletSpeed * Time.deltaTime;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, moveDistance, hitableMask); // hit something

        foreach (RaycastHit hit in hits)
        {
            OnHit(hit.collider);
            hitsToLive--;

            if(hitsToLive <= 0) // died
            {
                Destroy(gameObject);
                return;
            }
        }

        transform.position += transform.forward * moveDistance;

    }

    private void OnHit(Collider other)
    {
        if(other.TryGetComponent(out IDamagable damageble))
        {
            damageble.Damage(data.damage);
        }
    }
}
