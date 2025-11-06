using Game.Player;
using Unity.Cinemachine;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UIScriptTemp : MonoBehaviour
{
    [Range(0, 10)]
    [SerializeField] CinemachineCamera deathVirtualCamera;
    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject gameOverContainer;
    EventBinding<PlayerEvent> playerEventBinding;

    int currentHealth;
    int gameOverVitrualCameraPriority = 20;

    private void OnEnable()
    {

        playerEventBinding = new EventBinding<PlayerEvent>(HandlePlayerEvent);
        EventBus<PlayerEvent>.Register(playerEventBinding);
    }

    private void OnDisable()
    {
        EventBus<PlayerEvent>.Deregister(playerEventBinding);
    }

    void Awake()
    {
        AdjustHealthUI();
    }

    void HandlePlayerEvent(PlayerEvent playerEvent)
    {
        currentHealth = playerEvent.health;
        AdjustHealthUI();

        if (currentHealth <= 0)
        {
            PlayerGameOver();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        AdjustHealthUI();

        if (currentHealth <= 0)
        {
            PlayerGameOver();
        }
    }

    private void PlayerGameOver()
    {
        deathVirtualCamera.Priority = gameOverVitrualCameraPriority;
        gameOverContainer.SetActive(true);
        PlayerInputRouter playerInputRouter = FindFirstObjectByType<PlayerInputRouter>();
        playerInputRouter.SetCursorState(false);
        Destroy(this.gameObject);
    }

    void AdjustHealthUI()
    {
        healthSlider.value = currentHealth;
    }
}
