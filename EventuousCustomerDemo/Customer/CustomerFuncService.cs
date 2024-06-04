using Eventuous;
using EventuousCustomerDemo.Customer.Commands;
using EventuousCustomerDemo.Customer.Events;
using System.Collections.Immutable;
using System.Threading;

namespace EventuousCustomerDemo.Customer
{
    public static class CustomerFuncServiceExtensions
    {
        public static StreamName GetStream(this IFuncCommandService<CustomerState> _, string id) => new($"Customer-{id}");
    }
    public class CustomerFuncService : FunctionalCommandService<CustomerState>
    {
        public CustomerFuncService(IEventStore store, TypeMapper? typeMap = null) : base(store, typeMap)
        {
            OnNew<CreateCustomerCommand>(static cmd => GetStream(cmd.CustomerId), CreateCustomer);
            OnExisting<ChangeNameCommand>(static cmd => GetStream(cmd.CustomerId), ChangeName);
            OnExisting<ChangeTagsCommand>(static cmd => GetStream(cmd.CustomerId), ChangeTags);
            OnExisting<ChangeCashCommand>(static cmd => GetStream(cmd.CustomerId), ChangeCash);
        }

        public static StreamName GetStream(string id) => CustomerFuncServiceExtensions.GetStream(null!, id);

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
            ValidateName(cmd.Name);

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
            ValidateName(cmd.Name);

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
                if (toAdd.Any())
                {
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

        static IEnumerable<object> ChangeCash(
            CustomerState state,
            object[] originalEvents,
            ChangeCashCommand cmd
        )
        {
            if (state.Cash + cmd.Amount < 0)
            {
                throw new ArgumentException("Cash cannot be smaller than 0.");
            }

            yield return new CashChangedEvent(cmd.Amount);
        }

        static void ValidateName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.");
            }
        }
    }
}
