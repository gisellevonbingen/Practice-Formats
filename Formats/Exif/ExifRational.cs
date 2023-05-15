﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streams.IO;

namespace Formats.Exif
{
    public struct ExifRational : IEquatable<ExifRational>
    {
        public uint Numerator { get; set; }
        public uint Denominator { get; set; }

        public ExifRational(uint numerator, uint denominator) : this()
        {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }

        public ExifRational(DataProcessor processor) : this()
        {
            this.Read(processor);
        }

        public double Ratio => (double)this.Numerator / (double)this.Denominator;

        public void Read(DataProcessor processor)
        {
            this.Numerator = processor.ReadUInt();
            this.Denominator = processor.ReadUInt();
        }

        public void Write(DataProcessor processor)
        {
            processor.WriteUInt(this.Numerator);
            processor.WriteUInt(this.Denominator);
        }

        public override string ToString() => $"{this.Numerator} / {this.Denominator} => {this.Ratio}";

        public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

        public override bool Equals(object obj) => obj is ExifRational other && other.Equals(this);

        public bool Equals(ExifRational other) => this.Numerator == other.Numerator && this.Denominator == other.Denominator;

        public static bool operator ==(ExifRational left, ExifRational right) => left.Equals(right);

        public static bool operator !=(ExifRational left, ExifRational right) => !(left == right);
    }

}
