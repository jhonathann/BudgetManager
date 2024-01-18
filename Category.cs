
    public class Category{
    public static Dictionary<string, Category> categories = new();
    public string Name {get; set;}

    public Category(string name){
        Name = name;
        categories.Add(Name, this);
    }
}