using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    public GameObject sword;
    Collider swordCollider;

    void Awake()
    {
        swordCollider = sword.GetComponent<Collider>();
        // swordCollider = GetComponent<Collider>(); //このスクリプトと同じGameObjectにあるコライダーを取得
        swordCollider.enabled =false; //最初はコライダーを無効にしておく
    }

    // Update is called once per frame
    public void EnableSwordCollider()
    {
        swordCollider.enabled = true;
        Debug.Log("剣のコライダーを有効にしました。");
    }
    public void DisableSwordCollider()
    {
        swordCollider.enabled = false;
        Debug.Log("剣のコライダーを無効にしました。");
    }
}
