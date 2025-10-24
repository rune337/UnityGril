using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public static EnemyLeaderController Instance { get; private set; }
    
    static public bool isAttack = false; //攻撃中フラグ
    float lastAttackTime = 0f; //前回攻撃したときのTime.timeを記録しておく変数
    float comboResetTime = 1.0f; //攻撃の猶予時間
    float attackCount; //enemyの攻撃回数
    public Animator animator; //アニメーター
    float attackTimer; //攻撃可能になる時間
    public float attackInterval = 0.5f; //次の攻撃までの時間
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = Time.time;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public  void AttackCombo()
    {

        isAttack = true;

        if (Time.time - lastAttackTime > comboResetTime)
        {
            attackCount = 1;
        }
        else
        {
            attackCount++;
            if (attackCount > 5)
            {
                attackCount = 1;
            }
        }
        animator.SetFloat("Attack", attackCount);

        //攻撃インターバル
        attackTimer = Time.time + attackInterval;
        lastAttackTime = Time.time; //攻撃が終わった時間を記録

        Invoke("AttackEnd", 0.5f);
    }
    
    void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        animator.SetFloat("Attack", 0f);
        isAttack = false;
    }
}
