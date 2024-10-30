using AutoFixture;

namespace ChargeStationTests;

public abstract class TestBase 
{
    private Fixture Fixture { get; set; }
    protected TestBase() 
    {
        Fixture = new Fixture();
        Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(behavior => Fixture.Behaviors.Remove(behavior));
        Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }
}