using deVoid.UIFramework;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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