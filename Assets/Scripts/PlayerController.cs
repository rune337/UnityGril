using System.Collections;
using System.Collections.Generic;
using RPGCharacterAnims.Actions;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed, RunSpeed;
    float x, z;
    Vector3 moving, diff, Player_pos;
    Rigidbody rb;
    Animator animator;

    float clickCount = 0; //ゲームが開始されてからの経過時間（秒）
    float lastClickTime = 0f; //前回クリックしたときの Time.time を記録しておく変数
    float clickMaxDelay = 1.0f; //クリックの猶予時間

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

        diff = new Vector3(transform.position.x, 0, transform.position.z)
            - new Vector3(Player_pos.x, 0, Player_pos.z);

        Player_pos = transform.position;

        if (diff.magnitude > 0.001f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(diff), 0.2f);
        }

        moving = new Vector3(x, 0, z);
        moving = moving.normalized * Speed;

        animator.SetBool("isWalk", x != 0 || z != 0);

        if (Input.GetKey(KeyCode.LeftShift) && (x != 0 || z != 0))
        {
            moving = moving.normalized * RunSpeed;

            animator.SetBool("isRun", true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || (x == 0 && z == 0))
        {
            animator.SetBool("isRun", false);
        }

        //攻撃のアニメーション
        if (Input.GetMouseButtonDown(0))
        {

            AttackCombo();
            // Debug.Log(clickCount);
            Invoke("AttackEnd", 0.5f);
        }

    }

    void FixedUpdate()
    {
        rb.linearVelocity = moving;
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


    void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        animator.SetFloat("Attack", 0f);
    }

    void AttackCombo()
    {
        float timeSinceLastClick = Time.time - lastClickTime; //前回クリックしてからの経過時間
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
    }


}