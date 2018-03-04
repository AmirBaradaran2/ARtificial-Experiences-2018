using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    [Header("Adult Objects Dictionary")]
    public List<string> a_oKeys = new List<string>();
    public List<GameObject> a_oValues = new List<GameObject>();
    protected Dictionary<string, List<GameObject>> a_obj_dict;

    [Header("Chidren Objects Dictionary")]
    public List<string> c_oKeys = new List<string>();
    public List<GameObject> c_oValues = new List<GameObject>();
    protected Dictionary<string, List<GameObject>> c_obj_dict;

    [Header("Lay/Sit Iteractable Objects Dictionary")]
    public List<string> i_oKeys = new List<string>();
    public List<GameObject> i_oValues = new List<GameObject>();
    protected Dictionary<string, GameObject> i_obj_dict;

    [SerializeField] private MyTextToSpeech textToSpeech;
    [SerializeField] private MyRobotController robotController; 
    //[SerializeField] private MyRobotTurnForwardController robotController; 

    private GameObject pickedObj;

    private void Awake() {
        initObjDict(out a_obj_dict, a_oKeys, a_oValues);
        initObjDict(out c_obj_dict, c_oKeys, c_oValues);
        initObjDict(out i_obj_dict, i_oKeys, i_oValues);

        print(i_obj_dict.Count);

        // Adults have access to all the objects, while children have access to only the children objects

    }
    
    private void initObjDict(out Dictionary<string, List<GameObject>> dict, List<string> keys, List<GameObject> values) {
        dict = new Dictionary<string, List<GameObject>>();
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            if (dict.ContainsKey(keys[i]))
                dict[keys[i]].Add(values[i]);
            else
            {
                List<GameObject> objs = new List<GameObject>();
                objs.Add(values[i]);
                dict.Add(keys[i], objs);
            }
        }
        values.Clear();
    }

    private void initObjDict(out Dictionary<string, GameObject> dict, List<string> keys, List<GameObject> values) {
        dict = new Dictionary<string, GameObject>();
        for (int i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
            dict[keys[i]] = values[i];
        values.Clear();
    }

    public void PickupObj(string name, string material, string group) {
        print("[Pick up Interaction]: Pick up " + material + " " + name + " for " + (group == null ? "adult" : group));

        bool pickedup = false;
        GameObject obj = null;
        List<GameObject> objs = null;

        print("name = " + name);
        if (group != "child") {
            if (a_obj_dict.ContainsKey(name))
                objs = a_obj_dict[name];
            else if (c_obj_dict.ContainsKey(name))
                objs = c_obj_dict[name];

            print(objs);

            if(objs != null) {
                foreach (GameObject o in objs) {
                    if (o.GetComponent<Renderer>().sharedMaterial.name == material){
                        obj = o;
                        pickedup = true;
                    }
                }
            }
        }
        else {
            if (c_obj_dict.ContainsKey(name))
                objs = c_obj_dict[name];

            if (objs != null) {
                foreach (GameObject o in objs)
                {
                    if (o.GetComponent<Renderer>().sharedMaterial.name == material) {
                        obj = o;
                        pickedup = true;
                    }
                }
            }
        }

        print("Picked up = " + pickedup);

        if (!pickedup)
            playClip("There is no such object or the object cannot be reached!");
        else {
            //StartCoroutine(move(obj, obj.transform.position + new Vector3(0, 3, 0), 2));
            pickedObj = obj;
            robotController.moveToward(obj, obj.transform.position, new Vector3(0, 0, 0), true, false, false);
            //playClip("Object picked up!", true);
        }
    }

    public void Putdown() {
        print("[Game Manager]: Put down");
        if(pickedObj != null) {
            StartCoroutine(robotController.putDown(pickedObj));
            pickedObj = null;
        }
        else
            playClip("I have nothing to put down!");
    }

    public void LayonObj(string name) {
        print("Lay on " + name);

        if (i_obj_dict.ContainsKey(name)) {
            GameObject obj = i_obj_dict[name];
            robotController.moveToward(obj, obj.transform.position, new Vector3(0, 180, 0), false, true, false);
        }
        else
            playClip("There is no such object or the object cannot be reached!");
    }

    public void SitonObj(string name) {
        print("Sitting on " + name);
        
        if (i_obj_dict.ContainsKey(name)) {
            GameObject obj = i_obj_dict[name];
            robotController.moveToward(obj, obj.transform.position, new Vector3(0, 180, 0), false, false, true);
        }
        else
            playClip("There is no such object or the object cannot be reached!");

        // TO-DO: Store Dictionary <targetName, <targetPos, targetRotation>>

    }

    public void Standup() {
        print("Standing up");
        robotController.StandUp();

    }

    public void playClip(string text, bool goodNews = false) {
        textToSpeech.textToSpeech("Man", text, goodNews);
    }

    private IEnumerator move(GameObject obj, Vector3 target_pos, float duration) {
        yield return new WaitForSeconds(5);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            obj.transform.position = Vector3.Lerp(obj.transform.position, target_pos, t);
            yield return null;
        }
    }
}
