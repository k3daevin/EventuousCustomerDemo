using Eventuous;
using EventuousCustomerDemo.Customer.Events;
using System.Collections.Immutable;
using System.Linq;

namespace EventuousCustomerDemo.Customer
{
    public record CustomerState : State<CustomerState>
    {
        public string Name { get; init; } = null!;

        public ImmutableHashSet<string> Tags { get; init; } = ImmutableHashSet<string>.Empty;

        public int Cash { get; init; } = 0;

        public CustomerState()
        {            
            On<NameChangedEvent>((state, @event) =>
                state with { 
                    Name = @event.Name,
                }
            );
            
            On<TagsAddedEvent>((state, @event) =>
                state with { 
                    Tags = state.Tags.Union(@event.Tags),
                }
            );

            On<TagsRemovedEvent>((state, @event) =>
                state with
                {
                    Tags = state.Tags.Except(@event.Tags),
                }
            );

            On<CashChangedEvent>((state, @event) =>
                state with
                {
                    Cash = Cash + @event.Amount,
                }
            );
        }
    }
}
