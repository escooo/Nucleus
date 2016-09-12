﻿using FreeBuild.Extensions;
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

namespace FreeBuild.Geometry
{
    /// <summary>
    /// An angle expressed in radians.
    /// Represented by an immutable struct wrapping a double value.
    /// Implicit converters to and from the double type are provided,
    /// so this type can be used as though it were an ordinary double.
    /// </summary>
    public struct Angle : IComparable<Angle>
    {
        #region Constants

        /// <summary>
        /// An undefined, invalid angle
        /// </summary>
        public static readonly Angle Undefined = new Angle(double.NaN);

        /// <summary>
        /// A zero angle
        /// </summary>
        public static readonly Angle Zero = new Angle(0d);

        /// <summary>
        /// A right angle, PI/2 (or 90 degrees).
        /// </summary>
        public static readonly Angle Right = new Angle(Math.PI/2);

        /// <summary>
        /// A complete angle, 2*PI (or 360 degrees)
        /// </summary>
        public static readonly Angle Complete = new Angle(2 * Math.PI);

        #endregion

        #region Fields

        /// <summary>
        /// The angle expressed in radians
        /// </summary>
        public readonly double Radians;

        #endregion

        #region Properties

        /// <summary>
        /// Get the angle expressed in degrees
        /// </summary>
        public double Degrees { get { return 180 * Radians / Math.PI; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialises the angle to the specified radians value.
        /// </summary>
        /// <param name="radians">The value of the angle in radians</param>
        public Angle(double radians)
        {
            Radians = radians;
        }

        /// <summary>
        /// Initialises the angle as a copy of another
        /// </summary>
        /// <param name="other"></param>
        public Angle(Angle other)
        {
            Radians = other.Radians;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Normalize this angle to between -PI and +PI
        /// </summary>
        /// <returns>A normalised copy of this angle</returns>
        public Angle Normalize()
        {
            double result = Radians % (2 * Math.PI);
            result = (result + 2 * Math.PI) % (2 * Math.PI); //Ensure is positive
            if (result > Math.PI) result -= 2 * Math.PI;
            return result;
        }

        /// <summary>
        /// Normalise this angle to between 0 and 2*PI
        /// </summary>
        /// <returns>A normalised copy of this angle</returns>
        public Angle NormalizeTo2PI()
        {
            double result = Radians % (2 * Math.PI);
            result = (result + 2 * Math.PI) % (2 * Math.PI); //Ensure is positive
            return result;
        }

        /// <summary>
        /// Get a unit vector describing the direction of this angle on the
        /// XY plane.
        /// </summary>
        /// <returns></returns>
        public Vector Direction()
        {
            return new Vector(this);
        }

        /// <summary>
        /// Get the Explement of this angle.  This is the angle which,
        /// when summed with this angle, adds up to a complete angle.
        /// This will be either positive or negative to match the sign
        /// of the angle.
        /// </summary>
        /// <returns></returns>
        public Angle Explement()
        {
            return new Angle(Sign() * 2 * Math.PI - Radians);
        }

        /// <summary>
        /// Get the sine of this angle
        /// </summary>
        /// <returns></returns>
        public double Sin()
        {
            return Math.Sin(Radians);
        }

        /// <summary>
        /// Get the cosine of this angle
        /// </summary>
        /// <returns></returns>
        public double Cos()
        {
            return Math.Cos(Radians);
        }

        /// <summary>
        /// Get the tangent of this angle
        /// </summary>
        /// <returns></returns>
        public double Tan()
        {
            return Math.Tan(Radians);
        }

        /// <summary>
        /// Gets the sign of the angle, expressed as +1 for positive numbers
        /// and -1 for negative ones.  Zero is treated as being positive in this
        /// instance.
        /// </summary>
        /// <returns></returns>
        public double Sign()
        {
            return Radians.Sign();
        }

        public override bool Equals(object obj)
        {
            return Radians.Equals((double)obj);
        }

        public override int GetHashCode()
        {
            return Radians.GetHashCode();
        }

        public int CompareTo(Angle other)
        {
            return Radians.CompareTo(other.Radians);
        }

        public override string ToString()
        {
            return (Radians / Math.PI).ToString() + "π";
        }

        #endregion

        #region Static Methods

        public static Angle FromDegrees(double degrees)
        {
            return new Angle(Math.PI * degrees / 180);
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implicit conversion to a double
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator double(Angle value) { return value.Radians; }

        /// <summary>
        /// Implicit conversion from a double
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Angle(double value) { return new Angle(value); }

        public static Angle operator + (Angle a1, Angle a2) => new Angle(a1.Radians + a2.Radians);

        public static Angle operator - (Angle a1, Angle a2) => new Angle(a1.Radians - a2.Radians);

        public static Angle operator * (Angle a, double d) => new Angle(a.Radians * d);

        public static Angle operator * (double d, Angle a) => new Angle(a.Radians * d);

        public static Angle operator / (Angle a, double d) => new Angle(a.Radians / d);

        #endregion
    }
}
