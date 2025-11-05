using UnityEngine;

public class TitleUiController : MonoBehaviour
{
    public GameObject panel;

    void Start()
    {
        panel.SetActive(false); //パネル非表示
    }
    public void Show()
    {
        panel.SetActive(true); //パネル表示
    }

    public void Hide()
    {
        panel.SetActive(false); //パネル非表示
    }
}
