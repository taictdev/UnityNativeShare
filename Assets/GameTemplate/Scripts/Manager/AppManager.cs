using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Utils;

public class AppManager : AutoSingletonMono<AppManager>
{
    public string LastFilePath { get; set; }

    public override async void Awake()
    {
        base.Awake();
        //UIFrameManager.Instance.uIFrame.ShowPanel(ScreenIds.UILoading);
        await UniTask.Delay(1000);
        SceneManager.LoadScene("Main");
        DontDestroyOnLoad(this);
    }
}