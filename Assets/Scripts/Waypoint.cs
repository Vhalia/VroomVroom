using UnityEngine;

public enum EWaypointType
{
    DEFAULT,
    STOP,
}

public class Waypoint : MonoBehaviour
{
    [Header("Waypoint Configuration")]
    public EWaypointType type = EWaypointType.DEFAULT;
    
    [Header("Wait Settings")]
    [Range(0f, 10f)]
    public float waitTime = 0f;
    
    [Header("Visual Settings")]
    public bool showGizmo = true;

    void OnDrawGizmos()
    {
        if (!showGizmo) return;

        switch (type)
        {
            case EWaypointType.DEFAULT:
                Gizmos.color = Color.yellow;
                break;
            case EWaypointType.STOP:
                Gizmos.color = Color.red;
                break;
        }

        Gizmos.DrawWireSphere(transform.position, 1f);
    }

}