using System.Collections.Generic;
using System.Reflection;
using SkiaSharp;

namespace PKHeX.Drawing
{
    public static class ResourceFetch
    {
        private static readonly Assembly A = typeof(SpriteBuilder).GetTypeInfo().Assembly;
        private const string ResourcePrefix = "PKHeX.Drawing";

        private static readonly Dictionary<string, SKBitmap> Dict = new Dictionary<string, SKBitmap>();

        public static SKBitmap LoadBitmapResource(string resourceID)
        {
            try
            {
                // var all = A.GetManifestResourceNames();
                var file = $"{ResourcePrefix}.Images.{resourceID}.png";
                if (Dict.TryGetValue(file, out var val))
                    return val;
                using (var stream = A.GetManifestResourceStream(file))
                {
                    if (stream == null)
                        return null;
                    val = SKBitmap.Decode(stream);
                    Dict.Add(file, val);
                    return val;
                }
            }
            catch
            {
                return null;
            }
        }
    }
}