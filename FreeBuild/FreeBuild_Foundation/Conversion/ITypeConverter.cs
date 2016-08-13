﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBuild.Conversion
{
    /// <summary>
    /// Interface for classes which provide functionality to convert from one type to another
    /// </summary>
    public interface ITypeConverter
    {
        #region Properties

        /// <summary>
        /// The description of this converter
        /// </summary>
        string Description { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Perform the conversion operation on the specified object
        /// </summary>
        /// <param name="fromObject">The object to be converted</param>
        /// <returns></returns>
        object Convert(object fromObject);

        #endregion
    }
}
