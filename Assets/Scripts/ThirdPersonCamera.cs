using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    [Header("Rotation Limit")]
    [SerializeField] private float maxLimitY;
    [SerializeField] private float minLimitY;

    [Space(10)]
    [SerializeField] private float distanceToTarget;
    [SerializeField] private float minDistanceToTarget;
    [SerializeField] private float distanceOffsetFromCollisionHit;
    [SerializeField] private float sensitive;
    [SerializeField] private float distanceLerpRate;
    [SerializeField] private float changeOffsetRate;
    [SerializeField] private float rotateTargetLerpRate;
    [SerializeField] private Vector3 Offset;
    [SerializeField] private LayerMask ignoreTriggerLayerMask;

    [HideInInspector]
    public bool IsRotateTarget;
    [HideInInspector]
    public Vector2 RotationControl;

    private float deltaRotationX;
    private float deltaRotationY;

    private float currentDistance;

    private Vector3 targetOffset;
    private Vector3 defaultOffset;
    private Vector3 defaultCrouchOffset;


    private void Start()
    {
        targetOffset = Offset;
        defaultOffset = Offset;
        defaultCrouchOffset = new Vector3(Offset.x, Offset.y - 0.5f, Offset.z);
        transform.SetParent(null);
    }

    private void Update()
    {
        // Calculate rotation & translation
        deltaRotationX += RotationControl.x * sensitive;
        deltaRotationY += RotationControl.y * -sensitive;

        deltaRotationY = ClampAngle(deltaRotationY, minLimitY, maxLimitY);

        Offset = Vector3.MoveTowards(Offset, targetOffset, changeOffsetRate * Time.deltaTime);

        Quaternion finalRotation = Quaternion.Euler(deltaRotationY, deltaRotationX, 0);
        Vector3 finalPosition = targetTransform.position - (finalRotation * Vector3.forward * distanceToTarget);
        finalPosition = AddLocalOffset(finalPosition);

        // Calculate current distance
        float targetDistance = distanceToTarget;
        RaycastHit hit;
        Vector3 offsetPosition = targetTransform.position + new Vector3(Offset.x, Offset.y, Offset.z);

        if (Physics.Linecast(offsetPosition, finalPosition, out hit, ~ignoreTriggerLayerMask) == true)
        {
            float distanceToHit = Vector3.Distance(offsetPosition, hit.point);

            if (hit.transform != targetTransform && !hit.collider.isTrigger)
            {
                if (distanceToHit < distanceToTarget)
                    targetDistance = distanceToHit - distanceOffsetFromCollisionHit;
            }
        }

        currentDistance = Mathf.MoveTowards(currentDistance, targetDistance, Time.deltaTime * distanceLerpRate);
        currentDistance = Mathf.Clamp(currentDistance, minDistanceToTarget, distanceToTarget);

        // Correct camera position
        finalPosition = targetTransform.position - (finalRotation * Vector3.forward * currentDistance);

        // Apply transform
        transform.rotation = finalRotation;
        transform.position = finalPosition;
        transform.position = AddLocalOffset(transform.position);

        // Rotation target
        if (IsRotateTarget)
        {
            Quaternion targetRotation = Quaternion.Euler(targetTransform.eulerAngles.x, transform.eulerAngles.y, targetTransform.eulerAngles.z);
            targetTransform.rotation = Quaternion.RotateTowards(targetTransform.rotation, targetRotation, Time.deltaTime * rotateTargetLerpRate);
        }
    }

    private Vector3 AddLocalOffset(Vector3 position)
    {
        Vector3 result = position;
        result += new Vector3(0, Offset.y, 0);
        result += transform.right * Offset.x;
        result += transform.forward * Offset.z;

        return result;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    public void SetTargetOffset(Vector3 offset)
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.right, out hit, ~ignoreTriggerLayerMask))
        {
            float distanceToHit = hit.distance;

            if (hit.transform != targetTransform && !hit.collider.isTrigger)
            {
                if (distanceToHit < Offset.x)
                {
                    offset.x = distanceToHit - distanceOffsetFromCollisionHit;
                }
            }
        }

        targetOffset = offset;
    }

    public void SetDefaultTargetOffset()
    {
        targetOffset = defaultOffset;
    }

    public void SetDefaultTargetCrouchOffset()
    {
        targetOffset = defaultCrouchOffset;
    }

    public void SetTarget(Transform target)
    {
        targetTransform = target;
    }
}
