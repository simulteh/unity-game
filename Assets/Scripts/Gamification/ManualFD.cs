using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ManualFD : MonoBehaviour
{
    private notesBD notes = new notesBD();

    [SerializeField] public TextMeshProUGUI noteTextLeftPage;
    [SerializeField] public TextMeshProUGUI noteTextRightPage;
    private int currentID = 0;
    [SerializeField] public int levelID;

    public void Start()
    {
        ShowNotes();
    }

    public void ShowNotes()
    {
        noteTextLeftPage.text = notes.notes[levelID][currentID];
        noteTextRightPage.text = notes.notes[levelID][currentID + 1];
    }

    public void ChangePage(int count)
    {
        int temp = currentID + count;
        if (0 <= temp && temp < notes.notes[levelID].Count)
        {
            currentID = temp;
        }
        ShowNotes();
    }
}


public class notesBD
{
    public Dictionary<int, List<string>> notes = new Dictionary<int, List<string>>
    {
        {0, new List<string> {
                "Первым делом вам нужно открыть браузер. Это как достаточно знакомое окно, где вы обычно смотрите видео или читаете новости. Используйте его, чтобы получить доступ к настройкам маршрутизатора. В этой программе найдите адресную строку, куда обычно пишете названия сайтов. Здесь важно ввести IP-адрес вашего маршрутизатора. Этот адрес похож на телефонный номер и часто выглядит как 192.168.1.1 или что-то подобное. Это кетировочный код, который надо напечатать и подтвердить нажатием клавиши Enter.",
                "Настройка домашнего Wi-Fi может показаться сложной, но на самом деле это довольно просто. Начнем с главного — маршрутизатор. Это устройство, которое обеспечивает связь дома с интернетом и позволяет всем вашим гаджетам общаться между собой через сеть.",
                "Когда попадете на страницу входа, вы увидите, что нужно ввести логин и пароль. Эти данные часто предоставляются с устройством, но в процессе первой настройки их важно изменить. Это защита, чтобы никто другой не смог изменить ваши настройки. Придумайте что-то надёжное, но при этом такое, что вам будет легко запомнить.",
                "После этого ваш Wi-Fi будет готов к работе: все устройства смогут подключаться, и интернет будет доступен в вашем доме без дополнительных усилий. Просто и удобно!"
            }
        }
    };
}