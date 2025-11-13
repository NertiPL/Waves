using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class UISprite
{
    [SerializeField] public string name;
    [SerializeField] public Sprite sprite;
}

public class UIScript : MonoBehaviour
{
    [SerializeField] GameObject interactPopUp;
    [SerializeField] GameObject eqPanel;


    [SerializeField] List<UISprite> UISprites;

    List<GameObject> eqSpaces;
    [SerializeField]List<Image> eqImages;

    TMP_Text textMeshPro;
    EventBinding<InteractEvent> showInteractEventBinding;
    EventBinding<VisualEqSyncEvent> addItemEventBinding;
    EventBinding<VisualSelectedItemEq> selectedItemEventBinding;
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

        selectedItemEventBinding = new EventBinding<VisualSelectedItemEq>(HandleSelectedItemEqEvent);
        EventBus<VisualSelectedItemEq>.Register(selectedItemEventBinding);
    }

    private void OnDisable()
    {
        EventBus<InteractEvent>.Deregister(showInteractEventBinding);

        EventBus<VisualEqSyncEvent>.Deregister(addItemEventBinding);

        EventBus<VisualSelectedItemEq>.Deregister(selectedItemEventBinding);
    }

    void HandleSelectedItemEqEvent(VisualSelectedItemEq e)
    {
        Sprite spriteTemp = null;
        foreach (var slot in eqSpaces)
        {
            foreach(var sprite in UISprites)
            {
                if(sprite.name == "Slot")
                {
                    spriteTemp=sprite.sprite; 
                    break;
                }
            }
           slot.GetComponent<Image>().sprite = spriteTemp;
        }

        foreach (var sprite in UISprites)
        {
            if (sprite.name == "SelectedSlot")
            {
                spriteTemp = sprite.sprite;
                break;
            }
        }
        eqSpaces[e.index].GetComponent<Image>().sprite = spriteTemp;
    }

    void HandleItemEqEvent(VisualEqSyncEvent e)
    {
        foreach(var image in eqImages)
        {
            Color color = image.color;
            color.a = 0;
            image.color = color;

            image.sprite = null;
        }

        for (int i = 0; i < e.currentEq.Count; i++) 
        {
            Color color = eqImages[i].color;
            color.a = 1;
            eqImages[i].color = color;

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
