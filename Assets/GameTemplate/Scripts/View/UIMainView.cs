using deVoid.UIFramework;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMainView : APanelController
{
    [SerializeField] private Button btnPlay;
    [SerializeField] private TextMeshProUGUI tmpTime;
    private float timer;

    protected override void AddListeners()
    {
        base.AddListeners();
        btnPlay.onClick.AddListener(OnPlayGame);
    }

    protected override void RemoveListeners()
    {
        base.RemoveListeners();
        btnPlay.onClick.RemoveListener(OnPlayGame);
    }

    private void OnPlayGame()
    {
        StartCoroutine(TakeScreenshotAndShare());
    }

    private void Update()
    {
        timer += Time.deltaTime;
        tmpTime.text = timer.ToString("F2");
    }

    private IEnumerator TakeScreenshotAndShare()
    {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());

        // To avoid memory leaks
        Destroy(ss);

        new NativeShare().AddFile(filePath)
            .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://github.com/yasirkula/UnityNativeShare")
            .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
            .Share();

        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).AddTarget( "com.whatsapp" ).Share();
    }
}