using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed, RunSpeed;
    float x, z;
    Vector3 moving, diff, Player_pos;
    Rigidbody rb;
    Animator animator;
    public float jumpPower = 10.0f;

    public Transform groundCheck; // 接地判定用のオブジェクト
    public LayerMask groundLayer; // 接地判定で判定する地面のレイヤー
    private bool isGrounded; // 接地しているかどうかのフラグ

    bool isJumping;



    //HP周り
    bool isInvincible = false; // 無敵状態を表すフラグ
    int enemyHP = 10;
    public float invincibilityDuration = 0.5f; //無敵時間

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Player_pos = GetComponent<Transform>().position;
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");

        diff = new Vector3(transform.position.x, transform.position.y, transform.position.z) - new Vector3(Player_pos.x, transform.position.y, Player_pos.z);

        Player_pos = transform.position;

        if (diff.magnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff), 0.2f);
        }

        moving = new Vector3(x, 0, z);
        moving = moving.normalized * Speed;

        //アニメーション設定
        animator.SetBool("isWalk", x != 0 || z != 0);

        if (Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0))
        {
            moving = moving.normalized * RunSpeed;

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
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);
            isJumping = true; //ジャンプ中フラグをtrue
            Invoke("OffJumping", 0.5f);

        }


    }

    //時間差でジャンプフラグを自然解除 ※後でこルーチンでもできるか試す
    void OffJumping()
    {
        isJumping = false;
    }

    void FixedUpdate()
    {
        Vector3 velocity = rb.linearVelocity;
        Vector3 moveVelocity = new Vector3(moving.x, velocity.y, moving.z);
        rb.linearVelocity = moveVelocity;
        //x, z は移動入力から更新
        //y は物理エンジン（重力・ジャンプ）任せ
        //つまり、Y軸方向の力（ジャンプ・落下）を維持したまま移動できる。
        //結果：空中でも自然にジャンプ・落下が動作する


        // rb.linearVelocity = moving;
        //この書き方だと、毎フレーム Y軸（上方向）も上書きされる
        //ジャンプで rb.AddForce(Vector3.up * jumpPower) を与えても、
        //次の FixedUpdate() で rb.velocity が moving に置き換えられ、上方向の速度（y成分）が消える！
        //結果：プレイヤーは一瞬だけ浮くけど、すぐ落ちる or ほぼジャンプしない。
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
        if (isInvincible || !EnemyLeaderController.isAttack) //無敵状態または敵が攻撃中でなければ何もしない
            return;


        if (other.gameObject.CompareTag("Sward"))
        {
            isInvincible = true; //ダメージを受けたら無敵
            enemyHP--;
            Debug.Log("味方のHP " + enemyHP);

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
}