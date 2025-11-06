using UnityEngine;
public interface IEvent { }

public struct TestEvent : IEvent { }

public struct PlayerEvent : IEvent
{
    public int health;
}

public struct ShowInteractEvent :IEvent 
{ 
    public bool showInteract;
    public Object interactableObject;
}
