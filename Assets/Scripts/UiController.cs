using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public Slider lifeSlider;
    int currentPlayerHP;
    public GameObject gameOverPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayerHP = PlayerController.playerHP;
        lifeSlider.value = currentPlayerHP;
        gameOverPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerHP = PlayerController.playerHP;
        lifeSlider.value = currentPlayerHP;

        if (GameManager.gameState == GameState.gameOver)
        {
            gameOverPanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
            Cursor.visible = true; //カーソルを表示
        }
        else
        {
            gameOverPanel.SetActive(false);
        }
    }
}
