using System.Collections;
using UnityEngine;
using Proyecto26;
using TMPro;
using UnityEngine.UI; 

#if UNITY_ANDROID
using Unity.Notifications.Android;
using UnityEngine.Android;
#endif

public class ManageWearerOutputs : MonoBehaviour
{
    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/WearerStatus.json";
    public GameObject[] statusPanels; 
    public TextMeshProUGUI[] statusTexts;  
    public Button[] closeButtons; 

    public AudioSource notificationAudioSource;
    public AudioClip notificationSound;

    [SerializeField]
    AndroidNotifications androidNotifications;

    private int[] statusValues = { 0, 0, 0, 0, 0 }; 

    void Start()
    {
        for (int i = 0; i < statusPanels.Length; i++)
        {
            statusPanels[i].SetActive(false);
            if (closeButtons[i] != null)
            {
                int index = i; 
                closeButtons[i].onClick.AddListener(() => ClosePanel(index));
            }
        }

        #if UNITY_ANDROID
        androidNotifications.RequestAuthorization();
        androidNotifications.RegisterNotificationChannel();
        #endif

        StartCoroutine(GetWearerStatusRepeatedly());
    }

    IEnumerator GetWearerStatusRepeatedly()
    {
        while (true)
        {
            GetWearerStatus();
            yield return new WaitForSeconds(1f); //Check Firebase every second
        }
    }

    void GetWearerStatus()
    {
        RestClient.Get<WearerState>(firebaseURL).Then(response =>
        {
            WearerState status = response;

            int[] currentValues = { status.NeedWater, status.NeedFood, status.NeedWashroom, status.NeedHelp, status.Emergency };

            // Check for changes & Show Messages
            CheckAndShowMessage(0, currentValues[0], "Wearer Needs Water!");
            CheckAndShowMessage(1, currentValues[1], "Wearer Needs Food!");
            CheckAndShowMessage(2, currentValues[2], "Wearer Needs Washroom!");
            CheckAndShowMessage(3, currentValues[3], "Wearer Needs Help!");
            CheckAndShowMessage(4, currentValues[4], "EMERGENCY ALERT!");

            statusValues = currentValues;
        })
        .Catch(error =>
        {
            Debug.LogError("Error Fetching Wearer Status: " + error.Message);
        });
    }

    private void CheckAndShowMessage(int index, int newValue, string message)
    {
        if (newValue == 1 && statusValues[index] == 0) // Only show when changed from 0 to 1
        {
            statusTexts[index].text = message;
            statusPanels[index].SetActive(true);
            
            PlayNotificationSound();

            #if UNITY_ANDROID
            string title = "Wearer Alert";

            switch (index)
            {
                case 0: title = "Need Water"; break;
                case 1: title = "Need Food"; break;
                case 2: title = "Need Washroom"; break;
                case 3: title = "Need Help"; break;
                case 4: title = "Emergency Alert"; break;
            }

            androidNotifications.SendNotification(title, message, 1); // 1 second delay
            Debug.Log("NOTIFICATION: " + title + " , " + message);
            #endif
        }
    }

    public void ClosePanel(int index)
    {
        statusPanels[index].SetActive(false); // Hide the panel

        string[] keys = { "NeedWater", "NeedFood", "NeedWashroom", "NeedHelp", "Emergency" };
        string firebasePath = $"https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/WearerStatus.json";

        string json = $"{{ \"{keys[index]}\": 0 }}";

        RestClient.Patch(firebasePath, json)
            .Then(response => Debug.Log($"{keys[index]} reset to 0 in Firebase without deleting the key"))
            .Catch(error => Debug.LogError($"Error resetting {keys[index]}: " + error.Message));
    }

    private void PlayNotificationSound()
    {
        if (notificationAudioSource != null && notificationSound != null)
        {
            notificationAudioSource.PlayOneShot(notificationSound);
        }
    }
}