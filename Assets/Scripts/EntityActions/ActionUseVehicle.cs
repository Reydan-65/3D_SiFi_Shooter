using UnityEngine;

[System.Serializable]
public class ActionUseVehicleProperties : ActionInteractProperties
{
    public Vehicle m_Vehicle;
    public VehicleInputController m_VehicleInputController;
    public AudioSource m_VehicleAudioSource;
    public Transform[] m_InteractTransforms;
    public VehicleTurret m_VehicleTurret;
    public VehicleShooter m_VehicleShooter;
    public GameObject m_VehicleHeadlights;
}

public class ActionUseVehicle : ActionInteract
{
    [SerializeField] private GameObject m_VisualModel;
    [SerializeField] private CharacterInputController m_CharacterInputController;
    [SerializeField] private CharacterController m_CharacterController;
    [SerializeField] private CharacterMovement m_CharacterMovement;
    [SerializeField] private ThirdPersonCamera m_Camera;
    [SerializeField] private PlayerShooter m_PlayerShooter;

    private bool InVehicle;

    private void Start()
    {
        EventOnStart.AddListener(OnActionStarted);
        EventOnEnd.AddListener(OnActionEnded);
    }

    private void OnDestroy()
    {
        EventOnStart.RemoveListener(OnActionStarted);
        EventOnEnd.RemoveListener(OnActionEnded);
    }

    private void Update()
    {
        if (InVehicle == true)
        {
            IsCanEnd = (Properties as ActionUseVehicleProperties).m_Vehicle.LinearVelocity < 2;
        }
    }

    private void OnActionStarted()
    {
        ActionUseVehicleProperties prop = Properties as ActionUseVehicleProperties;

        InVehicle = true;

        // Camera
        prop.m_VehicleInputController.AssingCamera(m_Camera);

        // Vehicle Input
        prop.m_VehicleInputController.enabled = true;

        // Turret
        prop.m_VehicleTurret.enabled = true;

        // Shooter
        prop.m_VehicleShooter.gameObject.SetActive(true);
        m_PlayerShooter.gameObject.SetActive(false);

        // Headlights
        prop.m_VehicleHeadlights.gameObject.SetActive(true);

        // Character Input
        m_CharacterInputController.enabled = false;

        // Character Movement
        m_CharacterController.enabled = false;
        m_CharacterMovement.enabled = false;

        // Vehicle AudioSource
        prop.m_VehicleAudioSource.enabled = true;

        // Hide Visual Model
        m_VisualModel.transform.localPosition = m_VisualModel.transform.localPosition + new Vector3(0, 100000, 0);

        Timer.CreateTimer(0.1f, false).OnTimerRunOut += () => IsCanEnd = false;
    }

    private void OnActionEnded()
    {
        ActionUseVehicleProperties prop = Properties as ActionUseVehicleProperties;

        InVehicle = false;

        // Camera
        m_CharacterInputController.AssingCamera(m_Camera);

        // Vehicle Input
        prop.m_VehicleInputController.enabled = false;

        // Turret
        prop.m_VehicleTurret.enabled = false;

        // Shooter
        prop.m_VehicleShooter.gameObject.SetActive(false);
        m_PlayerShooter.gameObject.SetActive(true);

        // Headlights
        prop.m_VehicleHeadlights.gameObject.SetActive(false);

        // Character Input
        m_CharacterInputController.enabled = true;

        // Character Movement
        bool positionFound = false;

        for (int i = 0; i < prop.m_InteractTransforms.Length; i++)
        {
            Vector3 startPosition = prop.m_InteractTransforms[i].position;

            // Проверяем, есть ли объекты на этой позиции
            Collider[] colliders = Physics.OverlapSphere(startPosition, 0.3f, ~0, QueryTriggerInteraction.Ignore);

            if (colliders.Length == 0)
            {
                // Опускаем луч вниз до земли
                Ray ray = new Ray(startPosition, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10))
                {
                    m_Owner.position = hit.point;
                    positionFound = true;
                    break; // Переместили персонажа, выходим из цикла
                }
            }
        }

        if (!positionFound)
        {
            Debug.LogWarning("Не удалось найти свободную позицию для перемещения персонажа.");
        }

        m_CharacterController.enabled = true;
        m_CharacterMovement.enabled = true;

        // Vehicle AudioSource
        prop.m_VehicleAudioSource.enabled = false;

        // UnHide Visual Model
        m_VisualModel.transform.localPosition = m_VisualModel.transform.localPosition + new Vector3(0, -100000, 0);
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        ActionUseVehicleProperties prop = Properties as ActionUseVehicleProperties;

        if (prop == null || prop.m_InteractTransforms == null) return;

        foreach (var transform in prop.m_InteractTransforms)
        {
            if (transform != null)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 0.3f, ~0, QueryTriggerInteraction.Ignore);

                Gizmos.color = colliders.Length == 0 ? Color.green : Color.red;
                Gizmos.DrawSphere(transform.position, 0.3f);

                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 20))
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(transform.position, hit.point);
                }
            }
        }
    }

#endif
}
