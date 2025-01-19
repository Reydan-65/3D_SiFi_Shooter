using UnityEngine;

public class Drone : Destructible
{
    public enum DroneType
    {
        Null,
        Idle,
        Patrol,
        MoveToPoint
    };
    [SerializeField] private DroneType type;

    [Header("Main")]
    [SerializeField] private Transform mainMesh;

    [Header("View")]
    [SerializeField] private GameObject[] meshComponents;
    [SerializeField] private Renderer[] meshRenderers;
    [SerializeField] private Material[] deadMaterials;

    [Header("Movement")]
    [SerializeField] float hoverAmplitudeY = 1.0f;
    [SerializeField] float hoverSpeedY = 1.0f;
    [SerializeField] float hoverAmplitudeZ = 1.0f;
    [SerializeField] float hoverSpeedZ = 1.0f;
    [SerializeField] float rotationSpeed = 2.0f;
    [SerializeField] float moveSpeed;

    private bool isMovingForward = true;
    private float phaseOffsetY;
    private float phaseOffsetZ;

    private CubeArea area;
    private Vector3 moveToPoint;
    private bool isOnPosition = false;

    //[Space(10)]
    //[SerializeField] private float m_Mass;
    //[SerializeField] private float m_Thrust;
    //[SerializeField] private float m_Mobility;
    //[SerializeField] private float m_MaxLinearVelocity;
    //[SerializeField] private float m_MaxAngularVelocity;

    //public float Thrust { get => m_Thrust; set => m_Thrust = value; }
    //public float Mobility { get => m_Mobility; set => m_Mobility = value; }
    //public float MaxLinearVelocity { get => m_MaxLinearVelocity; set => m_MaxLinearVelocity = value; }
    //public float MaxAngularVelocity => m_MaxAngularVelocity;

    #region Public API

    /// <summary>
    /// ”правление линейной т€гой. -1.0 до +1.0
    /// </summary>
    public float ThrustControl { get; set; }

    /// <summary>
    /// ”правление вращательной т€гой. -1.0 до +1.0
    /// </summary>
    public float TorqueControl { get; set; }

    #endregion

    protected override void Start()
    {
        base.Start();

        phaseOffsetY = Random.Range(0f, 2f * Mathf.PI);
        phaseOffsetZ = Random.Range(0f, 2f * Mathf.PI);
        moveSpeed = Random.Range(2f, 5f);

        area = FindAnyObjectByType<CubeArea>();

        moveToPoint = area.GetRandomInsideZone();

        float initialRotationY = Random.Range(0f, 360f);
        mainMesh.rotation = Quaternion.Euler(0, initialRotationY, 0);
    }

    private void Update()
    {
        float timeWithPhaseY = Time.time + phaseOffsetY;
        float timeWithPhaseZ = Time.time + phaseOffsetZ;

        if (type == DroneType.Idle)
        {
            mainMesh.position += new Vector3(0, Mathf.Sin(timeWithPhaseY * hoverAmplitudeY) * hoverSpeedY * Time.deltaTime, 0);
        }

        if (type == DroneType.Patrol)
        {
            Vector3 newPosition = new Vector3(
            0,
            Mathf.Sin(timeWithPhaseY * hoverAmplitudeY) * hoverSpeedY * Time.deltaTime,
            Mathf.Sin(timeWithPhaseZ * hoverAmplitudeZ) * hoverSpeedZ * moveSpeed * Time.deltaTime
        );

            // ѕроверка направлени€ движени€
            if (newPosition.z > 0 && !isMovingForward)
            {
                isMovingForward = true;
            }
            else if (newPosition.z < 0 && isMovingForward)
            {
                isMovingForward = false;
            }

            mainMesh.position += newPosition;

            // ќпределение направлени€ движени€
            Vector3 direction = isMovingForward ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);

            // ѕлавный поворот в направлении движени€
            Quaternion targetRotation = Quaternion.LookRotation(-direction);
            mainMesh.rotation = Quaternion.Slerp(mainMesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (type == DroneType.MoveToPoint)
        {
            if (Vector3.Distance(mainMesh.position, moveToPoint) < 0.01) isOnPosition = true;

            if (isOnPosition)
            {
                mainMesh.rotation = Quaternion.Slerp(mainMesh.rotation, Quaternion.Euler(0, mainMesh.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);
                mainMesh.position += new Vector3(0, Mathf.Sin(timeWithPhaseY * hoverAmplitudeY) * hoverSpeedY * Time.deltaTime, 0);
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(mainMesh.position - moveToPoint, transform.up);
                mainMesh.rotation = Quaternion.Slerp(mainMesh.rotation, targetRotation, rotationSpeed * 5 * Time.deltaTime);
                mainMesh.position = Vector3.MoveTowards(mainMesh.position, moveToPoint, Time.deltaTime * moveSpeed);
            }
        }
    }

    protected override void OnDeath()
    {
        m_EventOnDeath?.Invoke();
        enabled = false;

        for (int i = 0; i < meshComponents.Length; i++)
        {
            if (meshComponents[i] != null)
            {
                if (meshComponents[i].GetComponent<Rigidbody>() == null)
                    meshComponents[i].AddComponent<Rigidbody>();
            }
        }

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (meshComponents[i] != null)
            {
                meshRenderers[i].material = deadMaterials[i];
            }
        }
    }
}
