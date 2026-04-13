namespace Kingkode.Chronos.Clock.Services
{
    public interface ISystemTickProvider 
    {
        long Frequency { get; }

        long GetTimestamp();
    }
}
