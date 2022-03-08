using System;

public class State<T>
{
    public event EventHandler ValueChanged;

    private T _val;
    public T Value
    {
        get
        {
            return _val;
        }
        set
        {
            _val = value;
            if (ValueChanged != null)
                ValueChanged.Invoke(this, EventArgs.Empty);
        }
    }
    public State(T initVal)
    {
        _val = initVal;
    }
}
