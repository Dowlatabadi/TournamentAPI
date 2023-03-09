namespace Tournament.Domain.Events;

public class QuestionCreatedEvent: BaseEvent
{
    public QuestionCreatedEvent(Question item)
    {
        Item = item;
    }

    public Question Item { get; }
}
