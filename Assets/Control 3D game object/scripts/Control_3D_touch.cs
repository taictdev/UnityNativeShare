using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace Haipeng.control_3D_game_object
{
    public class Control_3D_touch : MonoBehaviour
    {
        //init transform
        private Vector3 init_position;
        private Quaternion init_rotation;
        private Vector3 init_scale;

        [Header("---drag------------------------------------------------------------------------------------------------------------")]
        #region 
        [SerializeField]
        private bool is_dragable;

        [SerializeField]
        private float long_touch_time;

        [SerializeField]
        private Material[] materials;

        [SerializeField]
        private Image image_circle;

        [SerializeField]
        [ColorUsageAttribute(true, true, 0f, 10f, 0.125f, 3f)]
        [FormerlySerializedAs("colorValue")]
        public Color color_drag;

        private bool is_dragging = false;               //Whether dragging is in progress
        private float time_stamp_long_press;            //The timestamp when the ray started to be emitted
        private bool is_long_touching = false;          //Whether you are in long press state
        private float distance_z;                       //Sends the distance from the ray camera to the collider's Z axis
        private Vector3 drag_offset;                    //When clicking and dragging, the deviation distance from the mouse to the center of the object
        private bool is_show_selected_effect = false;   //Whether to display the selection effect
        private bool is_mouse_tap_on_game_obj = false;  //Whether the game object is touched
        [Space]
        [Space]
        #endregion

        [Header("---rotation variable-----------------------------------------------------------------------------------------------")]
        #region 
        [SerializeField]
        private bool is_rotatable;

        [Header("Rotational speed 0~1")]
        [SerializeField]
        [Range(0, 1)]
        private float rotation_speed;

        [Space]
        [Space]
        #endregion

        [Header("---scale variable--------------------------------------------------------------------------------------------------")]
        #region 
        [SerializeField]
        private bool is_scalable;

        [SerializeField]
        [Range(1f, 20f)]
        private float scale_speed;

        [SerializeField]
        private float max_scale;
        [SerializeField]
        private float min_scale;

        private Touch oldTouch1;  //Last touched point 1 (finger 1)
        private Touch oldTouch2;  //Last touched point 2 (finger 2)
        #endregion

        private RaycastHit hit;

        void Awake()
        {
            this.init_position = this.transform.position;
            this.init_rotation = this.transform.rotation;
            this.init_scale = this.transform.localScale;
        }
        
        void Update()
        {
            #region 
            //// no touch
            if (Input.touchCount <= 0)
            {
                this.not_drag();
                return;
            }
            //// touch over
            else if (Input.touches[0].phase == TouchPhase.Ended || Input.touches[0].phase == TouchPhase.Canceled)
            {
                this.not_drag();
                return;
            }
            ////touch on ui
            foreach (Touch touch in Input.touches)
            {
                int id = touch.fingerId;
                if (EventSystem.current.IsPointerOverGameObject(id))
                {
                    return;
                }
            }
            ////is touch on game object
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                this.is_mouse_tap_on_game_obj = false;
                for (int i = 0; i < Mathf.Min(2, Input.touches.Length); i++)
                {
                    //RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.touches[i].position);

                    if (Physics.Raycast(ray, out this.hit) && (hit.transform.name == this.gameObject.name))
                    {
                        this.is_mouse_tap_on_game_obj = true;
                        break;
                    }
                }
            }
            if (this.is_mouse_tap_on_game_obj == false)
            {
                return;
            }
            #endregion

            //one touch
            if (Input.touchCount == 1)
            {
                //获取触摸位置
                Touch touch = Input.touches[0];
  
                //Judge long press
                if (this.is_dragable == true && this.is_dragging == false)
                {
                    if (this.is_long_touching == false)
                    {
                        //Get timestamp
                        this.time_stamp_long_press = Time.time;

                        //Start timing when you hit an object
                        this.is_long_touching = true;

                    }
                    else
                    {
                        //set the circle image
                        if (this.image_circle != null)
                        {
                            if (this.image_circle.gameObject.activeSelf == false)
                            {
                                if ((Time.time - this.time_stamp_long_press) / this.long_touch_time > 0.25f)
                                {
                                    this.image_circle.gameObject.SetActive(true);
                                }
                            }

                            this.image_circle.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
                            this.image_circle.fillAmount = (Time.time - this.time_stamp_long_press) / this.long_touch_time;
                        }

                        if (Time.time - this.time_stamp_long_press >= this.long_touch_time)
                        {
                            if (this.image_circle != null)
                                this.image_circle.gameObject.SetActive(false);

                            this.set_selected_effect();

                            this.is_dragging = true;
                            this.is_long_touching = false;

                            //Get a deviation position and the Z-axis distance from the camera to the control object
                            this.distance_z = this.hit.transform.position.z - Camera.main.transform.position.z;
                            this.distance_z = Mathf.Abs(this.distance_z);
                            this.drag_offset = this.hit.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distance_z));
                        }
                    }
                }

                //drag game object
                else if (this.is_dragging && touch.phase == TouchPhase.Moved)
                {
                    this.transform.position =
                      Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.distance_z)) + drag_offset;
                }

                //rotate
                if (this.is_rotatable == true && this.is_dragging == false)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 deltaPos = touch.deltaPosition;

                        this.not_drag();

                        transform.Rotate(Vector3.down * deltaPos.x * this.rotation_speed, Space.Self);
                    }
                }
            }

            //multi touch, scale
            else if (Input.touchCount > 1 && this.is_scalable == true && this.is_dragging == false)
            {
                this.not_drag();

                //Multi-touch, zoom in and out
                Touch newTouch1 = Input.GetTouch(0);
                Touch newTouch2 = Input.GetTouch(1);

                //Point 2: just start touching the screen, only record, no processing
                if (newTouch2.phase == TouchPhase.Began)
                {
                    this.oldTouch2 = newTouch2;
                    this.oldTouch1 = newTouch1;
                    return;
                }

                //The difference between the two distances, a positive value indicates a zoom-in gesture, and a negative value indicates a zoom-out gesture.
                float offset = Vector2.Distance(newTouch1.position, newTouch2.position) - Vector2.Distance(oldTouch1.position, oldTouch2.position);

                //Magnification factor, one pixel is calculated as 0.01 times
                float scaleFactor = offset * this.scale_speed / 1000f;

                //Get current size
                Vector3 localScale = transform.localScale;

                //modify scale
                if ((localScale.x + scaleFactor) < this.max_scale && (localScale.x + scaleFactor) > this.min_scale)
                {
                    transform.localScale = new Vector3(localScale.x + scaleFactor, localScale.y + scaleFactor, localScale.z + scaleFactor);
                }

                //record the latest touch point for next time
                this.oldTouch1 = newTouch1;
                this.oldTouch2 = newTouch2;
            }
        }

        public void set_selected_effect()
        {
            if (this.is_show_selected_effect == false)
            {
                for (int i = 0; i < this.materials.Length; i++)
                {
                    Material mat = this.materials[i];
                    mat.SetColor("_EmissionColor", this.color_drag);
                    mat.EnableKeyword("_EMISSION");

                }
                this.is_show_selected_effect = true;
            }

        }

        private void set_unselected_effect()
        {
            if (this.is_show_selected_effect == true)
            {
                for (int i = 0; i < this.materials.Length; i++)
                {
                    Material mat = this.materials[i];
                    mat.SetColor("_EmissionColor", new Color(0, 0, 0));
                    mat.DisableKeyword("_EMISSION");

                }
                this.is_show_selected_effect = false;
            }
        }

        private void not_drag()
        {
            if (this.is_long_touching == true)
                this.is_long_touching = false;
            if (this.is_dragging == true)
                this.is_dragging = false;
            if (this.image_circle != null)
                this.image_circle.gameObject.SetActive(false);
            this.set_unselected_effect();
        }

        //reset the transform 
        public void reset()
        {
            this.transform.position = this.init_position;
            this.transform.rotation = this.init_rotation;
            this.transform.localScale = this.init_scale;

            if (this.is_long_touching == true)
                this.is_long_touching = false;
            if (this.is_dragging == true)
                this.is_dragging = false;
            if (this.image_circle != null)
                this.image_circle.gameObject.SetActive(false);

            this.set_unselected_effect();
        }

    }
}