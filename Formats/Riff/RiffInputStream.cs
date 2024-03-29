﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Streams.IO;

namespace Formats.Riff
{
    public class RiffInputStream : WrappedStream
    {
        public int FormType { get; }
        public string FormTypeToString => this.FormType.TypeKeyToString();
        public RiffChunkPath CurrentPath => this.ChunkPath.Count == 0 ? null : this.ChunkPath[^1];

        protected RiffChunkStream CurrentChunkStream { get; private set; } = null;
        private readonly List<RiffChunkPath> ChunkPath = new();

        public RiffInputStream(Stream baseStream) : this(baseStream, false)
        {

        }

        public RiffInputStream(Stream baseStream, bool leaveOpen) : base(baseStream, leaveOpen)
        {
            var chunkStream = new RiffChunkStream(baseStream);
            var processor = RiffChunk.CreateRiffDataProcessor(chunkStream);
            var formType = processor.ReadInt();
            this.FormType = formType;
            this.ChunkPath.Add(new RiffChunkPath(null, chunkStream.Header, chunkStream, formType));
        }

        public RiffChunkHeader ReadNextChunk()
        {
            if (this.CanRead == false)
            {
                throw new NotSupportedException();
            }
            else if (this.CurrentChunkStream != null)
            {
                this.CloseChunk();
            }

            while (this.BaseStream.Position < this.BaseStream.Length)
            {
                var currentPath = this.CurrentPath;
                currentPath.Index++;

                var stream = new RiffChunkStream(currentPath.Stream);
                var typeKey = stream.Header.TypeKey;

                if (typeKey == KnownRiffTypeKeys.Riff)
                {
                    throw new IOException();
                }
                else if (typeKey == KnownRiffTypeKeys.List)
                {
                    var processor = RiffChunk.CreateRiffDataProcessor(stream);
                    var formType = processor.ReadInt();
                    this.ChunkPath.Add(new RiffChunkPath(currentPath, stream.Header, stream, formType));
                }
                else
                {
                    this.CurrentChunkStream = stream;
                    return stream.Header;
                }

            }

            this.CurrentChunkStream = null;
            return null;
        }

        public void CloseChunk()
        {
            var c = this.CurrentChunkStream;

            if (c != null)
            {
                this.CloseChunk(c);
                this.CurrentChunkStream = null;
            }

            while (this.ChunkPath.Count > 0)
            {
                var index = this.ChunkPath.Count - 1;
                var pathStream = this.ChunkPath[index].Stream;

                if (pathStream.Position % 2 == 1)
                {
                    pathStream.ReadByte();
                }

                if (pathStream.Position >= pathStream.Length)
                {
                    this.CloseChunk(pathStream);
                    this.ChunkPath.RemoveAt(index);
                }
                else
                {
                    break;
                }

            }

        }

        private void CloseChunk(Stream c)
        {
            var remain = c.GetRemain();

            if (this.BaseStream.CanSeek == true)
            {
                this.BaseStream.Position += remain;
            }
            else
            {
                for (var i = 0; i < remain; i++)
                {
                    this.BaseStream.ReadByte();
                }

            }

            c.Dispose();
        }

        public override long Position
        {
            get => this.CurrentChunkStream?.Position ?? 0L;
            set { if (this.CurrentChunkStream != null) { this.CurrentChunkStream.Position = value; } }
        }

        public override long Length => this.CurrentChunkStream?.Length ?? 0L;

        public override void SetLength(long value) => this.CurrentChunkStream?.SetLength(value);

        public override int Read(byte[] buffer, int offset, int count) => this.CurrentChunkStream?.Read(buffer, offset, count) ?? 0;

        public override void Write(byte[] buffer, int offset, int count) => this.CurrentChunkStream?.Write(buffer, offset, count);

        public override long Seek(long offset, SeekOrigin origin) => this.CurrentChunkStream?.Seek(offset, origin) ?? 0L;

        public override void Flush() => this.CurrentChunkStream?.Flush();

        public class RiffChunkPath
        {
            public RiffChunkPath Parent { get; }
            public RiffChunkHeader Header { get; }
            internal Stream Stream { get; }
            public int FormType { get; }

            public RiffChunkPath(RiffChunkPath parent, RiffChunkHeader header, Stream stream, int formType)
            {
                this.Parent = parent;
                this.Header = header;
                this.Stream = stream;
                this.FormType = formType;
            }

            public string FormTypeToString => this.FormType.TypeKeyToString();

            public int Index { get; internal set; } = -1;
            public long Position => this.Stream.Position;
            public long Length => this.Stream.Length;

            public override string ToString()
            {
                return $"{this.Parent}{this.Header.TypeKeyToString}({this.FormTypeToString})[{this.Index}]";
            }

        }

    }

}
