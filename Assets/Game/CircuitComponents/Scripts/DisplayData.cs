public struct DisplayData
{
    public float MinVoltage { get; private set; }
    public float MaxVoltage { get; private set; }

    public DisplayData(float initialValue)
    {
        MinVoltage = initialValue;
        MaxVoltage = initialValue;
    }

    public float Height => MaxVoltage - MinVoltage;

    public float Center => (MaxVoltage + MinVoltage) / 2;

    public void EncapsulateVoltage(float value)
    {
        if (value < MinVoltage)
            MinVoltage = value;
        if (value > MaxVoltage)
            MaxVoltage = value;
    }
}