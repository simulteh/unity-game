using UnityEngine;

public abstract class NetworkNode : MonoBehaviour
{
    // Можно добавить общие свойства и методы для всех сетевых узлов (маршрутизаторов, компьютеров)
    public virtual string Repr()
    {
        return "NetworkNode()";
    }
}