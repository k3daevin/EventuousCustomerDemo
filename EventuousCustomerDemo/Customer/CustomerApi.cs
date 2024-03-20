using Eventuous.AspNetCore.Web;
using Eventuous;
using Microsoft.AspNetCore.Mvc;
using EventuousCustomerDemo.Customer.Commands;

namespace EventuousCustomerDemo.Customer
{

    [Route("/customer")]
    public class CustomerApi : CommandHttpApiBaseFunc<CustomerState>
    {
        private readonly IEventStore _eventStore;
        private readonly IFuncCommandService<CustomerState> _funcCommandService;

        public CustomerApi(
            IFuncCommandService<CustomerState> service,
            IEventStore eventStore
            ) : base(service) {
            _eventStore = eventStore;
            _funcCommandService = service;
        }

        [HttpPost]
        [Route("create")]
        public Task<ActionResult<Result>> Create(
            [FromBody] CreateCustomerCommand cmd,
            CancellationToken cancellationToken
        ) => Handle(cmd, cancellationToken);

        [HttpPost]
        [Route("changeName")]
        public Task<ActionResult<Result>> ChangeName(
            [FromBody] ChangeNameCommand cmd,
            CancellationToken cancellationToken
        ) => Handle(cmd, cancellationToken);

        [HttpPost]
        [Route("changeTags")]
        public Task<ActionResult<Result>> ChangeTags(
            [FromBody] ChangeTagsCommand cmd,
            CancellationToken cancellationToken
        ) => Handle(cmd, cancellationToken);

        [HttpPost]
        [Route("changeCash")]
        public Task<ActionResult<Result>> ChangeCash(
            [FromBody] ChangeCashCommand cmd,
            CancellationToken cancellationToken
        ) => Handle(cmd, cancellationToken);


        [HttpGet("getState/{customerId}")]
        public async Task<IActionResult> GetState(string customerId, CancellationToken cancellationToken)
        {
            var streamName = _funcCommandService.GetStream(customerId);
            var loadedState = await _eventStore.LoadState<CustomerState>(streamName, cancellationToken);
            return Ok(loadedState.State);
        }
        
    }
}
