using UnityEngine;
using UnityEngine.AI; // NavMesh関連を使うために必要
using System.Collections;

public class BaseRange : MonoBehaviour
{
    public GameObject baseCore;
    public static Color enemyColor = Color.red; //敵陣営のコアの色
    public static Color playerColor = Color.blue; //味方陣営のコアの色

    //ChangeColorクラスのメソッドを使用するために、呼び出し側のクラスで実体をインスタンスとして作る→これをしてもUnityはMonobehaviorでクラス継承していてGameobjectにスクリプトをアタッチすることでインスタンスを生成するので、
    //  自分でインスタンスにしても何もゲームオブジェクトがアタッチされておらず空になり何もできない
    // ChangeColor changeColor = new ChangeColor();

    private ChangeColor baseCoreChangeColor; // private にして Start() で取得するパターン
    private BaseCore baseCoreDamageTag;
    public GameObject allyPrefabs;
    public GameObject enemyPrefabs;

    public GameObject SpawnPoint; // 敵生成位置



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseCoreChangeColor = baseCore.GetComponent<ChangeColor>();
        baseCoreDamageTag = baseCore.GetComponent<BaseCore>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log("侵入呼び出し");

        if (this.tag == "Base")
        {
            // Debug.Log("タグ呼び出し");
            //プレイヤーが侵入
            if (other.gameObject.tag == "Player" || other.gameObject.tag == "PlayerAlly")
            {
                this.tag = "Player_Ba"; //拠点の陣営タグ変更
                baseCore.tag = "Player_Ba"; //コアの陣営タグ変更

                // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
                baseCoreChangeColor.SetColor(playerColor);
                GameManager.Instance.RefreshBaseCoreOnce();

                // GameObject obj = Instantiate(
                //     allyPrefabs,
                //     SpawnPoint.transform.position,
                //     Quaternion.identity
                //     ); //味方を生成

                //1秒まって適正を呼び出すコルーチン
                StartCoroutine(DelayedActionCoroutine());
            }

            //敵侵入
            else if (other.gameObject.tag.Contains("Enemy"))
            {
                this.tag = "Enemy_Ba";//拠点の陣営タグ変更
                baseCore.tag = "Enemy_Ba";//コアの陣営タグ変更

                // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
                baseCoreChangeColor.SetColor(enemyColor);
                GameManager.Instance.RefreshBaseCoreOnce();


                // GameObject obj = Instantiate(
                // enemyPrefabs,
                // SpawnPoint.transform.position,
                // Quaternion.identity
                // ); //敵を生成

                //1秒まって適正を呼び出すコルーチン
                 StartCoroutine(DelayedActionCoroutine());
            }
        }
    }

    //タグを変える用のメソッド
    public void ChangeCoreTag()
    {
        //敵からプレイヤーにする
        if (this.tag == "Enemy_Ba")
        {
            this.tag = "Player_Ba";
            baseCore.tag = "Player_Ba";
            // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
            baseCoreChangeColor.SetColor(playerColor);
            baseCoreDamageTag.DamageTag();
            StartCoroutine(DelayedActionCoroutine());
        }

        //プレイヤーから敵にする
        else if (this.tag == "Player_Ba")
        {
            this.tag = "Enemy_Ba";
            baseCore.tag = "Enemy_Ba";
            // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
            baseCoreChangeColor.SetColor(enemyColor);
            baseCoreDamageTag.DamageTag();
            StartCoroutine(DelayedActionCoroutine());

        }
    }

    // 1秒まって実行するコルーチン
    IEnumerator DelayedActionCoroutine()
    {
        // 1秒待つ
        yield return new WaitForSeconds(1f);

        if (this.tag == "Player_Ba")
        {
            //条件に応じて味方を生成するメソッドを呼び出す
            GameManager.Instance.OnAllyDestroyed();
        }
        else if (this.tag == "Enemy_Ba")
        {
            //条件に応じて敵を生成するメソッドを呼び出す
            GameManager.Instance.OnEnemyDestroyed();
        }
    }

}
