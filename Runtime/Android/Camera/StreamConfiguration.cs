
namespace MagicLeap.Android
{
    using System;
    using NDK.Media;
    using Unity.Collections;

    public readonly struct StreamConfiguration : IComparable<StreamConfiguration>, IEquatable<StreamConfiguration>
    {
        public readonly MediaFormat Format;
        public readonly int Height;
        public readonly int Width;

        public float AspectRatio => Width / (float)Height;

        public static readonly StreamConfiguration Invalid = default;

        public bool IsValid => this != Invalid;

        internal int Size => Width * Height;

        public StreamConfiguration(MediaFormat format, int width, int height)
        {
            Format = format;
            Height = height;
            Width = width;
        }

        public int CompareTo(StreamConfiguration other)
            => Size.CompareTo(other.Size);

        public bool Equals(StreamConfiguration other)
            => Format == other.Format && Height == other.Height && Width == other.Width;

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is StreamConfiguration other)
                return Equals(other);
            else
                return false;
        }

        public override int GetHashCode()
            => HashCode.Combine(Format, Height, Width);

        public override string ToString()
            => $"{nameof(StreamConfiguration)}[Format = {Format.ToNameOrHexValue()}, Width = {Width}, Height = {Height}]";

        public static bool operator >(StreamConfiguration lhs, StreamConfiguration rhs)
            => lhs.CompareTo(rhs) > 0;

        public static bool operator <(StreamConfiguration lhs, StreamConfiguration rhs)
            => lhs.CompareTo(rhs) < 0;

        public static bool operator ==(StreamConfiguration lhs, StreamConfiguration rhs)
            => lhs.Equals(rhs);

        public static bool operator !=(StreamConfiguration lhs, StreamConfiguration rhs)
            => !lhs.Equals(rhs);

    }

    public static class StreamConfigurationUtility
    {
        public static bool Contains(this NativeArray<StreamConfiguration> configs, in StreamConfiguration config)
        {
            CheckValidListAndThrow(configs);

            foreach (var cfg in configs)
            {
                if (cfg == config)
                    return true;
            }

            return false;
        }

        public static bool TryFindLargestConfigurationMatchingFormat(this NativeArray<StreamConfiguration> configs, MediaFormat format, out StreamConfiguration outConfig)
        {
            CheckValidListAndThrow(configs);
            outConfig = default;

            foreach (var cfg in configs)
                outConfig.UpdateIfLarger(cfg, format);

            return outConfig.IsValid;
        }

        public static bool TryFindSmallestConfigurationMatchingFormat(this NativeArray<StreamConfiguration> configs, MediaFormat format, out StreamConfiguration outConfig)
        {
            CheckValidListAndThrow(configs);
            outConfig = default;

            foreach (var cfg in configs)
                outConfig.UpdateIfSmaller(cfg, format);

            return outConfig.IsValid;
        }

        internal static void UpdateIfLarger(ref this StreamConfiguration config, in StreamConfiguration other, MediaFormat format, bool requireFormatMatch = true)
        {
            if (!config.IsValid)
            {
                if (other.Format == format)
                    config = other;
                return;
            }

            if (requireFormatMatch && config.Format != other.Format)
                return;
            if (other > config)
                config = other;
        }

        internal static void UpdateIfSmaller(ref this StreamConfiguration config, in StreamConfiguration other, MediaFormat format, bool requireFormatMatch = true)
        {
            if (!config.IsValid)
            {
                if (other.Format == format)
                    config = other;
                return;
            }

            if (requireFormatMatch && config.Format != other.Format)
                return;
            if (other < config)
                config = other;
        }

        private static void CheckValidListAndThrow(NativeArray<StreamConfiguration> configs)
        {
            if (!configs.IsCreated || configs.Length == 0)
                throw new ArgumentNullException(nameof(configs));
        }
    }
}
