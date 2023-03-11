namespace EventuousCustomerDemo.Customer.Commands
{
    public record ChangeNameCommand(
        string CustomerId,
        string Name
        );
}
