using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQを使うために必要

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
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public string tagToSearch = "Base";
    public static GameState gameState;

    private List<GameObject> foundBaseObjects = new List<GameObject>();

    void Start()
    {
        RefreshBaseCoreList();
    }

    void Update()
    {
        // ゲーム状態に応じた処理
    }

    // タグから BaseCore を探してリスト化
    void FindBaseObjects(string tag)
    {
        foundBaseObjects.Clear();

        // タグで検索
        GameObject[] baseObjects = GameObject.FindGameObjectsWithTag(tag);

        // 名前が "BaseCore" のものだけ追加
        foundBaseObjects = baseObjects.Where(obj => obj != null && obj.name == "BaseCore").ToList();

        Debug.Log($"登録された BaseCore の数: {foundBaseObjects.Count}");
    }

    public void RefreshBaseCoreList()
    {
        FindBaseObjects(tagToSearch);
    }

    public List<GameObject> GetFoundBaseObjects()
    {
        return foundBaseObjects;
    }
}
