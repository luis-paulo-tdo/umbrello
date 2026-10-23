var builder = DistributedApplication.CreateBuilder(args);

var owmApiKey = builder.AddParameter("owm-api-key", secret: true);

var sql = builder.AddSqlServer("sql")
    .WithImageTag("2025-latest")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("umbrello-sql-data");

var db = sql.AddDatabase("umbrellodb");

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithDataVolume("umbrello-rabbit-data");

var mailpit = builder.AddMailPit("mailpit");

builder.AddProject<Projects.Umbrello_MigrationService>("migrator")
    .WithReference(db)
    .WaitFor(db);

builder.AddProject<Projects.Umbrello_Api>("api")
    .WithReference(db)
    .WithReference(rabbitmq)
    .WithEnvironment("OpenWeatherMap__ApiKey", owmApiKey)
    .WaitFor(db)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Umbrello_Scheduler>("scheduler")
    .WithReference(db)
    .WithReference(rabbitmq)
    .WaitFor(db)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Umbrello_Worker_Forecast>("worker-forecast")
    .WithReference(db)
    .WithReference(rabbitmq)
    .WithEnvironment("OpenWeatherMap__ApiKey", owmApiKey)
    .WaitFor(db)
    .WaitFor(rabbitmq);

builder.AddProject<Projects.Umbrello_Worker_Notifications>("worker-notifications")
    .WithReference(db)
    .WithReference(rabbitmq)
    .WithReference(mailpit)
    .WaitFor(db)
    .WaitFor(rabbitmq);

builder.Build().Run();
