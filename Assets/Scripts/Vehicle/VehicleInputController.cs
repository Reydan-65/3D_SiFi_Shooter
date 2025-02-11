using UnityEngine;

public class VehicleInputController : MonoBehaviour
{
    [SerializeField] private Vehicle m_Vehicle;
    [SerializeField] private VehicleShooter m_VehicleShooter;
    [SerializeField] private ThirdPersonCamera m_Camera;
    [SerializeField] private Vector3 m_CameraOffset;

    protected virtual void Start()
    {
        if (m_Camera != null)
        {
            m_Camera.IsRotateTarget = false;
        }
    }

    protected virtual void Update()
    {
        m_Vehicle.SetTargetControl(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Jump"), Input.GetAxis("Vertical")));
        if (m_Camera != null)
            m_Camera.RotationControl = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Shoot
        if (Input.GetMouseButton(0))
        {
            m_VehicleShooter.Shoot();
        }
    }

    public void AssingCamera(ThirdPersonCamera camera)
    {
        m_Camera = camera;
        m_Camera.IsRotateTarget = false;
        m_Camera.SetTargetOffset(m_CameraOffset);
        m_Camera.SetTarget(m_Vehicle.transform);
    }
}
