using UnityEngine;

public class HearAreaScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        EventBus<PlayerInHearAreaEvent>.Raise(new PlayerInHearAreaEvent
        {
            isPlayerIn = true
        });
    }
    private void OnTriggerExit(Collider other)
    {
        EventBus<PlayerInHearAreaEvent>.Raise(new PlayerInHearAreaEvent
        {
            isPlayerIn = false
        });
    }
}
