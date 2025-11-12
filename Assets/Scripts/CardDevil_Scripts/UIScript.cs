using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] GameObject interactPopUp;
    [SerializeField] TMP_Text textMeshPro;
    EventBinding<InteractEvent> showInteractEventBinding;
    private void Awake()
    {
        textMeshPro = interactPopUp.GetComponentInChildren<TMP_Text>();
        interactPopUp.SetActive(false);
    }

    private void OnEnable()
    {

        showInteractEventBinding = new EventBinding<InteractEvent>(HandleShowInteractEvent);
        EventBus<InteractEvent>.Register(showInteractEventBinding);
    }

    private void OnDisable()
    {
        EventBus<InteractEvent>.Deregister(showInteractEventBinding);
    }

    void HandleShowInteractEvent(InteractEvent e)
    {
        interactPopUp.SetActive(e.showInteract);
        textMeshPro.text = "Click \"E\" to interact with ";
        if (e.type == TypesOfInteractables.Door)
        {
            textMeshPro.text += "The Door";
        }
        else if(e.type == TypesOfInteractables.Jar)
        {
            textMeshPro.text += "The Jar Of Sound";
        }
        else if(e.type == TypesOfInteractables.FlashBomb)
        {
            textMeshPro.text += "The Flash Bomb";
        }

    }
}
