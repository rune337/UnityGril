using System.Collections;
using UnityEngine;

public class BaseCore : MonoBehaviour
{
    bool isInvincible = false; //無敵フラグ
    public float invincibilityDuration = 0.5f; //無敵時間

    public  int coreHP = 10; //coreHP
    string target; //攻撃者の文字列変数

    // BaseRangeへの参照を追加
    public BaseRange baseRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        DamageTag();
    }

    // Update is called once per frame
    void Update()
    {
        if (coreHP <= 0)
        {
            baseRange.ChangeCoreTag();
            coreHP = 10;//HP0のままだと0で剣に触れるとすぐに切り替わってしまうので
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isInvincible) //無敵状態なら何もしない
            return;

        // Debug.Log(other.transform.root.tag);

        //攻撃者のタグに自分のタグに含む文字と同じ文字が含まれていない時=陣営同じじゃない時
        //other.transform.parent.tagだと直近の親とってくるだけなので一番上の親とりたい時はrootにする
        if (!other.transform.root.tag.Contains(target))
        {

            //剣に当たった時ダメージ処理
            if (other.gameObject.CompareTag("EnemySword") || other.gameObject.CompareTag("PlayerSword"))
            {
                isInvincible = true; //ダメージを受けたら無敵
                coreHP--;
                Debug.Log("コアのHP " + coreHP);
                Debug.Log(target);

                StartCoroutine(SetInvincibilityTimer());
            }
        }
    }

    //無敵時間のコルーチン
    IEnumerator SetInvincibilityTimer()
    {
        //指定された時間だけ待機
        yield return new WaitForSeconds(invincibilityDuration);

        //時間が経過したら無敵状態を解除
        isInvincible = false;

    }

    //ダメージを受ける攻撃者を決定する
    public void DamageTag()
    {
        //現在のcoreのタグを元に攻撃者のタグに含まれる文字を検索する文字を決定する
        if (this.tag == "Player_Ba")
        {
            target = "Player";
        }
        else if (this.tag == "Enemy_Ba")
        {
            target = "Enemy";
        }
        else if (this.tag == "Base")
        {
            target = "Base";
        }
    }
}
