using UnityEngine;
using RPGCharacterAnims.Actions;
using Unity.VisualScripting;

public class SwordAttack : MonoBehaviour
{

    float clickCount = 0; //クリック回数(プレイヤーの攻撃回数)
    float lastClickTime = 0f; //前回クリックしたときの Time.time を記録しておく変数
    float clickMaxDelay = 3.0f; //クリックの猶予時間
    public bool playerIsAttack = false; //攻撃中フラグ
    public SwordCollider swordCollider;

    Animator animator;
    Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        swordCollider = GetComponentInChildren<SwordCollider>(); //インスペクターでアタッチしてもnullになるので自動取得

    }
    
    
    void Update()
    {
        //攻撃のアニメーション
        if (Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.P))
        {
            playerIsAttack = true;
            AttackCombo();
            // Debug.Log(clickCount);
            Invoke("AttackEnd", 0.5f);
        }
    }

    void AttackCombo()
    {
        float timeSinceLastClick = Time.time - lastClickTime; //前回クリックしてからの経過時間

        //前回のクリックからの時間がコンボの猶予時間を超えている
        if (timeSinceLastClick > clickMaxDelay)
        {
            clickCount = 1; //時間を越えると新しい攻撃にする
        }
        else
        {
            clickCount++; //時間内なので攻撃増加
            if (clickCount > 5)
                clickCount = 1; //コンボ数最大4なのでそれを超えたら1にリセットする
        }
        lastClickTime = Time.time;
        animator.SetFloat("Attack", clickCount);
        swordCollider.EnableSwordCollider(); //剣のコライダーを有効にするのを呼び出す
    }

    void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        animator.SetFloat("Attack", 0f);
        playerIsAttack = false;
        swordCollider.DisableSwordCollider(); //剣のコライダーを無効にするのを呼び出す
    }
}
