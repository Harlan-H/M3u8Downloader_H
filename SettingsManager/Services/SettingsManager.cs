using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace M3u8Downloader_H.Settings.Services
{
    public class SettingsManager
    {

        [JsonIgnore]
        public Configuration Configuration { get; } = new Configuration();

        [JsonIgnore]
        public string FullDirectoryPath => string.IsNullOrEmpty(Configuration.DirectoryName) ? Configuration.StorageSpace.GetDirectoryPath() : Configuration.DirectoryName;

        [JsonIgnore]
        public string FullFilePath => Path.Combine(FullDirectoryPath, Configuration.FileName);

        public SettingsManager()
        {
        }

        public virtual void Save()
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(this);
                File.WriteAllText(FullFilePath, serialized);
            }
            catch
            {
                if (Configuration.ThrowIfCannotSave)
                    throw;
            }
        }

        public virtual void Load()
        {
            try
            {
                if (!File.Exists(FullFilePath)) return;

                var serialized = File.ReadAllText(FullFilePath);
                JsonConvert.PopulateObject(serialized, this);
            }
            catch (Exception)
            {
                if (Configuration.ThrowIfCannotLoad)
                    throw;
            }
        }


        public virtual void Delete()
        {
            try
            {
                File.Delete(FullFilePath);
            }
            catch (FileNotFoundException)
            {

            }
        }

    }
}
