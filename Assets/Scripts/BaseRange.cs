using UnityEngine;

public class BaseRange : MonoBehaviour
{
    public GameObject baseCore;
    public static Color enemyColor = Color.red; //敵陣営のコアの色
    public static Color playerColor = Color.blue; //味方陣営のコアの色

    //ChangeColorクラスのメソッドを使用するために、呼び出し側のクラスで実体をインスタンスとして作る→これをしてもUnityはMonobehaviorでクラス継承していてGameobjectにスクリプトをアタッチすることでインスタンスを生成するので、
    //  自分でインスタンスにしても何もゲームオブジェクトがアタッチされておらず空になり何もできない
    // ChangeColor changeColor = new ChangeColor();

    private ChangeColor baseCoreChangeColor; // private にして Start() で取得するパターン


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseCoreChangeColor = baseCore.GetComponent<ChangeColor>();
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
            if (other.gameObject.tag == "Player")
            {
                this.tag = "Player_Ba"; //拠点の陣営タグ変更
                baseCore.tag = "Player_Ba"; //コアの陣営タグ変更

                // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
                baseCoreChangeColor.SetColor(playerColor);
                GameManager.Instance.RefreshBaseCoreOnce();
            }

            //敵侵入
            else if (other.gameObject.tag.Contains("Enemy"))
            {
                this.tag = "Enemy_Ba";//拠点の陣営タグ変更
                baseCore.tag = "Enemy_Ba";//コアの陣営タグ変更

                // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
                baseCoreChangeColor.SetColor(enemyColor);
                GameManager.Instance.RefreshBaseCoreOnce();
            }
             GameManager.Instance.RefreshBaseCoreOnce();

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
        }

        //プレイヤーから敵にする
        else if (this.tag == "Player_Ba")
        {
            this.tag = "Enemy_Ba";
            baseCore.tag = "Enemy_Ba";
            // 取得した baseCoreChangeColor インスタンスの SetColor を呼び出す
            baseCoreChangeColor.SetColor(enemyColor);

        }
    }

}
