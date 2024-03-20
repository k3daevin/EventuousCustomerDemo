using Eventuous.Projections.MongoDB.Tools;

namespace EventuousCustomerDemo.Customer.ReadModels
{
    public record CustomerReadModel : ProjectedDocument
    {
        public CustomerReadModel(string Id) : base(Id)
        {
        }

        public string Name { get; init; } = null!;

        public List<string> Tags { get; init; } = new();
    }
}
