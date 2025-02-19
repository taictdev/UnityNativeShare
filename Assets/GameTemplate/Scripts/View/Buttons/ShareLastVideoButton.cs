using UnityEngine;

namespace GameTemplate
{
    public class ShareLastVideoButton : ButtonBase
    {
        public override void OnClick()
        {
            ShareLastVideo();
        }

        private void ShareLastVideo()
        {
            string lastFilePath = AppManager.Instance.LastFilePath;
            if (string.IsNullOrEmpty(lastFilePath))
            {
                Debug.LogError("LastFilePath is NULL");
                return;
            }
            Debug.Log("Video path: " + lastFilePath);
            new NativeShare().AddFile(lastFilePath)
                  .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://www.google.com")
                  .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                  .Share();
        }
    }
}