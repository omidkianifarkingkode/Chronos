using Kingkode.Chronos.Clock.Persistences;
using Kingkode.Chronos.Clock.Services;
using System;
using UnityEngine;

namespace Kingkode.Chronos.Clock.Infrasturctures
{
    public sealed class DefaultTimeSnapshotStorage : ITimeSnapshotStorage
    {
        private const string StorageKey = "timing.gameclock.snapshot.v1";

        public void Clear()
        {
            PlayerPrefs.DeleteKey(StorageKey);
        }

        public void Save(GameClockSnapshot snapshot)
        {
            PlayerPrefs.SetString(StorageKey, JsonUtility.ToJson(snapshot));
        }

        public bool TryLoad(out GameClockSnapshot snapshot)
        {
            if (!PlayerPrefs.HasKey(StorageKey))
            {
                snapshot = default;
                return false;
            }

            try
            {
                snapshot = JsonUtility.FromJson<GameClockSnapshot>(PlayerPrefs.GetString(StorageKey));
                return true;
            }
            catch (Exception ex)
            {
                snapshot = default;
                Debug.LogException(ex);
                return false;
            }
        }
    }
}
