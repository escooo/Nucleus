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

using Nucleus.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.Geometry
{
    /// <summary>
    /// An angle expressed in radians.
    /// Represented by an immutable struct wrapping a double value.
    /// Implicit converters to and from the double type are provided,
    /// so this type can be used as though it were an ordinary double.
    /// </summary>
    [Serializable]
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
        /// A right angle, PI/2 (or 90 degrees)
        /// </summary>
        public static readonly Angle Right = new Angle(Math.PI/2);

        /// <summary>
        /// A straignt angle, PI (or 180 degrees)
        /// </summary>
        public static readonly Angle Straight = new Angle(Math.PI);

        /// <summary>
        /// A complete angle, 2*PI (or 360 degrees)
        /// </summary>
        public static readonly Angle Complete = new Angle(2 * Math.PI);

        /// <summary>
        /// An angle value representing multiple angles together
        /// </summary>
        public static readonly Angle Multi = new Angle(double.NegativeInfinity);

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

        /// <summary>
        /// Is this an acute angle - i.e. is it lower than PI/2 (90 degrees)?
        /// </summary>
        public bool IsAcute { get { return Radians.Abs() < Right; } }

        /// <summary>
        /// Is this an obtuse angle - i.e is it greater than PI/2 (90 degrees) but
        /// lower than PI (180 degrees)?
        /// </summary>
        public bool IsObtuse { get { return Radians.Abs().InRange(Right, Straight); } }

        /// <summary>
        /// Is this a reflex angle - i.e. is it greater than PI (180 degrees)?
        /// </summary>
        public bool IsReflex { get { return Radians.Abs() > Straight; } }

        /// <summary>
        /// Is this angle undefined (i.e. is it's Radians value NaN?)
        /// </summary>
        public bool IsUndefined { get { return double.IsNaN(Radians); } }

        /// <summary>
        /// Does this value represent multiple different angles?
        /// </summary>
        public bool IsMulti { get { return double.IsNegativeInfinity(Radians); } }

        /// <summary>
        /// Is this angle so small it can be safely treated as 0?
        /// </summary>
        public bool IsTiny { get { return Radians.IsTiny(); } }

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

        /// <summary>
        /// Initialise the angle from a text string.
        /// Valid input values are numeric strings which may optionally
        /// end with 'π' or '°' to denote values specified in multiples
        /// of PI or degrees respectively.
        /// </summary>
        /// <param name="description"></param>
        public Angle(string description)
        {
            description = description.Trim();
            if (description.EndsWith("π"))
            {
                Radians = double.Parse(description.TrimEnd('π')) * Math.PI;
            }
            else if (description.EndsWith("°"))
            {
                Radians = double.Parse(description.TrimEnd('°')) / 180 * Math.PI;
            }
            else
            {
                Radians = double.Parse(description);
            }
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
        /// Change the sign of this angle to be relative to the specified side.
        /// Left-side angles will be flipped.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        public Angle RelativeToSide(HandSide side)
        {
            if (side == HandSide.Left) return -this;
            else return this;
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
        /// Get the supplementary angle which together with this one will add up to a straight line
        /// </summary>
        /// <returns></returns>
        public Angle Supplement()
        {
            return Straight - this;
        }

        /// <summary>
        /// Returns the either this angle or its supplementary angle, whichever is the smallest
        /// </summary>
        /// <returns></returns>
        public Angle SmallestToStraight()
        {
            if (this < Right) return this;
            else return Supplement();
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
        /// Return the absolute (i.e. unsigned) value of this angle
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Angle Abs()
        {
            return new Angle(Math.Abs(this));
        }

        /// <summary>
        /// Gets the sign of the angle, expressed as +1 for positive numbers
        /// and -1 for negative ones.  Zero is treated as being positive in this
        /// instance.
        /// </summary>
        /// <returns></returns>
        public int Sign()
        {
            return Radians.Sign();
        }

        /// <summary>
        /// Get this angle mapped to the specified sign.  If the input sign matches
        /// the sign of this angle, the same angle will be returned.  If a mismatch,
        /// the negative explement will be returned instead.  Input numbers >= 0 are
        /// taken as positive, < 0 as negative.
        /// </summary>
        /// <param name="sign"></param>
        /// <returns></returns>
        public Angle ToSign(int sign)
        {
            if (sign * Sign() < 0) return -Explement();
            else return this;
        }

        public override bool Equals(object obj)
        {
            if (obj is Angle) return Radians.Equals(((Angle)obj).Radians);
            else if (obj is double) return Radians.Equals((double)obj);
            else return false;
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
            if (IsUndefined) return "";
            else if (IsMulti) return "Multi";
            return (Radians / Math.PI).ToString() + "π (" + Degrees + "°)";
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Create an angle from a value expressed in degrees
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Angle FromDegrees(double degrees)
        {
            return new Angle(Math.PI * degrees / 180);
        }

        /// <summary>
        /// Calculate the equivalent number of meters per degree of latitude at the
        /// specified reference latitude, taking account of the elliptical nature of
        /// the Earth.  Should be accurate to within 1m per degree anywhere on the Earth.  
        /// Based on this article: 
        /// https://knowledge.safe.com/articles/725/calculating-accurate-length-in-meters-for-lat-long.html
        /// </summary>
        /// <param name="rLatitude">The reference latitude at which the conversion is to take place</param>
        /// <returns></returns>
        public static double MetersPerDegreeLatitude(Angle rLatitude)
        {
            return 111132.92 - 559.82 * Math.Cos(2 * rLatitude) + 1.175 * Math.Cos(4 * rLatitude) - 0.0023*Math.Cos(6 * rLatitude);
        }

        /// <summary>
        /// Calculate the equivalent number of meters per degree of longitude at the
        /// specified reference latitude, taking account of the elliptical nature of
        /// the Earth.  Should be accurate to within 1m per degree anywhere on the Earth.  
        /// Based on this article: 
        /// https://knowledge.safe.com/articles/725/calculating-accurate-length-in-meters-for-lat-long.html
        /// </summary>
        /// <param name="rLatitude">The reference latitude at which the conversion is to take place</param>
        /// <returns></returns>
        public static double MetersPerDegreeLongitude(Angle rLatitude)
        {
            return 111412.84 * Math.Cos(rLatitude) - 93.5 * Math.Cos(3 * rLatitude) + 0.118 * Math.Cos(5 * rLatitude);
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
