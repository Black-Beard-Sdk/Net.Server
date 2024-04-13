using Bb.ComponentModel.Loaders;

namespace Bb.Servers.Web.Loaders
{
    public class Initializers
    {

        public static void Initialize<T>(T builderbuilder, string path, Action<IApplicationBuilderInitializer<T>> action)
        {

            ExposedAssemblyRepositories assemblies = null;
            var file = path.AsFile();
            if (file.Exists)
                assemblies = file.LoadFromFileAndDeserialize<ExposedAssemblyRepositories>();

            var loader = new InitializationLoader<T>();
            if (assemblies != null)
                loader.InitializeAssemblies(assemblies);

            loader.LoadModules(action).Execute(builderbuilder);

        }

        public static string ResolveFilename<T>(T self, string root)
        {
            return root.Combine(self.GetType().Name + ".json");
        }

    }
}
