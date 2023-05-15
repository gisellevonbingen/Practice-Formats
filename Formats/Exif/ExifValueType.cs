﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formats.Exif
{
    public class ExifValueType
    {
        public static IEnumerable<ExifValueType> Values => _Values.AsReadOnly();
        private readonly static List<ExifValueType> _Values = new();
        private readonly static Dictionary<short, ExifValueType> Lookups = new();

        public static readonly ExifValueType Byte = Register(new ExifValueType("Byte", 1, false, 1, () => new ExifValueBytes()));
        public static readonly ExifValueType ASCII = Register(new ExifValueType("ASCII", 2, true, 1, () => new ExifValueASCII()));
        public static readonly ExifValueType Short = Register(new ExifValueType("Short", 3, false, 2, () => new ExifValueShorts()));
        public static readonly ExifValueType Long = Register(new ExifValueType("Long", 4, false, 4, () => new ExifValueLongs()));
        public static readonly ExifValueType Rational = Register(new ExifValueType("Rational", 5, true, 8, () => new ExifValueRationals()));
        public static readonly ExifValueType SByte = Register(new ExifValueType("SByte", 6, false, 1, () => new ExifValueSBytes()));
        public static readonly ExifValueType Undefined = Register(new ExifValueType("Undefined", 7, false, () => new ExifValueUndefineds()));
        public static readonly ExifValueType SShort = Register(new ExifValueType("SShort", 8, false, 2, () => new ExifValueSShorts()));
        public static readonly ExifValueType SLong = Register(new ExifValueType("SLong", 9, false, 4, () => new ExifValueSLongs()));
        public static readonly ExifValueType SRational = Register(new ExifValueType("SRational", 10, true, 8, () => new ExifValueSRationals()));
        public static readonly ExifValueType Float = Register(new ExifValueType("Float", 11, false, 4, () => null));
        public static readonly ExifValueType Double = Register(new ExifValueType("Double", 12, false, 8, () => null));

        public static ExifValueType FromId(short id, ExifValueType fallback = default) => Lookups.TryGetValue(id, out var value) ? value : fallback;

        private static T Register<T>(T value) where T : ExifValueType
        {
            _Values.Add(value);
            Lookups[value.Id] = value;
            return value;
        }

        public string Name { get; }
        public short Id { get; }
        public bool DefaultOffset { get; }
        public int ElementSize { get; }

        public Func<ExifValue> ValueGenerator { get; }

        public ExifValueType(string name, short id, bool defaultOffset, Func<ExifValue> valueGenerator)
        {
            this.Name = name;
            this.Id = id;
            this.DefaultOffset = defaultOffset;
            this.ElementSize = 1;
            this.ValueGenerator = valueGenerator;
        }

        public ExifValueType(string name, short id, bool defaultOffset, int elementSize, Func<ExifValue> valueGenerator) : this(name, id, defaultOffset, valueGenerator)
        {
            this.ElementSize = elementSize;
        }

        public override string ToString() => this.Name;

        public override int GetHashCode() => this.Id;

        public override bool Equals(object obj) => this == obj;

    }

    public class TiffValueType<H, V> : ExifValueType where H : ExifValue
    {
        public TiffValueType(string name, short id, bool defaultOffset, Func<H> valueGenerator) : base(name, id, defaultOffset, valueGenerator)
        {

        }

    }

}
