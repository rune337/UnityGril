using UnityEngine;
using System.Collections; // コルーチンを使うために必要

public class EnemyController : MonoBehaviour
{
    bool isInvincible = false; // 無敵状態を表すフラグ
    int enemyHP = 10;
    public float invincibilityDuration = 0.5f; //無敵時間

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
        if (isInvincible || !SwaordAttack.isAttack) //無敵状態またはプレイヤーが攻撃中でなければ何もしない
            return;


        if (other.gameObject.CompareTag("Sward"))
        {
            isInvincible = true; //ダメージを受けたら無敵
            enemyHP--;
            Debug.Log("敵のHP " + enemyHP);

            //無敵時間を開始するコルーチンを呼び出す
            StartCoroutine(SetInvincibilityTimer());

        }

        if (enemyHP < 1)
        {
            Destroy(this.gameObject);
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
