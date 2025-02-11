using UnityEngine;

public class Weapon : WeaponBase
{
    private const float AIMING_OFFSET = 15.0f;

    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private ParticleSystem targetMuzzleFlashParticles;

    protected float m_RefireTimer;
    public bool CanFire => m_RefireTimer <= 0 && EnergyIsRestored == true;

    [SerializeField] protected Transform firePoint;
    [SerializeField] private float primaryMaxEnergy;

    private float primaryEnergy;

    private bool EnergyIsRestored;
    public bool ReadyToFire => EnergyIsRestored == true;

    public float PrimaryMaxEnergy => primaryMaxEnergy;
    public float PrimaryEnergy => primaryEnergy;
    public Transform FirePoint => firePoint;

    private Destructible owner;

    private AudioSource audioSource;

    private Quaternion baseRotation;

    private void Start()
    {
        primaryEnergy = primaryMaxEnergy;
        owner = transform.root.GetComponent<Destructible>();
        audioSource = GetComponent<AudioSource>();
        baseRotation = transform.localRotation;
    }

    protected override void FixedUpdate()
    {
        if (m_RefireTimer > 0)
            m_RefireTimer -= Time.deltaTime;

        if (targetCharacterMovement != null)
        {
            if (targetCharacterMovement.IsAiming && !targetCharacterMovement.IsCrouch && targetCharacterMovement.GetComponent<AIAlienSoldier>() == false)
            {
                transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + AIMING_OFFSET, transform.localRotation.z);
            }

            if (targetCharacterMovement.IsAiming && targetCharacterMovement.IsCrouch)
                transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + AIMING_OFFSET * 2, transform.localRotation.z);

            if (!targetCharacterMovement.IsAiming)
                transform.localRotation = Quaternion.Euler(baseRotation.x, baseRotation.y - 5, baseRotation.z + AIMING_OFFSET);

            if (targetCharacterMovement.transform.GetComponent<CharacterController>().velocity.magnitude > 0.01f && !targetCharacterMovement.IsAiming)
                transform.localRotation = Quaternion.Euler(baseRotation.x + 10, baseRotation.y + 10, baseRotation.z + 15);
        }

        UpdateEnergy();
    }

    public override void Fire()
    {
        if (!CanFire) return;

        //m_Drone = transform.root.GetComponent<Drone>();

        if (weaponProperties == null) return;
        if (m_RefireTimer > 0) return;

        if (TryDrawEnergy(weaponProperties.EnergyUsage) == false)
        {
            EnergyIsRestored = false;
            return;
        }

        //if (m_Drone.DrawAmmo(m_TurretProperties.AmmoUsage) == false) return;

        Projectile projectile = Instantiate(weaponProperties.ProjectilePrefab).GetComponent<Projectile>();
        projectile.transform.position = firePoint.position;
        projectile.transform.forward = firePoint.forward;
        projectile.SetParentShooter(owner);

        //if (m_Drone.TeamId == 0) projectile.Nickname = "Neutral Projectile";
        //if (m_Drone.TeamId == 1) projectile.Nickname = "Frendly Projectile";
        //if (m_Drone.TeamId == 2) projectile.Nickname = "Enemies Projectile";
        //if (m_Drone.TeamId == 3) projectile.Nickname = "Debris Projectile";

        m_RefireTimer = weaponProperties.RateOfFire;

        targetMuzzleFlashParticles.time = 0;
        targetMuzzleFlashParticles.Play();

        float pitch = Random.Range(0.975f, 1.025f);
        audioSource.clip = weaponProperties.LaunchSFX;
        audioSource.pitch = pitch;
        audioSource.Play();

        SoundEvent.EmitSound(transform.position, 20f);
    }

    public override void AssingLoadout(WeaponProperties properties)
    {
        if (m_Mode != properties.Mode) return;

        m_RefireTimer = 0;
        weaponProperties = properties;
    }

    public void FirePointLookAt(Vector3 position)
    {
        Vector3 offset = Random.insideUnitSphere * weaponProperties.SpreadShootRange;

        if (weaponProperties.SpreadShootDistanceFactor != 0)
        {
            offset = offset * Vector3.Distance(firePoint.position, position) * weaponProperties.SpreadShootDistanceFactor;
        }

        firePoint.LookAt(position + offset);
    }

    private bool TryDrawEnergy(int count)
    {
        if (count == 0) return true;

        if (primaryEnergy >= count)
        {
            primaryEnergy -= count;
            return true;
        }

        return false;
    }

    protected void UpdateEnergy()
    {
        primaryEnergy += (float)weaponProperties.EnergyRegenPerSecond * Time.deltaTime;
        primaryEnergy = Mathf.Clamp(primaryEnergy, 0, primaryMaxEnergy);

        if (primaryEnergy >= weaponProperties.EnergyAmoutToStartFire) EnergyIsRestored = true;
    }
}
