using UnityEngine;

public class SlashEffect : MonoBehaviour
{
    
    public GameObject slashVFXPrefab; //Prefab変数
    public Transform sword; // 剣のオブジェクト変数
    
    // エフェクトのインスタンスを保持する変数
    private GameObject currentSlashVFX;


    void Start()
    {
        // ゲーム開始時にエフェクトを生成し、非アクティブにしておく
        if (slashVFXPrefab != null)
        {
            // 剣先（swordTip）の位置を基準に生成し、親として設定
            currentSlashVFX = Instantiate(slashVFXPrefab, sword.position, sword.rotation);
            currentSlashVFX.transform.SetParent(sword); // 剣の動きに追従させる
            currentSlashVFX.SetActive(false); // 初期状態では非表示
        }
    }

    // 剣を振り始めた瞬間にアニメーションイベントから呼び出す
    public void StartSlashVFX()
    {
        if (currentSlashVFX != null)
        {
            // 剣のモーションにエフェクトを合わせるため、回転をリセットするなど調整が必要な場合もあります
            currentSlashVFX.SetActive(true);
            
            // Particle Systemの場合、ここで再生
            ParticleSystem ps = currentSlashVFX.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
            // Trail Rendererの場合、emittingをONにする
            TrailRenderer tr = currentSlashVFX.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.emitting = true;
            }
            
            // エフェクトがParticle Systemなどで自動的に消滅しない場合、少し遅れて非アクティブにする処理を追加
            // Invoke("StopSlashVFX", 0.5f); // 例: 0.5秒後に止める
        }
    }

    // 剣の振りが終わった瞬間にアニメーションイベントから呼び出す（Trail Rendererなどの場合）
    public void StopSlashVFX()
    {
        if (currentSlashVFX != null)
        {
            // Trail Rendererの場合、emittingをOFFにする
            TrailRenderer tr = currentSlashVFX.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.emitting = false;
            }

            // Particle Systemの場合は自動で終了するので、ActiveのON/OFFは必須ではない
            // 必要に応じて非アクティブにする
            // currentSlashVFX.SetActive(false);
        }
    }
}