﻿using System.Diagnostics.CodeAnalysis;

namespace TransitWP7.Resources
{
    // This class is needed to workaround an issue with the internal constructor on resource files.
    public class SRHelper
    {
        private static SR _sr = new SR();

        public SR SR
        {
            get
            {
                return _sr;
            }
        }
    }
}