﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Formats.Exif
{
    public abstract class ExifValueNumbers<T> : ExifValueArray<T> where T : IConvertible
    {

    }

}
