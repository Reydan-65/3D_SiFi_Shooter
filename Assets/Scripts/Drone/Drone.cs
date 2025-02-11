using UnityEngine;

public class Drone : Destructible
{
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
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationFactor;

    private float phaseOffsetY;
    private float baseMovementSpeed;

    public Transform MainMesh => mainMesh;

    protected override void Start()
    {
        base.Start();

        phaseOffsetY = Random.Range(0f, 2f * Mathf.PI);
        movementSpeed = Random.Range(2f, 5f);
        baseMovementSpeed = movementSpeed;

        float initialRotationY = Random.Range(0f, 360f);
        mainMesh.rotation = Quaternion.Euler(0, initialRotationY, 0);
    }

    private void Update()
    {
        float timeWithPhaseY = Time.time + phaseOffsetY;

        if (movementSpeed == 0)
            Hover(timeWithPhaseY);
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
        mainMesh.rotation = Quaternion.RotateTowards(mainMesh.rotation, Quaternion.LookRotation(target - mainMesh.position, transform.up), rotationFactor * Time.deltaTime);
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

    public void StartMoving()
    {
        movementSpeed = baseMovementSpeed;
    }

    public void StopMoving()
    {
        movementSpeed = 0;
    }
}
