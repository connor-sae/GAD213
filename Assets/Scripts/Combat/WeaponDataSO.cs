using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Combat/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    public new string name;

    [Header("Ammo")]

    public int maxAmmo;
    public bool countBurstAmmo = true;
    public int ammoConsumption = 1;

    [Header("Firing")]

    
    public float damage;
    public float spoolupTime = 0;
    public float fireRate = 1;
    public bool automatic = false;
    public float spreadAngle;
    public float range = 10f;
    public int passthrough = 1;

    [Header("Raycast")]
    
    public bool isRaycast;
    public float castRaduis = 0.1f;

    [Header("Object")]

    public Bullet bulletBrefab;
    public float bulletSpeed = 1f;

    [Header("Burst")]

    public int burstSize = 1;
    public int burstDelay = 0;


}
