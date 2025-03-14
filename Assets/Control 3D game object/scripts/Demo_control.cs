using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Haipeng.control_3D_game_object
{
    public class Demo_control : MonoBehaviour
    {
        [Header("index")]
        public int index = 0;

        [Header("What is used for display is the object and the corresponding camera")]
        public GameObject[] array_game_obj_display;
        public GameObject[] array_game_obj_camera;


        [Header("mat_variant for build")]
        [SerializeField]
        private Material[] mat_variants;

        void Start()
        {
            //for (int i = 0; i < this.array_game_obj_display.Length; i++)
            //{
            //    if (this.index == i)
            //    {
            //        if (this.array_game_obj_display[i].activeSelf == false)
            //        {
            //            this.array_game_obj_display[i].SetActive(true);
            //        }
            //        if (this.array_game_obj_camera[i].activeSelf == false)
            //        {
            //            this.array_game_obj_camera[i].SetActive(true);
            //        }

            //    }
            //    else
            //    {
            //        if (this.array_game_obj_display[i].activeSelf == true)
            //        {
            //            this.array_game_obj_display[i].SetActive(false);
            //        }
            //        if (this.array_game_obj_camera[i].activeSelf == true)
            //        {
            //            this.array_game_obj_camera[i].SetActive(false);
            //        }
            //    }
            //}
        }

        public void on_previous_btn()
        {
            Audio_manager.instance.play_btn();

            this.index--;
            if (this.index <= -1)
                this.index = this.array_game_obj_display.Length - 1;

            for (int i = 0; i < this.array_game_obj_display.Length; i++)
            {
                if (this.index == i)
                {
                    if (this.array_game_obj_display[i].activeSelf == false)
                    {
                        this.array_game_obj_display[i].SetActive(true);
                    }
                    if (this.array_game_obj_camera[i].activeSelf == false)
                    {
                        this.array_game_obj_camera[i].SetActive(true);
                    }

                }
                else
                {
                    if (this.array_game_obj_display[i].activeSelf == true)
                    {
                        this.array_game_obj_display[i].SetActive(false);
                    }
                    if (this.array_game_obj_camera[i].activeSelf == true)
                    {
                        this.array_game_obj_camera[i].SetActive(false);
                    }
                }
            }
        }

        public void on_next_btn()
        {

            Audio_manager.instance.play_btn();

            this.index++;
            if (this.index >= this.array_game_obj_display.Length)
                this.index = 0;

            for (int i = 0; i < this.array_game_obj_display.Length; i++)
            {
                if (this.index == i)
                {
                    if (this.array_game_obj_display[i].activeSelf == false)
                    {
                        this.array_game_obj_display[i].SetActive(true);
                    }
                    if (this.array_game_obj_camera[i].activeSelf == false)
                    {
                        this.array_game_obj_camera[i].SetActive(true);
                    }

                }
                else
                {
                    if (this.array_game_obj_display[i].activeSelf == true)
                    {
                        this.array_game_obj_display[i].SetActive(false);
                    }
                    if (this.array_game_obj_camera[i].activeSelf == true)
                    {
                        this.array_game_obj_camera[i].SetActive(false);
                    }
                }
            }
        }

        public void on_reset_btn()
        {
            Audio_manager.instance.play_btn();

            for (int i = 0; i < this.array_game_obj_display.Length; i++)
            {
                if (this.array_game_obj_display[i].activeSelf == true)
                {
                    if (this.array_game_obj_display[i].GetComponent<Control_3D_mouse>() != null)
                    {
                        this.array_game_obj_display[i].GetComponent<Control_3D_mouse>().reset();
                    }
                    else if (this.array_game_obj_display[i].GetComponent<Control_3D_touch>() != null)
                    {
                        this.array_game_obj_display[i].GetComponent<Control_3D_touch>().reset();
                    }
                }
            }
        }
    }
}
