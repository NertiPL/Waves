using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] Image interactPopUp;
    EventBinding<ShowInteractEvent> showInteractEventBinding;
    private void Awake()
    {
        interactPopUp.enabled = false;
    }

    private void OnEnable()
    {

        showInteractEventBinding = new EventBinding<ShowInteractEvent>(HandleShowInteractEvent);
        EventBus<ShowInteractEvent>.Register(showInteractEventBinding);
    }

    private void OnDisable()
    {
        EventBus<ShowInteractEvent>.Deregister(showInteractEventBinding);
    }

    void HandleShowInteractEvent(ShowInteractEvent e)
    {
        interactPopUp.enabled = e.showInteract;
    }
}
