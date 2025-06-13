using Unity.Multiplayer.Center.Common;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class HTTP
{
    readonly string VERSION = "1.0"; //версия 
    Enum_HTTPMethods Method; //метод
    string URL; //юрл
    SHTTPHeading Heading; //заголовок

    public HTTP(string url, Enum_HTTPMethods method, SHTTPHeading heading) //конструктор
    { 
        this.URL = url;
        this.Method = method;
        this.Heading = heading;
    }

    public string Request(string[] BodyRequest) // запрос на сервер. BodyRequest это тело запроса, данные которые нам нужны по сути
    {
        // тут должен быть запрос на сервер, а потом сервер вызывать ответ, но вот как бы так
        return Answer(BodyRequest);
    }

    string Answer(string[] BodyRequest) //ответ сервера
    {
        //имитация бурной работы
        return "типо какие то данные и строка состояния";
    }

}
