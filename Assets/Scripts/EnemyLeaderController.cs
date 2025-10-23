using System.Collections;
using System.Threading;
using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Extensions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.LowLevel;
using System.Linq; // LINQを使用するために必要

public class EnemyLeaderController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //NavMeshAgentコンポーネント
    GameObject player;

    GameObject targetBaseCore;

    public float attackRange = 0.5f; //攻撃を開始する距離
    public float stopRange = 2f; //接近限界距離

    public float baseDiscrimination = 10f; //Base判別範囲
    public string targetTag = "Player_Base"; //Base判別範囲でターゲットとするベースコアのタグ

    public float detectionRange = 80f; //索敵範囲
    public float detectionRangeBaseCore = 1000f; //ベースコア捜索範囲
    public Animator animator; //アニメーター
    public float enemySpeed = 5.0f;

    static public bool isAttack = false; //攻撃中フラグ

    bool lockOn = true; //ターゲット

    public float attackInterval = 0.5f; //次の攻撃までの時間

    float attackTimer; //攻撃可能になる時間
    float DistanceToPlayer; //プレイヤーとの距離
    float DistanceToBaseCore; //ベースコアとの距離


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

        targetBaseCore = GameManager.Instance.GetFoundBaseObjects().FirstOrDefault(obj => obj != null && obj.name == "BaseCore");

        if (targetBaseCore == null)
        {
            Debug.LogWarning("BaseCore がまだ登録されていません。StartCoroutineで待機します。");
            StartCoroutine(WaitForBaseCore());
        }
    }

    IEnumerator WaitForBaseCore()
    {
        while (GameManager.Instance.GetFoundBaseObjects().Count == 0)
        {
            yield return null; // 次のフレームまで待つ
        }

        targetBaseCore = GameManager.Instance.GetFoundBaseObjects()
            .FirstOrDefault(obj => obj != null && obj.name == "BaseCore");

        Debug.Log("BaseCore を取得しました: " + targetBaseCore.name);
    }



    // Update is called once per frame
    void Update()
    {
        if (targetBaseCore == null)
            return; // まだ BaseCore を取得していなければ何もしない

        //プレイヤーいなくなった時
        if (player == null)
        {
            return;
        }

        //プレイヤーとの距離
        DistanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        //ベースコアとの距離
        DistanceToBaseCore = Vector3.Distance(targetBaseCore.transform.position, transform.position);
        // Debug.Log("DistanceToBaseCore: " + DistanceToBaseCore); // 距離をログ出力

        //索敵範囲の時
        if (DistanceToPlayer <= detectionRange)
        {
            MovePlayer();
        }

        //ベースコアとの距離よりベースコアの索敵範囲が広い時
        else if (DistanceToBaseCore <= detectionRangeBaseCore)
        {
            MoveBaseCore();
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


    void MovePlayer()
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


    void MoveBaseCore()
    {
        //player索敵範囲外の時は拠点を取得しにいくもしくは攻撃しにいく
        if (GameManager.Instance.GetFoundBaseObjects().Count > 0)
        {

            // NavMeshAgentが停止状態であれば解除する
            if (navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = false;
            }

            if (lockOn)
            {
                //BaseCoreの方を向く (Y軸は固定して水平に回転)
                Vector3 lookAtPosition = targetBaseCore.transform.position;
                lookAtPosition.y = transform.position.y;
                transform.LookAt(lookAtPosition);
            }

            if (DistanceToBaseCore > 2.7f)
            {
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetBaseCore.transform.position);
                animator.SetBool("isRun", true);
            }

            else if (DistanceToBaseCore <= 2.7f)
            {
                navMeshAgent.isStopped = true;
                animator.SetBool("isRun", false);

                // 範囲内にプレイヤーのベースコアがいるとき
                if (CheckForSpecificTagInRadius())
                {
                    //攻撃中でないかつ前回の攻撃から0.5経過
                    if (!isAttack && Time.time >= attackTimer)
                        AttackCombo();
                }

            }
        }
    }

    //範囲内に対象タグを持つオブジェクトがいるかどうか
    bool CheckForSpecificTagInRadius()
    {
        // transform.positionを中心に、BaseDiscriminationの半径内でColliderを検出
        // detectionLayer は、検出したいオブジェクトが属するレイヤーのみを指定することで、不要なオブジェクトの検出を避ける
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, baseDiscrimination);

        //↑にした detectionLayer は、検出したいオブジェクトが属するレイヤーのみを指定することで、不要なオブジェクトの検出を避ける
        // Collider[] hitColliders = Physics.OverlapSphere(transform.position, BaseDiscrimination, detectionLayer);

        // 検出されたColliderの中に、targetTagを持つオブジェクトがあるかチェック
        foreach (Collider hitCollider in hitColliders)
        {
            // Rootオブジェクトのタグをチェックするなら (other.transform.root.tag)
            // 直接当たったオブジェクトのタグをチェックするなら (hitCollider.gameObject.tag)
            if (hitCollider.gameObject.CompareTag(targetTag))
            {
                // 特定のタグを持つオブジェクトが見つかった
                return true;
            }
        }
        // 特定のタグを持つオブジェクトが一つも見つからなかった
        return false;
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
