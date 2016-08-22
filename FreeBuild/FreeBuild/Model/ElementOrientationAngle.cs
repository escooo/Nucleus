﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBuild.Model
{
    public class ElementOrientationAngle : ElementOrientation
    {
        #region Properties

        /// <summary>
        /// The angle value, in radians
        /// </summary>
        public double Value { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialisa an ElementOrientationAngle with the specified angle value
        /// </summary>
        /// <param name="value"></param>
        public ElementOrientationAngle(double value)
        {
            Value = value;
        }

        #endregion

        #region Operators

        /// <summary>
        /// Implicit conversion from an ElementOrientationAngle to a double
        /// </summary>
        /// <param name="angle"></param>
        public static implicit operator double(ElementOrientationAngle angle)
        {
            return angle.Value;
        }

        /// <summary>
        /// Implicit conversion from a double to an ElementOrientationAngle
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator ElementOrientationAngle(double value)
        {
            return new ElementOrientationAngle(value);
        }

        #endregion
    }
}