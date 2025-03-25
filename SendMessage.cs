using UnityEngine;
using Proyecto26;
using TMPro;

public class SendMessage : MonoBehaviour
{   
    public TMP_InputField messageInput;
    public static string message;

    public void OnEnter()
    {
        message = messageInput.text.Trim(); // ✅ Remove spaces

        if (string.IsNullOrEmpty(message)) // ✅ Prevent empty messages
        {
            Debug.LogWarning("⚠ Message is empty! Not sending.");
            return;
        }

        PostToDatabase(message);
        messageInput.text = ""; // ✅ Clear input after sending
    }

    private void PostToDatabase(string message)
    {
        MessageToSend messageToSend = new  MessageToSend();

        RestClient.Put("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/message.json", messageToSend)
        .Then(response => Debug.Log("✅ Message overwritten in Firebase: " + message));
    }
}