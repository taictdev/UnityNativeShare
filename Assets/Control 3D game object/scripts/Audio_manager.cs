using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haipeng.control_3D_game_object
{
    public class Audio_manager : MonoBehaviour
    {
        public static Audio_manager instance;

        [Header("Audio Source")]
        public AudioSource audio_source;


        [Header("button audioclip")]
        public AudioClip audio_clip_btn;

        void Awake()
        {
            Audio_manager.instance = this;
        }


        public void play_btn()
        {
            if (this.audio_clip_btn != null)
                this.audio_source.PlayOneShot(this.audio_clip_btn);
        }
    }
}
