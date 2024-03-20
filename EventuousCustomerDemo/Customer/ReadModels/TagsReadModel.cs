using Eventuous.Projections.MongoDB;
using Eventuous.Projections.MongoDB.Tools;

namespace EventuousCustomerDemo.Customer.ReadModels
{
    public record TagsReadModel : ProjectedDocument
    {
        public TagsReadModel(string Id) : base(Id)
        {
        }

        public List<string> Customers { get; init; } = new();
    }
}
