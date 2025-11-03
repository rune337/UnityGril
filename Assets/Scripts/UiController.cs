using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public Slider playerLifeSlider;
    public Slider enemyLifeSlider;

    int currentPlayerHP;
    int currentEnemyHP;

    public GameObject gameOverPanel;
    public GameObject gameClearPanel;
    public GameObject playerHPPanel;
    public GameObject enemyHPPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //プレイヤーのHPゲージ初期化
        currentPlayerHP = PlayerController.playerHP;
        playerLifeSlider.value = currentPlayerHP;

        //敵リーダーのHPゲージ初期化
        currentEnemyHP = EnemyLeaderController.enemyLeaderHP;
        enemyLifeSlider.value = currentEnemyHP;

        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーのHPゲージ更新
        currentPlayerHP = PlayerController.playerHP;
        playerLifeSlider.value = currentPlayerHP;

        //敵リーダのHPゲージ更新
        currentEnemyHP = EnemyLeaderController.enemyLeaderHP;
        enemyLifeSlider.value = currentEnemyHP;

        if (GameManager.gameState == GameState.gameOver)
        {
            gameOverPanel.SetActive(true);
            playerHPPanel.SetActive(false);
            enemyHPPanel.SetActive(false);


            Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
            Cursor.visible = true; //カーソルを表示

        }
        else if (GameManager.gameState == GameState.gameClear)
        {
            gameClearPanel.SetActive(true);
            playerHPPanel.SetActive(false);
            enemyHPPanel.SetActive(false);


            Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
            Cursor.visible = true; //カーソルを表示
        }
        else
        {
            gameOverPanel.SetActive(false);
            gameClearPanel.SetActive(false);
            playerHPPanel.SetActive(true);
            enemyHPPanel.SetActive(true);
        }
    }
}
