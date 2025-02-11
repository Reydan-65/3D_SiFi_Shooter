using UnityEngine;

public class VehicleTurret : Weapon
{
    [SerializeField] private ThirdPersonCamera m_Camera;
    [SerializeField] private Transform m_ArmTransform;
    [SerializeField] private Transform m_LaserTransform;

    protected override void FixedUpdate()
    {
        if (m_RefireTimer > 0)
            m_RefireTimer -= Time.deltaTime;

        m_ArmTransform.rotation = Quaternion.Euler(0, m_Camera.transform.eulerAngles.y -90, 0);
        m_LaserTransform.rotation = Quaternion.Euler(m_Camera.transform.eulerAngles.z, m_ArmTransform.eulerAngles.y, -m_Camera.transform.eulerAngles.x);
        
        UpdateEnergy();
    }
}
