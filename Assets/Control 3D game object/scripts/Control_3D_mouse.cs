using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Haipeng.control_3D_game_object
{
    public class Control_3D_mouse : MonoBehaviour
    {
        [Header("----scale variable----------------------------------------------------------------------------------------------------------------------------------------")]
        #region 
        [SerializeField]
        private bool is_scalable;

        [SerializeField]
        [Range(0, 0.1f)]
        private float speed_scale;


        [SerializeField]
        private float max_scale;
        [SerializeField]
        private float min_scale;

        [SerializeField]
        private Texture2D mouse_texture_scale;
        [Space]
        [Space]
        #endregion

        [Header("----rotation variable----------------------------------------------------------------------------------------------------------------------------------------")]
        #region 
        [SerializeField]
        private bool is_rotatable;

        [SerializeField]
        [Range(0, 1)]
        private float speed_rotate;

        [SerializeField]
        private Texture2D mouse_texture_rotate;
        [Space]
        [Space]
        #endregion

        [Header("-----drag variable---------------------------------------------------------------------------------------------------------------------------------------")]
        #region 
        [SerializeField]
        private bool is_dragable;

        [SerializeField]
        private Texture2D mouse_texture_drag;

        [SerializeField]
        private Material[] materials;

        [SerializeField]
        [ColorUsageAttribute(true, true, 0f, 10f, 0.125f, 3f)]
        [FormerlySerializedAs("colorValue")]
        public Color color_drag;

        [SerializeField]
        private float long_press_time = 0.75f;

        [SerializeField]
        private Image image_circle;

        //is dragging
        private bool is_dragging;

        //Sends the distance from the ray camera to the collider's Z axis 
        private float z_distance;

        //When clicking and dragging, the deviation distance from the mouse to the center of the object 
        private Vector3 drag_drop_offset;
        [Space]
        [Space]
        #endregion

        //init transform
        private Vector3 init_position;
        private Quaternion init_rotation;
        private Vector3 init_scale;


        //the mouse position
        private float mouse_position_x;
        private float mouse_position_y;

        // Cursor statu
        private Cursor_statu cursor_statu = Cursor_statu.nothing;
        private float time_stamp_cursor;


        //long press
        private float long_press_time_stamp;
        private Vector3 mouse_long_press_position;
        private bool is_detecting_long_pressing = false;
        private bool is_mouse_tap_on_game_obj = false;

        void Awake()
        {
            this.init_position = this.transform.position;
            this.init_rotation = this.transform.rotation;
            this.init_scale = this.transform.localScale;
        }

        // Update is called once per frame
        void Update()
        {
            //set the mouse is down
            #region 
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                this.mouse_position_x = Input.mousePosition.x;
                this.mouse_position_y = Input.mousePosition.y;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == this.gameObject.name)
                    {
                        this.is_mouse_tap_on_game_obj = true;


                        if (this.is_detecting_long_pressing == false)
                        {
                            this.long_press_time_stamp = Time.time;
                            this.mouse_long_press_position = Input.mousePosition;
                            this.is_detecting_long_pressing = true;
                        }

                        this.z_distance = Camera.main.transform.position.z - hit.transform.position.z;
                        this.z_distance = Mathf.Abs(this.z_distance);

                        this.drag_drop_offset = hit.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.z_distance));
                    }
                    else
                    {
                        this.is_mouse_tap_on_game_obj = false;
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                this.is_detecting_long_pressing = false;
                this.is_mouse_tap_on_game_obj = false;
                this.mouse_long_press_position = new Vector3(-1f, -1f, -1f);

                //Lift the mouse and stop dragging, the size will be restored.
                if (this.is_dragable == true && this.is_dragging == true)
                {
                    this.is_dragging = false;
                    //this.transform.localScale = this.transform.localScale / 9f * 10;


                    //modify materials not glowing
                    for (int i = 0; i < this.materials.Length; i++)
                    {
                        Material mat = this.materials[i];
                        mat.DisableKeyword("_EMISSION");
                        Color c = new Color(0, 0, 0);

                        mat.SetColor("_EmissionColor", c);
                    }

                }

                this.set_cursor(Cursor_statu.nothing);
            }
            #endregion

            //detect long press
            #region 
            if (this.is_mouse_tap_on_game_obj == true && this.is_dragable && this.is_dragging == false && this.is_detecting_long_pressing == true && this.mouse_long_press_position == Input.mousePosition)
            {
                //set the circle image
                if (this.image_circle != null)
                {
                    if (this.image_circle.gameObject.activeSelf == false)
                        this.image_circle.gameObject.SetActive(true);

                    this.image_circle.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);


                    this.image_circle.fillAmount = (Time.time - this.long_press_time_stamp) / this.long_press_time;
                }


                if (Time.time - this.long_press_time_stamp > this.long_press_time)
                {
                    this.is_detecting_long_pressing = false;

                    //Start dragging
                    this.is_dragging = true;

                    //modify materials glowing
                    for (int i = 0; i < this.materials.Length; i++)
                    {
                        Material mat = this.materials[i];
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor("_EmissionColor", this.color_drag);
                    }
                }
            }
            else
            {
                this.is_detecting_long_pressing = false;

                if (this.image_circle != null)
                    this.image_circle.gameObject.SetActive(false);
            }
            #endregion

            //drag and rotate
            #region 
            if (this.is_mouse_tap_on_game_obj == true && this.is_detecting_long_pressing == false)
            {
                //drag
                #region 
                if (this.is_dragable && this.is_dragging)
                {
                    this.transform.position =
                        Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, this.z_distance)) + drag_drop_offset;

                    //Set mouse texture
                    this.set_cursor(Cursor_statu.drag);
                }
                #endregion

                //rotation
                #region 
                else if (this.is_rotatable == true && this.is_mouse_tap_on_game_obj == true)
                {
                    //if (this.rotation_type == Rotation_type.rotation_along_x)
                    //{
                    //    transform.Rotate(Vector3.down * (Input.mousePosition.x - this.mouse_position_x) * this.rotation_speed, Space.World);
                    //}
                    //else if (this.rotation_type == Rotation_type.rotation_along_y)
                    //{
                    //    transform.Rotate(Vector3.right * (Input.mousePosition.y - this.mouse_position_y) * this.rotation_speed, Space.World);
                    //}


                    //--------------------------------------------------------------------------------------------
                    Vector2 deltaPos = new Vector2((Input.mousePosition.x - this.mouse_position_x), (Input.mousePosition.y - this.mouse_position_y));

                    //if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
                    //{
                    //    if (Mathf.Abs(deltaPos.x) > 5)
                    //    {
                    //        transform.Rotate(Vector3.down * deltaPos.x * this.speed_rotate, Space.Self); 
                    //    }
                    //}
                    //else
                    //{
                    //    if (Mathf.Abs(deltaPos.y) > 5)
                    //    {
                    //        transform.Rotate(Vector3.right * deltaPos.y * this.speed_rotate, Space.World); 
                    //    }
                    //}

                    transform.Rotate(Vector3.down * deltaPos.x * this.speed_rotate, Space.Self);


                    this.set_cursor(Cursor_statu.rotate);


                    this.mouse_position_x = Input.mousePosition.x;
                    this.mouse_position_y = Input.mousePosition.y;
                }
                else
                {
                    this.set_cursor(Cursor_statu.nothing);
                }
                #endregion
            }
            #endregion

            //scale
            #region 
            else if (this.is_scalable)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform.name == this.gameObject.name)
                    {
                        if (Input.GetAxis("Mouse ScrollWheel") < 0)
                        {
                            if (this.transform.localScale.x > this.min_scale)
                            {
                                this.transform.localScale = this.transform.localScale * (1 - this.speed_scale);
                            }

                            this.set_cursor(Cursor_statu.scale);
                        }
                        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
                        {
                            if (this.transform.localScale.x < this.max_scale)
                            {
                                this.transform.localScale = this.transform.localScale * (1 + this.speed_scale);
                            }

                            this.set_cursor(Cursor_statu.scale);
                        }
                        else
                        {
                            if (Time.time - this.time_stamp_cursor > 0.5f)
                                this.set_cursor(Cursor_statu.nothing);
                        }
                    }

                    else
                    {
                        //if (Time.time - this.time_stamp_cursor > 0.25f)
                        this.set_cursor(Cursor_statu.nothing);
                    }
                }
            }
            #endregion
        }


        void OnDestroy()
        {
            //modify materials not glowing
            for (int i = 0; i < this.materials.Length; i++)
            {
                Material mat = this.materials[i];
                mat.DisableKeyword("_EMISSION");
                Color c = new Color(0, 0, 0);

                mat.SetColor("_EmissionColor", c);
            }
        }
        //set cursor
        private void set_cursor(Cursor_statu statu)
        {
            this.time_stamp_cursor = Time.time;

            if (this.cursor_statu == statu)
                return;

            switch (statu)
            {
                case Cursor_statu.nothing:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    break;
                case Cursor_statu.drag:
                    if (this.mouse_texture_drag != null)
                        Cursor.SetCursor(this.mouse_texture_drag, Vector2.zero, CursorMode.Auto);
                    break;
                case Cursor_statu.scale:
                    if (this.mouse_texture_scale != null)
                        Cursor.SetCursor(this.mouse_texture_scale, Vector2.zero, CursorMode.Auto);
                    break;
                case Cursor_statu.rotate:
                    if (this.mouse_texture_rotate != null)
                        Cursor.SetCursor(this.mouse_texture_rotate, Vector2.zero, CursorMode.Auto);
                    break;
                default:
                    break;
            }

            this.cursor_statu = statu;


        }

        //reset the transform 
        public void reset()
        {
            this.transform.position = this.init_position;
            this.transform.rotation = this.init_rotation;
            this.transform.localScale = this.init_scale;
        }


    }

    public enum Cursor_statu
    {
        nothing,
        drag,
        scale,
        rotate
    }
}