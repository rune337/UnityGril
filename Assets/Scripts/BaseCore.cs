using System.Collections;
using UnityEngine;

public class Core : MonoBehaviour
{
    bool isInvincible = false; //無敵フラグ
    public float invincibilityDuration = 0.5f; //無敵時間

    public float coreHP = 10; //coreHP
    string target;

     // BaseControllerへの参照を追加
    public BaseRange baseRange; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //現在のcoreのタグを元に攻撃者のタグに含まれる文字を検索する文字を決定する
        if (this.tag == "Player_Base")
        {
            target = "Player";
        }
        else if(this.tag == "Enemy_Base")
        {
            target = "Enemy";
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (isInvincible || !SwordAttack.isAttack) //無敵状態またはプレイヤーが攻撃中でなければ何もしない
            return;

        Debug.Log(other.transform.root.tag);

        //攻撃者のタグに自分のタグに含む文字と同じ文字が含まれていない時=陣営同じじゃない時
        //other.transform.parent.tagだと直近の親とってくるだけなので一番上の親とりたい時はrootにする
        if (!other.transform.root.tag.Contains(target))
        {
            //剣に当たった時ダメージ処理
            if (other.gameObject.CompareTag("Sward"))
            {
                isInvincible = true; //ダメージを受けたら無敵
                coreHP--;
                Debug.Log("コアのHP " + coreHP);

                if (coreHP <= 0)
                {
                    baseRange.ChangeCoreTag();
                }
                StartCoroutine(SetInvincibilityTimer());
            }
        }
    }

    IEnumerator SetInvincibilityTimer()
    {
        //指定された時間だけ待機
        yield return new WaitForSeconds(invincibilityDuration);

         //時間が経過したら無敵状態を解除
        isInvincible = false;
        
    }
}
