﻿using FreeBuild.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBuild.IO
{
    /// <summary>
    /// Serialisation class that can write out a model document to text using a defined
    /// format.
    /// </summary>
    public class ModelDocumentTextSerialiser : DocumentTextSerialiser<ModelDocument>
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ModelDocumentTextSerialiser() : base() { }

        /// <summary>
        /// Format constructor
        /// </summary>
        /// <param name="format"></param>
        public ModelDocumentTextSerialiser(TextFormat format) : base(format) { }

        #endregion

        #region Methods

        public override bool WriteAll(ModelDocument source)
        {
            Write(source); // Document header
            WriteModel(source.Model); // Model data
            return true;
        }

        #endregion
    }
}