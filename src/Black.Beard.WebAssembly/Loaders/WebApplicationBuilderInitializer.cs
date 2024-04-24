//using Bb.ComponentModel.Attributes;
//using Bb.ComponentModel;
//using Bb.ComponentModel.Loaders;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

//namespace Black.Beard.WebAssembly.Loaders
//{

//    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebAssemblyHostBuilder>), LifeCycle = IocScopeEnum.Transiant)]
//    public class WebApplicationBuilderInitializer : ApplicationInitializerBase<WebAssemblyHostBuilder>
//    {

//        public override void Execute(WebAssemblyHostBuilder builder)
//        {

//            //// Add services to the container.
//            //builder.Services.AddRazorPages();
//            //builder.Services.AddServerSideBlazor();
//            //builder.Services.AddSingleton<WeatherForecastService>();

//            builder.RootComponents.Add<App>("#app");
//            builder.RootComponents.Add<HeadOutlet>("head::after");

//            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
//            builder.Services.AddMudServices();

//        }

//    }



//}
