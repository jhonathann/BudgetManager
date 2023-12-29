public class Rubro{
    public static Dictionary<string,Rubro> rubros = new Dictionary<string, Rubro>();
    public string Name {get; set;}
    
    public Concept concept;

    public Rubro(string name, Concept concept){
        Name = name;
        this.concept = concept;
        rubros.Add(Name,this);
    }
}

