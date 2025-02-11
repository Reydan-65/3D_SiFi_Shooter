using UnityEngine;

[System.Serializable]
public class WheelAxle
{
    [SerializeField] private WheelCollider m_LeftWheelCollider;
    [SerializeField] private WheelCollider m_RightWheelCollider;
    [SerializeField] private Transform m_LeftWheelMesh;
    [SerializeField] private Transform m_RightWheelMesh;

    [SerializeField] private bool m_Motor;
    public bool Motor => m_Motor;

    [SerializeField] private bool m_Steering;
    public bool Steering => m_Steering;

    private float currentSteerAngle;
    private float currentTorque;

    public void SetTorque(float torque)
    {
        if (m_Motor == false) return;

        m_LeftWheelCollider.motorTorque = torque;
        m_RightWheelCollider.motorTorque = torque;
    }

    public float GetTorque()
    {
        return currentTorque;
    }

    public void SetBrakeTorque(float brakeTorque)
    {
        m_LeftWheelCollider.brakeTorque = brakeTorque;
        m_RightWheelCollider.brakeTorque = brakeTorque;
    }

    public void SetSteerAngle(float angle)
    {
        if (m_Steering == false) return;

        currentSteerAngle = angle;
        m_LeftWheelCollider.steerAngle = currentSteerAngle;
        m_RightWheelCollider.steerAngle = currentSteerAngle;
    }

    public float GetSteerAngle()
    {
        return currentSteerAngle;
    }

    public void UpdateMeshTransform()
    {
        UpdateWheelTransform(m_LeftWheelCollider, ref m_LeftWheelMesh);
        UpdateWheelTransform(m_RightWheelCollider, ref m_RightWheelMesh);
    }

    private void UpdateWheelTransform(WheelCollider wheelCollider, ref Transform wheelTransform)
    {
        Vector3 position;
        Quaternion rotation;
        wheelCollider.GetWorldPose(out position, out rotation);

        wheelTransform.position = position;
        wheelTransform.rotation = rotation;
    }
}

[RequireComponent(typeof(Rigidbody))]
public class WheeledVehicle : Vehicle
{
    [SerializeField] private WheelAxle[] m_WheelAxles;

    [SerializeField] private float m_MaxBrakeTorque;
    [SerializeField] private float m_MaxSteerAngle;

    private Rigidbody m_Rigidbody;

    public override float LinearVelocity => m_Rigidbody.velocity.magnitude;

    protected override void Start()
    {
        base.Start();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float targetMotor = m_MaxMotorTorque * TargetInputControl.z;
        float brakeTorque = m_MaxBrakeTorque * TargetInputControl.y;
        float targetSteerAngle = m_MaxSteerAngle * TargetInputControl.x;

        float steerSmoothness = 0.1f;
        float torqueSmoothness = 0.5f;

        for (int i = 0; i < m_WheelAxles.Length; i++)
        {
            if (brakeTorque == 0 && LinearVelocity < m_MaxLinearVelocity)
            {
                m_WheelAxles[i].SetBrakeTorque(0);

                float currentTorque = m_WheelAxles[i].GetTorque();
                float smoothedTorque = Mathf.Lerp(currentTorque, targetMotor, torqueSmoothness);
                m_WheelAxles[i].SetTorque(smoothedTorque);
            }

            if (LinearVelocity > m_MaxLinearVelocity)
            {
                m_WheelAxles[i].SetBrakeTorque(brakeTorque * 0.2f);
            }
            else
            {
                m_WheelAxles[i].SetBrakeTorque(brakeTorque);
            }

            if (m_WheelAxles[i].Steering)
            {
                float currentSteerAngle = m_WheelAxles[i].GetSteerAngle();
                float smoothedSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, steerSmoothness);
                m_WheelAxles[i].SetSteerAngle(smoothedSteerAngle);
            }

            m_WheelAxles[i].UpdateMeshTransform();
        }
    }
}
