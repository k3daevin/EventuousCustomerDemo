using Eventuous;

namespace EventuousCustomerDemo.Customer.Events
{
    [EventType("V1.CashChanged")]
    public record CashChangedEvent(int Amount);
}
