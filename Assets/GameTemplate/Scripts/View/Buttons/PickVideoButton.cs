using UnityEngine;

namespace GameTemplate
{
    public class PickVideoButton : ButtonBase
    {
        public override void OnClick()
        {
            PickVideo();
        }

        private void PickVideo()
        {
            NativeGallery.Permission permission = NativeGallery.GetVideoFromGallery((path) =>
            {
                Debug.Log("Video path: " + path);
                if (path != null)
                {
                    // Play the selected video
                    Handheld.PlayFullScreenMovie("file://" + path);
                }
            }, "Select a video");

            Debug.Log("Permission result: " + permission);
        }
    }
}