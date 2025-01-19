using UnityEngine;

public enum WeaponMode
{
    Primary,
    Secondary
}

public abstract class WeaponPropertiesBase : ScriptableObject
{
    [SerializeField] private WeaponMode m_Mode;
    [SerializeField] private Projectile m_ProjectilePrefab;
    [SerializeField] private float m_RateOfFire;

    public WeaponMode Mode => m_Mode;
    public Projectile ProjectilePrefab => m_ProjectilePrefab;
    public float RateOfFire => m_RateOfFire;
}
