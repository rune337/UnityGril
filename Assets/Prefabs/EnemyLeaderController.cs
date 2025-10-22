using System.Collections;
using System.Threading;
using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Extensions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;

public class EnemyLeaderController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //NavMeshAgentコンポーネント
    GameObject player;

    public float attackRange = 0.5f; //攻撃を開始する距離
    public float stopRange = 2f; //接近限界距離
    public float detectionRange = 80f; //索敵範囲
    public Animator animator; //アニメーター
    public float enemySpeed = 5.0f;

    static public bool isAttack = false; //攻撃中フラグ

    bool lockOn = true; //ターゲット

    public float attackInterval = 0.5f; //次の攻撃までの時間

    float attackTimer; //攻撃可能になる時間
    float DistanceToPlayer; //プレイヤーとの距離


    float lastAttackTime = 0f; //前回攻撃したときのTime.timeを記録しておく変数
    float comboResetTime = 1.0f; //攻撃の猶予時間
    float attackCount; //enemyの攻撃回数


    //HP周り
    bool isInvincible = false; // 無敵状態を表すフラグ
    int enemyHP = 10;
    public float invincibilityDuration = 0.5f; //無敵時間

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTimer = Time.time;
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        //プレイヤーいなくなった時
        if (player == null)
        {
            return;
        }
        
        DistanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        //索敵範囲の時
        if (DistanceToPlayer <= detectionRange)
        {
            //これを書かないとプレイヤーと逆方向に走って行ってしまう
            if (lockOn)
            {
                //プレイヤーの方を向く
                transform.LookAt(player.transform.position);
            }

            //プレイヤーとの距離が接近限界以上の時
            if (DistanceToPlayer >= stopRange)
            {
                navMeshAgent.SetDestination(player.transform.position);
                animator.SetBool("isRun", true);
            }

            //プレイヤーとの距離が接近距離より小さい時
            else if (DistanceToPlayer < stopRange)
            {
                navMeshAgent.isStopped = true;
                animator.SetBool("isRun", false);

                //攻撃中でないかつ前回の攻撃から0.5経過
                if (!isAttack && Time.time >= attackTimer)
                    AttackCombo();
            }
        }
        //ここに索敵範囲外の時は拠点は破壊に行く処理をかく
        else
        {
            navMeshAgent.isStopped = true;
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.ResetPath();
            }

            animator.SetBool("isRun", false);
            animator.SetBool("isIdle", true);


            //player索敵範囲外の時は拠点を取得しにいくもしくは攻撃しにいく

        }

    }

    void AttackCombo()
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


    void FootR()
    {
        //本当はここに足音入れる
    }


    void FootL()
    {
        //本当はここに足音入れる
    }

    void Hit()
    {
        //攻撃ヒット時に使う？
        // Debug.Log("攻撃ヒット");
    }



    //ダメージ処理
    void OnTriggerEnter(Collider other)
    {
        if (isInvincible || !SwordAttack.isAttack) //無敵状態またはプレイヤーが攻撃中でなければ何もしない
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
            GameManager.gameState = GameState.gameClear;
        }


    }

    IEnumerator SetInvincibilityTimer()
    {
        //指定された時間だけ待機
        yield return new WaitForSeconds(invincibilityDuration);

        //時間が経過したら無敵状態を解除
        isInvincible = false;

    }


    //デバッグ表示
    void OnDrawGizmos()
    {
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;
            // 目的地へのパスを描画
            Vector3[] pathCorners = navMeshAgent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
            // 最終目的地
            Gizmos.DrawSphere(navMeshAgent.destination, 0.2f);
        }

        // 索敵範囲と停止範囲も描画すると分かりやすい
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }

}
