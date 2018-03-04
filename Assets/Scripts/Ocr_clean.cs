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

public class Ocr_clean : MonoBehaviour
{
    public Camera cam;
    private RenderTexture rendTexture;

    private Tesseract _ocr;
    [SerializeField]
    private GameObject progress_indicator;
    [SerializeField]
    private GameObject text_parent;
    [SerializeField]
    private GameObject text_templates;
    [SerializeField]
    private GameObject single_text_parent;

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

    private Coroutine main_coroutine;

    private String outputPath = Path.Combine("C:\\Emgu/emgucv-windesktop 3.1.0.2504/Emgu.CV.World", "tessdata");

    private static List<List<Tesseract.Character>> word_list = new List<List<Tesseract.Character>>();

    IEnumerator wait()
    {
        yield return new WaitForSeconds(3.0f);
        Init();
    }


    IEnumerator ocr_async()
    {
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
        print("OCR engine loaded.");
        print("OCR processing..");
        Image<Bgr, Byte> img = TextureConvert.Texture2dToImage<Bgr, Byte>(original_texture);
        yield return Ninja.JumpBack;
        _ocr.Recognize(img);

        Tesseract.Character[] characters = _ocr.GetCharacters();
        String messageOcr = _ocr.GetText().TrimEnd('\n', '\r'); // remove end of line from ocr-ed text  

        yield return Ninja.JumpToUnity;
        Debug.Log("Detected text: " + messageOcr);

        build_char_list(characters);

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
    }

    void scrollBy(int y)
    {
        if (web_view != null)
        {
            web_view.InjectMouseWheel(y, 0);
            reload_ocr();
        }
    }

    void reload_ocr()
    {
        word_list = new List<List<Tesseract.Character>>();
        if(main_coroutine != null)
            StopCoroutine(main_coroutine);
        progress_indicator.SetActive(true);
        for (int i = 0; i < words.Count; i++)
        {
            words[i].GetComponent<Rigidbody>().useGravity = true;
            Destroy(words[i], 3.5f);
        }
        words = new List<GameObject>();
        main_coroutine = StartCoroutine(wait());
    }

    void reload()
    {

        if (web_view != null)
        {
            web_view.Reload();
            //text_templates = GameObject.Find("Template texts (Retro)");
            reload_ocr();
        }
    }
    
    private string[] parse_words(string text)
    {
        char[] split_symbols = { ' ', ',', '.', ':' };
        string[] words = text.Split(split_symbols);
        return words;
    }

    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            scrollBy(-1000);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            reload();
        }
    }

    private void build_char_list(Tesseract.Character[] chars)
    { // build list for chars of each word
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
                if (last_width != 0 && last_x != 0 && x - last_x >= 1.2 * (last_width / 2.0f + width / 2.0f))
                { // extract outliers
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
                    print(word);
                    word_list.Add(single_word_chars);
                    //render_text_in_center(single_word_chars);
                    //yield return new WaitForSeconds(1);
                }
                word = "";
                single_word_chars = new List<Tesseract.Character>();
                last_x = 0;
                last_width = 0;
            }
        }

        main_coroutine = StartCoroutine(renderWords());
    }

    private IEnumerator renderWords() {
        for (int i = 0; i < word_list.Count; i++) {
            render_text_in_center(word_list[i]);
            yield return new WaitForSeconds(1);
        }
    }

    // render single word (dirty implementation)
    private void render_text_in_center(List<Tesseract.Character> chars)
    {
        Rectangle rect = new Rectangle();
        Rectangle temp = new Rectangle();
        rect.X = 0;
        rect.Y = 0;
        for (int i = 0; i < chars.Count; i++)
        {
            temp = chars[i].Region;
            if (rect.X == 0 && rect.Y == 0)
            { //first letter
                rect = temp;
            }
            else
            {
                if (temp.Y > rect.Y)
                {
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
            c_temp = Instantiate(text_templates.transform.Find("Letter_" + chars[i].Text + "").gameObject, text_parent.transform);
            //c_temp = Instantiate(Resources.Load<GameObject>("Assets/3D_Font_Art_Pack/Steampunk/Prefab/Hi/"+ chars[i].Text + ""), text_parent.transform);
            x = start_x_in_scene + (r.X + r.Width / 2.0f) / original_texture.width * texture_width_in_scene;
            z = parent.transform.localPosition.z;
            y = -3f;
            c_temp.transform.localPosition = new Vector3(x, y, z);
            c_temp.transform.SetParent(parent.transform);
            c_temp.SetActive(true);
        }

        float base_height = 2.0f;
        float scale = rect.Height / base_height;
        parent.transform.localScale = new Vector3(scale, scale, parent.transform.localScale.z);

        parent.AddComponent<BoxCollider>();
        parent.GetComponent<BoxCollider>().center += new Vector3(rect.Width / 6.0f / original_texture.width * 10, rect.Height / 3.0f / original_texture.height * 10, 0);
        parent.GetComponent<BoxCollider>().size = new Vector3((float)rect.Width / original_texture.width * 10, (float)rect.Height / original_texture.height * 10, 1);

        parent.GetComponent<Rigidbody>().AddForce(new Vector3(0, scale / 5.0f, 0), ForceMode.Impulse);
        words.Add(parent);
    }

    private void combine_texts(List<Tesseract.Character> chars, GameObject parent)
    {
        GameObject c_temp;
        Rectangle r = new Rectangle();
        for (int i = 0; i < chars.Count; i++)
        {
            r = chars[i].Region;
            c_temp = Instantiate(text_templates.transform.Find(chars[i].Text + "").gameObject, parent.transform);
            c_temp.transform.localPosition = new Vector3((r.X + r.Width / 2.0f) / original_texture.width * 10, 0, 0);
            c_temp.SetActive(true);
        }

    }

    private Vector3 image_xy_to_world_xyz(Vector2 image_xy, float world_y, float texture_width_in_scene, float texture_height_in_scene)
    { // translate (x, y) on webpage image to world space (x, y, z)
        float start_x_in_scene = 5f, start_y_in_scene = -3.048f;
        float x = start_x_in_scene + (image_xy.x) / original_texture.width * texture_width_in_scene;
        float z = start_y_in_scene - (image_xy.y) / original_texture.height * texture_height_in_scene;
        return new Vector3(x, world_y, z);
    }


}
