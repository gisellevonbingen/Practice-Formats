﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streams.IO;

namespace Formats.Exif
{
    public interface IExifValue
    {
        ExifValueType Type { get; }

        void Read(ExifRawEntry entry, DataProcessor processor);

        void Write(ExifRawEntry entry, DataProcessor processor);
    }

    public abstract class ExifValue : IExifValue
    {
        public abstract ExifValueType Type { get; }

        public abstract int RawValueCount { get; }

        public ExifValue()
        {

        }

        public abstract void Read(ExifRawEntry entry, DataProcessor processor);

        public abstract void Write(ExifRawEntry entry, DataProcessor processor);
    }

}
