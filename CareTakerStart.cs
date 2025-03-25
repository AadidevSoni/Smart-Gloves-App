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
        StartCoroutine(GetHeartRateAndBloodStatusRepeatedly());
        StartCoroutine(BlinkText()); // Start blinking coroutine
    }

    IEnumerator GetHeartRateAndBloodStatusRepeatedly()
    {
        while (true)
        {
            GetHeartRateStatus();
            GetBloodRateStatus();
            yield return new WaitForSeconds(1f); // Fetch every second
        }
    }

    void GetHeartRateStatus()
    {
        RestClient.Get(firebaseURL).Then(response => 
        {
            string heartStatus = response.Text.Trim('"');

            if (string.IsNullOrEmpty(heartStatus))
            {
                heartRateStatusText.text = "HEART RATE: No Data"; // Handle missing data
                return;
            }

            if (int.TryParse(heartStatus, out heartRate))
            {
                UpdateHeartRateUI(heartRate);
            }
            else
            {
                heartRateStatusText.text = "Invalid Heart Rate Data ";
            }
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching Heart Rate: " + error.Message);
            heartRateStatusText.text = "Error Fetching Data";
        });
    }

    void GetBloodRateStatus()
    {
        RestClient.Get(firebaseURL1).Then(response => 
        {
            string O2BloodString = response.Text.Trim('"'); // Store as string first

            if (string.IsNullOrEmpty(O2BloodString))
            {
                bloodO2Text.text = "BLOOD O2 LEVEL: No Data";
                return;
            }

            if (int.TryParse(O2BloodString, out bloodO2)) // Corrected parsing
            {
                UpdateBloodO2UI(bloodO2);
            }
            else 
            {
                bloodO2Text.text = "Invalid Blood O2 Data";
            }
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching Blood O2 Data: " + error.Message);
            bloodO2Text.text = "Error Fetching Data";
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

    // Coroutine to Blink Heart Rate & Blood O₂ Text
    IEnumerator BlinkText()
    {
        while (true)
        {
            heartRateStatusText.enabled = !heartRateStatusText.enabled; // Toggle visibility
            bloodO2Text.enabled = !bloodO2Text.enabled;     // Toggle visibility
            yield return new WaitForSeconds(1f); // Blink every 1 second
        }
    }
}