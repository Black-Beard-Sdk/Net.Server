using Bb.ComponentModel;
using Bb.ComponentModel.Attributes;
using Bb.Servers.Web.Models;

namespace Bb.Servers.Web.Middlewares.EntryFullLogger
{

    [ExposeClass(Context = ConstantsCore.Service, ExposedType = typeof(IRequestResponseLogModelCreator), LifeCycle = IocScopeEnum.Singleton)]
    public class RequestResponseLogModelCreator : IRequestResponseLogModelCreator
    {


        public RequestResponseLogModelCreator()
        {
            LogModel = new RequestResponseLogModel();
        }

        public RequestResponseLogModel LogModel { get; private set; }


        public string LogString()
        {
            var jsonString = LogModel.Serialize(true);
            return jsonString;
        }

    }


}
