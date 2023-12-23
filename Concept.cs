public class Concept{
    public static List<Concept> concepts = new List<Concept>();
    public string Name {get; set;}
    public Category category;

    public Concept(string name, Category category){
        Name = name;
        this.category = category;
        concepts.Add(this);
    }
}