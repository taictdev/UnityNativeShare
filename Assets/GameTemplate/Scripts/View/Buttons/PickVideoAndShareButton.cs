using UnityEngine;

namespace GameTemplate
{
    public class PickVideoAndShareButton : ButtonBase
    {
        public override void OnClick()
        {
            PickVideoAndShare();
        }

        private void PickVideoAndShare()
        {
            NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
            {
                Debug.Log("Video path: " + path);
                if (path != null)
                {
                    new NativeShare().AddFile("file://" + path)
                   .SetSubject("Subject goes here").SetText("Hello world!").SetUrl("https://www.google.com")
                   .SetCallback((result, shareTarget) => Debug.Log("Share result: " + result + ", selected app: " + shareTarget))
                   .Share();
                }
            }, "Select a video");

            Debug.Log("Permission result: " + permission);
        }
    }
}