using System.Collections;
using UnityEngine;
using TMPro;
using Proyecto26;
using UnityEngine.UI;

public class CareTakerStatusChanger : MonoBehaviour
{
    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/status.json";

    public GameObject[] popupPanels; // ✅ Array of panels
    public TextMeshProUGUI[] notificationTexts; // ✅ Array of text elements

    public AudioSource notificationAudioSource; // ✅ Audio source
    public AudioClip notificationSound; // ✅ Notification sound

    private string lastReceivedStatus = ""; // ✅ Tracks the last status

    void Start()
    {
        HideAllPanels(); // ✅ Hide all panels initially
        StartCoroutine(GetCareTakerStatusRepeatedly()); // ✅ Start checking Firebase
    }

    IEnumerator GetCareTakerStatusRepeatedly()
    {
        while (true)
        {
            GetCareTakerStatus();
            yield return new WaitForSeconds(1f); // ✅ Check every second
        }
    }

    void GetCareTakerStatus()
    {
        RestClient.Get<StatusToGet>(firebaseURL).Then(response => 
        {
            if (response.message != lastReceivedStatus) // ✅ Show popup only if message changes
            {
                lastReceivedStatus = response.message; // ✅ Store new message
                ShowPopup(response.message);
            }
        })
        .Catch(error =>
        {
            Debug.LogError("❌ Error Fetching Status: " + error.Message);
        });
    }

    void ShowPopup(string message)
    {
        HideAllPanels(); // ✅ Hide all previous panels

        int panelIndex = GetPanelIndex(message); // ✅ Get index based on message
        if (panelIndex == -1) return; // ✅ If message is invalid, do nothing

        popupPanels[panelIndex].SetActive(true); // ✅ Show correct panel
        notificationTexts[panelIndex].text = message; // ✅ Update text

        // ✅ Play notification sound
        if (notificationAudioSource != null && notificationSound != null)
        {
            notificationAudioSource.PlayOneShot(notificationSound);
        }
    }

    // ✅ Hide all panels initially
    void HideAllPanels()
    {
        foreach (GameObject panel in popupPanels)
        {
            panel.SetActive(false);
        }
    }

    // ✅ Determine which panel to show based on the message
    int GetPanelIndex(string message)
    {
        switch (message)
        {
            case "COMING": return 0;
            case "WAIT 5 MIN": return 1;
            case "BUSY": return 2;
            default: return -1; // ✅ Invalid message (do nothing)
        }
    }

    public void ClosePanel(int index)
    {
        popupPanels[index].SetActive(false); // ✅ Hide the specific panel
    }
}

[System.Serializable]
public class StatusToGet
{
    public string message; // ✅ Stores the Firebase message
}