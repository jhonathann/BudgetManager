public class Concept{
    public static Dictionary<int ,Concept> concepts = new();
    private static int idCount = 0;
    public int Id{get; private set;}
    public string Name {get; set;}
    public Category category;

    public Concept(string name, Category category){
        Name = name;
        this.category = category;
        idCount++;
        Id=idCount;
        concepts.Add(Id, this);
    }
}