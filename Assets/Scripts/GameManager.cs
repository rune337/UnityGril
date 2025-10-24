using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQを使うために必要
using System;

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

        FindBaseObjects(tagToSearch);
    }

    public string tagToSearch = "Base";
    public static GameState gameState;

    private List<GameObject> foundBaseObjects = new List<GameObject>();

    public event Action<List<GameObject>> OnBaseCoreUpdated;

    void Start()
    {

    }

    void Update()
    {
    }


    public void RefreshBaseCoreOnce()
    {
        FindBaseObjects(tagToSearch);
        OnBaseCoreUpdated?.Invoke(foundBaseObjects);
    }

    // タグから BaseCore を探してリスト化
    void FindBaseObjects(string tag)
    {
        foundBaseObjects.Clear();

        // タグで検索
        GameObject[] baseObjects = GameObject.FindGameObjectsWithTag(tag);

        // 名前に "BaseCore" を含むものをリストに追加
        foundBaseObjects = baseObjects.Where(obj => obj != null && obj.name.Contains("BaseCore", StringComparison.OrdinalIgnoreCase)).ToList();


        //  Debug.Log($"登録された BaseCore の数: {foundBaseObjects.Count}");
        //   if (foundBaseObjects.Count> 0)
        //   {
        //       foreach (GameObject obj in foundBaseObjects)
        //       {
        //           if (obj != null)
        //           {
        //               Debug.Log($"- {obj.name} (Tag: {obj.tag}, Instance ID: {obj.GetInstanceID()})");
        //           }
        //       }
        //   }
    }

    // public void RefreshBaseCoreList()
    // {
    //     FindBaseObjects(tagToSearch);
    // }



    public List<GameObject> GetFoundBaseObjects()
    {
        return foundBaseObjects;
    }
}
