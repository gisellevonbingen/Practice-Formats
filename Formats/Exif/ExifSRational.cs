using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streams.IO;

namespace Formats.Exif
{
    public struct ExifSRational : IEquatable<ExifSRational>
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }

        public ExifSRational(int numerator, int denominator) : this()
        {
            this.Numerator = numerator;
            this.Denominator = denominator;
        }

        public ExifSRational(DataProcessor processor) : this()
        {
            this.Read(processor);
        }

        public double Ratio => (double)this.Numerator / (double)this.Denominator;

        public void Read(DataProcessor processor)
        {
            this.Numerator = processor.ReadInt();
            this.Denominator = processor.ReadInt();
        }

        public void Write(DataProcessor processor)
        {
            processor.WriteInt(this.Numerator);
            processor.WriteInt(this.Denominator);
        }

        public override string ToString() => $"{this.Numerator} / {this.Denominator} => {this.Ratio}";

        public override int GetHashCode()
        {
            return HashCode.Combine(Numerator, Denominator);
        }

        public override bool Equals(object obj) => obj is ExifSRational other && other.Equals(this);

        public bool Equals(ExifSRational other) => this.Numerator == other.Numerator && this.Denominator == other.Denominator;

        public static bool operator ==(ExifSRational left, ExifSRational right) => left.Equals(right);

        public static bool operator !=(ExifSRational left, ExifSRational right) => !(left == right);

    }

}
