using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] List<Item> allItems;

    [SerializeField] List<Item> eq;

    EventBinding<AddItemToEq> eqAddBinding;
    EventBinding<UseItemInEq> itemUseBinding;

    private void OnEnable()
    {
        eqAddBinding = new EventBinding<AddItemToEq>(HandleAddItemEvent);
        EventBus<AddItemToEq>.Register(eqAddBinding);

        itemUseBinding = new EventBinding<UseItemInEq>(HandleUseItemEvent);
        EventBus<UseItemInEq>.Register(itemUseBinding);
    }

    private void OnDisable()
    {
        EventBus<AddItemToEq>.Deregister(eqAddBinding);

        EventBus<UseItemInEq>.Deregister(itemUseBinding);
    }
    private void Start()
    {
        EventBus<AddItemToEq>.Raise(new AddItemToEq
        {
            name="Lira"
        });
    }

    private void Update()
    {

    }
    void HandleAddItemEvent(AddItemToEq e)
    {
        foreach (var item in allItems) 
        { 
            if(item.name == e.name)
            {
                eq.Add(item);
                EventBus<VisualEqSyncEvent>.Raise(new VisualEqSyncEvent
                {
                    currentEq = eq
                });
                break;
            }
        }
    }

    void HandleUseItemEvent(UseItemInEq e)
    {
        try
        {
            if (eq[e.index] != null)
            {
                if (eq[e.index].name == "Lira")
                {
                    EventBus<LiraEvent>.Raise(new LiraEvent());
                }
                else if (eq[e.index].name == "Flash Bomb")
                {
                    EventBus<FlashBombEvent>.Raise(new FlashBombEvent());
                }

                Debug.Log($"Used {eq[e.index]}");

                if (eq[e.index].oneUse)
                {
                    eq.RemoveAt(e.index);
                }
            }

        }
        catch
        {
            Debug.Log("No Item To Use");
        }

    }
}
