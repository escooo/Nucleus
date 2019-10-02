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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.Geometry
{
    /// <summary>
    /// Static class containing mutable tolerance values used during geometric operations
    /// where no overriding tolerance value is specified.
    /// </summary>
    public static class Tolerance
    {
        /// <summary>
        /// The current geometric tolerance used to determine coincidence
        /// </summary>
        public static double Distance { get; set; } = 0.000001;

        /// <summary>
        /// The square of the current geometric tolerance used to determine coincidence
        /// </summary>
        public static double DistanceSquared { get { return Distance * Distance; } }

        /// <summary>
        /// The angle tolerance used for facetting arcs
        /// </summary>
        public static Angle Angle { get; set; } = Angle.FromDegrees(10);

        /// <summary>
        /// The current tolerance value used to determine level inclusivity
        /// </summary>
        public static double Layer { get; set; } = 0.5;

    }
}
