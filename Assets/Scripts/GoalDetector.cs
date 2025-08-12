using UnityEngine;

namespace Assets.Scripts
{
    public class GoalDetector : MonoBehaviour
    {
        [Header("Goal Settings")]
        public string playerTag = "Player";
        
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerTag))
            {
                EventBus.TriggerEvent(EEventType.GOAL_REACHED);
            }
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            
            BoxCollider boxCollider = GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
            }
        }
    }
}