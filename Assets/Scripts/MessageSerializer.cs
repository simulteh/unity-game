using System.Text;
using UnityEngine;
public static class MessageSerializer
{
    // Сериализация объекта в JSON и байты
    public static byte[] Serialize<T>(T obj)
    {
        string json = JsonUtility.ToJson(obj);
        return Encoding.UTF8.GetBytes(json);
    }
    // Десериализация байтов в объект T
    public static T Deserialize<T>(byte[] data)
    {
        string json = Encoding.UTF8.GetString(data);
        return JsonUtility.FromJson<T>(json);
    }
}
