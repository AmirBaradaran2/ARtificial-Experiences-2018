using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pie_menu : MonoBehaviour {

    public List<PieMenuButton> btns;
    public Canvas canvas;

    private SteamVR_TrackedObject tracked_obj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)tracked_obj.index); } }
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    //private ulong button_mask = SteamVR_Controller.ButtonMask.Touchpad;

    private int num_btns;
    private int cur_btn = -1;
    private Vector2 cursor;

    private bool activated = false;
    private bool select = false;
    private bool recognize_word = false;
    private bool paint = false;
    private bool deselect = true;

    private Wand_behavior wand_script;

	// Use this for initialization
	void Start () {
        if (canvas != null) {
            canvas.enabled = false;
        }

        tracked_obj = GetComponent<SteamVR_TrackedObject>();
        wand_script = this.gameObject.GetComponent<Wand_behavior>();

        num_btns = btns.Count;
        if (btns.Count == 0)
            Debug.Log("No menu items assigned!");
    }

    public bool _select {
        get {
            return select;
        }
    }

    public bool _rec_word {
        get {
            return recognize_word;
        }
    }

    public bool _paint {
        get {
            return paint;
        }
    }

    public bool _deselect {
        get {
            return deselect;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (controller.GetPress(touchpad))
        {
            Debug.Log("Touchpad pressed");
            if (!activated)
            {
                canvas.enabled = true;
                activated = true;
                return;
            }
            //Debug.Log("Touch on Touchpad");
            int btn = current_button();
            //Debug.Log("Current btn: " + cur_btn);
            if (cur_btn != btn)
            {
                //Debug.Log("cur_btn = " + cur_btn);
                if (cur_btn != -1)
                    btns[cur_btn].onDeselect();
                cur_btn = btn;
                btns[cur_btn].onSelect();

                switch (btns[cur_btn].name) {
                    case "Select":
                        wand_script.active_select();
                        paint = false;
                        recognize_word = false;
                        deselect = false;
                        select = true;
                        break;
                    case "Recognize Word":
                        paint = false;
                        select = false;
                        deselect = false;
                        recognize_word = true;
                        wand_script.active_recognize();
                        break;
                    case "Paint":
                        select = false;
                        deselect = false;
                        recognize_word = false;
                        paint = true;
                        wand_script.active_brush();
                        break;
                    case "Exit":
                        wand_script.deactive_wand();
                        deselect_all();  break;
                    default: break; 
                }
            }
        }
        /*else if (controller.GetTouch(touchpad)) {
            if (canvas == null || !canvas.enabled)
                return;
            //Debug.Log("Hover on Touchpad");
            int btn = current_button();
            if (cur_btn != btn) {
                if (btns[btn].btn_selected)
                    return;
                if(cur_btn != -1)
                    btns[cur_btn].onDeselect();
                cur_btn = btn;
                btns[cur_btn].onHover();
            }
        }*/
        
	}

    private int current_button() {
        int btn = -1;
        cursor = controller.GetAxis(touchpad);
        //Debug.Log("Cursor: " + cursor);
        float radian;
        if (cursor.y == 0) {
            if (cursor.x >= 0)
                radian = Mathf.PI / 2.0f;
            else
                radian = -Mathf.PI / 2.0f;
        }
        else
            radian = Mathf.Atan(cursor.x/cursor.y);
        float degree = radian / Mathf.PI * 180.0f;
        //Debug.Log("Degree: " + degree);
        if ((degree < 45.0f & cursor.x >= 0 & cursor.y >= 0) || (degree >= -45.0f & cursor.x <= 0 & cursor.y >= 0))
        {
            btn = 3; //exit
        }
        else if ((degree >= 45.0f & cursor.x >= 0 & cursor.y >= 0) || (degree < -45.0f & cursor.x >= 0 & cursor.y < 0))
        {
            btn = 0; //select
        }
        else if ((degree >= -45.0f & cursor.x >= 0 & cursor.y < 0) || (degree < 45.0f & cursor.x < 0 & cursor.y < 0))
        {
            btn = 1; //recognize word
        }
        else {
            btn = 2; //paint
        }
        //Debug.Log("Current button: " + cur_btn);
        return btn;
    }

    private void deselect_all() {
        if (select) select = false;
        if (recognize_word) recognize_word = false;
        if (paint) paint = false;
        deselect = true;
    }

    [System.Serializable]
    public class PieMenuButton {
        public string name;
        public Image image;
        public Color select_color = new Color(0.52f, 0.59f, 0.69f, 0.71f);
        public Color hover_color = new Color(0.62f, 0.76f, 0.9f, 0.71f);
        public Color unselect_color = new Color(0.76f, 0.97f, 1, 0.71f);
        public float degree;

        private bool transformed = false;
        private bool selected = false;

        public bool btn_selected {
            get{
                return selected;
            }
        }

        public void onSelect() {
            image.color = select_color;
            selected = true;
        }

        public void onDeselect() {
            image.color = unselect_color;
            selected = false;
            if (transformed)
            {
                translate_button(false);
            }
        }

        public void onHover() {
            if (!transformed)
            {
                translate_button(true);
                transformed = true;
            }
            image.color = hover_color;
        }

        public void translate_button(bool outward) {
            float radian = degree / 180.0f * Mathf.PI;
            Vector3 move_direction = (outward ? 0.1f : -0.1f) * new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0); // true: outward; false: inward
            image.rectTransform.transform.position += move_direction;
        }
    }
}
