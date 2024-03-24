namespace Bb.Servers.Web.Middlewares.EntryFullLogger
{
    public interface IRequestResponseLogger
    {
        void Log(IRequestResponseLogModelCreator logCreator);
    }


}
