using Bb.ComponentModel.Attributes;
using Bb.ComponentModel;
using Bb.ComponentModel.Loaders;

namespace Bb.Servers.Web.Loaders
{


    public static class Initializers
    {

        public static T Initialize<T>(this T self, Action<IApplicationBuilderInitializer<T>> action = null)
        {

            var loader = new InitializationLoader<T>(ConstantsCore.Initialization)
                .LoadModules(action)
                .Execute(self);
            return self;
        }

        public static string ResolveFilename<T>(this T self, string root)
        {
            return root.Combine(self.GetType().Name + ".json");
        }

    }


}
