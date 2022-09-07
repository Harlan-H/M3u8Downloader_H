using Newtonsoft.Json;
using System;
using System.IO;

namespace Settings
{
    public class SettingsManager : ICloneable
    {

        [JsonIgnore]
        public Configuration Configuration { get; } = new Configuration();

        [JsonIgnore]
        public string FullDirectoryPath => string.IsNullOrEmpty(Configuration.DirectoryName) ? Configuration.StorageSpace.GetDirectoryPath() : Configuration.DirectoryName;

        [JsonIgnore]
        public string FullFilePath => Path.Combine(FullDirectoryPath,Configuration.FileName);

        public SettingsManager()
        {
        }

        public object Clone()
        {
            var clone = (SettingsManager)Activator.CreateInstance(GetType());
            clone.CopyFrom(this);
            return clone;
        }


        public virtual void Save()
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(this);
                File.WriteAllText(FullFilePath, serialized);
            }catch
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
            }catch(FileNotFoundException)
            {

            }
        }

        public virtual void Reset()
        {
            var referenceSettings = (SettingsManager)Activator.CreateInstance(GetType());
            CopyFrom(referenceSettings);
        }

        public virtual void CopyFrom(SettingsManager settingsManager)
        {
            if (settingsManager == null)
                throw new ArgumentNullException(nameof(settingsManager));

            var serialize = JsonConvert.SerializeObject(settingsManager);
            JsonConvert.PopulateObject(serialize, this);
        }

    }
}
