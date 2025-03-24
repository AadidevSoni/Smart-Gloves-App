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
    public AudioSource audioSource; // ✅ Audio source for playing sound
    public AudioClip notificationSound; // ✅ Assign this in Inspector

    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/led/state.json";
    private int previousLedState = -1; // Store last known state

    void Start()
    {
        StartCoroutine(CheckFirebaseRepeatedly()); // ✅ Start checking Firebase
        popupPanel.SetActive(false); // ✅ Hide popup initially
        closeButton.onClick.AddListener(ClosePopup); // ✅ Attach ClosePopup() to button click
    }

    IEnumerator CheckFirebaseRepeatedly()
    {
        while (true)
        {
            CheckFirebaseForChanges();
            yield return new WaitForSeconds(1f); // ✅ Check every second
        }
    }

    void CheckFirebaseForChanges()
    {
        RestClient.Get(firebaseURL).Then(response =>
        {
            string ledStatus = response.Text.Trim('"'); // ✅ Remove extra quotes
            int newLedState = int.Parse(ledStatus); // ✅ Convert to int

            // ✅ Show popup only if state changes
            if (newLedState != previousLedState)
            {
                ShowPopup(newLedState);
                previousLedState = newLedState; // ✅ Update state
            }
        })
        .Catch(error =>
        {
            Debug.LogError("❌ Error Fetching LED Status: " + error.Message);
        });
    }

    void ShowPopup(int ledState)
    {
        popupPanel.SetActive(true); // ✅ Show popup
        notificationText.text = "LED is Turned " + (ledState == 1 ? "ON" : "OFF");

        // ✅ Play notification sound
        if (audioSource != null && notificationSound != null)
        {
            audioSource.PlayOneShot(notificationSound);
        }
    }

    // ✅ Function to close the popup
    public void ClosePopup()
    {
        popupPanel.SetActive(false); // ✅ Hide popup
    }
}