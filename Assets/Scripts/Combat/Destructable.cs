using UnityEngine;

public class Destructable : MonoBehaviour, IDamagable
{
    public void Damage(float damage)
    {
        Destroy(gameObject);
    }
}
