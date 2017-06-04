//----------------------------------------------------------------------------
//  Copyright (C) 2004-2016 by EMGU Corporation. All rights reserved.       
//----------------------------------------------------------------------------


using System.IO;
using Emgu.CV.CvEnum;
using UnityEngine;
using System;
using System.Drawing;
using System.Collections;
using System.Text;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Runtime.InteropServices;
using Emgu.CV.OCR;

using System.Collections.Generic;
using System.Threading;
using CielaSpike;

public class Ocr : MonoBehaviour
{

    public GameObject prefab;

    public Camera cam;
    public Renderer render_test;
    public SpriteRenderer render_sprite;

    private RenderTexture rendTexture;

    private Tesseract _ocr;
    [SerializeField]
    private GameObject progress_indicator;
    [SerializeField]
    private GameObject text_sample;
    [SerializeField]
    private GameObject text_parent;
    [SerializeField]
    private GameObject text_templates;
    [SerializeField]
    private GameObject single_text_parent;

    private List<Region_map> word_region_map = new List<Region_map>(); // region map for detected words
    private List<List<Tesseract.Character>> char_region_map = new List<List<Tesseract.Character>>(); // region map for single characters
    private List<GameObject> words = new List<GameObject>(); // list of generated word objects

    [SerializeField]
    private AwesomiumUnityWebTexture web_texture;
    private AwesomiumUnityWebView web_view;
    private int scroll_v = 0, scroll_h = 0;
    public Texture2D original_texture;
    private Renderer rend;

    private bool init = false;
    private bool init_done = false;

    private Texture2D before;
    private Texture m_texture;

    private int scrollX = 0;
    private int scrollY = 0;

    private Coroutine main_coroutine;

    private String outputPath = Path.Combine("C:\\Emgu/emgucv-windesktop 3.1.0.2504/Emgu.CV.World", "tessdata");

    IEnumerator wait() {
        yield return new WaitForSeconds(3.0f);
        Init();
    }
    

    IEnumerator ocr_async() {
        String[] names = new string[] { "eng.cube.bigrams", "eng.cube.fold", "eng.cube.lm", "eng.cube.nn", "eng.cube.params", "eng.cube.size", "eng.cube.word-freq", "eng.tesseract_cube.nn", "eng.traineddata" };


        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        yield return Ninja.JumpToUnity;
        foreach (String n in names)
        {
            TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine("tessdata", n));
            String filePath = Path.Combine(outputPath, n);
#if UNITY_METRO
           UnityEngine.Windows.File.WriteAllBytes(filePath, textAsset.bytes);
#else
            if (!File.Exists(filePath))
                File.WriteAllBytes(filePath, textAsset.bytes);
#endif
        }

        yield return Ninja.JumpBack;
        _ocr = new Tesseract(outputPath, "eng", OcrEngineMode.TesseractCubeCombined);

        yield return Ninja.JumpToUnity;
        Debug.Log("OCR engine loaded.");
        print("OCR processing..");
        Image<Bgr, Byte> img = TextureConvert.Texture2dToImage<Bgr, Byte>(original_texture);
        yield return Ninja.JumpBack;
        _ocr.Recognize(img);

        Tesseract.Character[] characters = _ocr.GetCharacters();
        /*foreach (Tesseract.Character c in characters)
        { //draw rect for each character
            CvInvoke.Rectangle(img, c.Region, new MCvScalar(255, 0, 0));
        }*/

        String messageOcr = _ocr.GetText().TrimEnd('\n', '\r'); // remove end of line from ocr-ed text  

        yield return Ninja.JumpToUnity;
        Debug.Log("Detected text: " + messageOcr);

        /*Texture2D test = TextureConvert.InputArrayToTexture2D(img, FlipType.Vertical);
        render_sprite.sprite = Sprite.Create(test, new Rect(0, 0, rendTexture.width, rendTexture.height), new Vector2(0.5f, 0.5f));
        Debug.Log("News sprite width: " + rendTexture.width + "; height: " + rendTexture.height);*/

        //StartCoroutine(build_map(characters));
        StartCoroutine(build_char_list(characters));

        progress_indicator.SetActive(false);
    }

    void Init()
    {
        original_texture = (Texture2D)GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        web_view = web_texture.WebView;

        cam.Render();
        RenderTexture.active = rendTexture;

        original_texture.ReadPixels(new Rect(0, 0, rendTexture.width, rendTexture.height), 0, 0);
        original_texture.Apply();

        //original_texture = (Texture2D)GetComponent<MeshRenderer>().sharedMaterial.mainTexture;

        this.StartCoroutineAsync(ocr_async());

    }
    
    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        m_texture = rend.sharedMaterial.mainTexture;
        Debug.Log("Texture width: " + m_texture.width + "; height: " + m_texture.height);
        rendTexture = new RenderTexture(m_texture.width, m_texture.height, 16);
        cam.targetTexture = rendTexture;

        main_coroutine = StartCoroutine(wait());
        //before = (Texture2D)GetComponent<MeshRenderer>().sharedMaterial.mainTexture;
        //Init();

    }

    void scrollBy(int y) {
        /*cam.Render();
        RenderTexture.active = rendTexture;

        original_texture.ReadPixels(new Rect(0, scrollY+y, rendTexture.width, rendTexture.height), 0, 0);
        original_texture.Apply();

        reload();*/
        if (web_view != null)
        {
            web_view.InjectMouseWheel(y, 0);
            reload_ocr();
        }
    }

    void reload_ocr() {
       // StopCoroutine(main_coroutine);
        progress_indicator.SetActive(true);
        for (int i = 0; i < words.Count; i++)
        {
            words[i].GetComponent<Rigidbody>().useGravity = true;
            Destroy(words[i], 3.5f);
        }
        words = new List<GameObject>();
        main_coroutine = StartCoroutine(wait());
    }

    void reload() {
        
        if (web_view != null)
        {
            web_view.Reload();
            //text_templates = GameObject.Find("Template texts (Retro)");
            reload_ocr();
        }
    }

    public List<Region_map> word_map {
        get{
            return word_region_map;
        }
    }

    public Texture2D texture {
        get {
            return original_texture;
        }
    }

    private string[] parse_words(string text) {
        char[] split_symbols = { ' ', ',', '.', ':' };
        string[] words = text.Split(split_symbols);
        return words;
    }

    void Update()
    {
        //original_texture = this.gameObject.GetComponent<AwesomiumUnityWebTexture>().WebTexture;
        /*if (Input.GetMouseButtonDown(0))
        {
             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
             RaycastHit hit = new RaycastHit();
             if (Physics.Raycast(ray, out hit))
             {
                  Vector2 hit_coord = hit.textureCoord; //get the hit uv-coordinate on the texture
                  Debug.Log("Hit coord: " + hit_coord);
                  float texture_x = hit_coord.x * original_texture.width;
                  float texture_y = (1 - hit_coord.y) * original_texture.height;
                  Vector2 texture_point = new Vector2(texture_x, texture_y);
                  Debug.Log("Texture coord: " + texture_x + ", " + texture_y);
                foreach (Region_map map in word_region_map)
                {
                    if (within_region(texture_point, map.getRect()))
                    {
                        string word = map.getWord();
                        Debug.Log("Clicking on " + word);
                        GameObject text = GameObject.Find("Draggable text");
                        text.GetComponent<TextMesh>().text = word;
                        text.AddComponent<BoxCollider>();
                        //render_text(hit.point, word);
                        break;
                    }
                }
                   
             }
        }*/

        if (Input.GetKeyDown("space")) {
            /*if (web_view != null)
            {
                scroll_v += 100;
                web_view.InjectMouseWheel(scroll_v, scroll_h);
                web_view.Reload();
            }*/
            scrollBy(-1000);
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            reload();
        }
    }

    public bool within_region(Vector2 point, Rectangle rect) {
        if (point.x >= rect.Left && point.x <= rect.Right && point.y >= rect.Top && point.y <= rect.Bottom)
            return true;
        else
            return false;
    }

    private void render_text(Vector3 position, string word) {
        Instantiate(prefab, new Vector3(position.x, position.y, transform.position.z), Quaternion.identity, transform);
        prefab.GetComponent<TextMesh>().text = word;
    }

    private IEnumerator build_map(Tesseract.Character[] chars) { // build hashmap for words and regions
        string word = "";
        Rectangle rect = new Rectangle();
        Rectangle temp = new Rectangle();
        rect.X = 0;
        rect.Y = 0;
        List<Tesseract.Character> char_list = new List<Tesseract.Character>();
        for (int i = 0; i < chars.Length; i++) {
            //if (!chars[i].Text.Equals(" ") && !chars[i].Text.Equals(",") && !chars[i].Text.Equals(".") && !chars[i].Text.Equals(":") && !chars[i].Text.Equals("\t") && !chars[i].Text.Equals("\r"))
            int c = System.Convert.ToInt32((chars[i].Text)[0]);
            if ((c >= 65 & c <= 90) | (c >= 97 & c <= 122)) // is character a-z/A-Z
            {
                // part of a word
                word += chars[i].Text;
                temp = chars[i].Region;
                char_list.Add(chars[i]);
                if (rect.IsEmpty) //first letter
                {
                    rect = temp;
                    rect.X = temp.X;
                }
                else {
                    if (temp.Y > rect.Y)
                    {
                        rect.Y = temp.Y;
                        rect.Height = temp.Height;
                    }
                    rect.Width += temp.Width;
                }
            }
            else if (!word.Equals("") && !rect.IsEmpty)
            { //end of a word
                //Debug.Log(word + ": " + rect.ToString());
                if (word.Length > 2 && _ocr.IsValidWord(word) != 0)
                {
                    word_region_map.Add(new Region_map(rect, word));
                    //Debug.Log("Valid word:" + word);
                    render_text_in_center(word, rect, char_list);
                    //Debug.Log(word + ": " + rect.ToString());
                    yield return new WaitForSeconds(1);
                }
                rect = new Rectangle();
                word = "";
                char_list = new List<Tesseract.Character>();
            }
        }
        word_region_map.Sort();
    }

    private IEnumerator build_char_list(Tesseract.Character[] chars) { // build list for chars of each word
        List<Tesseract.Character> single_word_chars = new List<Tesseract.Character>();
        string word = "";
        int last_x = 0;
        int last_width = 0;
        for (int i = 0; i < chars.Length; i++)
        {
            int c = System.Convert.ToInt32((chars[i].Text)[0]);
            if ((c >= 65 & c <= 90) | (c >= 97 & c <= 122)) // is character a-z/A-Z
            {
                int x = chars[i].Region.X;
                int width = chars[i].Region.Width;
                if (last_width != 0 && last_x != 0 && x - last_x >= 1.2 * (last_width/2.0f + width/2.0f)) { // extract outliers
                    word = "";
                    single_word_chars = new List<Tesseract.Character>();
                    last_x = 0;
                    last_width = 0;
                    continue;
                }
                // part of word
                single_word_chars.Add(chars[i]);
                word += chars[i].Text;

                last_x = x;
                last_width = width;
            }
            else if (single_word_chars.Count > 0)
            {
                // end of word
                if (word.Length > 2 && _ocr.IsValidWord(word) != 0)
                {
                    //char_region_map.Add(single_word_chars);
                    print(word);
                    render_text_in_center(single_word_chars);
                    yield return new WaitForSeconds(1);
                }
                word = "";
                single_word_chars = new List<Tesseract.Character>();
                last_x = 0;
                last_width = 0;
            }
        }
    }

    // text_sample:                         local under single-parent           local under text-parent         under text_parent
    // left-up: (4.95, 21, -3.29)           (-2.936, -0.176, 4.634)             (-6.627, 116.6, -13.42)         (6, -2, -3.048)
    // left-bottum: (4.95, 21, 4.93);       (-2.936, -3.221, 4.634)             (-6.627, 116.6, -3.644)         (6, -2, 6.68)
    // right-up: (-15.42, 21, -2.75)        (2.806, -0.176, 4.634)              (-31.28, 116.6, -13.42)         (-18.72, -2, -3.048)
    // right-bottum: (-15.42, 21, 4.38);    (2.806, -3.221, 4.634)              (-31.28, 116.6, -3.644)         (-18.72, -2, 6.68)
    // base rect.Height = 5;
    private void render_text_in_center(string word, Rectangle rect, List<Tesseract.Character> char_list) {
        // center image coordinate
        //float texture_width_in_scene = -24.72f, texture_height_in_scene = -9.728f;
        float texture_width_in_scene = -23f, texture_height_in_scene = -9.73f;
        //float start_x_in_scene = 6f, start_y_in_scene = -3.048f;
        float start_x_in_scene = 5f, start_y_in_scene = -3.048f;
        float x = start_x_in_scene + (rect.X + rect.Width/2.0f) / original_texture.width * texture_width_in_scene;
        float z = start_y_in_scene - (rect.Y - rect.Height/2.0f) / original_texture.height * texture_height_in_scene;
        float y = -2f;
        // Debug.Log("World coord: " + x + ", " + y + ", " + z);
        /*GameObject text = Instantiate(text_sample, text_parent.transform);
        text.transform.localPosition = new Vector3(x, y, z);
        text.GetComponent<TextMesh>().text = word;
        text.SetActive(true);*/
        GameObject parent = Instantiate(single_text_parent, text_parent.transform);
        parent.transform.localPosition = new Vector3(x, y, z);
        parent.SetActive(true);
        /*float size = (float)rect.Width / original_texture.width * 1.2f;
        parent.GetComponent<BoxCollider>().size = new Vector3(size, size, parent.GetComponent<BoxCollider>().size.z);
        parent.GetComponent<BoxCollider>().center = new Vector3(0.2f*size, 0.2f, 0);*/
        BoxCollider col = parent.AddComponent<BoxCollider>();
        col.size = new Vector3(col.size.x, 0.03f, col.size.z);
        col.center = new Vector3(col.size.x/10.0f, 0.03f, 0);
        combine_texts(word, parent, rect.Height, char_list);
        float base_height = 5.0f;
        float scale = rect.Height / base_height * 2.0f;
        parent.transform.localScale = new Vector3(scale, scale, parent.transform.localScale.z);
        parent.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0.6f, 0), ForceMode.Impulse);
        words.Add(parent);
        /*for (int i = 0; i < 10; i++) {
            parent.transform.localPosition += new Vector3(0, 0, 0.1f);
        }*/
    }

    private void set_physical_boundary(GameObject parent) {

    }

    // render single word (dirty implementation)
    private void render_text_in_center(List<Tesseract.Character> chars) { 
        Rectangle rect = new Rectangle();
        Rectangle temp = new Rectangle();
        rect.X = 0;
        rect.Y = 0;
        for (int i = 0; i < chars.Count; i++) {
            temp = chars[i].Region;
            if (rect.X == 0 && rect.Y == 0) { //first letter
                rect = temp;
            }
            else {
                if (temp.Y > rect.Y) {
                    rect.Y = temp.Y;
                    rect.Height = temp.Height;
                }
                rect.Width += temp.Width;
            }
        }

        print(rect.ToString());

        float texture_width_in_scene = -25.02f, texture_height_in_scene = -10.5f;
        float start_x_in_scene = 6.07f, start_y_in_scene = -3.24f;
        float x = start_x_in_scene + (rect.X + rect.Width / 2.0f) / original_texture.width * texture_width_in_scene;
        float z = start_y_in_scene - (rect.Y - rect.Height / 2.0f) / original_texture.height * texture_height_in_scene;
        float y = -2f;

        GameObject parent = Instantiate(single_text_parent, text_parent.transform);
        parent.transform.localPosition = new Vector3(x, y, z);
        parent.SetActive(true);

        GameObject c_temp;
        Rectangle r = new Rectangle();
        for (int i = 0; i < chars.Count; i++)
        {
            r = chars[i].Region;
            c_temp = Instantiate(text_templates.transform.Find(chars[i].Text + "").gameObject, text_parent.transform);
            x = start_x_in_scene + (r.X + r.Width / 2.0f) / original_texture.width * texture_width_in_scene;
            z = parent.transform.localPosition.z;
            y = -3f;
            c_temp.transform.localPosition = new Vector3(x,y,z);
            c_temp.transform.SetParent(parent.transform);
            c_temp.SetActive(true);
        }

        float base_height = 2.0f;
        float scale = rect.Height / base_height;
        parent.transform.localScale = new Vector3(scale, scale, parent.transform.localScale.z);
        
        parent.AddComponent<BoxCollider>();
        parent.GetComponent<BoxCollider>().center += new Vector3(rect.Width/6.0f / original_texture.width * 10, rect.Height/3.0f / original_texture.height * 10, 0);
        parent.GetComponent<BoxCollider>().size = new Vector3((float)rect.Width/original_texture.width * 10, (float)rect.Height / original_texture.height * 10, 1);
        
        parent.GetComponent<Rigidbody>().AddForce(new Vector3(0, scale/5.0f, 0), ForceMode.Impulse);
        words.Add(parent);
    }

    private void combine_texts(string word, GameObject parent, float height, List<Tesseract.Character> char_list) {
        char[] chars = word.ToCharArray();
        float base_dis = 0.045f, base_height = 5.0f;
        base_dis *= height / base_height;
        float x = -base_dis * (chars.Length / 2);
        GameObject c_temp;
        for (int i = 0; i < chars.Length; i++) {
            base_dis = get_char_width(chars[i]);
            x += base_dis * (char_list[i].Region.Height / base_height / 1.2f) /2.0f;
            c_temp = Instantiate(text_templates.transform.Find(chars[i] + "").gameObject, parent.transform);
            c_temp.transform.localPosition = new Vector3(x, 0, 0);
            c_temp.SetActive(true);
            x += base_dis * (char_list[i].Region.Height / base_height) / 2;
        }
    }

    private void combine_texts(List<Tesseract.Character> chars, GameObject parent) {
        GameObject c_temp;
        Rectangle r = new Rectangle();
        for (int i = 0; i < chars.Count; i++) {
            r = chars[i].Region;
            c_temp = Instantiate(text_templates.transform.Find(chars[i].Text + "").gameObject, parent.transform);
            c_temp.transform.localPosition = new Vector3((r.X+r.Width/2.0f)/original_texture.width * 10, 0, 0);
            c_temp.SetActive(true);
        }

    }

    private Vector3 image_xy_to_world_xyz(Vector2 image_xy) { // translate (x, y) on webpage image to world space (x, y, z)
        float texture_width_in_scene = -23f, texture_height_in_scene = -9.73f;
        float start_x_in_scene = 5f, start_y_in_scene = -3.048f;
        float x = start_x_in_scene + (image_xy.x) / original_texture.width * texture_width_in_scene;
        float z = start_y_in_scene - (image_xy.y) / original_texture.height * texture_height_in_scene;
        float y = -2f;
        return new Vector3(x, y, z);
    }

    private float get_char_width(char c) {
        switch (c) {
            case 'a': case 'b': case 'c': case 'd': case 'e': case 'g': case 'h': case 'k': case 'n':
            case 'o': case 'p': case 'q': 
            case 's': case 'u': 
                return 0.05f;
            case 'm': case 'w':
                return 0.05f;
            case 'f': case 'j': case 'v': case 'r': case 't': case 'x': case 'y': case 'z': 
                return 0.03f;
            case 'i': case 'l':
                return 0.005f;
            case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': case 'G': case 'H': case 'L':
            case 'K': case 'N': case 'O': case 'P': case 'Q': case 'R': case 'T': case 'U': case 'V':
            case 'X': case 'Y': case 'Z':
                return 0.055f;
            case 'M': case 'W':
                return 0.06f;
            case 'I': case 'J':
                return 0.04f;
        }
        return 0;
    }

    
}

public class Region_map : IComparable<Region_map> { //region-word map

    private Rectangle rect;
    private string word;

    public Region_map(Rectangle rect, string word) {
        this.rect = rect;
        this.word = word;
    }

    public int CompareTo(Region_map other) {
        if (rect.Y > other.rect.Y)
            return 1;
        else if (rect.X == other.rect.X && rect.Y == other.rect.Y)
            return 0;
        else
            return -1;
    }

    public Rectangle getRect() {
        return rect;
    }

    public string getWord() {
        return word;
    }
}