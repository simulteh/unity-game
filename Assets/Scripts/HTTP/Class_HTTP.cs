using Unity.Multiplayer.Center.Common;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class HTTP
{
    readonly string VERSION = "1.0"; //������ 
    Enum_HTTPMethods Method; //�����
    string URL; //���
    SHTTPHeading Heading; //���������

    public HTTP(string url, Enum_HTTPMethods method, SHTTPHeading heading) //�����������
    { 
        this.URL = url;
        this.Method = method;
        this.Heading = heading;
    }

    public string Request(string[] BodyRequest) // ������ �� ������. BodyRequest ��� ���� �������, ������ ������� ��� ����� �� ����
    {
        // ��� ������ ���� ������ �� ������, � ����� ������ �������� �����, �� ��� ��� �� ���
        return Answer(BodyRequest);
    }

    string Answer(string[] BodyRequest) //����� �������
    {
        //�������� ������ ������
        return "���� ����� �� ������ � ������ ���������";
    }

}
