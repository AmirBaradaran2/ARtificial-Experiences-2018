using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickPoemGameManager : MonoBehaviour {
    [SerializeField] private MyTextToSpeech textToSpeech;
    [SerializeField] private PoemBehavior poemController;

    private void Start() {
        StartCoroutine(pickStory());
    }

    private IEnumerator pickStory() {
        yield return new WaitForSeconds(5f);
        poemController.OnLift();
        yield return new WaitForSeconds(0.6f);
        playClip("Your love should never be offered,     , to the mouth of a stranger,        , " 
            + "Only to someone who has the,     , valor and daring,       , To cut pieces of their soul off,      , with a knife,      ,Then weave them into a blanket,      , To protect you.");

    }

    public void playClip(string text, bool goodNews = false) {
        textToSpeech.textToSpeech("Man", text, goodNews);
    }
}
