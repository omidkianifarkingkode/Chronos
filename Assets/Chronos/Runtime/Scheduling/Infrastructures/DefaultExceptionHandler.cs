using System;
using UnityEngine;

namespace Kingkode.Chronos.Scheduling
{
    public class DefaultExceptionHandler
    {
        private readonly ILogger _logger;
        public DefaultExceptionHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void Handle(Exception ex)
        {
            _logger.Log(LogType.Error, $"Exception in scheduled action: {ex}");
        }
    }
}
