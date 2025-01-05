namespace YarikVor.Json.LazyLinq;

public class LazyPerson : LazyClass<Person>
{
    public readonly LazyProperty<string> FirstName;
    public readonly LazyProperty<string> LastName;
    public readonly LazyProperty<int> Age;
    
    public override Person Generate()
    {
        return new Person()
        {
            FirstName = FirstName.Value,
            LastName = LastName.Value,
            age = Age.Value
        };
    }
}