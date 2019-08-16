using System;

public class DataExchange<T>
{
    private Action<T> onDataChanged;
    public event Action<T> OnDataChanged
    {
        add
        {
            onDataChanged += value;
        }

        remove
        {
            onDataChanged -= value;
        }
    }

    private T dataValue;
    public T DataValue
    {
        get
        {
            return dataValue;
        }

        set
        {
            if (!value.Equals(dataValue))
            {
                dataValue = value;
                onDataChanged.Invoke(dataValue);
            }
        }
    }
}