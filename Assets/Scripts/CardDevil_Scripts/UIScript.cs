using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    [SerializeField] GameObject interactPopUp;
    [SerializeField] GameObject eqPanel;

    List<GameObject> eqSpaces;
    [SerializeField]List<Image> eqImages;

    TMP_Text textMeshPro;
    EventBinding<InteractEvent> showInteractEventBinding;
    EventBinding<VisualEqSyncEvent> addItemEventBinding;
    private void Awake()
    {
        textMeshPro = interactPopUp.GetComponentInChildren<TMP_Text>();
        interactPopUp.SetActive(false);
    }
    private void Start()
    {
        eqSpaces = new List<GameObject>();
        eqImages = new List<Image>();
        foreach (Transform transform in eqPanel.GetComponentInChildren<Transform>()) 
        {
            eqSpaces.Add(transform.gameObject);
            eqImages.Add(transform.GetChild(0).GetComponent<Image>());
        }
    }

    private void OnEnable()
    {
        showInteractEventBinding = new EventBinding<InteractEvent>(HandleShowInteractEvent);
        EventBus<InteractEvent>.Register(showInteractEventBinding);

        addItemEventBinding = new EventBinding<VisualEqSyncEvent>(HandleItemEqEvent);
        EventBus<VisualEqSyncEvent>.Register(addItemEventBinding);
    }

    private void OnDisable()
    {
        EventBus<InteractEvent>.Deregister(showInteractEventBinding);

        EventBus<VisualEqSyncEvent>.Deregister(addItemEventBinding);
    }

    void HandleItemEqEvent(VisualEqSyncEvent e)
    {
        foreach(var image in eqImages)
        {
            image.sprite = null;
        }

        for (int i = 0; i < e.currentEq.Count; i++) 
        {
            eqImages[i].sprite = e.currentEq[i].sprite;
        }
        
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
