[System.Serializable]
public class PriceDetails
{
    public Type type;
    public string productId;
    public float price;

    public enum Type
    {
        InAppConsumable,InAppNonConsumable,Video
    }
}