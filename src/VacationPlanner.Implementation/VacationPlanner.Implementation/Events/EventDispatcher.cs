using Microsoft.Extensions.DependencyInjection;
using VacationPlanner.Core.Events;

namespace VacationPlanner.Implementation.Events
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public async Task PublishAsync<TEvent>(TEvent @event)
            where TEvent : IEvent
        {
            var handlers = _serviceProvider
                .GetServices<IEventHandler<TEvent>>();

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(@event);
            }
        }
    }
}
