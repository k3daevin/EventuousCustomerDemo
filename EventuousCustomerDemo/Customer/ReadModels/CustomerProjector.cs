using Eventuous;
using Eventuous.Projections.MongoDB;
using EventuousCustomerDemo.Customer.Events;
using MongoDB.Driver;

namespace EventuousCustomerDemo.Customer.ReadModels
{
    public class CustomerProjector : MongoProjector<CustomerReadModel>
    {
        public CustomerProjector(IMongoDatabase database, TypeMapper? typeMap = null) : base(database, typeMap)
        {

            On<NameChangedEvent>(b => b
                .UpdateOne
                .DefaultId()
                .Update((@event, update) => update
                    .Set(customer => customer.Name, @event.Name)   
                )
            );

            On<TagsAddedEvent>(b => b
                .UpdateOne
                .DefaultId()
                .Update((@event, update) => update
                    .AddToSetEach(customer => customer.Tags, @event.Tags)
                )
            );

            On<TagsRemovedEvent>(b => b
                .UpdateOne
                .DefaultId()
                .Update((@event, update) => update
                    .PullAll(customer => customer.Tags, @event.Tags)
                )
            );

        }


    }
}
