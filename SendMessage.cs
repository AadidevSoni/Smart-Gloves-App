using UnityEngine;
using Proyecto26;
using TMPro;
using System.Collections;

public class SendMessage : MonoBehaviour
{   
    public TMP_InputField messageInput;
    public static string message;
    public TextMeshProUGUI messageText; // Reference to UI text panel
    public GameObject messagePanel; // Fixed the variable name
    public float panelDisplayTime = 3f; // Time before panel disappears

    public void OnEnter()
    {
        message = messageInput.text.Trim(); // Remove spaces

        if (string.IsNullOrEmpty(message)) // Prevent empty messages
        {
            Debug.LogWarning("⚠ Message is empty! Not sending.");
            return;
        }

        PostToDatabase(message);
        ShowMessageOnPanel(); // Show confirmation panel
        messageInput.text = ""; // Clear input after sending
    }

    private void PostToDatabase(string message)
    {
        MessageToSend messageToSend = new MessageToSend();

        RestClient.Put("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/message.json", messageToSend)
        .Then(response => Debug.Log("Message overwritten in Firebase: " + message))
        .Catch(error => Debug.LogError("Error sending message: " + error.Message));
    }

    private void ShowMessageOnPanel()
    {
        messageText.text = "Message Sent to Wearer";
        messagePanel.SetActive(true); // Show panel when message is sent
        StartCoroutine(HidePanelAfterDelay()); // Start coroutine to hide panel
    }

    private IEnumerator HidePanelAfterDelay()
    {
        yield return new WaitForSeconds(panelDisplayTime);
        messagePanel.SetActive(false); // Hide panel after delay
    }
}