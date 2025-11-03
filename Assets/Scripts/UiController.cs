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
    public GameObject isUnderPlayerLamp; //プレイヤー集合フラグのランプ

    public GameObject player;
    PlayerController playerController;
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

        playerController = player.GetComponent<PlayerController>(); //プレイヤーオブジェクトのコンポーネントを取得

        isUnderPlayerLamp.SetActive(false); //プレイヤー集合フラグのランプ初期はfalseなので表示しない


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

        //プレイヤー集合フラグに合わせてランプをオンオフ
        if (playerController.isUnderPlayer)
        {
            isUnderPlayerLamp.SetActive(true);
        }
        else if (!playerController.isUnderPlayer)
        {
            isUnderPlayerLamp.SetActive(false);
        }

        if (GameManager.gameState == GameState.gameOver)
        {
            gameOverPanel.SetActive(true);
            playerHPPanel.SetActive(false);
            enemyHPPanel.SetActive(false);
            isUnderPlayerLamp.SetActive(false);


            Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
            Cursor.visible = true; //カーソルを表示

        }
        else if (GameManager.gameState == GameState.gameClear)
        {
            gameClearPanel.SetActive(true);
            playerHPPanel.SetActive(false);
            enemyHPPanel.SetActive(false);
            isUnderPlayerLamp.SetActive(false);


            Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
            Cursor.visible = true; //カーソルを表示
        }

    }
}
