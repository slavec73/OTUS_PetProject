namespace VacationPlanner.Core.Events
{
    public interface IEventDispatcher
    {
        Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : IEvent;
    }
}
