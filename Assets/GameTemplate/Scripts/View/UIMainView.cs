using deVoid.UIFramework;
using TMPro;
using UnityEngine;

public class UIMainView : APanelController
{
    [SerializeField] private TextMeshProUGUI tmpTime;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        tmpTime.text = timer.ToString("F2");
    }
}