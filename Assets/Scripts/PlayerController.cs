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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(diff), 0.2f);
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
        Debug.Log("攻撃ヒット");
    }
}