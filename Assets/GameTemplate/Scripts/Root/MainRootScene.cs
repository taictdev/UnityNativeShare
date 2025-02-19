using GameTemplate;
using Utils;

public class MainRootScene : ManualSingletonMono<MainRootScene>
{
    private StateManager gameStateManager;

    public override void Awake()
    {
        base.Awake();
        gameStateManager = new StateManager();
    }

    private void Start()
    {
        UIFrameManager.Instance.uIFrame.ShowPanel(ScreenIds.UIMain);
    }

    public async void ChangeState(IState state)
    {
        await gameStateManager.ChangeState(state);
    }
}