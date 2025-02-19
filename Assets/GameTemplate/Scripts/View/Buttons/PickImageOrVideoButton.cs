using UnityEngine;

namespace GameTemplate
{
    public class PickImageOrVideoButton : ButtonBase
    {
        public override void OnClick()
        {
            PickImageOrVideo();
        }

        // Example code doesn't use this function but it is here for reference
        private void PickImageOrVideo()
        {
            if (NativeGallery.CanSelectMultipleMediaTypesFromGallery())
            {
                NativeGallery.Permission permission = NativeGallery.GetMixedMediaFromGallery((path) =>
                {
                    Debug.Log("Media path: " + path);
                    if (path != null)
                    {
                        // Determine if user has picked an image, video or neither of these
                        switch (NativeGallery.GetMediaTypeOfFile(path))
                        {
                            case NativeGallery.MediaType.Image: Debug.Log("Picked image"); break;
                            case NativeGallery.MediaType.Video: Debug.Log("Picked video"); break;
                            default: Debug.Log("Probably picked something else"); break;
                        }
                    }
                }, NativeGallery.MediaType.Image | NativeGallery.MediaType.Video, "Select an image or video");

                Debug.Log("Permission result: " + permission);
            }
        }
    }
}