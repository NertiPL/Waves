using Game.Player;
using Unity.Cinemachine;
using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Range(0, 10)]
    [SerializeField] int startingHealth = 100;
    [SerializeField] CinemachineCamera deathVirtualCamera;
    [SerializeField] Slider healthSlider;
    [SerializeField] GameObject gameOverContainer;

    int currentHealth;
    int gameOverVitrualCameraPriority = 20;

    void Awake()
    {
        currentHealth = startingHealth;
        AdjustHealthUI();
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
