using UnityEngine;
using Proyecto26;

public class WearerStart : MonoBehaviour
{
    private string firebaseURL = "https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/.json";

    void Start()
    {
        SetInitialStatus();
    }

    void SetInitialStatus()
    {
        // ✅ Correct JSON format to update nested fields
        string json = "{ \"status\": { \"message\": \"BUSY\" }, \"message\": { \"message\": \"\" } }";

        // ✅ Use PATCH to update only the specified fields without deleting others
        RestClient.Patch(firebaseURL, json)
            .Then(response => Debug.Log("✅ Firebase updated: status/message = BUSY, messages/message = \"\""))
            .Catch(error => Debug.LogError("❌ Error updating Firebase: " + error.Message));
    }
}