namespace ChemFactory.scripts.Models;

public class Atom(AtomElement element, int count = 1)
{
    public AtomElement Element { get; set; } = element;

    public int Count { get; set; } = count;

    public override string ToString()
        => Count > 1
            ? $"{Element}{Count}"
            : $"{Element}";
}