using UnityEngine;

namespace GameTemplate
{
    public abstract class ButtonBase : MonoBehaviour
    {
        public virtual void OnClick()
        { }

        // Example code doesn't use this function but it is here for reference. It's recommended to ask for permissions manually using the
        // RequestPermissionAsync methods prior to calling NativeGallery functions
        protected async void RequestPermissionAsynchronously(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
        {
            NativeGallery.Permission permission = await NativeGallery.RequestPermissionAsync(permissionType, mediaTypes);
            Debug.Log("Permission result: " + permission);
        }
    }
}