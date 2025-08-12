using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.PlayerLoop;
using System;
using System.Collections;
using NUnit.Framework;

public class CarPathFollower : MonoBehaviour
{
    [Header("Path Settings")]
    public GameObject Path;
    public bool loop = true;
    public bool autoStart = true;

    [Header("Movement Settings")]
    public float maxSpeed = 5f;
    public float acceleration = 2f;
    public float deceleration = 4f;
    public float rotationSpeed = 5f;
    public float stoppingDistance = 0.1f;

    [Header("Debug")]
    public bool showPath = true;
    public Color pathColor = Color.green;
    public float sphereRadius = 0.5f;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private List<Waypoint> waypoints;
    private float currentSpeed = 0f;
    private Vector3 currentTargetPosition;

    void Start()
    {
        waypoints = Path.GetComponentsInChildren<Waypoint>()
            .ToList();

        if (waypoints.Count > 0)
            transform.position = waypoints[0].transform.position;

        if (autoStart && waypoints != null && waypoints.Count > 0)
        {
            isMoving = true;
        }
    }
    
    void Update()
    {
        if (isMoving && waypoints != null && waypoints.Count > 0)
        {
            MoveTowardsWaypoint();
        }
        else if (!isMoving && currentSpeed > 0f)
        {
            Decelerate();
        }
    }
    
    void MoveTowardsWaypoint()
    {
        Waypoint targetWaypoint = waypoints[currentWaypointIndex];
        if (targetWaypoint == null) return;
        
        switch (targetWaypoint.type)
        {
            case EWaypointType.DEFAULT:
                MoveToWaypoint(targetWaypoint, () => MoveToNextWaypoint());
                break;
            case EWaypointType.STOP:
                MoveToWaypoint(targetWaypoint, () => HandleStopWaypoint(targetWaypoint));
                break;
            default:
                Debug.LogWarning("Unknown waypoint type: " + targetWaypoint.type);
                break;
        }
    }

    void MoveToWaypoint(Waypoint waypoint, Action onReached)
    {
        currentTargetPosition = new Vector3(
            waypoint.transform.position.x,
            transform.position.y,
            waypoint.transform.position.z);

        Accelerate();

        Turn();

        if (Vector3.Distance(
            transform.position,
            currentTargetPosition) < GetStoppingDistance(waypoint))
        {
            onReached.Invoke();
        }
    }

    private float GetStoppingDistance(Waypoint waypoint)
    {
        if (waypoint.type == EWaypointType.STOP)
            return 5f;

        return stoppingDistance;
    }


    private void Turn()
    {
        Vector3 direction = (currentTargetPosition - transform.position).normalized;
        if (direction != Vector3.zero && currentSpeed > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            float rotationFactor = Mathf.Clamp01(currentSpeed / maxSpeed) * rotationSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactor * Time.deltaTime);
        }
    }

    void HandleStopWaypoint(Waypoint waypoint)
    {
        if (waypoint.waitTime > 0f)
            StartCoroutine(WaitAtWaypoint(waypoint));
        else
            MoveToNextWaypoint();
    }

    private IEnumerator WaitAtWaypoint(Waypoint waypoint)
    {
        isMoving = false;
        yield return new WaitForSeconds(waypoint.waitTime);
        MoveToNextWaypoint();
    }

    public void MoveToNextWaypoint()
    {
        currentWaypointIndex++;
        isMoving = true;

        if (currentWaypointIndex >= waypoints.Count)
        {
            if (loop)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                isMoving = false;
                return;
            }
        }
    }

    private void Accelerate()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        MoveToTarget();
    }

    private void Decelerate(float target = 0f)
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, target, deceleration * Time.deltaTime);
        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            currentTargetPosition,
            currentSpeed * Time.deltaTime);
    }

    public void AddWaypoint(Waypoint waypoint)
    {
        if (waypoint == null) return;

        waypoints.Add(waypoint);
    }

    public void ClearWaypoints()
    {
        waypoints.Clear();
    }
    
    public void ResetPath()
    {
        currentWaypointIndex = 0;
        isMoving = false;
        currentSpeed = 0f;
    }
    
    public void SetSpeed(float newSpeed)
    {
        maxSpeed = newSpeed;
    }
    
    void OnDrawGizmos()
    {
        var wps = Path.GetComponentsInChildren<Transform>()
            .Where(t => t != Path.transform)
            .ToList();
        if (!showPath || wps == null || wps.Count < 2) return;

        Gizmos.color = pathColor;

        for (int i = 0; i < wps.Count - 1; i++)
        {
            if (wps[i] != null && wps[i + 1] != null)
            {
                Gizmos.DrawLine(wps[i].position, wps[i + 1].position);
            }
        }

        if (loop && wps.Count > 2 && wps[0] != null && wps[wps.Count - 1] != null)
        {
            Gizmos.DrawLine(wps[wps.Count - 1].position, wps[0].position);
        }

        if (Application.isPlaying && isMoving && currentWaypointIndex < wps.Count && wps[currentWaypointIndex] != null)
        {
            Gizmos.DrawWireSphere(wps[currentWaypointIndex].position, sphereRadius * 1.5f);
        }
    }
}