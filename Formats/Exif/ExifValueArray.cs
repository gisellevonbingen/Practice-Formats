﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streams.IO;

namespace Formats.Exif
{
    public abstract class ExifValueArray<T> : ExifValue
    {
        private T[] _Values = Array.Empty<T>();
        public T[] Values
        {
            get => this._Values;
            set => this._Values = value ?? Array.Empty<T>();
        }

        public T Value
        {
            get => this.Values[0];
            set => this.Values = new T[] { value };
        }

        public override int RawValueCount => this.Values.Length;

        public ExifValueArray()
        {

        }

        public override void Read(ExifRawEntry entry, DataProcessor processor)
        {
            var count = entry.ValueCount;
            this.Values = new T[count];

            for (var i = 0; i < count; i++)
            {
                this.Values[i] = this.ReadElement(processor);
            }

        }

        public override void Write(ExifRawEntry entry, DataProcessor processor)
        {
            var count = this.Values.Length;

            for (var i = 0; i < count; i++)
            {
                this.WriteElement(this.Values[i], processor);
            }

        }

        public abstract T ReadElement(DataProcessor processor);

        public abstract void WriteElement(T element, DataProcessor processor);

        public override string ToString()
        {
            return $"Type: {this.Type.Name}, Values: [{string.Join(", ", this.Values)}]";
        }

    }

}
