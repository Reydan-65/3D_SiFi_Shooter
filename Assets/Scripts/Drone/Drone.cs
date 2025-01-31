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
    [SerializeField] private Weapon[] turrets;

    [Header("View")]
    [SerializeField] private GameObject[] meshComponents;
    [SerializeField] private Renderer[] meshRenderers;
    [SerializeField] private Material[] deadMaterials;

    [Header("Movement")]
    [SerializeField] private float hoverAmplitudeY = 1.0f;
    [SerializeField] private float hoverSpeedY = 1.0f;
    //[SerializeField] private float hoverAmplitudeZ = 1.0f;
    //[SerializeField] private float hoverSpeedZ = 1.0f;
    //[SerializeField] private float rotationSpeed = 2.0f;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationFactor;

    //private bool isMovingForward = true;
    private float phaseOffsetY;
    private float phaseOffsetZ;

    private CubeArea area;
    private Vector3 moveToPoint;
    //private bool isOnPosition = false;

    public Transform MainMesh => mainMesh;

    protected override void Start()
    {
        base.Start();

        phaseOffsetY = Random.Range(0f, 2f * Mathf.PI);
        phaseOffsetZ = Random.Range(0f, 2f * Mathf.PI);
        movementSpeed = Random.Range(2f, 5f);

        area = FindAnyObjectByType<CubeArea>();

        moveToPoint = area.GetRandomInsideZone();

        float initialRotationY = Random.Range(0f, 360f);
        mainMesh.rotation = Quaternion.Euler(0, initialRotationY, 0);
    }

    private void Update()
    {
        float timeWithPhaseY = Time.time + phaseOffsetY;
        float timeWithPhaseZ = Time.time + phaseOffsetZ;

        //if (type == DroneType.Idle)
        //{
            Hover(timeWithPhaseY);
        //}

        //if (type == DroneType.Patrol)
        //{
        //    Vector3 newPosition = new Vector3(
        //    0,
        //    Mathf.Sin(timeWithPhaseY * hoverAmplitudeY) * hoverSpeedY * Time.deltaTime,
        //    Mathf.Sin(timeWithPhaseZ * hoverAmplitudeZ) * hoverSpeedZ * movementSpeed * Time.deltaTime
        //);

        //    // Проверка направления движения
        //    if (newPosition.z > 0 && !isMovingForward)
        //    {
        //        isMovingForward = true;
        //    }
        //    else if (newPosition.z < 0 && isMovingForward)
        //    {
        //        isMovingForward = false;
        //    }

        //    mainMesh.position += newPosition;

        //    // Определение направления движения
        //    Vector3 direction = isMovingForward ? new Vector3(0, 0, 1) : new Vector3(0, 0, -1);

        //    // Плавный поворот в направлении движения
        //    Quaternion targetRotation = Quaternion.LookRotation(-direction);
        //    mainMesh.rotation = Quaternion.Slerp(mainMesh.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        //}

        //if (type == DroneType.MoveToPoint)
        //{
        //    if (Vector3.Distance(mainMesh.position, moveToPoint) < 0.01) isOnPosition = true;

        //    if (isOnPosition)
        //    {
        //        mainMesh.rotation = Quaternion.Slerp(mainMesh.rotation, Quaternion.Euler(0, mainMesh.eulerAngles.y, 0), rotationSpeed * Time.deltaTime);
        //        mainMesh.position += new Vector3(0, Mathf.Sin(timeWithPhaseY * hoverAmplitudeY) * hoverSpeedY * Time.deltaTime, 0);
        //    }
        //    else
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(mainMesh.position - moveToPoint, transform.up);
        //        mainMesh.rotation = Quaternion.Slerp(mainMesh.rotation, targetRotation, rotationSpeed * 5 * Time.deltaTime);
        //        mainMesh.position = Vector3.MoveTowards(mainMesh.position, moveToPoint, Time.deltaTime * movementSpeed);
        //    }
        //}
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

    private void Hover(float time)
    {
        mainMesh.position += new Vector3(0, Mathf.Sin(time * hoverAmplitudeY) * hoverSpeedY * Time.deltaTime, 0);
    }

    // Public API
    public void LookAt(Vector3 target)
    {
        mainMesh.rotation = Quaternion.RotateTowards(mainMesh.rotation, Quaternion.LookRotation(mainMesh.position - target, transform.up), rotationFactor * Time.deltaTime);
    }

    public void MoveAt(Vector3 target)
    {
        transform.position = Vector3.MoveTowards(transform.position, target, movementSpeed * Time.deltaTime);
    }

    public void Fire(Vector3 target)
    {
        for (int i = 0; i < turrets.Length; i++)
        {
            turrets[i].FirePointLookAt(target);
            turrets[i].Fire();
        }
    }
}
