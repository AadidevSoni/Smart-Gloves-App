using System.Collections;
using UnityEngine;
using Proyecto26;
using TMPro;

public class WearerStart : MonoBehaviour
{   
    public TextMeshProUGUI ledStatusText;
    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/led/state.json"; 
    private int ledState = 0; // Stores current LED state

    void Start()
    {
        StartCoroutine(GetLEDStatusRepeatedly()); // Fetch LED status every second
    }

    IEnumerator GetLEDStatusRepeatedly()
    {
        while (true)
        {
            GetLEDStatus();
            yield return new WaitForSeconds(0.2f); // Fetch every second
        }
    }

    void GetLEDStatus()
    {
        RestClient.Get(firebaseURL).Then(response =>
        {
            string ledStatus = response.Text.Trim('"'); // Remove extra quotes

            ledState = int.Parse(ledStatus); // Convert to integer

            UpdateLEDUI(ledState);
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching LED Status: " + error.Message);
            ledStatusText.text = "Error Fetching Data";
        });
    }

    public void OnLEDClick() // Toggle state on button click
    {
        RestClient.Get(firebaseURL).Then(response =>
        {
            string ledStatus = response.Text.Trim('"'); // ✅ Remove extra quotes

            // Toggle LED state
            int newLedState = (ledStatus == "1") ? 0 : 1; 

            // Send new value to Firebase
            PostToDatabase(newLedState);

        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching LED Status: " + error.Message);
            ledStatusText.text = "Error Fetching Data";
        });
    }

    private void PostToDatabase(int newState)
    {   
        string json = "{ \"state\": " + newState + " }"; // Send JSON with "state" key

        RestClient.Patch("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/led.json", json)
        .Then(response =>
        {
            Debug.Log("LED state updated to: " + newState);
        })
        .Catch(error =>
        {
            Debug.LogError("Error Updating LED State: " + error.Message);
        });
    }

    private void UpdateLEDUI(int state)
    {
        if (state == 1)
        {
            ledStatusText.text = "LED STATUS: ON";
        }
        else
        {
            ledStatusText.text = "LED STATUS: OFF";
        }
    }
}