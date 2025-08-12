using Unity.VisualScripting;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [Header("Path Creation")]
    public GameObject waypointPrefab;
    public Transform pathParent;
    public CarPathFollower CarPathFollower;
    
    [Header("Waypoint Settings")]
    public float waypointHeight = 0.5f;

    void Start()
    {
        if (pathParent == null)
        {
            pathParent = transform;
        }
    }
    
    [ContextMenu("Clear All Waypoints")]
    void ClearAllWaypointsContext()
    {
        ClearAllWaypoints();
    }

    public void CreateWaypointAt(Vector3 position, EWaypointType waypointType = EWaypointType.DEFAULT)
    {
        if (waypointPrefab == null)
        {
            Debug.LogError("Waypoint prefab is not assigned!");
            return;
        }

        if (CarPathFollower == null)
        {
            Debug.LogError("CarPathFollower is not assigned!");
            return;
        }

        position.y = waypointHeight;

        GameObject waypoint = Instantiate(waypointPrefab, position, Quaternion.identity);
        waypoint.name = "Waypoint ((" + waypointType.ToString() + ") " + (pathParent.childCount + 1).ToString() + ")";
        waypoint.transform.parent = pathParent;
        
        // Set the waypoint type
        Waypoint waypointComponent = waypoint.GetComponent<Waypoint>();
        if (waypointComponent != null)
        {
            waypointComponent.type = waypointType;
        }
    }
    
    public void ClearAllWaypoints()
    {
        if (pathParent == null) return;

        for (int i = pathParent.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            DestroyImmediate(pathParent.GetChild(i).gameObject);
#else
            Destroy(pathParent.GetChild(i).gameObject);
#endif
        }

        Debug.Log("Cleared all waypoints");
    }

    public Transform[] GetAllWaypoints()
    {
        if (pathParent == null) return new Transform[0];
        
        Transform[] waypoints = new Transform[pathParent.childCount];
        for (int i = 0; i < pathParent.childCount; i++)
        {
            waypoints[i] = pathParent.GetChild(i);
        }
        return waypoints;
    }
}