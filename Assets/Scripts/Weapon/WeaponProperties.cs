using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_n_Propeties", menuName = "3DSci-FiShooter/WeaponProperties")]
public sealed class WeaponProperties : WeaponPropertiesBase
{
    [SerializeField] private int m_EnergyUsage;
    public int EnergyUsage => m_EnergyUsage;

    [SerializeField] private int m_EnergyAmoutToStartFire;
    public int EnergyAmoutToStartFire => m_EnergyAmoutToStartFire;

    [SerializeField] private int m_EnergyRegenPerSecond;
    public float EnergyRegenPerSecond => m_EnergyRegenPerSecond; 
    
    [SerializeField] private float m_SpreadShootRange;
    public float SpreadShootRange => m_SpreadShootRange;

    [SerializeField] private float m_SpreadShootDistanceFactor = 0.1f;
    public float SpreadShootDistanceFactor => m_SpreadShootDistanceFactor;

    [SerializeField] private AudioClip m_LaunchSFX;
    public AudioClip LaunchSFX => m_LaunchSFX;

    //[SerializeField] private int m_AmmoUsage;
    //public int AmmoUsage => m_AmmoUsage;
}
