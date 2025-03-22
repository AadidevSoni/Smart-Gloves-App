using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class User
{
    public string userName;
    public string userEmail;
    public string userType;

    public User()
    {
        userName = Submit_Button.userName;
        userEmail = Submit_Button.userEmail;
        userType = Submit_Button.userType;
    }
}