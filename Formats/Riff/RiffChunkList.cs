using System;
using System.Collections.Generic;
using System.Text;

namespace Formats.Riff
{
    public class RiffChunkList : RiffChunkAbstractList
    {
        public RiffChunkList()
        {

        }

        public override int TypeKey => KnownRiffTypeKeys.List;

    }

}
