using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // タイトルボタンから呼び出す用
    public void GoToMainScene()
    {
        SceneManager.LoadScene("Main");
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameState == GameState.gameClear)
        {

            StartCoroutine(ReturnToTitleAfterDelay(3f)); // ← 3秒後にタイトルへ
        }
    }

    //数秒待ってタイトルシーンに戻るコルーチン
    private System.Collections.IEnumerator ReturnToTitleAfterDelay(float delay)
    {
        Debug.Log("ゲームクリア！ " + delay + "秒後にタイトルへ戻ります...");
        yield return new WaitForSeconds(delay);

        // タイトルシーンへ移動（※Build Settingsに登録してある必要あり）
        SceneManager.LoadScene("Title");
    }
}
