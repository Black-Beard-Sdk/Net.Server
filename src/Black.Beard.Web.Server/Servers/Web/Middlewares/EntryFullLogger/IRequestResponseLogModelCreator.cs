namespace Bb.Servers.Web.Middlewares.EntryFullLogger
{
    public interface IRequestResponseLogModelCreator
    {
        RequestResponseLogModel LogModel { get; }
        string LogString();
    }


}
