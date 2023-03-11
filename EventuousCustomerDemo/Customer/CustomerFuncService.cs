using Eventuous;
using EventuousCustomerDemo.Customer.Commands;
using EventuousCustomerDemo.Customer.Events;
using System.Collections.Immutable;
using System.Threading;

namespace EventuousCustomerDemo.Customer
{
    public class CustomerFuncService : FunctionalCommandService<CustomerState>
    {
        public CustomerFuncService(IEventStore store, TypeMapper? typeMap = null) : base(store, typeMap)
        {
            OnNew<CreateCustomerCommand>(static cmd => GetStream(cmd.CustomerId), CreateCustomer);
            OnExisting<ChangeNameCommand>(static cmd => GetStream(cmd.CustomerId), ChangeName);
            OnExisting<ChangeTagsCommand>(static cmd => GetStream(cmd.CustomerId), ChangeTags);
        }

        public static StreamName GetStream(string id) => new StreamName($"Customer-{id}");

        // POTENTIAL integrate in FunctionalCommandService
        public async Task<CustomerState> GetState(string customerId, CancellationToken cancellationToken)
        {
            var streamName = GetStream(customerId);
            var loadedState = await Store.LoadState<CustomerState>(streamName, cancellationToken);
            return loadedState.State;
        }

        static IEnumerable<object> CreateCustomer(
            CreateCustomerCommand cmd
            )
        {
            if (string.IsNullOrEmpty(cmd.Name))
            {
                throw new ArgumentException("Name cannot be null.");
            }

            yield return new NameChangedEvent(cmd.Name);

            var tagsDistinct = cmd.Tags?.Distinct().ToArray() ?? Array.Empty<string>();
            if (tagsDistinct.Any())
            {
                yield return new TagsAddedEvent(tagsDistinct);
            }
        }

        static IEnumerable<object> ChangeName(
            CustomerState state,
            object[] originalEvents,
            ChangeNameCommand cmd
        )
        {
            if (string.IsNullOrEmpty(cmd.Name))
            {
                throw new ArgumentException("Name cannot be null.");
            }

            if (cmd.Name == state.Name)
            {
                yield break;
            }

            yield return new NameChangedEvent(cmd.Name);
        }

        static IEnumerable<object> ChangeTags(
            CustomerState state,
            object[] originalEvents,
            ChangeTagsCommand cmd
        )
        {
            if (cmd.Add != null && cmd.Remove != null)
            {
                var intersect = cmd.Add.Intersect(cmd.Remove);
                if (intersect.Any())
                {
                    throw new ArgumentException("Add and Remove intersect.");
                }
            }

            if (cmd.Add != null && cmd.Add.Any())
            {
                var toAdd = cmd.Add.Except(state.Tags);
                if (toAdd.Any()) {
                    yield return new TagsAddedEvent(toAdd.ToArray());
                }
            }

            if (cmd.Remove != null && cmd.Remove.Any())
            {
                var toRemove = cmd.Remove.Intersect(state.Tags);
                if (toRemove.Any())
                {
                    yield return new TagsRemovedEvent(toRemove.ToArray());
                }
            }
        }

    }
}
