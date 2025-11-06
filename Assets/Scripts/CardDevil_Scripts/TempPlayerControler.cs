using System;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempPlayerControler : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;
    Vector3 moveDir;

    int health = 10;
    int mana = 20;


    public InputActionReference move;

    //---------------------------------------------------
    public InputActionReference interact;


    //--------------------------------------------------
    EventBinding<TestEvent> testEventBinding;
    EventBinding<PlayerEvent> playerEventBinding;

    void HandleTestEvent()
    {
        Debug.Log("Test Event recieved!");
    }

    void HandlePlayerEvent(PlayerEvent playerEvent)
    {
        Debug.Log($"Player Event recieved! Health: {playerEvent.health}");
    }

    private void Update()
    {
        moveDir = move.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity= new Vector3(moveDir.x*speed,0f,moveDir.y*speed);
    }

    private void OnEnable()
    {
        interact.action.Enable();

        testEventBinding=new EventBinding<TestEvent>(HandleTestEvent);
        EventBus<TestEvent>.Register(testEventBinding);

        playerEventBinding = new EventBinding<PlayerEvent>(HandlePlayerEvent);
        EventBus<PlayerEvent>.Register(playerEventBinding);
    }

    private void OnDisable()
    {
        interact.action.Disable();

        EventBus<TestEvent>.Deregister(testEventBinding);
        EventBus<PlayerEvent>.Deregister(playerEventBinding);
    }

    private void Start()
    {
        interact.action.started += Interact;
    }

    private void Interact(InputAction.CallbackContext context)
    {
        EventBus<TestEvent>.Raise(new TestEvent());
        EventBus<PlayerEvent>.Raise(new PlayerEvent
        {
            health = health,
        });
        Debug.Log("Interacted");
    }
}
