using System.Collections.Immutable;

namespace EventuousCustomerDemo.Customer.Commands
{
    public record ChangeTagsCommand(
        string CustomerId,
        string[] Add,
        string[] Remove
        );
}
