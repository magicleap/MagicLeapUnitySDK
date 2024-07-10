namespace MagicLeap.Android
{
    using System;

    public struct SequenceId : IEquatable<SequenceId>
    {
        private int id;
        private bool isSet;

        public bool IsSet => isSet;
        public int Value => IsSet ? id : throw new InvalidOperationException("SequenceId is empty");

        public void Clear()
            => isSet = false;

        public override bool Equals(object obj)
        {
            if (obj is SequenceId seq)
                return Equals(seq);
            return false;
        }

        public bool Equals(SequenceId other)
        {
            return false;
        }

        public override int GetHashCode()
            => isSet ? id.GetHashCode() : -1;

        public static implicit operator SequenceId(int value)
            => new SequenceId
            {
                id = value,
                isSet = true
            };

        public static implicit operator int(SequenceId value)
            => value.Value;
    }
}
