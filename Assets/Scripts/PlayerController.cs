using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed, RunSpeed;
    float x, z;
    Vector3 moving; // diff, Player_pos は不要になります
    Rigidbody rb;
    Animator animator;
    public float jumpPower = 10.0f; //ジャンプ力

    public Transform groundCheck; // 接地判定用のオブジェクト
    public LayerMask groundLayer; // 接地判定で判定する地面のレイヤー
    private bool isGrounded; // 接地しているかどうかのフラグ

    bool isJumping; //ジャンプフラグ

    public EnemyLeaderController enemyLeaderController; //スクリプト参照用のオブジェクト
    public EnemyController enemyController; //スクリプト参照用のオブジェクト

    public Transform cameraTransform; // ここにメインカメラのTransformを設定します

    //HP周り
    public bool isInvincible = false; // 無敵状態を表すフラグ
    public static int playerHP = 10;
    public float invincibilityDuration = 0.5f; //無敵時間

    public bool isUnderPlayer = false; //プレイヤーのところに集まるフラグ

    //音にまつわるコンポーネントとSE音情報
    AudioSource audio;
    public AudioClip se_attack;
    public AudioClip se_jump;
    public AudioClip se_footsteps;

    SwordAttack swordAttack;

    void Awake()
    {
        //リトライ時にHP初期化
        playerHP = 10;
    }
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        swordAttack = GetComponent<SwordAttack>();

        // cameraTransformが設定されていない場合は、メインカメラを自動で取得
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            // 現在の状態を反転させる (trueならfalseに、falseならtrueに)
            isUnderPlayer = !isUnderPlayer;
        }

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        // 入力に基づいて移動方向を計算
        Vector3 inputDirection = new Vector3(x, 0, z);

        // カメラのTransformを基準に移動方向を変換
        // カメラのY軸回転のみを考慮して、XZ平面での移動を計算します
        Vector3 forward = cameraTransform.forward;
        forward.y = 0; // Y成分を0にして、水平方向のベクトルにする
        forward.Normalize(); // 正規化

        Vector3 right = cameraTransform.right;
        right.y = 0; // Y成分を0にして、水平方向のベクトルにする
        right.Normalize(); // 正規化

        // カメラの向きに合わせて移動ベクトルを計算
        moving = (forward * inputDirection.z + right * inputDirection.x).normalized * Speed;

        // プレイヤーの向きを移動方向に向ける
        if (moving.magnitude > 0.01f) // わずかな移動でも向きを変えるように調整
        {
            // Y軸の回転のみを更新し、傾きをなくす
            Quaternion targetRotation = Quaternion.LookRotation(moving);
            targetRotation.x = 0; // X軸の回転をリセット
            targetRotation.z = 0; // Z軸の回転をリセット
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        }


        //アニメーション設定
        animator.SetBool("isWalk", x != 0 || z != 0);

        if (Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0))
        {
            moving = (forward * inputDirection.z + right * inputDirection.x).normalized * RunSpeed;
            animator.SetBool("isRun", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || (x == 0 && z == 0))
        {
            animator.SetBool("isRun", false);
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        if (isGrounded && !isJumping) //落下完了時
        {
            animator.SetBool("isJump", false);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            SEPlay(SEType.Jump); //ジャンプ音を鳴らす
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            isJumping = true; //ジャンプ中フラグをtrue
            StartCoroutine(OffJumping()); //ジャンプフラグを解除するコルーチンを呼び出す
        }
    }

    //ジャンプフラグを解除するコルーチン
    IEnumerator OffJumping()
    {
        yield return new WaitForSeconds(0.5f);
        isJumping = false;
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        // movingのY成分は0なので、既存のy速度を維持
        
        //攻撃中の時と攻撃中でない時
        if (swordAttack.playerIsAttack)
        {
            rb.linearVelocity = new Vector3(0, velocity.y, 0);//攻撃中は止まる
        }
        else
        {
            Vector3 moveVelocity = new Vector3(moving.x, velocity.y, moving.z);//攻撃中じゃなければ動く
            rb.linearVelocity = moveVelocity;
        }
    }

    void FootR()
    {
        //足音を鳴らす
        SEPlay(SEType.FootSteps);
    }

    void FootL()
    {
        //足音を鳴らす
        SEPlay(SEType.FootSteps);
    }

    void Hit()
    {
        //攻撃音を鳴らす
        SEPlay(SEType.Attack);
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
            playerHP--;
            Debug.Log("味方のHP " + playerHP);

            //無敵時間を開始するコルーチンを呼び出す
            StartCoroutine(SetInvincibilityTimer());
        }

        if (playerHP < 1)
        {
            Destroy(this.gameObject);
            GameManager.gameState = GameState.gameOver;
            SoundManager.instance.StopBgm(); //曲を止める
        }

        //playerベースに入った時HP回復
        if (other.gameObject.tag == "Player_Ba")
        {
            playerHP = 10;
        }
    }

    IEnumerator SetInvincibilityTimer()
    {
        //指定された時間だけ待機
        yield return new WaitForSeconds(invincibilityDuration);

        //時間が経過したら無敵状態を解除
        isInvincible = false;
    }

    public void SEPlay(SEType type)
    {
        switch (type)
        {
            case SEType.Attack:
                audio.PlayOneShot(se_attack);
                break;

            case SEType.Jump:
                audio.PlayOneShot(se_jump);
                break;

            case SEType.FootSteps:
                audio.PlayOneShot(se_footsteps);
                break;
        }
    }

}