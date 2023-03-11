using Eventuous;
using Eventuous.AspNetCore;
using Eventuous.EventStore;
using EventuousCustomerDemo.Customer;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);


TypeMap.RegisterKnownEventTypes(typeof(Program).Assembly);



// Add services to the container.
builder.Services.AddEventStoreClient("esdb://admin:changeit@localhost:2113?tls=false");
//tcp://admin:changeit@localhost:1113
builder.Services.AddAggregateStore<EsdbEventStore>();
builder.Services.AddFunctionalService<CustomerFuncService, CustomerState>();


builder.Services.AddSingleton(builder.Services);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAggregateFactory();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
