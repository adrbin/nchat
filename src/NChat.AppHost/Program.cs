var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder
    .AddPostgres("postgres")
    .WithDataVolume("nchat-postgres-data");

var nchatDb = postgres.AddDatabase("nchatdb");

builder.AddProject<Projects.NChat_Web>("web")
    .WithReference(nchatDb)
    .WaitFor(nchatDb);

builder.Build().Run();
