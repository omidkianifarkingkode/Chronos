using UnityEngine;
using UnityEngine.UI;

public class OfferpackCheat : MonoBehaviour
{
    [SerializeField] DateTimeUiElement startTimer;
    [SerializeField] DateTimeUiElement endTimer;
    [SerializeField] Button button;

    private void Awake()
    {
        button.onClick.AddListener(() => 
        {
            var newOfferpack = new CreateOfferpackDto(startTimer.TimeOffset, endTimer.TimeOffset);

            OfferpackManager.Instance.CreateNewOfferpack(newOfferpack);
        });
    }
}
