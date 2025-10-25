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



    public string tagToSearch = "Base";
    public string playerTagToSearch = "Player_Ba";
    public static GameState gameState;

    private List<GameObject> foundBaseObjects = new List<GameObject>();
    private List<GameObject> foundPlayerBaseObjects = new List<GameObject>();

    public event Action<List<GameObject>> OnBaseCoreUpdated;
    public event Action<List<GameObject>> OnPlayerBaseCoreUpdated;

     private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        FindBaseObjects(tagToSearch);
        FindPlayerBaseObjects(playerTagToSearch);
    }

    public void RefreshBaseCoreOnce()
    {
        FindBaseObjects(tagToSearch);
        OnBaseCoreUpdated?.Invoke(foundBaseObjects);
    }

    void Update()
    {
        FindPlayerBaseObjects(playerTagToSearch);
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

    //タグからPlayer_Baを探してリスト化
     void FindPlayerBaseObjects(string tag)
    {
        foundPlayerBaseObjects.Clear();

        // タグで検索
        GameObject[] PlayerBaseObjects = GameObject.FindGameObjectsWithTag(tag);

        // 名前に "player_Ba" を含むものをリストに追加
        foundPlayerBaseObjects = PlayerBaseObjects.Where(obj => obj != null && obj.name.Contains("BaseCore", StringComparison.OrdinalIgnoreCase)).ToList();


        //  Debug.Log($"登録された BaseCore の数: {foundBaseObjects.Count}");
           if (foundPlayerBaseObjects.Count> 0)
           {
               foreach (GameObject obj in foundPlayerBaseObjects)
               {
                   if (obj != null)
                   {
                       Debug.Log($"- {obj.name} (Tag: {obj.tag}, Instance ID: {obj.GetInstanceID()})");
                   }
               }
           }
    }

    // public void RefreshBaseCoreList()
    // {
    //     FindBaseObjects(tagToSearch);
    // }



    public List<GameObject> GetFoundBaseObjects()
    {
        return foundBaseObjects;
    }


    public List<GameObject> PlayerGetFoundBaseObjects()
    {
        return foundPlayerBaseObjects;
    }
}
