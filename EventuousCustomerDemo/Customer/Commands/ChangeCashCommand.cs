namespace EventuousCustomerDemo.Customer.Commands
{
    public record ChangeCashCommand(
        string CustomerId,
        int Amount
    );
}
