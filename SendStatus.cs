using UnityEngine;
using Proyecto26;
using TMPro;
using System.Collections;

public class SendStatus : MonoBehaviour
{   
    public TextMeshProUGUI statusText; // ✅ Reference to UI text panel
    public GameObject statusPanel; // ✅ Reference to the panel
    public float panelDisplayTime = 3f; // ✅ Time before panel disappears

    void Start()
    {
        statusPanel.SetActive(false); // Hide panel initially
    }

    public void OnComing()
    {
        PostToDatabase("COMING");
    }

    public void OnWaiting()
    {
        PostToDatabase("WAIT 5 MIN");
    }

    public void OnBusy()
    {
        PostToDatabase("BUSY");
    }

    private void PostToDatabase(string message)
    {
        StatusMessage statusMessage = new StatusMessage(message); // ✅ Pass message correctly

        RestClient.Put("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/status.json", statusMessage)
        .Then(response =>
        {
            Debug.Log("✅ Status sent to Firebase: " + message);
            ShowMessageOnPanel(message); // ✅ Show message on panel
        })
        .Catch(error =>
        {
            Debug.LogError("❌ Error sending status: " + error.Message);
        });
    }

    private void ShowMessageOnPanel(string message)
    {
        statusText.text = "Message Sent to Wearer: " + message;
        statusPanel.SetActive(true); // ✅ Show panel when message is sent
        StartCoroutine(HidePanelAfterDelay()); // ✅ Start coroutine to hide panel
    }

    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(panelDisplayTime);
        statusPanel.SetActive(false); // ✅ Hide panel after delay
    }
}