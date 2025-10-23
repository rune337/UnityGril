using UnityEngine;

public class BaseRange : MonoBehaviour
{
    public GameObject baseCore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
                this.tag = "Player_Base"; //拠点の陣営タグ変更
                baseCore.tag = "Player_Base"; //コアの陣営タグ変更

            }

            //敵侵入
            else if (other.gameObject.tag.Contains("Enemy"))
            {
                this.tag = "Enemy_Base";//拠点の陣営タグ変更
                baseCore.tag = "Enemy_Base";//コアの陣営タグ変更
            }

        }

    }
    
    //タグを変える用のメソッド
    public void ChangeCoreTag()
    {
        //敵からプレイヤーにする
        if (this.tag == "Enemy_Base")
        {
            this.tag = "Player_Base";
            baseCore.tag = "Player_Base";
        }

        //プレイヤーから敵にする
        else if (this.tag == "Player_Base")
        {
            this.tag = "Enemy_Base";
            baseCore.tag = "Enemy_Base";

        }
    }
}
