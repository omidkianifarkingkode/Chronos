using System;

namespace Kingkode.Chronos.Clock.Services
{
    public interface IServerTimeSyncer
    {
        void Synce(Action<long> callback);
        void Synce(string url, Action<long> callback);
    }
}