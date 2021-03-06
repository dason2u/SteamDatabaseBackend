﻿/*
 * Copyright (c) 2013-2015, SteamDB. All rights reserved.
 * Use of this source code is governed by a BSD-style license that can be
 * found in the LICENSE file.
 */

using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;

namespace SteamDatabaseBackend
{
    static class LocalConfig
    {
        private static readonly JsonSerializerSettings JsonFormatted = new JsonSerializerSettings { Formatting = Formatting.Indented };

        [JsonObject(MemberSerialization.OptIn)]
        public class CDNAuthToken
        {
            [JsonProperty]
            public string Server { get; set; }

            [JsonProperty]
            public string Token { get; set; }

            [JsonProperty]
            public DateTime Expiration { get; set; }
        }

        [JsonObject(MemberSerialization.OptIn)]
        public class LocalConfigJson
        {
            [JsonProperty]
            public uint CellID { get; set; }

            [JsonProperty]
            public uint ChangeNumber { get; set; }

            [JsonProperty]
            public string SentryFileName { get; set; } 

            [JsonProperty]
            public byte[] Sentry { get; set; } 

            [JsonProperty]
            public ConcurrentDictionary<uint, CDNAuthToken> CDNAuthTokens { get; set; } 

            public LocalConfigJson()
            {
                CDNAuthTokens = new ConcurrentDictionary<uint, CDNAuthToken>();
            }
        }

        private static readonly string ConfigPath = Path.Combine(Application.Path, "files", ".support", "localconfig.json");

        public static LocalConfigJson Current { get; private set; } = new LocalConfigJson();
        public static uint CellID => Current.CellID;
        public static byte[] Sentry => Current.Sentry;
        public static ConcurrentDictionary<uint, CDNAuthToken> CDNAuthTokens => Current.CDNAuthTokens;

        public static void Load()
        {
            if (File.Exists(ConfigPath))
            {
                Current = JsonConvert.DeserializeObject<LocalConfigJson>(File.ReadAllText(ConfigPath));
            }
            else
            {
                Save();
            }
        }

        public static void Save()
        {
            Log.WriteDebug("Local Config", "Saving...");
            
            File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Current, JsonFormatted));
        }
    }
}
