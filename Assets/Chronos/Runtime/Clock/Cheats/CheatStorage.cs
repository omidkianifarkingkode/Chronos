using UnityEngine;

namespace Kingkode.Chronos.Clock.Cheats
{
    public class CheatStorage 
    {
        private const string key_server_datetime = "clock-cheat-server-datetime";
        private const string key_local_datetime = "clock-cheat-local-datetime";
        private const string key_system_tick = "clock-cheat-system-tick";

        public int? ServerDateTime { 
            get 
            {
                return PlayerPrefs.GetInt(key_server_datetime, 0);
            }
            set 
            {
                if (value == null)
                {
                    PlayerPrefs.DeleteKey(key_server_datetime);
                    return;
                }

                PlayerPrefs.SetInt(key_server_datetime, value.Value);
                PlayerPrefs.Save();
            }
        }

        public int? LocalDateTime
        {
            get
            {
                return PlayerPrefs.GetInt(key_local_datetime, 0);
            }
            set
            {
                if (value == null)
                {
                    PlayerPrefs.DeleteKey(key_local_datetime);
                    return;
                }

                PlayerPrefs.SetInt(key_local_datetime, value.Value);
                PlayerPrefs.Save();
            }
        }

        public long? SystemTick
        {
            get
            {
                if(long.TryParse(PlayerPrefs.GetString(key_system_tick, "0"), out var loadedTicks))
                    return loadedTicks;

                return 0;
            }
            set
            {
                if (value == null)
                {
                    PlayerPrefs.DeleteKey(key_system_tick);
                    return;
                }

                PlayerPrefs.SetString(key_system_tick, value.Value.ToString());
                PlayerPrefs.Save();
            }
        }
    }
}
