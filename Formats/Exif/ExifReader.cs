using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streams.IO;

namespace Formats.Exif
{
    public class ExifReader : IDisposable
    {
        private readonly SiphonBlock Siphon;
        private readonly SiphonStream Stream;
        private readonly DataProcessor Processor;

        private readonly List<ExifRawEntry> RawEntries;
        private long NextIfdOffset;

        public ExifReader(Stream input)
        {
            this.Siphon = SiphonBlock.ByRemain(input);
            this.Stream = this.Siphon.SiphonStream;
            this.Processor = ExifContainer.CreateExifProcessor(this.Stream);

            try
            {
                var signature = this.Processor.ReadBytes(ExifContainer.SignatureLength);

                if (ExifContainer.TryGetEndian(signature, out var isLittleEndian) == false)
                {
                    throw new IOException($"Signature not found");
                }

                this.Processor.IsLittleEndian = isLittleEndian;
                var endianChecker = this.Processor.ReadShort();

                if (endianChecker != ExifContainer.EndianChecker)
                {
                    throw new IOException($"Endian Check Failed : Reading={endianChecker:X4}, Require={ExifContainer.EndianChecker:X4}");
                }

            }
            catch (Exception)
            {
                this.Dispose();
                throw;
            }

            this.RawEntries = new List<ExifRawEntry>();
            this.NextIfdOffset = this.Processor.ReadInt();
        }

        public bool NextDitrectory()
        {
            if (this.NextIfdOffset == 0)
            {
                return false;
            }

            var siphon = this.Stream;
            var processor = this.Processor;
            siphon.Position = this.NextIfdOffset;

            var entryCount = processor.ReadShort();
            this.RawEntries.Clear();

            for (var i = 0; i < entryCount; i++)
            {
                var entry = new ExifRawEntry();
                entry.ReadInfo(processor);
                this.RawEntries.Add(entry);
            }

            this.NextIfdOffset = processor.ReadInt();
            return true;
        }

        public bool NextEntry(out KeyValuePair<ExifTagId, ExifValue> pair)
        {
            if (this.RawEntries.Count == 0)
            {
                pair = new(ExifTagId.None, null);
                return false;
            }

            var rawEntry = this.RawEntries[0];
            this.RawEntries.RemoveAt(0);

            var value = rawEntry.ValueType.ValueGenerator();

            if (rawEntry.IsOffset == true)
            {
                this.Stream.Position = rawEntry.ValueOrOffset;
                value.Read(rawEntry, this.Processor);
            }
            else
            {
                using var ms = new MemoryStream();
                var entryProcessor = ExifContainer.CreateExifProcessor(ms, this.Processor);
                entryProcessor.WriteInt(rawEntry.ValueOrOffset);

                ms.Position = 0L;
                value.Read(rawEntry, entryProcessor);
            }

            pair = new(rawEntry.TagId, value);
            return true;
        }

        public bool IsLittleEndian => this.Processor.IsLittleEndian;

        protected virtual void Dispose(bool disposing)
        {
            this.Stream.Dispose();
            this.Siphon.Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            this.Dispose(true);
        }

        ~ExifReader()
        {
            this.Dispose(false);
        }

    }

}
