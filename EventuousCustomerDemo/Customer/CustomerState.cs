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
            On<NameChangedEvent>(static (state, @event) =>
                state with { 
                    Name = @event.Name,
                }
            );
            
            On<TagsAddedEvent>(static (state, @event) =>
                state with { 
                    Tags = state.Tags.Union(@event.Tags),
                }
            );

            On<TagsRemovedEvent>(static (state, @event) =>
                state with
                {
                    Tags = state.Tags.Except(@event.Tags),
                }
            );

            On<CashChangedEvent>(static (state, @event) =>
                state with
                {
                    Cash = state.Cash + @event.Amount,
                }
            );
        }
    }
}
