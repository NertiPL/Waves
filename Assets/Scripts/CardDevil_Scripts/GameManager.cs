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

    void HandleAddItemEvent(AddItemToEq e)
    {
        foreach (var item in allItems) 
        { 
            if(item.name == e.name)
            {
                eq.Add(item);
                break;
            }
        }
        EventBus<VisualEqSyncEvent>.Raise(new VisualEqSyncEvent{
            currentEq = eq
        });
    }

    void HandleUseItemEvent(UseItemInEq e)
    {
        if (eq[e.index] != null) 
        {
            if (eq[e.index].oneUse)
            {
                eq.RemoveAt(e.index);
            }
        }
    }
    private void Update()
    {
        
    }
}
