using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.TextToSpeech.v1;
using IBM.Watson.DeveloperCloud.Logging;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections;

public class MyTextToSpeech : MonoBehaviour {

    #region Credential
    private string _username = "0c1f7cfa-e948-43a3-9dba-a8be83d141aa";
    private string _password = "skQRorwjNecZ";
    private string _url = "https://stream.watsonplatform.net/text-to-speech/api";
    #endregion

    TextToSpeech _textToSpeech;

    private bool _synthesizeTested = false;
    private bool _getVoicesTested = false;
    private bool _getVoiceTested = false;
    private bool _getPronuciationTested = false;
    private bool _getCustomizationsTested = false;
    private bool _createCustomizationTested = false;
    private bool _deleteCustomizationTested = false;
    private bool _getCustomizationTested = false;
    private bool _updateCustomizationTested = false;
    private bool _getCustomizationWordsTested = false;
    private bool _addCustomizationWordsTested = false;
    private bool _deleteCustomizationWordTested = false;
    private bool _getCustomizationWordTested = false;

    private void Start() {
        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);
        _textToSpeech = new TextToSpeech(credentials);

        //textToSpeech("Woman", "This is a test for speech to text, conversation, and text to speech!", false);
        //textToSpeech("Man", "Your love should never be offered\nto the mouth of a stranger,\n\nOnly to someone who has the\nvalor and daring\n\nTo cut pieces of their soul off\nwith a knife\n\nThen weave them into a blanket\n\nTo protect you.", false);
    }

    public void textToSpeech(string gender, string text, bool goodNews) {
        //StopAllCoroutines();
        LogSystem.InstallDefaultReactors();

        if(goodNews)
            Runnable.Run(Examples(gender, expressAsGoodNews(text)));
        else
            Runnable.Run(Examples(gender, neutral(text)));
    }

    private string neutral(string text) {
        return "<speak version=\"1.0\"><prosody pitch=\"120Hz\">" + text + "</prosody></speak>";
    }

    private string expressAsGoodNews(string text) {
        return "<speak version=\"1.0\"><express-as type=\"GoodNews\">" + text + "</express-as></speak>";
    }

    private IEnumerator Examples(string gender, string ssml)
    {
        //  Synthesize
        //Log.Debug("ExampleTextToSpeech", "Attempting synthesize.");
        if (gender.Equals("Woman"))
            _textToSpeech.Voice = VoiceType.en_US_Allison;
        if (gender.Equals("Man"))
            _textToSpeech.Voice = VoiceType.en_US_Michael;
        _textToSpeech.ToSpeech(ssml, HandleToSpeechCallback, true);
        while (!_synthesizeTested)
            yield return null;

        //	Get Voices
        //Log.Debug("ExampleTextToSpeech", "Attempting to get voices.");
        _textToSpeech.GetVoices(OnGetVoices);
        while (!_getVoicesTested)
            yield return null;

        //	Get Voice
        //Log.Debug("ExampleTextToSpeech", "Attempting to get voice {0}.", VoiceType.en_US_Allison);
        _textToSpeech.GetVoice(OnGetVoice, VoiceType.en_US_Allison);
        while (!_getVoiceTested)
            yield return null;
        
    }

    void HandleToSpeechCallback(AudioClip clip, string customData)
    {
        PlayClip(clip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            GameObject audioObject = new GameObject("AudioObject");
            AudioSource source = audioObject.AddComponent<AudioSource>();
            source.spatialBlend = 0.0f;
            source.loop = false;
            source.clip = clip;
            source.Play();

            Destroy(audioObject, clip.length);

            _synthesizeTested = true;
        }
    }

    private void OnGetVoices(Voices voices, string customData)
    {
        //Log.Debug("ExampleTextToSpeech", "Text to Speech - Get voices response: {0}", customData);
        _getVoicesTested = true;
    }

    private void OnGetVoice(Voice voice, string customData)
    {
        //Log.Debug("ExampleTextToSpeech", "Text to Speech - Get voice  response: {0}", customData);
        _getVoiceTested = true;
    }
    
}
