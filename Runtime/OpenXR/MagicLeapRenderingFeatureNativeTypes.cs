using System;

namespace MagicLeap.OpenXR.Features
{
    public partial class MagicLeapRenderingExtensionsFeature
    {
        internal enum XrRenderingStructTypes
        {
            XrTypeFrameEndInfoML = 1000135000,
            XrTypeGlobalDimmerFrameEndInfo = 1000136000,
        }

        [Flags]
        internal enum XrFrameEndInfoFlagsML : ulong
        {
            Protected = 1,
            Vignette = 2
        }

        [Flags]
        internal enum XrGlobalDimmerFrameEndInfoFlags : ulong
        {
            Enabled = 1,
        }

        internal struct XrFrameEndInfoML
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal float FocusDistance;
            internal XrFrameEndInfoFlagsML Flags;
        }

        internal struct XrGlobalDimmerFrameEndInfoML
        {
            internal XrRenderingStructTypes Type;
            internal IntPtr Next;
            internal float DimmerValue;
            internal XrGlobalDimmerFrameEndInfoFlags Flags;
        }
    }
}
