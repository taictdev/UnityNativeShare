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