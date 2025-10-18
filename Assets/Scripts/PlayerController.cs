using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed, RunSpeed;
    float x,z;
    Vector3 moving, diff, Player_pos;
    Rigidbody rb;
    Animator animator;
    public float jumpPower = 10.0f;

    public Transform groundCheck; // 接地判定用のオブジェクト
    public LayerMask groundLayer; // 接地判定で判定する地面のレイヤー
    private bool isGrounded; // 接地しているかどうかのフラグ


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
        if (isGrounded && rb.linearVelocity.y <= 0.01f) //落下完了時
        {
            animator.SetBool("isJump", false);
        }

 
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            animator.SetBool("isJump", true);

        }


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
        Debug.Log("攻撃ヒット");
    }
}