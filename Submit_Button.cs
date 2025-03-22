using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proyecto26;
using TMPro;

public class Submit_Button : MonoBehaviour
{   
    public TMP_InputField user_name;
    public TMP_InputField user_email;

    public static string userName;
    public static string userEmail;

    void Start()
    {

    }

    public void OnSubmit()
    {
        userName = user_name.text;
        userEmail = user_email.text;
        PostToDatabase();
    }

    private void PostToDatabase()
    {   
        User user = new User();

        RestClient.Post("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/.json",user);
    }
}