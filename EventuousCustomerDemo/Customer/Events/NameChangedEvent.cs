using Eventuous;

namespace EventuousCustomerDemo.Customer.Events
{
    [EventType("V1.NameChanged")]
    public record NameChangedEvent(string Name);
}
