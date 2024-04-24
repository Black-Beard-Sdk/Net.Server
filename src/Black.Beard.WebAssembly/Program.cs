using Bb.ComponentModel;
using Bb.Servers.Web.Loaders;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args).Initialize(ConstantsCore.Initialization,
    x =>
    {
    
    });


WebApplication app = builder.Build().Initialize(ConstantsCore.Initialization, 
    x =>
    {

    });


app.Run();
