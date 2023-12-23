public class Rubro{
    public static List<Rubro> rubros = new List<Rubro>();
    public string Name {get; set;}
    
    public Concept concept;

    public Rubro(string name, Concept concept){
        Name = name;
        this.concept = concept;
        rubros.Add(this);
    }
}

