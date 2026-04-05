using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace M3u8Downloader_H.Settings.Services;

public abstract class SettingsBase
{
    private readonly JsonTypeInfo _rootTypeInfo;
    private string FullDirectoryPath => string.IsNullOrEmpty(Configuration.DirectoryName) ? Configuration.StorageSpace.GetDirectoryPath() : Configuration.DirectoryName;
    private string FullFilePath => Path.Combine(FullDirectoryPath, Configuration.FileName);

    [JsonIgnore]
    public Configuration Configuration { get; } = new Configuration();


    protected SettingsBase(IJsonTypeInfoResolver jsonTypeInfoResolver)
        :this(new JsonSerializerOptions
    {
        TypeInfoResolver = jsonTypeInfoResolver
    })
    {
    }

    protected SettingsBase(JsonSerializerOptions jsonOptions)
    {
        _rootTypeInfo = jsonOptions.GetTypeInfo(GetType());
    }

    public virtual void Save()
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(this, _rootTypeInfo);
        string directoryName = Path.GetDirectoryName(FullFilePath);
        if (!string.IsNullOrWhiteSpace(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllBytes(FullFilePath, bytes);
    }

    public virtual bool Load()
    {
        try
        {
            using FileStream utf8Json = File.OpenRead(FullFilePath);
            using JsonDocument jsonDocument = JsonDocument.Parse(utf8Json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            });
            foreach (JsonProperty jsonProperty in jsonDocument.RootElement.EnumerateObject())
            {
                JsonPropertyInfo jsonPropertyInfo = _rootTypeInfo.Properties.FirstOrDefault((JsonPropertyInfo p) => string.Equals(p.Name, jsonProperty.Name, StringComparison.Ordinal));
                if (jsonPropertyInfo is not null)
                {
                    JsonSerializerOptions jsonSerializerOptions = new(jsonPropertyInfo.Options);
                    if (jsonPropertyInfo.CustomConverter != null)
                    {
                        jsonSerializerOptions.Converters.Add(jsonPropertyInfo.CustomConverter);
                    }

                    jsonPropertyInfo.Set?.Invoke(this, jsonProperty.Value.Deserialize(jsonSerializerOptions.GetTypeInfo(jsonPropertyInfo.PropertyType)));
                }
            }

            return true;
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
        {
            return false;
        }
    }


    public virtual void Delete()
    {
        try
        {
            File.Delete(FullFilePath);
        }
        catch (Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
        {
        }
    }

    public T Clone<T>() where T : SettingsBase, new()
    {
        T obj = new();
        foreach (var item in _rootTypeInfo.Properties)
        {
            item.Set?.Invoke(obj, item.Get?.Invoke(this));
        }
        return obj;
    }

    public void CopyFrom<T>(T settings) where T : SettingsBase
    {
        foreach (var item in _rootTypeInfo.Properties)
        {
            item.Set?.Invoke(this, item.Get?.Invoke(settings));
        }
    }

}
