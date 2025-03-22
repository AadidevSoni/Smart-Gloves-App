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
    public TMP_Dropdown dropdown;

    public static string userName;
    public static string userEmail;
    public static string userType;

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { DropdownChanged(dropdown); });
    }

    public void OnSubmit()
    {
        userName = user_name.text;
        userEmail = user_email.text;
        userType = dropdown.options[dropdown.value].text;
        PostToDatabase();
    }

    private void PostToDatabase()
    {   
        User user = new User();

        RestClient.Post("https://smart-gloves-29-default-rtdb.asia-southeast1.firebasedatabase.app/.json",user);
    }

    void DropdownChanged(TMP_Dropdown change)
    {
        Debug.Log("Dropdown Changed: " + change.options[change.value].text);
    }
}