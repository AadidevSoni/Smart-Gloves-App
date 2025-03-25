using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioSource audioSource; // ✅ Assign in Inspector (on the Empty GameObject)
    public AudioClip buttonClickSound; // ✅ Assign button sound in Inspector
    public Button[] buttons; // ✅ Assign multiple buttons in Inspector

    void Start()
    {
        if (buttons.Length == 0)
        {
            Debug.LogError("⚠ No buttons assigned in the Inspector!");
            return;
        }

        foreach (Button btn in buttons) // ✅ Loop through all buttons
        {
            btn.onClick.AddListener(PlaySound); // ✅ Attach function to each button
        }
    }

    void PlaySound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound); // ✅ Play sound on click
        }
        else
        {
            Debug.LogError("⚠ AudioSource or AudioClip is missing in the Inspector!");
        }
    }
}