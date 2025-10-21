using UnityEngine;

[System.Serializable]
public class Meta<T>
{
    [SerializeField] T _value = default(T);
    public T Value
    {
        get { return _value; }
        set
        {
            if (!Equals(_value, value))
            {
                _value = value;
                isDirty = true;
            }
        }
    }
    public Meta() { }
    public Meta(T value)
    {
        _value = value;
    }
    public bool isDirty { get; private set; } = false;
    public T GetAndClean()
    {
        isDirty = false;
        return _value;
    }
}