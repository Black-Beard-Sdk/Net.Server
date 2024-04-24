//using Bb.ComponentModel.Attributes;
//using Bb.ComponentModel;
//using Bb.ComponentModel.Loaders;

//namespace Black.Beard.WebAssembly.Loaders
//{

//    [ExposeClass(ConstantsCore.Initialization, ExposedType = typeof(IApplicationBuilderInitializer<WebApplication>), LifeCycle = IocScopeEnum.Transiant)]
//    public class WebApplicationInitializer : ApplicationInitializerBase<WebApplication>
//    {

//        public override void Execute(WebApplication app)
//        {

//            // Configure the HTTP request pipeline.
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Error");
//            }


//            app.UseStaticFiles();

//            app.UseRouting();

//            app.MapBlazorHub();
//            app.MapFallbackToPage("/_Host");

//        }

//    }



//}
