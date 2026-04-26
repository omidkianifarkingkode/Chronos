using Kingkode.Chronos.Clock.Configurations;
using Kingkode.Chronos.Clock.Services;
using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace Kingkode.Chronos.Clock.Infrasturctures
{
    public class DefaultServerTimeSyncer : IServerTimeSyncer
    {
        private readonly ServerTimeSyncOptions _options;
        private readonly ILogger _logger;

        public DefaultServerTimeSyncer(ServerTimeSyncOptions options, ILogger logger)
        {
            _options = options;
            _logger = logger;
        }

        public void Synce(Action<long> callback)
        {
            Synce(_options.ServerUrl, callback);
        }

        public void Synce(string url, Action<long> callback)
        {
            _logger.Log(LogType.Log, $"[UnityServerTimeSyncer] Sending HEAD request to: {url}");

            var request = UnityWebRequest.Head(url);

            request.certificateHandler = new BypassCertificateHandler();

            // Prevent caches
            request.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            request.SetRequestHeader("Pragma", "no-cache");
            request.SetRequestHeader("Expires", "0");

            var operation = request.SendWebRequest();

            operation.completed += _ =>
            {
                HandleResponse(callback, request);
            };
        }

        private void HandleResponse(Action<long> callback, UnityWebRequest request)
        {
            try
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    _logger.Log(LogType.Error, $"[UnityServerTimeSyncer] Request failed: {request.error}");
                    callback?.Invoke(-1);
                    return;
                }

                string dateHeader = request.GetResponseHeader("Date");
                _logger.Log(LogType.Log, $"[UnityServerTimeSyncer] Date header received: {dateHeader}");

                if (string.IsNullOrEmpty(dateHeader))
                {
                    _logger.Log(LogType.Warning, "[UnityServerTimeSyncer] Missing 'Date' header in the response.");
                    callback?.Invoke(-1);
                    return;
                }

                if (DateTime.TryParse(
                    dateHeader,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal,
                    out DateTime serverTime))
                {
                    long unixMillis = new DateTimeOffset(serverTime).ToUnixTimeMilliseconds();

                    _logger.Log(LogType.Log, $"[UnityServerTimeSyncer] Parsed server time (UTC): {serverTime:O}");

                    callback?.Invoke(unixMillis);
                }
                else
                {
                    _logger.Log(LogType.Error, $"[UnityServerTimeSyncer] Failed to parse Date header: {dateHeader}");
                    callback?.Invoke(-1);
                }
            }
            catch (Exception ex)
            {
                _logger.Log(LogType.Exception, $"[UnityServerTimeSyncer] Exception while handling response: {ex}");
                callback?.Invoke(-1);
            }
            finally
            {
                request.certificateHandler?.Dispose();
                request.Dispose();
            }
        }
    }

    public class BypassCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            // Always return true to bypass SSL certificate validation errors
            // caused by wrong local device time.
            return true;
        }
    }
}
