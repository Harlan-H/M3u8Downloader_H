using M3u8Downloader_H.RestServer.Attributes;
using M3u8Downloader_H.RestServer.Models;

namespace M3u8Downloader_H.RestServer.Extensions
{
    internal static  class ValidateExtension
    {
        public static void Validate<T>(this T obj) where T : IValidate
        {
            Type type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.IsDefined(typeof(BaseAttribute), false)) continue;

                BaseAttribute attribute = (BaseAttribute)prop.GetCustomAttributes(typeof(BaseAttribute), false)[0];
                if (!attribute.Validate(obj, prop.GetValue(obj)!))
                {
                    throw new Exception(attribute.ExceptionMsg);
                }
            }
        }
    }
}
