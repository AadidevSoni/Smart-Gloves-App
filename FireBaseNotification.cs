using System.Collections;
using UnityEngine;
using Proyecto26;
using TMPro;
using UnityEngine.UI;

public class FirebaseNotification : MonoBehaviour
{
    public TextMeshProUGUI notificationText;  // Popup text
    public GameObject popupPanel;  // Popup panel
    public Button closeButton; // Close button reference
    public AudioSource audioSource; // Audio source for playing sound
    public AudioClip notificationSound; // Assign this in Inspector

    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/message.json"; 
    private string lastReceivedMessage = ""; // Stores last received message

    void Start()
    {
        StartCoroutine(CheckFirebaseRepeatedly()); // Start checking Firebase
        popupPanel.SetActive(false); // Hide popup initially
        closeButton.onClick.AddListener(ClosePopup); // Attach ClosePopup() to button click
    }

    IEnumerator CheckFirebaseRepeatedly()
    {
        while (true)
        {
            CheckFirebaseForMessageUpdates();
            yield return new WaitForSeconds(1f); // Check every second
        }
    }

    void CheckFirebaseForMessageUpdates()
    {
        RestClient.Get<MessageToSend>(firebaseURL).Then(response =>
        {
            if (response.message != lastReceivedMessage) // Show popup only if the message changed
            {
                lastReceivedMessage = response.message; // Store new message
                ShowPopup(response.message);
            }
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching Message: " + error.Message);
        });
    }

    void ShowPopup(string message)
    {
        popupPanel.SetActive(true); // Show popup
        notificationText.text = "CareTaker: " + message;

        // Play notification sound
        if (audioSource != null && notificationSound != null)
        {
            audioSource.PlayOneShot(notificationSound);
        }

        // Automatically close popup after 5 seconds
        StartCoroutine(AutoClosePopup());
    }

    IEnumerator AutoClosePopup()
    {
        yield return new WaitForSeconds(5f);
        popupPanel.SetActive(false); // ✅ Hide popup automatically
    }

    // ✅ Function to close the popup manually
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}