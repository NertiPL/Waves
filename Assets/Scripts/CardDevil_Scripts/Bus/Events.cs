using System.Collections.Generic;
using UnityEngine;
public interface IEvent { }

public struct TestEvent : IEvent { }

public struct PlayerEvent : IEvent
{
    public int health;
}

public struct InteractEvent :IEvent 
{ 
    public bool showInteract;
    public GameObject interactableObject;
    public TypesOfInteractables type;
}

public struct MakeSoundEvent : IEvent { }
public struct AddItemToEq : IEvent 
{
    public string name;
}
public struct UseItemInEq : IEvent
{
    public int index;
}
public struct VisualEqSyncEvent : IEvent 
{
    public List<Item> currentEq;
}

