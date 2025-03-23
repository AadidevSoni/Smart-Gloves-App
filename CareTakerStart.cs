using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using TMPro;

public class CareTakerStart : MonoBehaviour
{   
    public TextMeshProUGUI userText;
    public static string userName;
    User user = new User();
    void Start()
    {
        RetrieveFromDatabase();
    }

    private void updateUserText(){  //this function only gets executed once retrivation of data from firebase is done
        userText.text = "USER: " + user.userName;
    }

    private void RetrieveFromDatabase()
    {
        RestClient.Get<User>("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/" + userName + ".json").Then(response => 
        {
            user = response;
            updateUserText();
        });
    }

}