using UnityEngine;

public class Weapon : WeaponBase
{
    private const float AIMING_OFFSET_Y = 15.0f;

    [SerializeField] private CharacterMovement targetCharacterMovement;
    [SerializeField] private ParticleSystem targetMuzzleFlashParticles;

    private float m_RefireTimer;
    public bool CanFire => m_RefireTimer <= 0 && EnergyIsRestored == true;

    [SerializeField] private Transform firePoint;
    [SerializeField] private float primaryMaxEnergy;

    private float primaryEnergy;

    private bool EnergyIsRestored;
    public bool ReadyToFire => EnergyIsRestored == true;

    public float PrimaryMaxEnergy => primaryMaxEnergy;
    public float PrimaryEnergy => primaryEnergy;
    public Transform FirePoint => firePoint;

    private Destructible owner;

    private AudioSource audioSource;

    private void Start()
    {
        primaryEnergy = primaryMaxEnergy;
        owner = transform.root.GetComponent<Destructible>();
        audioSource = GetComponent<AudioSource>();
    }

    protected override void FixedUpdate()
    {
        if (m_RefireTimer > 0)
            m_RefireTimer -= Time.deltaTime;

        if (targetCharacterMovement.IsAiming && !targetCharacterMovement.IsCrouch)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + AIMING_OFFSET_Y, transform.localRotation.z);
        }

        if (targetCharacterMovement.IsAiming && targetCharacterMovement.IsCrouch)
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, transform.localRotation.y + AIMING_OFFSET_Y * 2, transform.localRotation.z);
        

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
        audioSource.clip = weaponProperties.LaunchSFX;
        audioSource.Play();

        /// <summary>
        /// Звук выстрела.
        /// Звук слегка изменяется.
        /// </summary>

        //if (Mode == TurretMode.Primary && m_TurretProperties.ProjectilePrefab.GetComponent<SmallPlasmaProjectile>() == true)
        //    PlaySoundByIndex(0);
        //if (Mode == TurretMode.Primary && m_TurretProperties.ProjectilePrefab.GetComponent<PlasmaCannonProjectile>() == true)
        //    PlaySoundByIndex(1);
        //if (Mode == TurretMode.Secondary && m_TurretProperties.ProjectilePrefab.GetComponent<Missle>() == true)
        //    PlaySoundByIndex(2);
        //if (Mode == TurretMode.Secondary && m_TurretProperties.ProjectilePrefab.GetComponent<EMPBomb>() == true)
        //    PlaySoundByIndex(3);
    }

    public override void AssingLoadout(WeaponProperties properties)
    {
        if (m_Mode != properties.Mode) return;

        m_RefireTimer = 0;
        weaponProperties = properties;
    }

    private void PlaySoundByIndex(int index)
    {
        float pitch = Random.Range(0.95f, 1.05f);

        transform.root.GetComponent<AudioSource>().pitch = pitch;

        //SoundManager.Instance.PlayOneShot(SoundManager.Instance.AudioProperties.ProjectileLaunchClips, index,
        //             transform.root.GetComponent<AudioSource>(), SoundManager.Instance.AudioProperties.SoundsVolume);
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

    private void UpdateEnergy()
    {
        primaryEnergy += (float)weaponProperties.EnergyRegenPerSecond * Time.deltaTime;
        primaryEnergy = Mathf.Clamp(primaryEnergy, 0, primaryMaxEnergy);

        if (primaryEnergy >= weaponProperties.EnergyAmoutToStartFire) EnergyIsRestored = true;
    }
}
