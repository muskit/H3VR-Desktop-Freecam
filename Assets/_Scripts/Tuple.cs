public class Tuple<T, U>
{
    public Tuple()
    {
    }

    public Tuple(T first, U second)
    {
        this.First = first;
        this.Second = second;
    }

    public T First { get; set; }
    public U Second { get; set; }
};