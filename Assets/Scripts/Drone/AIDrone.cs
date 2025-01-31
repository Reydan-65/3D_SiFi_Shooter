using UnityEngine;

[RequireComponent(typeof(Drone))]
public class AIDrone : MonoBehaviour
{
    [SerializeField] private float detectionDistance;

    private Drone drone;
    private Vector3 movementPosition;
    private Vector3 shootTarget;
    private Transform player;
    private CubeArea movementArea;
    public CubeArea MovementArea { get => movementArea; set => movementArea = value; }

    // Unity Evelts
    private void Start()
    {
        drone = GetComponent<Drone>();
        drone.EventOnDeath.AddListener(OnDroneDeath);
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        UpdateAI();
    }

    // Handlers
    private void OnDroneDeath()
    {
        enabled = false;
    }

    private void OnDestroy()
    {
        drone.EventOnDeath.RemoveListener(OnDroneDeath);
    }

    // UpdateAI
    private void UpdateAI()
    {
        // Update movement position
        if (transform.position == movementPosition)
        {
            movementPosition = movementArea.GetRandomInsideZone();
        }

        if (Physics.Linecast(drone.MainMesh.position, movementPosition))
        {
            movementPosition = movementArea.GetRandomInsideZone();
        }

        // Target Detection
        if (player.root.GetComponent<SpaceSoldier>().SoldierIsDead == false &&
            Vector3.Distance(drone.MainMesh.position, player.position) < detectionDistance)
        {
            float offset = player.GetComponent<CharacterController>().height / 2;
            shootTarget = new Vector3(player.position.x, player.position.y + offset, player.position.z);

            // Rotate
            if (shootTarget != null)
            {
                drone.LookAt(shootTarget);
            }
            else
            {
                drone.LookAt(movementPosition);
            }

            // Fire
            if (shootTarget != null)
            {
                drone.Fire(shootTarget);
            }
        }
        else
        {
            drone.MoveAt(movementPosition);
            drone.LookAt(movementPosition);
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionDistance);

        if (shootTarget != null)
            Gizmos.DrawLine(transform.position, shootTarget);
    }

#endif

}
