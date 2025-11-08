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

