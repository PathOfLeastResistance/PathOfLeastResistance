public struct DisplayData
{
    public float MinVoltage { get; private set; }
    public float MaxFoltage { get; private set; }

    public DisplayData(float initialValue)
    {
        MinVoltage = initialValue;
        MaxFoltage = initialValue;
    }

    public float Height => MaxFoltage - MinVoltage;

    public float Center => (MaxFoltage + MinVoltage) / 2;

    public void EncapsulateVoltage(float value)
    {
        if (value < MinVoltage)
            MinVoltage = value;
        if (value > MaxFoltage)
            MaxFoltage = value;
    }
}