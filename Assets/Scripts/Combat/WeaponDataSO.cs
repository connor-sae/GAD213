using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Combat/Weapon Data")]
public class WeaponDataSO : ScriptableObject
{
    public new string name;

    [Header("Assignables")]

    public GameObject bulletBrefab;

    [Header("Ammo")]

    public int maxAmmo;

    [Header("Firing")]

    
    public float damage;
    public float spoolupTime = 0;
    public float fireRate = 1;
    public bool automatic = false;
    public float spreadAngle;

    [Header("Raycast")]
    
    public bool isRaycast;
    public float raycastrange;

    [Header("Burst")]

    public int burstSize = 1;
    public int burstDelay = 0;


}
