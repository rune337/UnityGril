using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private Renderer objectRenderer; // オブジェクトのRendererコンポーネント

    public float colorChangeDuration = 1.0f; // 色が変化するまでの時間 (滑らかに変化させる場合)
    public bool changeOnStart = false; // Start時に色を変えるか
    public bool changeOnButtonClick = false; // ボタンクリックで色を変えるか (デバッグ用)
     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         objectRenderer = GetComponent<Renderer>(); //このスクリプトがアタッチされたオブジェクトのRendererコンポーネントを取得
    }

    // Update is called once per frame
    void Update()
    {
         // デバッグ用: スペースキーを押したら色を変える
        if (changeOnButtonClick && Input.GetKeyDown(KeyCode.Space))
        {
            SetColor(BaseRange.playerColor);
        }

    }
    
    //色を変えるメソッド
    public void SetColor(Color color)
    {
         if (objectRenderer != null)
         {

            // Renderer.material を直接変更すると、マテリアルアセット自体が変更される可能性があるので注意。
            // 基本的には Renderer.material.color で問題ないが、複数のオブジェクトで同じマテリアルを使っている場合は Renderer.sharedMaterial.color を変更するとすべて変わってしまう。
            // 適切なのは、Renderer.material プロパティを通して、そのインスタンスの色を変えること。
            objectRenderer.material.color = color;
            Debug.Log(gameObject.name + " の色を " + color + " に変更しました。");
         }
    }
}
