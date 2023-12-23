
    public class Category{
    public static List<Category> categories = new List<Category>();
    public string Name {get; set;}

    public Category(string name){
        Name = name;
        categories.Add(this);
    }
}