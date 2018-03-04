using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TypeWord : MonoBehaviour{

    [SerializeField]
    private string str;

    void Start() {
        StartCoroutine(type(this.GetComponent<Text>(), str));
    }

    public static IEnumerator type(Text text, string str) {
        str = "Your love should never be offered\nto the mouth of a stranger,\n\nOnly to someone who has the\nvalor and daring\n\nTo cut pieces of their soul off\nwith a knife\n\nThen weave them into a blanket\n\nTo protect you.";
        text.text = "_";
        yield return new WaitForSeconds(0.1f);
        char[] chars = str.ToCharArray();
        string temp = "";
        for (int i = 0; i < str.Length; i++)
        {
            temp += chars[i];
            if (i == str.Length - 1){
                text.text = temp;
                break;
            }
            text.text = temp + "_";
            yield return new WaitForSeconds(0.05f);
        }
    }


	
}
