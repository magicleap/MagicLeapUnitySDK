

namespace MagicLeap.Android
{
    using System;
    using NDK.Camera;

    public enum RequestTemplate
    {
        Invalid = 0,
        Preview = 1,
        StillCapture = 2,
        Record = 3,
        VideoSnapshot = 4,
        ZeroShutterLag = 5,
        Manual = 6,
    }

    internal static class RequestTemplateExtensions
    {
        public static ACaptureRequest.Template ToNDKTemplate(this RequestTemplate self)
        {
            if (self == RequestTemplate.Invalid)
                throw new InvalidOperationException();
            return (ACaptureRequest.Template)self;
        }
    }
}
