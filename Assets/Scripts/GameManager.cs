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
    public bool IsQuitting { get; private set; } = false; //ゲーム終了中かどうか



    public string tagToSearch = "Base";
    public string playerTagToSearch = "Player_Ba";
    public string enemyTagToSearch = "Enemy_Ba";
    public static GameState gameState;

    private List<GameObject> foundBaseObjects = new List<GameObject>();
    private List<GameObject> foundPlayerBaseObjects = new List<GameObject>();
    private List<GameObject> foundEnemyBaseObjects = new List<GameObject>();

    public event Action<List<GameObject>> OnBaseCoreUpdated;
    public event Action<List<GameObject>> OnPlayerBaseCoreUpdated;
    public event Action<List<GameObject>> OnEnemyBaseCoreUpdated;
    GameObject[] enemy;
    GameObject[] enemyBaseCore;

    GameObject[] playerAlly;
    GameObject[] playerBaseCore;

    public GameObject playerAllyPrefabs; //味方NPC生成プレハブ素材
    public GameObject enemyPrefabs; //敵NPC生成プレハブ素材

    //ゲーム終了中かどうかは判別する元々定義されているメソッド
    private void OnApplicationQuit()
    {
        IsQuitting = true;
    }

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
        FindEnemyBaseObjects(enemyTagToSearch);

        gameState = GameState.playing;
    }

    public void RefreshBaseCoreOnce()
    {
        FindBaseObjects(tagToSearch);
        OnBaseCoreUpdated?.Invoke(foundBaseObjects);
    }

    void Update()
    {
        FindPlayerBaseObjects(playerTagToSearch);
        FindEnemyBaseObjects(enemyTagToSearch);
    }

    // タグから BaseCore を探してリスト化
    void FindBaseObjects(string tag)
    {
        foundBaseObjects.Clear();

        // Baseタグで検索
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

        // player_Baタグで検索
        GameObject[] PlayerBaseObjects = GameObject.FindGameObjectsWithTag(tag);

        // 名前に "BaseCore" を含むものをリストに追加
        foundPlayerBaseObjects = PlayerBaseObjects.Where(obj => obj != null && obj.name.Contains("BaseCore", StringComparison.OrdinalIgnoreCase)).ToList();


        //  Debug.Log($"登録された BaseCore の数: {foundBaseObjects.Count}");
        if (foundPlayerBaseObjects.Count > 0)
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

    //タグからEnemy_Baを探してリスト化
    void FindEnemyBaseObjects(string tag)
    {
        foundEnemyBaseObjects.Clear();

        // Enemy_Baタグで検索
        GameObject[] EnemyBaseObjects = GameObject.FindGameObjectsWithTag(tag);

        // 名前に "BaseCore" を含むものをリストに追加
        foundEnemyBaseObjects = EnemyBaseObjects.Where(obj => obj != null && obj.name.Contains("BaseCore", StringComparison.OrdinalIgnoreCase)).ToList();


        //  Debug.Log($"登録された BaseCore の数: {foundBaseObjects.Count}");
        if (foundEnemyBaseObjects.Count > 0)
        {
            foreach (GameObject obj in foundEnemyBaseObjects)
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

    //味方NPC敗北時に味方NPCの数が味方拠点数より少なければランダムな味方拠点に生成
    public void OnAllyDestroyed()
    {
        if (foundPlayerBaseObjects.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, foundPlayerBaseObjects.Count);
            PlayerNPCGenerate(playerAllyPrefabs, foundPlayerBaseObjects[randomIndex].transform.position); //味方生成メソッド呼び出し
        }
    }

    //敵NPC敗北時に敵NPCの数が敵拠点数より少なければランダムな敵拠点に生成
    public void OnEnemyDestroyed()
    {
        if (foundEnemyBaseObjects.Count != 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, foundEnemyBaseObjects.Count);
            EnemyNPCGenerate(enemyPrefabs, foundEnemyBaseObjects[randomIndex].transform.position); //敵生成メソッド呼び出し
        }
    }
    


    //敵NPCを生成する
    public void EnemyNPCGenerate(GameObject generateObj, Vector3 generatePoint)
    {
        enemy = GameObject.FindGameObjectsWithTag("Enemy");
        enemyBaseCore = GameObject.FindGameObjectsWithTag("Enemy_Ba");
        if (enemy.Length < enemyBaseCore.Length)
        {
            GameObject obj = Instantiate(
            generateObj,
            generatePoint,
            Quaternion.identity
            );
        }
    }

    //味方NPCを生成する
    public void PlayerNPCGenerate(GameObject generateObj, Vector3 generatePoint)
    {
        playerAlly = GameObject.FindGameObjectsWithTag("PlayerAlly");
        playerBaseCore = GameObject.FindGameObjectsWithTag("Player_Ba");

        if (playerAlly.Length < playerBaseCore.Length)
        {
            GameObject obj = Instantiate(
            generateObj,
            generatePoint,
            Quaternion.identity
            );
        }
    }


    //BaseCoreのオブジェクトを返す
    public List<GameObject> GetFoundBaseObjects()
    {
        return foundBaseObjects;
    }

    //Player_Baのオブジェクトを返す
    public List<GameObject> PlayerGetFoundBaseObjects()
    {
        return foundPlayerBaseObjects;
    }

    //Enemy_Baのオブジェクトを返す
    public List<GameObject> EnemyGetFoundBaseObjects()
    {

        return foundEnemyBaseObjects;
    }
}
