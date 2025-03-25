using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MessageToSend
{
    public string message;

    public MessageToSend()
    {
        message = SendMessage.message;
    }
}