using System.Collections;
using System.IO;
using RenderHeads.Media.AVProMovieCapture;
using TMPro;
using UnityEngine;

namespace GameTemplate
{
    public class ScreenRecordButton : ButtonBase
    {
        [SerializeField] private TextMeshProUGUI tmpStatus;
        [SerializeField] private bool isRecording = false;
        [SerializeField]private CaptureFromScreen capture;

        void Awake()
        {
            capture.CompletedFileWritingAction += CompletedFileWritingAction;
        }

        void Start()
        {
            StartCoroutine(RequestAccess());
        }

        private IEnumerator RequestAccess()
        {
        #if (UNITY_STANDALONE_OSX || UNITY_IOS) && !UNITY_EDITOR
            CaptureBase.PhotoLibraryAccessLevel photoLibraryAccessLevel = CaptureBase.PhotoLibraryAccessLevel.AddOnly;

            // If we're trying to write to the photo library, make sure we have permission
            if (capture.OutputFolder == CaptureBase.OutputPath.PhotoLibrary)
            {
                // Album creation (album name is taken from the output folder path) requires read write access.
                if (capture.OutputFolderPath != null && capture.OutputFolderPath.Length > 0)
                    photoLibraryAccessLevel = CaptureBase.PhotoLibraryAccessLevel.ReadWrite;

                switch (CaptureBase.HasUserAuthorisationToAccessPhotos(photoLibraryAccessLevel))
                {
                    case CaptureBase.PhotoLibraryAuthorisationStatus.Authorised:
                        // All good, nothing to do
                        break;

                    case CaptureBase.PhotoLibraryAuthorisationStatus.Unavailable:
                        Debug.LogWarning("The photo library is unavailable, will use RelativeToPeristentData instead");
                        capture.OutputFolder = CaptureBase.OutputPath.RelativeToPeristentData;
                        break;

                    case CaptureBase.PhotoLibraryAuthorisationStatus.Denied:
                        // User has denied access, change output path
                        Debug.LogWarning("User has denied access to the photo library, will use RelativeToPeristentData instead");
                        capture.OutputFolder = CaptureBase.OutputPath.RelativeToPeristentData;
                        break;

                    case CaptureBase.PhotoLibraryAuthorisationStatus.NotDetermined:
                        // Need to ask permission
                        yield return CaptureBase.RequestUserAuthorisationToAccessPhotos(photoLibraryAccessLevel);
                        // Nested switch, everbodies favourite
                        switch (CaptureBase.HasUserAuthorisationToAccessPhotos(photoLibraryAccessLevel))
                        {
                            case CaptureBase.PhotoLibraryAuthorisationStatus.Authorised:
                                // All good, nothing to do
                                break;

                            case CaptureBase.PhotoLibraryAuthorisationStatus.Denied:
                                // User has denied access, change output path
                                Debug.LogWarning("User has denied access to the photo library, will use RelativeToPeristentData instead");
                                capture.OutputFolder = CaptureBase.OutputPath.RelativeToPeristentData;
                                break;

                            case CaptureBase.PhotoLibraryAuthorisationStatus.NotDetermined:
                                // We were unable to request access for some reason, check the logs for any error information
                                Debug.LogWarning("Authorisation to access the photo library is still undetermined, will use RelativeToPeristentData instead");
                                capture.OutputFolder = CaptureBase.OutputPath.RelativeToPeristentData;
                                break;
                        }
                        break;
                }
            }
        #endif
            yield return null;
        }

        private void OnDestroy()
        {
            capture.CompletedFileWritingAction -= CompletedFileWritingAction;
        }

        private void CompletedFileWritingAction(FileWritingHandler handler)
        {
            Debug.Log($"CompletedFileWritingAction LastFileSaved => {CaptureBase.LastFileSaved}");
            AppManager.Instance.LastFilePath = CaptureBase.LastFileSaved;
        }

        public override void OnClick()
        {
            isRecording = !isRecording;
            if (isRecording)
            {
                tmpStatus.text = "Recording...";
                capture.StartCapture();
            }
            else
            {
                tmpStatus.text = "Screen Record";
                // delete the last file
                if (File.Exists(CaptureBase.LastFileSaved))
                {
                    File.Delete(CaptureBase.LastFileSaved);
                }
                // await for the last file to be deleted before stopping the capture
                while (File.Exists(CaptureBase.LastFileSaved))
                {
                    Debug.Log("Waiting for the last file to be deleted...");
                    // asign text into tpStatus to show the user that the file is being deleted
                    tmpStatus.text = "Deleting the last file...";
                }
                // update tmpStatus text
                tmpStatus.text = "Screen Record";
                // stop the capture after the last file is deleted
                capture.StopCapture();
            }
        }
    }
}