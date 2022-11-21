namespace M3u8Downloader_H.RestServer.Attributes
{
    internal class RequiredAttribute : BaseAttribute
    {
        public override bool Validate(object obj, object value)
        {
            if (value is null)
                return false;
            else if (value is string s)
                return !string.IsNullOrWhiteSpace(s);
            else if (value is Uri uri)
                return !string.IsNullOrWhiteSpace(uri.OriginalString);
            else
                return true;
        }
    }
}
