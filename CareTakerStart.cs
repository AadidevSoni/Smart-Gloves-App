using System.Collections;
using UnityEngine;
using Proyecto26;
using TMPro;

public class CareTakerStart : MonoBehaviour
{   
    public TextMeshProUGUI heartRateStatusText;
    public TextMeshProUGUI bloodO2Text;

    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/heart/rate.json"; 
    private string firebaseURL1 = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/blood/O2.json"; 

    private int heartRate = 60; 
    private int bloodO2 = 95;

    void Start()
    {
        StartCoroutine(UpdateDataRepeatedly()); // Fetch data every second
        StartCoroutine(BlinkTextRepeatedly()); // Start blinking with updates
    }

    // Fetch New Data Every Second
    IEnumerator UpdateDataRepeatedly()
    {
        while (true)
        {
            GetHeartRateStatus();
            GetBloodRateStatus();
            yield return new WaitForSeconds(1f); // Fetch new values every second
        }
    }

    void GetHeartRateStatus()
    {
        RestClient.Get(firebaseURL).Then(response => 
        {
            string heartStatus = response.Text.Trim('"');

            if (string.IsNullOrEmpty(heartStatus))
            {
                heartRateStatusText.text = "No Data"; // Handle missing data
                return;
            }

            if (int.TryParse(heartStatus, out heartRate))
            {
                UpdateHeartRateUI(heartRate);
            }
            else
            {
                heartRateStatusText.text = "Invalid Data";
            }
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching Heart Rate: " + error.Message);
            heartRateStatusText.text = "Error";
        });
    }

    void GetBloodRateStatus()
    {
        RestClient.Get(firebaseURL1).Then(response => 
        {
            string O2BloodString = response.Text.Trim('"');

            if (string.IsNullOrEmpty(O2BloodString))
            {
                bloodO2Text.text = "No Data";
                return;
            }

            if (int.TryParse(O2BloodString, out bloodO2))
            {
                UpdateBloodO2UI(bloodO2);
            }
            else 
            {
                bloodO2Text.text = "Invalid Data";
            }
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching Blood O2: " + error.Message);
            bloodO2Text.text = "Error";
        });
    }

    private void UpdateHeartRateUI(int heartRate)
    {
        heartRateStatusText.text = heartRate + " BPM";
    }

    private void UpdateBloodO2UI(int O2Blood)
    {
        bloodO2Text.text = O2Blood + " %";
    }

    // Coroutine to Blink While Updating
    IEnumerator BlinkTextRepeatedly()
    {
        while (true)
        {
            heartRateStatusText.enabled = !heartRateStatusText.enabled; // Toggle visibility
            bloodO2Text.enabled = !bloodO2Text.enabled; // Toggle visibility
            yield return new WaitForSeconds(0.5f); // Blink every 0.5s (Adjust if needed)
        }
    }
}