using Eventuous;

namespace EventuousCustomerDemo.Customer.Events
{
    [EventType("V1.TagsRemoved")]
    public record TagsRemovedEvent(string[] Tags);
}
