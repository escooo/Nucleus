﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FreeBuild.Results
{
    /// <summary>
    /// Abstract base class for dictionaries that store analysis results,
    /// either directly or within further sub-dictionaries.
    /// </summary>
    [Serializable]
    public abstract class ResultsDictionary<TKey, TValue> : Dictionary<TKey,TValue>
    {
        #region Constructors

        /// <summary>
        /// Deserialisation constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected ResultsDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Initialises a new empty ResultsDictionary
        /// </summary>
        public ResultsDictionary() : base() {}

        #endregion
    }
}