using AutoFixture;

namespace TestProject1;

public abstract class TestBase 
{
    protected Fixture Fixture { get; set; }
    protected TestBase() 
    {
        Fixture = new Fixture();
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(behavior => Fixture.Behaviors.Remove(behavior));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}