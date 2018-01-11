using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.Conversation.v1;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Logging;
using System.Collections;
using FullSerializer;
using System.Collections.Generic;
using IBM.Watson.DeveloperCloud.Widgets;
using System;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;

public class MyConversation : Widget {

    #region Credential_and_version
    private string _username = "3e739a81-60a8-41fe-a34b-9c7e78265585";
    private string _password = "Jytoinf3c6lM";
    private string _url = "https://gateway.watsonplatform.net/conversation/api";
    private string _workspaceId = "58f01ad4-7d74-4ecd-b1f8-cc5aeff75507";  // Pick up Interaction

    private Conversation _conversation;
    private string _conversationVersionDate = "2017-05-26";
    #endregion

    [SerializeField] private Input input = new Input("SpeechInput", typeof(SpeechToTextData), "OnSpeechInput");
    [SerializeField] private GameManager gameManager;
    
    private fsSerializer _serializer = new fsSerializer();
    private Dictionary<string, object> _context = null;
    private bool _waitingForResponse = true;

    #region Init_and_lifecycle
    protected override string GetName() {
        return "MyConversation";
    }

    protected override void Start() {
        base.Start();

        LogSystem.InstallDefaultReactors();

        //  Create credential and instantiate service
        Credentials credentials = new Credentials(_username, _password, _url);

        _conversation = new Conversation(credentials);
        _conversation.VersionDate = _conversationVersionDate;

        //Runnable.Run(Examples());
    }
    #endregion

    private void OnSpeechInput(Data data) {
        SpeechRecognitionEvent result = ((SpeechToTextData)data).Results;
        if (result != null && result.results.Length > 0)
        {
            foreach (var res in result.results)
            {
                foreach (var alt in res.alternatives)
                {
                    if (res.final && alt.confidence > 0)
                    {
                        string text = alt.transcript;
                        Debug.Log("Result: " + text + " Confidence: " + alt.confidence);
                        _conversation.Message(OnMessage, _workspaceId, text);
                    }
                }
            }
        }
    }

    private void OnMessage(object resp, string data)
    {
        Log.Debug("MyConversation", "Conversation: Message Response: {0}", data);

        //  Convert resp to fsdata
        fsData fsdata = null;
        fsResult r = _serializer.TrySerialize(resp.GetType(), resp, out fsdata);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        //  Convert fsdata to MessageResponse
        MessageResponse messageResponse = new MessageResponse();
        object obj = messageResponse;
        r = _serializer.TryDeserialize(fsdata, obj.GetType(), ref obj);
        if (!r.Succeeded)
            throw new WatsonException(r.FormattedMessages);

        if (messageResponse != null && (messageResponse.intents.Length > 0 || messageResponse.entities.Length > 0))
        {
            string intent = messageResponse.intents[0].intent;
            Debug.Log("Intent: " + intent);
            string mat = null;
            string group = null;
            string name = null;

            switch(intent) {
                case "pick_up":
                    bool pickedup = false;
                    foreach (RuntimeEntity entity in messageResponse.entities) {
                        Debug.Log("entityType: " + entity.entity + " , value: " + entity.value);
                        switch (entity.entity) {
                            case "material":
                                mat = entity.value;
                                break;
                            case "person_group":
                                group = entity.value;
                                break;
                            case "object":
                                name = entity.value;
                                pickedup = true;
                                break;
                            default: break;
                        }
                    }

                    if (!pickedup)
                        gameManager.playClip("Sorry I didn't get you.");
                    else
                        gameManager.PickupObj(name, mat, group);

                    break;

                case "put_down":
                    gameManager.Putdown();

                    break;

                case "lay":
                    /*bool lying = false;
                    foreach (RuntimeEntity entity in messageResponse.entities) {
                        Debug.Log("entityType: " + entity.entity + " , value: " + entity.value);
                        switch (entity.entity) {
                            case "object":
                                name = entity.value;
                                lying = true;
                                break;
                            default: break;
                        }
                    }
                    if (!lying)
                        gameManager.playClip("Sorry I didn't get you.");
                    else
                        gameManager.LayonObj(name);*/

                    gameManager.LayonObj("bed");

                    break;

                case "sit":
                    /*bool sitting = false;
                    foreach (RuntimeEntity entity in messageResponse.entities) {
                        Debug.Log("entityType: " + entity.entity + " , value: " + entity.value);
                        switch (entity.entity) {
                            case "object":
                                name = entity.value;
                                sitting = true;
                                break;
                            default: break;
                        }
                    }
                    if (!sitting)
                        gameManager.playClip("Sorry I didn't get you.");
                    else
                        gameManager.SitonObj(name);*/

                    gameManager.SitonObj("chair");

                    break;

                case "stand_up":
                    gameManager.Standup();

                    break;

                default:
                    break;
            }
        }
        else
        {
            Debug.Log("Failed to invoke OnMessage();");
        }

        _waitingForResponse = false;
    }

    
}
