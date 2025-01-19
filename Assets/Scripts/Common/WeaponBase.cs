using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] protected WeaponMode m_Mode;
    [SerializeField] protected WeaponProperties weaponProperties;

    public WeaponMode Mode => m_Mode;
    public WeaponProperties WeaponProperties => weaponProperties;

    protected virtual void FixedUpdate() { }

    // Public API
    public virtual void Fire() { }
    public virtual void AssingLoadout(WeaponProperties properties) { }
}
