
public class AttributeModifier
{
    public readonly float Value;
    public readonly AttributeModType Type;
    public readonly int Order;
    public readonly object Source;

    public AttributeModifier(float value, AttributeModType modType, int order, object source)
    {
        Value = value;
        Type = modType;
        Order = order;
        Source = source;
    }
    public AttributeModifier(float value, AttributeModType modType) : this(value, modType, (int)modType, null) { }
    public AttributeModifier(float value, AttributeModType modType, int order) : this(value, modType, order, null) { }
    public AttributeModifier(float value, AttributeModType modType, object source) : this(value, modType, (int)modType, source) { }  
}
