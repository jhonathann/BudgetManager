public class Rubro{
    public static Dictionary<int,Rubro> rubros = new();
    private static int idCount = 0;
    public int Id {get; private set;}
    public string Name {get; set;}
    
    public Concept concept;

    public Rubro(string name, Concept concept){
        Name = name;
        this.concept = concept;
        idCount++;
        Id=idCount;
        rubros.Add(Id,this);
    }
}

