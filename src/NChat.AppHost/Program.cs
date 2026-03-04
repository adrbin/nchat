var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameterFromConfiguration(
    "postgres-password",
    "Parameters:postgres-password",
    secret: true);

var postgres = builder
    .AddPostgres("postgres", password: postgresPassword)
    .WithDataVolume("nchat-postgres-data");

var nchatDb = postgres.AddDatabase("nchatdb");

builder.AddProject<Projects.NChat_Web>("web")
    .WithReference(nchatDb)
    .WaitFor(nchatDb);

builder.Build().Run();
