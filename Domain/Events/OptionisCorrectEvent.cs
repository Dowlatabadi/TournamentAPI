namespace Tournament.Domain.Events;

public class OptionisCorrectEvent: BaseEvent
{
    public OptionisCorrectEvent(Option item)
    {
        Item = item;
    }

    public Option Item { get; }
}
