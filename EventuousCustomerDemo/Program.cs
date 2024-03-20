using Eventuous;
using Eventuous.AspNetCore;
using Eventuous.EventStore;
using Eventuous.EventStore.Subscriptions;
using Eventuous.Projections.MongoDB;
using EventuousCustomerDemo.Customer;
using EventuousCustomerDemo.Customer.ReadModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

var builder = WebApplication.CreateBuilder(args);


TypeMap.RegisterKnownEventTypes(typeof(Program).Assembly);

builder.Services.AddFunctionalService<CustomerFuncService, CustomerState>();

builder.Services.AddEventStoreClient("esdb://admin:changeit@localhost:2113?tls=false");
builder.Services.AddAggregateStore<EsdbEventStore>();

builder.Services.AddSingleton(GetMongoDatabase());
builder.Services.AddCheckpointStore<MongoCheckpointStore>();
builder.Services.AddSubscription<AllStreamSubscription, AllStreamSubscriptionOptions>(
    "CustomerProjector",
    builder => builder
        .UseCheckpointStore<MongoCheckpointStore>()
        .AddEventHandler<CustomerProjector>()
        .AddEventHandler<TagsProjector>()
    );

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


IMongoDatabase GetMongoDatabase()
{
    var mongoClient = new MongoClient("mongodb://localhost:27017/?readPreference=primary&ssl=false");
    var database = mongoClient.GetDatabase("customer");
    return database;
}