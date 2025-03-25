using System.Collections;
using UnityEngine;
using Proyecto26;
using TMPro;
using UnityEngine.UI; // ✅ Add for button handling

public class ManageWearerOutputs : MonoBehaviour
{
    // ✅ Firebase URL
    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/WearerStatus.json";

    // ✅ Arrays for UI elements (for 5 messages)
    public GameObject[] statusPanels;  // Array of 5 panels
    public TextMeshProUGUI[] statusTexts;  // Array of 5 text fields
    public Button[] closeButtons; // ✅ Array of Close Buttons for each panel

    // ✅ Variables to track previous state
    private int[] statusValues = { 0, 0, 0, 0, 0 }; // Stores previous states

    void Start()
    {
        // Hide all panels initially & Assign close button functionality
        for (int i = 0; i < statusPanels.Length; i++)
        {
            statusPanels[i].SetActive(false);
            if (closeButtons[i] != null)
            {
                int index = i; // ✅ Store index in local variable for lambda function
                closeButtons[i].onClick.AddListener(() => ClosePanel(index));
            }
        }

        StartCoroutine(GetWearerStatusRepeatedly());
    }

    IEnumerator GetWearerStatusRepeatedly()
    {
        while (true)
        {
            GetWearerStatus();
            yield return new WaitForSeconds(1f); // ✅ Check Firebase every second
        }
    }

    void GetWearerStatus()
    {
        RestClient.Get<WearerState>(firebaseURL).Then(response =>
        {
            WearerState status = response;

            // ✅ Store current values in an array
            int[] currentValues = { status.NeedWater, status.NeedFood, status.NeedWashroom, status.NeedHelp, status.Emergency };

            // ✅ Check for changes & Show Messages
            CheckAndShowMessage(0, currentValues[0], "Wearer Needs Water!");
            CheckAndShowMessage(1, currentValues[1], "Wearer Needs Food!");
            CheckAndShowMessage(2, currentValues[2], "Wearer Needs Washroom!");
            CheckAndShowMessage(3, currentValues[3], "Wearer Needs Help!");
            CheckAndShowMessage(4, currentValues[4], "EMERGENCY ALERT!");

            // ✅ Update local status tracking
            statusValues = currentValues;
        })
        .Catch(error =>
        {
            Debug.LogError("❌ Error Fetching Wearer Status: " + error.Message);
        });
    }

    // ✅ Function to Show Message on Specific Panel
    private void CheckAndShowMessage(int index, int newValue, string message)
    {
        if (newValue == 1 && statusValues[index] == 0) // Only show when changed from 0 to 1
        {
            statusTexts[index].text = message;
            statusPanels[index].SetActive(true);
        }
    }

    public void ClosePanel(int index)
    {
        statusPanels[index].SetActive(false); // Hide the panel

        // ✅ Create key names based on the index
        string[] keys = { "NeedWater", "NeedFood", "NeedWashroom", "NeedHelp", "Emergency" };
        string firebasePath = $"https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/WearerStatus.json";

        // ✅ Create a JSON object to update the specific key
        string json = $"{{ \"{keys[index]}\": 0 }}";

        // ✅ Use PATCH instead of PUT to update without deleting the key
        RestClient.Patch(firebasePath, json)
            .Then(response => Debug.Log($"✅ {keys[index]} reset to 0 in Firebase without deleting the key"))
            .Catch(error => Debug.LogError($"❌ Error resetting {keys[index]}: " + error.Message));
    }


}