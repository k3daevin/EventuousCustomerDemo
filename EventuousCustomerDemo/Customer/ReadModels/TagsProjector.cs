using Eventuous;
using Eventuous.Projections.MongoDB;
using EventuousCustomerDemo.Customer.Events;
using MongoDB.Driver;
using ZstdSharp.Unsafe;

namespace EventuousCustomerDemo.Customer.ReadModels
{
    public class TagsProjector : MongoProjector<TagsReadModel>
    {
        public TagsProjector(IMongoDatabase database, TypeMapper? typeMap = null) : base(database, typeMap)
        {
            On<TagsAddedEvent>(b => b
                .UpdateMany
                .Filter((ctx, doc) => ctx.Message.Tags.Contains(doc.Id))
                .UpdateFromContext((ctx, update) => update
                    .AddToSet(tags => tags.Customers, ctx.Stream.GetId())
                )
            );
        }
    }
}
