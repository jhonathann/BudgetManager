public class Rubro
{
    public static Dictionary<int, Rubro> Rubros { get; private set; } = new();
    public static int IdCount { get; private set; } = 0;
    public int Id { get; private set; }
    public string Name { get; private set; }

    public Concept Concept { get; private set; }

    public Rubro(string name, Concept concept)
    {
        Name = name;
        this.Concept = concept;
        IdCount++;
        Id = IdCount;
        Rubros.Add(Id, this);
    }
}

