using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

public class AllyController : MonoBehaviour
{
    Animator animator;

    GameObject player;
    GameObject[] enemy; //敵配列、敵は複数
    GameObject enemyLeader; //敵リーダ変数,敵リーダーは1人なので変数

    float distanceToEnemyLeader; //敵リーダとの距離
    float[] distanceEnemy; //敵との距離

    GameObject closestEnemy; //一番近い敵オブジェクト
    float closestEnemyDistance; //一番近い敵との距離

    GameObject closestBaseCore; //一番近いベースコアオブジェクト
    float closestBaseCoreDistance; //一番近いベースコアとの距離

    GameObject closestEnemyBaseCore; //一番近い敵ベースコアオブジェクト
    float closestEnemyBaseCoreDistance; //一番近い敵ベースコアオブジェクトとの距離

    GameObject closestPlayerBaseCore; //一番近い敵ベースコアオブジェクト
    float closestPlayerBaseCoreDistance; //一番近い敵ベースコアオブジェクトとの距離


    float lastAttackTime = 0f; //前回攻撃したときのTime.timeを記録しておく変数
    float comboResetTime = 1.0f; //攻撃の猶予時間
    float attackTimer; //攻撃可能になる時間
    float attackInterval = 0.5f; //次の攻撃までの時間
    bool allyIsAttack = false; //攻撃中フラグ
    float attackCount; //攻撃回数 (コンボカウント)
    bool isInvincible = false; // 無敵状態を表すフラグ
    float invincibilityDuration = 0.5f; //無敵時間

    float detectionRange = 30f; //敵・敵リーダー索敵範囲
    float stopRange = 2f; //停止距離
    float baseStopRange = 8.0f; //拠点サーバコア停止距離
    float baseCoreDetectionRange = 1000f; //コア探索範囲
    float playerRange = 1000f; //プレイヤー探索範囲

    NavMeshAgent navMeshAgent; //NavMeshAgentコンポーネント
    public float allySpeed = 5.0f; //移動速度

    public SwordCollider swordCollider;

    bool lockOn = true;

    float allyHP = 10;


    //敵オブジェクトと距離を紐付けるクラス
    public class EnemyDistance
    {
        public GameObject enemyObject;
        public float enemyDistance;

        //コンストラクタ
        public EnemyDistance(GameObject obj, float dist)
        {
            enemyObject = obj;
            enemyDistance = dist;
        }

    }

    //ベースコアと距離を結びつけるクラス
    public class BaseCoreDistanceInfo
    {
        public GameObject baseCoreObject;
        public float baseCoreDistance;

        //コンストラクタ
        public BaseCoreDistanceInfo(GameObject obj, float dist)
        {
            baseCoreObject = obj;
            baseCoreDistance = dist;
        }
    }

    //敵ベースコアと距離を結びつけるクラス
    public class EnemyBaseCoreDistanceInfo
    {
        public GameObject enemyBaseCoreObject; // GameObject は参照型なので、ここに格納されるのは参照
        public float enemyBaseCoreDistance;

        // コンストラクタ
        public EnemyBaseCoreDistanceInfo(GameObject obj, float dist)
        {
            enemyBaseCoreObject = obj;
            enemyBaseCoreDistance = dist;
        }

        // デバッグ用
        // public override string ToString()
        // {
        //     string name = enemyBaseCoreObject != null ? enemyBaseCoreObject.name : "null";
        //     return $"BaseCore: {name}, Distance: {enemyBaseCoreDistance:F2}";
        // }

    }

    //味方ベースコアと距離を結びつけるクラス
    public class PlayerBaseCoreDistanceInfo
    {
        public GameObject playerBaseCoreObject; // GameObject は参照型なので、ここに格納されるのは参照
        public float playerBaseCoreDistance;

        // コンストラクタ
        public PlayerBaseCoreDistanceInfo(GameObject obj, float dist)
        {
            playerBaseCoreObject = obj;
            playerBaseCoreDistance = dist;
        }

    }

    //敵の距離クラス型を使って敵の距離リスト定義
    private List<EnemyDistance> enemyDistance = new List<EnemyDistance>();

    //ベースコア距離クラスを使ってベースコアの距離リスト定義
    private List<BaseCoreDistanceInfo> baseCoreDistanceInfo = new List<BaseCoreDistanceInfo>();

    //敵ベースコア距離クラス型を使って敵ベースコアの距離リストを定義
    private List<EnemyBaseCoreDistanceInfo> enemyBaseCoreDistanceInfo = new List<EnemyBaseCoreDistanceInfo>();

    //味方ベースコア距離クラス型を使って敵ベースコアの距離リストを定義
    private List<PlayerBaseCoreDistanceInfo> playerBaseCoreDistanceInfo = new List<PlayerBaseCoreDistanceInfo>();

    void Awake()
    {
        //リトライ時にHP初期化
        allyHP = 10;
    }

    //スタート処理
    void Start()
    {
        attackTimer = Time.time;
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyLeader = GameObject.FindGameObjectWithTag("Enemy_Leader");
        enemy = GameObject.FindGameObjectsWithTag("Enemy");
        player = GameObject.FindGameObjectWithTag("Player");


    }

    //メイン処理
    void Update()
    {
        //敵の数は変化するのでマイフレーム取得
        enemy = GameObject.FindGameObjectsWithTag("Enemy");


        //行動順序
        //1 敵が範囲にいる
        //2 敵リーダが範囲にいる
        //3 Base Core(フリー拠点サーバー)が範囲にいる
        //4 Enemy_Ba(敵拠点サーバー)が範囲にいる
        //5 プレイヤーについていく
        //6 Player_Ba(味方拠点サーバー)が範囲にいる →これないと味方拠点だけになった時に止まれなくなる


        //敵がいる時
        if (enemy.Length != 0)
        {
            enemyDistance.Clear();
            distanceEnemy = new float[enemy.Length]; //敵の距離を入れる配列の初期化

            for (int i = 0; i < enemy.Length; i++)
            {
                distanceEnemy[i] = Vector3.Distance(enemy[i].transform.position, transform.position);
                enemyDistance.Add(new EnemyDistance(enemy[i], distanceEnemy[i]));
                if (enemyDistance.Count != 0)
                {
                    closestEnemy = ClosestObject(enemyDistance);
                    closestEnemyDistance = Vector3.Distance(closestEnemy.transform.position, transform.position);

                    if (closestEnemyDistance <= detectionRange)
                    {
                        Move(closestEnemy, closestEnemyDistance);
                        return;
                    }
                }

            }
        }

        //敵リーダーがいる時
        if (enemyLeader != null)
        {
            distanceToEnemyLeader = Vector3.Distance(enemyLeader.transform.position, transform.position);
            if (distanceToEnemyLeader <= detectionRange)
            {
                Move(enemyLeader, distanceToEnemyLeader);
                return;
            }
        }

        //ベースコアがいる時
        if (GameManager.Instance.GetFoundBaseObjects().Count != 0) //nullで空ではなく0になるので
        {
            baseCoreDistanceInfo.Clear();

            for (int i = 0; i < GameManager.Instance.GetFoundBaseObjects().Count; i++)
            {
                float distance = Vector3.Distance(GameManager.Instance.GetFoundBaseObjects()[i].transform.position, transform.position);
                baseCoreDistanceInfo.Add(new BaseCoreDistanceInfo(GameManager.Instance.GetFoundBaseObjects()[i], distance));
            }

            if (baseCoreDistanceInfo.Count != 0)
            {
                closestBaseCore = ClosestObject(baseCoreDistanceInfo);
                closestBaseCoreDistance = Vector3.Distance(closestBaseCore.transform.position, transform.position);
                Debug.Log(closestBaseCore);
                // Debug.Log(closestBaseCoreDistance);
                if (closestBaseCoreDistance <= baseCoreDetectionRange)
                {
                    MoveCore(closestBaseCore, closestBaseCoreDistance);
                    return;
                }
            }
        }

        //敵ベースコアがいる時
        if (GameManager.Instance.EnemyGetFoundBaseObjects().Count != 0) //nullで空ではなく0になるので
        {
            enemyBaseCoreDistanceInfo.Clear();

            for (int i = 0; i < GameManager.Instance.EnemyGetFoundBaseObjects().Count; i++)
            {
                float distance = Vector3.Distance(GameManager.Instance.EnemyGetFoundBaseObjects()[i].transform.position, transform.position);
                enemyBaseCoreDistanceInfo.Add(new EnemyBaseCoreDistanceInfo(GameManager.Instance.EnemyGetFoundBaseObjects()[i], distance));
            }

            if (enemyBaseCoreDistanceInfo.Count != 0)
            {
                Debug.Log("敵コア");
                closestEnemyBaseCore = ClosestObject(enemyBaseCoreDistanceInfo);
                closestEnemyBaseCoreDistance = Vector3.Distance(closestEnemyBaseCore.transform.position, transform.position);
                Debug.Log(closestEnemyBaseCore);
                Debug.Log(closestEnemyBaseCoreDistance);
                if (closestEnemyBaseCoreDistance <= baseCoreDetectionRange)
                {
                    MoveCore(closestEnemyBaseCore, closestEnemyBaseCoreDistance);
                    return;
                }
            }
        }

        //プレイヤーがいる時
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= playerRange)
            {
                Move(player, distanceToPlayer);
                return;
            }
        }


        //味方に近づいた時に止めないと全部拠点制圧して入ってきた味方止まるところないから
        //味方ベースコアがいる時
        if (GameManager.Instance.PlayerGetFoundBaseObjects().Count != 0)
        {
            Debug.Log("味方コア");
            playerBaseCoreDistanceInfo.Clear();

            for (int i = 0; i < GameManager.Instance.PlayerGetFoundBaseObjects().Count; i++)
            {
                float distance = Vector3.Distance(GameManager.Instance.PlayerGetFoundBaseObjects()[i].transform.position, transform.position);
                playerBaseCoreDistanceInfo.Add(new PlayerBaseCoreDistanceInfo(GameManager.Instance.PlayerGetFoundBaseObjects()[i], distance));
            }

            if (playerBaseCoreDistanceInfo.Count != 0)
            {
                closestPlayerBaseCore = ClosestObject(playerBaseCoreDistanceInfo);
                closestPlayerBaseCoreDistance = Vector3.Distance(closestPlayerBaseCore.transform.position, transform.position);
                Debug.Log(closestPlayerBaseCore);
                Debug.Log(closestPlayerBaseCoreDistance);
                if (closestPlayerBaseCoreDistance <= baseCoreDetectionRange)
                {
                    MoveCore(closestPlayerBaseCore, closestPlayerBaseCoreDistance);
                    return;
                }
            }
        }



    }

    //オブジェクトへ移動するメソッド
    void Move(GameObject obj, float distance)
    {
        if (lockOn)
        {
            //オブジェクトの方を向く
            transform.LookAt(obj.transform.position);
        }

        //オブジェクトとの距離が停止距離より遠い時
        if (distance >= stopRange)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(obj.transform.position);
            animator.SetBool("isRun", true);
        }
        //オブジェクトとの距離が停止距離より近い時
        else if (distance < stopRange)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("isRun", false);
            //攻撃中でないかつ前回の攻撃から0.5経過
            if (!allyIsAttack && Time.time >= attackTimer && obj != player)
                AttackCombo();
        }
    }

    //拠点サーバへ移動するメソッド
    void MoveCore(GameObject obj, float distance)
    {
        //ベースコア攻撃判別フラグ
        bool playerBase = false;
        if (obj.tag.Contains("Player_Ba"))
            playerBase = true;

        //オブジェクトとの距離が停止距離より遠い時
        if (distance >= baseStopRange)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(obj.transform.position);
            animator.SetBool("isRun", true);
        }
        //オブジェクトとの距離が停止距離より近い時
        else if (distance < baseStopRange)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("isRun", false);
            //攻撃中でないかつ前回の攻撃から0.5経過
            if (!playerBase)
            {
                if (!allyIsAttack && Time.time >= attackTimer)
                    AttackCombo();
            }
        }
    }


    //攻撃メソッド
    void AttackCombo()
    {
        Debug.Log("ここ");

        allyIsAttack = true;

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
        animator.SetFloat("AllyAttack", attackCount);
        swordCollider.EnableSwordCollider(); //剣のコライダーを有効にするのを呼び出す

        //攻撃インターバル
        attackTimer = Time.time + attackInterval;
        lastAttackTime = Time.time; //攻撃が終わった時間を記録

        Invoke("AttackEnd", 0.5f);
    }

    //攻撃終了メソッド
    void AttackEnd()
    {
        animator.SetFloat("AllyAttack", 0f);
        allyIsAttack = false;

        if (swordCollider != null)
            swordCollider.DisableSwordCollider();
    }


    //ダメージ処理
    void OnTriggerEnter(Collider other)
    {
        if (isInvincible) //無敵状態
        {

            return;

        }

        if (other.gameObject.CompareTag("EnemySword"))
        {
            isInvincible = true; //ダメージを受けたら無敵
            allyHP--;
            Debug.Log("敵のHP " + allyHP);

            //無敵時間を開始するコルーチンを呼び出す
            StartCoroutine(SetInvincibilityTimer());

        }

        if (allyHP < 1)
        {
            Destroy(this.gameObject);
        }
    }

    //オブジェクト破壊時に作動するよう元々定義されているメソッドOnDestroy
    private void OnDestroy()
    {
        //GameManagerが存在しない、またはアプリ終了中なら何もしない
        if (GameManager.Instance == null || GameManager.Instance.IsQuitting)
            return;

        //プレイ中のみ生成
        if (GameManager.gameState == GameState.playing)
        {
            // 味方が倒れたら条件に応じて味方を生成するメソッドを呼び出す
            GameManager.Instance.OnAllyDestroyed();
        }
    }

    //無敵時間
    IEnumerator SetInvincibilityTimer()
    {
        //指定された時間だけ待機
        yield return new WaitForSeconds(invincibilityDuration);

        //時間が経過したら無敵状態を解除
        isInvincible = false;

    }


    void FootR() { /* 足音処理など */ }
    void FootL() { /* 足音処理など */ }
    void Hit() { /* 攻撃ヒット時の処理など */ }

    //以下ClosestObjectのオーバロードで同じメソッドを複数
    //オーバーロード最も近い敵のオブジェクトを返す
    public GameObject ClosestObject(List<EnemyDistance> enemyDistance)
    {
        if (enemyDistance.Count == 0)
        {
            return null;
        }

        //リストの最初の要素を最も近い値として定義
        EnemyDistance firstInfo = enemyDistance[0];

        // 2番目の要素から最後までループして比較
        for (int i = 1; i < enemyDistance.Count; i++)
        {
            if (enemyDistance[i].enemyDistance < firstInfo.enemyDistance)
            {
                // より近いものが見つかったら更新
                firstInfo = enemyDistance[i];
            }
        }
        return firstInfo.enemyObject;
    }

    //オーバーロード最も近いベースコアのオブジェクトを返す
    public GameObject ClosestObject(List<BaseCoreDistanceInfo> baseCoreDistanceInfo)
    {
        Debug.Log("ベースコアのオーバーロード");
        if (baseCoreDistanceInfo.Count == 0)
        {
            return null;
        }
        //リストの最初の要素を最も近い値として定義
        BaseCoreDistanceInfo firstInfo = baseCoreDistanceInfo[0];

        // 2番目の要素から最後までループして比較
        for (int i = 1; i < baseCoreDistanceInfo.Count; i++)
        {
            if (baseCoreDistanceInfo[i].baseCoreDistance < firstInfo.baseCoreDistance)
            {
                // より近いものが見つかったら更新
                firstInfo = baseCoreDistanceInfo[i];
            }
        }
        return firstInfo.baseCoreObject;
    }

    //オーバーロード最も近い敵ベースコアのオブジェクトを返す
    public GameObject ClosestObject(List<EnemyBaseCoreDistanceInfo> enemyBaseCoreDistanceInfo)
    {
        Debug.Log("敵ベースコアのオーバーロード");
        if (enemyBaseCoreDistanceInfo.Count == 0)
        {
            return null;
        }
        //リストの最初の要素を最も近い値として定義
        EnemyBaseCoreDistanceInfo firstInfo = enemyBaseCoreDistanceInfo[0];

        // 2番目の要素から最後までループして比較
        for (int i = 1; i < enemyBaseCoreDistanceInfo.Count; i++)
        {
            if (enemyBaseCoreDistanceInfo[i].enemyBaseCoreDistance < firstInfo.enemyBaseCoreDistance)
            {
                // より近いものが見つかったら更新
                firstInfo = enemyBaseCoreDistanceInfo[i];
            }
        }
        return firstInfo.enemyBaseCoreObject;
    }

    //オーバーロード最も近い味方ベースコアのオブジェクトを返す
    public GameObject ClosestObject(List<PlayerBaseCoreDistanceInfo> playerBaseCoreDistanceInfo)
    {

        if (playerBaseCoreDistanceInfo.Count == 0)
        {
            return null;
        }
        //リストの最初の要素を最も近い値として定義
        PlayerBaseCoreDistanceInfo firstInfo = playerBaseCoreDistanceInfo[0];

        // 2番目の要素から最後までループして比較
        for (int i = 1; i < playerBaseCoreDistanceInfo.Count; i++)
        {
            if (playerBaseCoreDistanceInfo[i].playerBaseCoreDistance < firstInfo.playerBaseCoreDistance)
            {
                // より近いものが見つかったら更新
                firstInfo = playerBaseCoreDistanceInfo[i];
            }
        }
        return firstInfo.playerBaseCoreObject;
    }
    //オーバーロードここまで


    //デバッグ表示
    void OnDrawGizmos()
    {
        if (navMeshAgent != null && navMeshAgent.hasPath)
        {
            Gizmos.color = Color.blue;
            Vector3[] pathCorners = navMeshAgent.path.corners;
            for (int i = 0; i < pathCorners.Length - 1; i++)
            {
                Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
            }
            Gizmos.DrawSphere(navMeshAgent.destination, 0.2f);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);
    }

}

