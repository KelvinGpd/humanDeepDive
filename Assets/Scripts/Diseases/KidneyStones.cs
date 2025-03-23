using System.Collections;
using UnityEngine;
using Meta.WitAi.TTS.Utilities;

public class KidneyStones : MonoBehaviour
{
    // Both arrays now have matching elements with descriptive messages about kidney stones.
    private string[] Organs = { "Kidneys", "Ureters", "Bladder", "Urethra", "" };
    private string[] Descriptions =
    {
        "Kidney stones begin to form when minerals and salts in the urine crystallize in the kidneys, often triggered by dehydration or dietary factors.",
        "As these crystalline deposits move down the ureters, they can obstruct urine flow and cause intense pain.",
        "When kidney stones reach the bladder, they may lead to discomfort and increase the risk of urinary tract infections.",
        "The passage of stones through the urethra can further irritate the urinary tract, intensifying the pain.",
        "Maintaining proper hydration, adopting a balanced diet low in sodium and animal protein, and managing underlying metabolic conditions can significantly reduce the risk of kidney stone formation."
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
