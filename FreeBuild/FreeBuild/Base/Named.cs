﻿// Copyright (c) 2016 Paul Jeffries
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using FreeBuild.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBuild.Base
{
    /// <summary>
    /// Abstract base class for unique objects that can be named
    /// </summary>
    [Serializable]
    public abstract class Named : Unique, INamed
    {

        #region Properties

        /// <summary>
        /// Private backing field for Name property
        /// </summary>
        protected string _Name;

        /// <summary>
        /// The name, or mark, of this object
        /// </summary>
        [AutoUI(Order = 100)]
        public virtual string Name
        {
            get
            {
                return _Name;
            }

            set
            {
                _Name = value;
                NotifyPropertyChanged("Name");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Protected default constructor
        /// </summary>
        protected Named() : base() { }

        /// <summary>
        /// Protected duplication constructor
        /// </summary>
        /// <param name="other"></param>
        protected Named(Named other) : base()
        {
            Name = other.Name;
        }

        /// <summary>
        /// Name constructor
        /// </summary>
        /// <param name="name"></param>
        protected Named(string name) : base()
        {
            Name = name;
        }

        #endregion
    }
}
