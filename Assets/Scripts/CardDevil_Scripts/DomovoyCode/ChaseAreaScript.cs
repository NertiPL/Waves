using UnityEngine;

public class ChaseAreaScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EventBus<PlayerInChaseAreaEvent>.Raise(new PlayerInChaseAreaEvent
        {
            isPlayerIn = true
        });
    }
    private void OnTriggerExit(Collider other)
    {
        EventBus<PlayerInChaseAreaEvent>.Raise(new PlayerInChaseAreaEvent
        {
            isPlayerIn = false
        });
    }
}
