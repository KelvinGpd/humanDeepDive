using Meta.WitAi;
using Oculus.Voice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Voice;
using Oculus.Platform;
using Meta.WitAi.TTS.Utilities;
using System.IO;

public class EventHandler : MonoBehaviour
{
    [SerializeField] public OrganInterface[] organs;

    [SerializeField] public GameObject targetPosition;
    public AppVoiceExperience voiceExperience;
    public float distanceFromCamera = 0.01f;

    [SerializeField] public Transform xrCamera;
    private OrganInterface prevObj = null;

    [SerializeField] public LLMService lLMService;
    [SerializeField] public TTSSpeaker tts;

    void Start()
    {
        string welcomeTextPath = Path.Combine(UnityEngine.Application.dataPath, "Scripts", "welcome.txt"); // Specify UnityEngine.Application
        string welcomeText = File.ReadAllText(welcomeTextPath);

        tts.Speak(welcomeText);
        foreach (var organ in organs)
        {
            Debug.Log(organ.gameObject.name);
        }
        //handleOrganChange("brain");
    }

    public void handleOrganChange(string newOrgan)
    {
        string newOrganName = "id:"+newOrgan;

        if (prevObj)
        {
            if (prevObj.gameObject.name.Equals(newOrganName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            prevObj.MoveBack();
            prevObj.setInteractible(false);
            new WaitForSeconds(1);

        }

        foreach (var organ in organs)
        {
            if(organ.gameObject.name.Equals(newOrganName, StringComparison.OrdinalIgnoreCase))
            {
                Vector3 pos = xrCamera.position + xrCamera.forward * 0.5f;
                GameObject go = new GameObject();
                go.transform.position = pos;
                organ.MoveToSpotlight(go.transform);
                prevObj = organ;
                organ.setInteractible(true);
                return;
            }
        }
    }

    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            voiceExperience.Activate();
        }
    }

    public void onStartListening()
    {
        Debug.Log("LISTENING"); 
    }

    public void onFullTranscription(string transcription)
    {
        Debug.Log("Transcript");
        lLMService.AskAnatomyQuestion(transcription);
    }

}
