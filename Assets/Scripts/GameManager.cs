using UnityEngine;
using UnityEngine.SceneManagement; // SceneManagerを使用するために必要
using System.Collections.Generic;   // Dictionaryを使用するために必要

public enum GameState
{
    playing,
    pause,
    option,
    gameOver,
    gameClear,
}
public class GameManager : MonoBehaviour
{
    public static GameState gameState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CountRootObjectTags();

    }

    // Update is called once per frame
    void Update()
    {

    }


    void CountRootObjectTags()
    {
        // タグとその個数を格納する辞書
        Dictionary<string, int> tagCounts = new Dictionary<string, int>();

        // 現在のアクティブなシーンを取得
        Scene currentScene = SceneManager.GetActiveScene();

        // シーンのRoot GameObjectをすべて取得
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        Debug.Log("--- シーンのRootオブジェクトのタグをカウント ---");

        // 各Rootオブジェクトのタグを調べてカウント
        foreach (GameObject obj in rootObjects)
        {
            string objTag = obj.tag;

            // Debug.Log($"オブジェクト名: {obj.name}, タグ: {objTag}"); // デバッグ用

            // タグが既に辞書に存在すればカウントを増やし、存在しなければ新しく追加
            if (tagCounts.ContainsKey(objTag))
            {
                tagCounts[objTag]++;
            }
            else
            {
                tagCounts.Add(objTag, 1);
            }
        }

        // 結果をコンソールに表示
        Debug.Log("--- タグごとのRootオブジェクトの個数 ---");
        foreach (KeyValuePair<string, int> entry in tagCounts)
        {
            Debug.Log($"タグ: \"{entry.Key}\", 個数: {entry.Value}");
        }
        Debug.Log("------------------------------------");
    }
}
