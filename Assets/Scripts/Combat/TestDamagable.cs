using UnityEngine;

public class TestDamagable : MonoBehaviour, IDamagable
{
    public void Damage(float damage)
    {
        GetComponent<Rigidbody>().AddForce((transform.position - FindAnyObjectByType<WeaponController>().transform.position).normalized * 200, ForceMode.Impulse);
    }
}
