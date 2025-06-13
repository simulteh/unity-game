using System;
using UnityEngine;

//����� �� �������
public struct SHTTPHeading // ���������
{
    public Enum_ContentType ContentType;
    public string Authorization;
    public string User_Agent;
}

public struct SHTTPBody // ���� �������
{
    public string Name;
    public string? Value;
}

