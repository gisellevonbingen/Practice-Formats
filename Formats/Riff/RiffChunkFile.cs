using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Formats.Riff
{
    public class RiffChunkFile : RiffChunkAbstractList
    {
        public RiffChunkFile()
        {

        }

        public override int TypeKey => KnownRiffTypeKeys.Riff;

    }

}
