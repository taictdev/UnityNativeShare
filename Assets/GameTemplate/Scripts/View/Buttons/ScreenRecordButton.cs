using RenderHeads.Media.AVProMovieCapture;
using TMPro;
using UnityEngine;

namespace GameTemplate
{
    public class ScreenRecordButton : ButtonBase
    {
        [SerializeField] private TextMeshProUGUI tmpStatus;
        [SerializeField] private bool isRecording = false;
        private CaptureFromScreen capture;

        private void Start()
        {
            GameObject go = new GameObject();
            capture = go.AddComponent<CaptureFromScreen>();
            capture.IsRealTime = false;
            capture.FrameRate = 30f;
            capture.StopMode = StopMode.None;
            capture.OutputTarget = OutputTarget.VideoFile;
            capture.OutputFolder = CaptureBase.OutputPath.PhotoLibrary;
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
                capture.StopCapture();
            }
        }
    }
}