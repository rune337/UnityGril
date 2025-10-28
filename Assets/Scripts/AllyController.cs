using System.Collections;
using System.Threading;
using RPGCharacterAnims.Actions;
using RPGCharacterAnims.Extensions;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AllyController : MonoBehaviour
{
    public Animator animator; //アニメーター

    private NavMeshAgent navMeshAgent; //NavMeshAgentコンポーネント
    GameObject enemy;
    GameObject enemyLeader;
    GameObject closestObject; //一番近いベースコア
    GameObject enemyClosestObject; //一番近いプレイヤーベースコア

    public float attackRange = 0.5f; //攻撃を開始する距離
    public float stopRange = 2f; //接近限界距離

    public float baseDiscrimination = 10f; //Base判別範囲
    public string targetTag = "Enemy_Ba"; //Base判別範囲でターゲットとするベースコアのタグ
    public float detectionRange = 80f; //索敵範囲
    public float detectionRangeBaseCore = 1000f; //ベースコア捜索範囲

    public float enemySpeed = 5.0f;

    bool lockOn = true; //ターゲット //壁にぶつかるのでコメントアウト

    float DistanceToEnemy; //敵との距離
    float DistanceToEnemyLeader; //敵リーダとの距離
    float distanceToClosestObject; //一番近いベースコアとの距離
    float EnemyDistanceToClosestObject; //一番近いベースコアとの距離

    float lastAttackTime = 0f; //前回攻撃したときのTime.timeを記録しておく変数
    float comboResetTime = 1.0f; //攻撃の猶予時間
    float attackTimer; //攻撃可能になる時間
    public float attackInterval = 0.5f; //次の攻撃までの時間
    public bool allyIsAttack = false; //攻撃中フラグ
    float attackCount; //enemyの攻撃回数
    bool isInvincible = false; // 無敵状態を表すフラグ

    public List<GameObject> baseCoreList; //GameManagerから引っ張ってきたリストBaseCoreをリスト
    public List<GameObject> EnemyBaseCoreList; //GameManagerから引っ張ってきたリストEnemy_Baをリスト


    public EnemyController enemyController; //スクリプト参照用のスクリプトのついているオブジェクトをアタッチする変数
    public EnemyLeaderController enemyLeaderController;

    //HP周り
    int allyHP = 10;
    public float invincibilityDuration = 0.5f; //無敵時間

    public SwordCollider swordCollider; //剣をアタッチ、剣にアタッチしている剣のコライダーを有効にするスクリプトを参照するため

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //ベースコアとの距離とオブジェクトを紐づけるクラスを宣言
    public class BaseCoreDistanceInfo
    {
        public GameObject baseCoreObject; // GameObject は参照型なので、ここに格納されるのは参照
        public float distance;

        // コンストラクタ
        public BaseCoreDistanceInfo(GameObject obj, float dist)
        {
            baseCoreObject = obj;
            distance = dist;
        }

        // デバッグ用
        public override string ToString()
        {
            string name = baseCoreObject != null ? baseCoreObject.name : "null";
            return $"BaseCore: {name}, Distance: {distance:F2}";
        }

    }


    //プレイヤーベースコアとの距離とオブジェクトを紐づけるクラスを宣言
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
        public override string ToString()
        {
            string name = enemyBaseCoreObject != null ? enemyBaseCoreObject.name : "null";
            return $"BaseCore: {name}, Distance: {enemyBaseCoreDistance:F2}";
        }

    }

    private List<BaseCoreDistanceInfo> allBaseCoreDistances = new List<BaseCoreDistanceInfo>(); //ベースコアリスト定義
    private List<EnemyBaseCoreDistanceInfo> EnemyAllBaseCoreDistances = new List<EnemyBaseCoreDistanceInfo>(); //敵ベースコアリスト定義


    void Start()
    {
        attackTimer = Time.time;
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemy = GameObject.FindGameObjectWithTag("Enemy");
        enemyLeader = GameObject.FindGameObjectWithTag("Enemy_Leader");
        animator = GetComponent<Animator>();

        baseCoreList = GameManager.Instance.GetFoundBaseObjects(); //スタート時にリストにリストを入れる
        swordCollider = GetComponentInChildren<SwordCollider>(); //インスペクターでアタッチしてもnullになることあるので自動取得


        if (baseCoreList == null || baseCoreList.Count == 0)
        {
            Debug.LogWarning("BaseCore がまだ登録されていません。StartCoroutineで待機します。");
            StartCoroutine(WaitForBaseCoreAndPopulateDistances());
        }
        else
        {
            // Start時点でベースコアがある場合はここで距離情報を生成
            PopulateAllBaseCoreDistances();
        }

    }

    //スタート時とベースコア到達時にリストリセットするためのコルーチン
    IEnumerator WaitForBaseCoreAndPopulateDistances()
    {
        while (GameManager.Instance.GetFoundBaseObjects().Count == 0)
        {
            yield return null; // 次のフレームまで待つ
        }

        //配列に入れて順番に距離を算出していく
        baseCoreList.Clear();
        baseCoreList = GameManager.Instance.GetFoundBaseObjects();
        Debug.Log("BaseCore リストを GameManager から取得しました。");

        // コルーチンで待機後に距離情報を生成する
        PopulateAllBaseCoreDistances();
    }

    // allBaseCoreDistances に距離情報を生成する新しいメソッド
    void PopulateAllBaseCoreDistances()
    {
        if (baseCoreList == null || baseCoreList.Count == 0)
        {
            // Debug.LogWarning("PopulateAllBaseCoreDistances: baseCoreList が null または空のため、距離情報を生成できません。");
            return;
        }

        allBaseCoreDistances.Clear(); // ！！毎回クリアすることが非常に重要！！

        for (int i = 0; i < baseCoreList.Count; i++)
        {
            if (baseCoreList[i] == null)
            {
                Debug.LogWarning($"BaseCoreList[{i}] が null です。スキップします。");
                continue;
            }

            float distance = Vector3.Distance(baseCoreList[i].transform.position, transform.position);
            allBaseCoreDistances.Add(new BaseCoreDistanceInfo(baseCoreList[i], distance));
            //Debug.Log($"距離情報追加: {baseCoreList[i].name}, 距離: {distance}");
        }
        // Debug.Log($"PopulateAllBaseCoreDistances 完了。要素数: {allBaseCoreDistances.Count}");
    }



    // Update is called once per frame
    void Update()
    {
        EnemyBaseCoreList = GameManager.Instance.EnemyGetFoundBaseObjects(); //リストにenemy_Baのリストを入れる
        //  if (baseCoreList.Count <= 0)
        //  {
        //      allBaseCoreDistances.Clear();
        //  }

        //ベースコアがリストにいなければ止める
        if (GameManager.Instance.GetFoundBaseObjects().Count <= 0 || GameManager.Instance.EnemyGetFoundBaseObjects().Count <= 0)
        {
            navMeshAgent.isStopped = true;
            animator.SetBool("isRun", false);
        }

        if (baseCoreList == null)
            return; // まだ BaseCore を取得していなければ何もしない

        //敵がいなくなった時
        if (enemy == null || enemyLeader == null)
        {
            return;
        }

        //敵との距離
        DistanceToEnemy = Vector3.Distance(enemy.transform.position, transform.position);
        DistanceToEnemyLeader = Vector3.Distance(enemyLeader.transform.position, transform.position);


        // Debug.Log(ClosestBaseCoreObject()); //一番近いベースコアオブジェクト
        closestObject = ClosestBaseCoreObject();
        if (closestObject != null)
        {
            distanceToClosestObject = Vector3.Distance(closestObject.transform.position, transform.position); //一番近いベースコアオブジェクトとの距離
            // Debug.Log(distanceToClosestObject);

            //ベースコアとの距離よりベースコアの索敵範囲が広い時
            if (distanceToClosestObject <= detectionRangeBaseCore)
            {
                MoveBaseCore(closestObject);
            }

        }


        if (GameManager.Instance.EnemyGetFoundBaseObjects().Count > 0)
        {
            for (int i = 0; i < EnemyBaseCoreList.Count; i++)
            {
                float distance = Vector3.Distance(EnemyBaseCoreList[i].transform.position, transform.position);
                EnemyAllBaseCoreDistances.Add(new EnemyBaseCoreDistanceInfo(EnemyBaseCoreList[i], distance));
            }

            enemyClosestObject = EnemyClosestBaseCoreObject(); //一番近いプレイヤーベースコアオブジェクトを算出
            EnemyDistanceToClosestObject = Vector3.Distance(enemyClosestObject.transform.position, transform.position); ////一番近いプレイヤーベースコアオブジェクトとの距離を算出

            //プレイヤーベースコアとの距離よりベースコアの索敵範囲が広い時
            if (EnemyDistanceToClosestObject <= detectionRangeBaseCore)
            {
                MovePlayerBaseCore(enemyClosestObject);
            }


        }

        if (DistanceToEnemyLeader <= detectionRange)
        {
            MoveEnemy();
        }

        // ★デバッグログを追加
        // if (closestObject == null)
        // {
        //     Debug.LogError("Update: ClosestBaseCoreObject は null でした。この後の処理はスキップされます。");
        //     return;
        // }

        //索敵範囲の時

        // foreach (var info in allBaseCoreDistances)
        // {
        //     Debug.Log(info); // ToString() が自動的に呼ばれる
        // }
    }

    void AttackCombo()
    {

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



    void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        animator.SetFloat("AllyAttack", 0f);
        allyIsAttack = false;
        swordCollider.DisableSwordCollider(); //剣のコライダーを無効にするのを呼び出す
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

    IEnumerator SetInvincibilityTimer()
    {
        //指定された時間だけ待機
        yield return new WaitForSeconds(invincibilityDuration);

        //時間が経過したら無敵状態を解除
        isInvincible = false;

    }


    void MoveEnemy()
    {
        //これを書かないとプレイヤーと逆方向に走って行ってしまう //壁にぶつかることあるのでコメントアウト
        if (lockOn)
        {
            //プレイヤーの方を向く
            transform.LookAt(enemyLeader.transform.position);
        }

        //敵との距離が接近限界以上の時
        if (DistanceToEnemyLeader >= stopRange)
        {
            navMeshAgent.SetDestination(enemyLeader.transform.position);
            animator.SetBool("isRun", true);
            Debug.Log("Run");
        }

        //敵との距離が接近距離より小さい時
        else if (DistanceToEnemyLeader < stopRange)
        {
            Debug.Log("attack");
            navMeshAgent.isStopped = true;
            animator.SetBool("isRun", false);

            //攻撃中でないかつ前回の攻撃から0.5経過
            if (!allyIsAttack && Time.time >= attackTimer)
                AttackCombo();
        }
    }


    void MoveBaseCore(GameObject closestObject)
    {

        //敵索敵範囲外の時は拠点を取得しにいくもしくは攻撃しにいく
        if (GameManager.Instance.GetFoundBaseObjects().Count > 0)
        {
            // NavMeshAgentが停止状態であれば解除する
            if (navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = false;
            }

            // if (lockOn)　//壁にぶつかるのでコメントアウト
            // {
            //     //BaseCoreの方を向く (Y軸は固定して水平に回転)
            //     Vector3 lookAtPosition = closestObject.transform.position;
            //     lookAtPosition.y = transform.position.y;
            //     transform.LookAt(lookAtPosition);
            // }

            if (distanceToClosestObject > 2.7f)
            {
                Debug.Log(distanceToClosestObject);
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(closestObject.transform.position);
                animator.SetBool("isRun", true);
            }

            else if (distanceToClosestObject <= 2.7f)
            {
                Debug.Log("呼び出し");
                navMeshAgent.isStopped = true;
                animator.SetBool("isRun", false);

                //GameManagerの大元のリストを参照して、リストに要素がある時
                if (GameManager.Instance.GetFoundBaseObjects().Count > 0)
                {
                    StartCoroutine(WaitForBaseCoreAndPopulateDistances()); //すでにBaseからtagが変更されたリストを対象から外すためにリストをクリア、再設定するコルーチンを呼び出す
                    navMeshAgent.isStopped = false;
                    animator.SetBool("isRun", true);//まだベースコアがいれば次なる拠点へ向かうために停止を解除する
                }



                //GameManagerの大元のリストを参照して、リストに要素がない時
                // 範囲内にプレイヤーのベースコアがいるとき
                if (CheckForSpecificTagInRadius())
                {
                    //攻撃中でないかつ前回の攻撃から0.5経過
                    if (!allyIsAttack && Time.time >= attackTimer)
                        AttackCombo();
                }

            }

        }
    }


    void MovePlayerBaseCore(GameObject enemyClosestObject)
    {

        //player索敵範囲外の時は拠点を取得しにいくもしくは攻撃しにいく
        if (GameManager.Instance.EnemyGetFoundBaseObjects().Count > 0)
        {
            // NavMeshAgentが停止状態であれば解除する
            if (navMeshAgent.isStopped)
            {
                navMeshAgent.isStopped = false;
            }

            // if (lockOn) //壁にぶつかるのでコメントアウト
            // {
            //     //BaseCoreの方を向く (Y軸は固定して水平に回転)
            //     Vector3 lookAtPosition = enemyClosestObject.transform.position;
            //     lookAtPosition.y = transform.position.y;
            //     transform.LookAt(lookAtPosition);
            // }

            if (EnemyDistanceToClosestObject > 2.7f)
            {
                Debug.Log(EnemyDistanceToClosestObject);
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(enemyClosestObject.transform.position);
                animator.SetBool("isRun", true);
            }

            else if (EnemyDistanceToClosestObject <= 2.7f)
            {
                Debug.Log("呼び出し");
                navMeshAgent.isStopped = true;
                animator.SetBool("isRun", false);

                //GameManagerの大元のリストを参照して、リストに要素がある時
                if (GameManager.Instance.EnemyGetFoundBaseObjects().Count > 0)
                {

                    EnemyBaseCoreList.Clear(); //すでにBaseからtagが変更されたリストを対象から外すためにPlayerBaseCoreのリストをクリア
                    EnemyAllBaseCoreDistances.Clear(); //PlayerBaseCoreはいないのでPlayerBaseCoreと距離の紐付けのクラスもクリア
                    navMeshAgent.isStopped = false;
                    animator.SetBool("isRun", true);//まだベースコアがいれば次なる拠点へ向かうために停止を解除する
                }

                //GameManagerの大元のリストを参照して、リストに要素がない時
                // 範囲内にプレイヤーのベースコアがいるとき
                if (CheckForSpecificTagInRadius())
                {
                    //攻撃中でないかつ前回の攻撃から0.5経過
                    if (!allyIsAttack && Time.time >= attackTimer)
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

    //最も近いベースコアオブジェクトを見つけるメソッド
    public GameObject ClosestBaseCoreObject()
    {
        if (allBaseCoreDistances == null || allBaseCoreDistances.Count == 0)
        {
            // Debug.LogWarning("ClosestBaseCoreObject: 距離情報リストが空です。");
            return null; // 要素がない場合は null を返す
        }

        // 最初の要素を仮の最小値として設定
        BaseCoreDistanceInfo firstInfo = allBaseCoreDistances[0];
        // 2番目の要素から最後までループして比較
        for (int i = 1; i < allBaseCoreDistances.Count; i++)
        {
            if (allBaseCoreDistances[i].distance < firstInfo.distance)
            {
                // より近いものが見つかったら更新
                firstInfo = allBaseCoreDistances[i];
            }
        }

        // Debug.Log(firstInfo.baseCoreObject);
        return firstInfo.baseCoreObject;

    }

    //最も近いプレイヤーベースコアオブジェクトを見つけるメソッド
    public GameObject EnemyClosestBaseCoreObject()
    {
        if (EnemyAllBaseCoreDistances == null || EnemyAllBaseCoreDistances.Count == 0)
        {
            // Debug.LogWarning("ClosestBaseCoreObject: 距離情報リストが空です。");
            return null; // 要素がない場合は null を返す
        }

        // 最初の要素を仮の最小値として設定
        EnemyBaseCoreDistanceInfo firstInfo = EnemyAllBaseCoreDistances[0];
        // 2番目の要素から最後までループして比較
        for (int i = 1; i < EnemyAllBaseCoreDistances.Count; i++)
        {
            if (EnemyAllBaseCoreDistances[i].enemyBaseCoreDistance < firstInfo.enemyBaseCoreDistance)
            {
                // より近いものが見つかったら更新
                firstInfo = EnemyAllBaseCoreDistances[i];
            }
        }

        // Debug.Log(firstInfo.baseCoreObject);
        return firstInfo.enemyBaseCoreObject;

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

        //プレイヤー索敵範囲
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        //停止範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRange);

        //BaseCore索敵範囲
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRangeBaseCore);
    }
}
