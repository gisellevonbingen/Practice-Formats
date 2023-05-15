﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streams.IO;

namespace Formats.Exif
{
    public class ExifValueBytes : ExifValueNumbers<byte>
    {
        public ExifValueBytes()
        {

        }

        public override ExifValueType Type => ExifValueType.Byte;

        public override byte ReadElement(DataProcessor processor) => processor.ReadByte();

        public override void WriteElement(byte element, DataProcessor processor) => processor.WriteByte(element);
    }

}
