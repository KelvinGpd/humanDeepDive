using System.Collections;
using UnityEngine;
using Meta.WitAi.TTS.Utilities;

public class Stroke : MonoBehaviour
{
    // Both arrays now have matching elements with more descriptive messages.
    private string[] Organs = { "heart", "blood_vessels", "brain", "lungs", "" };
    private string[] Descriptions =
    {
        "The heart is pumping blood  through the body, nothing is wrong at this stage.",
        "A blood clot starts to form, partially blocking the flow of blood to the brain.",
        "The brain is starved of oxygen, at this stage, sudden numbness or weakness, confusion, trouble speaking, trouble seeing, dizziness, loss of balance, severe headache, double vision, drowsiness, nausea or vomiting may occur.",
        "As the brain shuts down, so do other important organs, like the lungs, as their capacity to exchange oxygen dwindles to nothing.",
        "Maintaining a healthy lifestyle through blood pressure control, regular exercise, a balanced diet, and avoiding smoking or excessive alcohol can significantly reduce the risk of stroke."
    };
    [SerializeField] public TTSSpeaker tts;
    [SerializeField] public EventHandler eventHandler;

    public IEnumerator PlayAnimation()
    {
        for (int i = 0; i < Organs.Length; i++)
        {
            string organName = Organs[i];
            string description = Descriptions[i];

            eventHandler.handleOrganChange(organName);
            tts.Speak(description);


            yield return new WaitForSeconds(8);

        }
    }
}

//https://www.nia.nih.gov/health/stroke/stroke-signs-causes-and-treatment