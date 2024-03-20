namespace EventuousCustomerDemo.Customer.Commands
{
    public interface ICustomerCommand
    {
        string CustomerId { get; }
    }
    public record CreateCustomerCommand(
        string CustomerId,
        string Name,
        string[] Tags
        ) : ICustomerCommand;
}
