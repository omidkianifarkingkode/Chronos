using Kingkode.Chronos;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OfferpackView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI label;
    [SerializeField] TextMeshProUGUI timer;
    [SerializeField] Button button;

    private static int OfferpackNumber = 1;

    public void Initialize(CreateOfferpackDto data)
    {
        label.SetText("Offerpack" + OfferpackNumber++);

        Chronos.Scheduler.Every(1.Seconds(), () =>
        {
            Debug.Log(label.text);
            timer.SetText((data.EndAt - DateTimeOffset.Now).ToString(@"hh\:mm\:ss"));
        });

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => { Debug.Log(label.text); });

        gameObject.SetActive(true);
    }
}
