namespace EventuousCustomerDemo.Customer.Commands
{
    public record CreateCustomerCommand(
        string CustomerId,
        string Name,
        string[] Tags
        );
}
