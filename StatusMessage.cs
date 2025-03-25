using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StatusMessage
{
    public string message;

    public StatusMessage(string message) // ✅ Pass message directly
    {
        this.message = message;
    }
}