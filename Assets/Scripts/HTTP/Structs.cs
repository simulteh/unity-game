using System;
using UnityEngine;

//какие то структы
public struct SHTTPHeading // заголовок
{
    public Enum_ContentType ContentType;
    public string Authorization;
    public string User_Agent;
}

public struct SHTTPBody // тело запроса
{
    public string Name;
    public string? Value;
}

