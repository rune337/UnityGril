using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    public Slider playerLifeSlider; //プレイヤーHPスライダーをアタッチする変数
    public Slider enemyLifeSlider; //敵HPスライダーをアタッチする変数

    int currentPlayerHP; //現在のプレイヤーHPを入れる変数
    int currentEnemyHP; //現在の敵HPを入れる変数

    public GameObject gameOverPanel; //ゲームオーバパネルオブジェクトをアタッチする変数
    public GameObject gameClearPanel; //ゲームクリアパネルオブジェクトをアタッチする変数
    public GameObject playerHPPanel; //プレイヤーHPパネルオブジェクトをアタッチする変数
    public GameObject enemyHPPanel; //エネミーHPパネルオブジェクトをアタッチする変数
    public GameObject isUnderPlayerLamp; //プレイヤー集合フラグのランプ

    public GameObject player;
    PlayerController playerController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = player.GetComponent<PlayerController>(); //プレイヤーオブジェクトのコンポーネントを取得

        //プレイヤーのHPゲージ初期化
        currentPlayerHP = PlayerController.playerHP;
        playerLifeSlider.value = currentPlayerHP;

        //敵リーダーのHPゲージ初期化
        currentEnemyHP = EnemyLeaderController.enemyLeaderHP;
        enemyLifeSlider.value = currentEnemyHP;

        //パネルを非表示にする
        gameOverPanel.SetActive(false);
        gameClearPanel.SetActive(false);

       //HPパネルを表示する
        playerHPPanel.SetActive(true);
        enemyHPPanel.SetActive(true);
        
        isUnderPlayerLamp.SetActive(false); //プレイヤー集合フラグのランプ初期はfalseなので表示しない
    }

    void Update()
    {
        //プレイヤーのHPゲージ更新
        currentPlayerHP = PlayerController.playerHP;
        playerLifeSlider.value = currentPlayerHP;

        //敵リーダのHPゲージ更新
        currentEnemyHP = EnemyLeaderController.enemyLeaderHP;
        enemyLifeSlider.value = currentEnemyHP;

        //プレイヤー集合フラグに応じて表示非表示にする
        if (playerController.isUnderPlayer)
        {
            isUnderPlayerLamp.SetActive(true);
        }
        else if (!playerController.isUnderPlayer)
        {
            isUnderPlayerLamp.SetActive(false);
        }

        
        if (GameManager.gameState == GameState.gameOver) //ゲームオーバの時ゲームオーバーパネルを表示してHPとプレイヤー集合フラグを非表示にする
        {
            gameOverPanel.SetActive(true);
            playerHPPanel.SetActive(false);
            enemyHPPanel.SetActive(false);
            isUnderPlayerLamp.SetActive(false);


            Cursor.lockState = CursorLockMode.None; //画面中心にカーソルのロック解除
            Cursor.visible = true; //カーソルを表示

        }
        else if (GameManager.gameState == GameState.gameClear) //ゲームクリアの時ゲームクリアパネルを表示してHPとプレイヤー集合フラグを非表示にする
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
