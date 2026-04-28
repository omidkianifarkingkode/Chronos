using UnityEngine;

public class OfferpackManager : MonoBehaviour 
{
    public static OfferpackManager Instance { get; private set; }

    [SerializeField] OfferpackView sample;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateNewOfferpack(CreateOfferpackDto offerpack) 
    {
        var newOfferpack = Instantiate(sample, transform);
        newOfferpack.Initialize(offerpack);
    }
}
