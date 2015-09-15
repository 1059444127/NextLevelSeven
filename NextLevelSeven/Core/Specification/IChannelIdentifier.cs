﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextLevelSeven.Core.Specification
{
    /// <summary>
    ///     Contains identifiers for a channel in the HL7 specification. (WVI)
    /// </summary>
    public interface IChannelIdentifier : ISpecificationElement
    {
        /// <summary>
        ///     Number of the channel. (WVI.1)
        /// </summary>
        int? Number { get; set; }

        /// <summary>
        ///     Name of the channel. (WVI.2)
        /// </summary>
        string Name { get; set; }
    }
}