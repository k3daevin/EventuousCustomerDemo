using Eventuous;

namespace EventuousCustomerDemo.Customer.Events
{
    [EventType("V1.TagsAdded")]
    public record TagsAddedEvent(string[] Tags);
}
