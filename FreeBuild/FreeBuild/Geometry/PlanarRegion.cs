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
    /// A planar surface described by a perimeter boundary curve and (optionally)
    /// a set of 'cut out' perimeter voids.
    /// </summary>
    [Serializable]
    public class PlanarRegion : Surface
    {
        #region Properties

        /// <summary>
        /// Private backing field for Perimeter property
        /// </summary>
        private Curve _Perimeter;

        /// <summary>
        /// The outer perimeter boundary curve of this surface
        /// </summary>
        public Curve Perimeter
        {
            get { return _Perimeter; }
            set
            {
                _Perimeter = value;
                NotifyGeometryUpdated();
                NotifyPropertyChanged("Perimeter");
            }
        }

        /// <summary>
        /// Private backing field for Voids property
        /// </summary>
        private CurveCollection _Voids;

        /// <summary>
        /// The collection of curves that describe the boundaries of void regions
        /// 'cut out' of this surface
        /// </summary>
        public CurveCollection Voids
        {
            get
            {
                if (_Voids == null) _Voids = new CurveCollection();
                return _Voids;
            }
        }

        /// <summary>
        /// Does this surface have any voids defined?
        /// </summary>
        public bool HasVoids
        {
            get
            {
                return !(_Voids == null || _Voids.Count == 0);
            }
        }

        /// <summary>
        /// Private backing field for Plane property
        /// </summary>
        private Plane _Plane;

        /// <summary>
        /// The plane of this surface.
        /// This may be null if the definition is not valid.
        /// </summary>
        public Plane Plane
        {
            get
            {
                if (_Plane == null && Perimeter != null)
                {
                    _Plane = Perimeter.Plane();
                }
                return _Plane;
            }
        }

        public override bool IsValid
        {
            get
            {
                return (Perimeter != null && Perimeter.IsValid && Perimeter.Closed && Plane != null);
            }
        }

        /// <summary>
        /// The collection of vertices which are used to define the geometry of this shape.
        /// Different shapes will provide different means of editing this collection.
        /// DO NOT directly modify the collection returned from this property unless you are
        /// sure you know what you are doing.
        /// For PlanarSurfaces, this will generate a combined collection of all definition curve
        /// vertices.
        /// </summary>
        public override VertexCollection Vertices
        {
            get
            {
                VertexCollection combined = new VertexCollection();
                combined.AddRange(Perimeter.Vertices);
                foreach (Curve voidCrv in Voids)
                {
                    combined.AddRange(voidCrv.Vertices);
                }
                //TODO: Additional vertices?
                return combined;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.  Initialises a new PlanarSurface with no geometry.
        /// </summary>
        public PlanarRegion() { }

        /// <summary>
        /// Initialises a new PlanarSurface with the specified perimeter curve.
        /// </summary>
        /// <param name="perimeter"></param>
        public PlanarRegion(Curve perimeter, GeometryAttributes attributes = null)
        {
            _Perimeter = perimeter;
            Attributes = attributes;
        }

        /// <summary>
        /// Initialises a new PlanarSurface with the specified perimeter and void curves.
        /// </summary>
        /// <param name="perimeter"></param>
        /// <param name="voids"></param>
        public PlanarRegion(Curve perimeter, CurveCollection voids, GeometryAttributes attributes = null) : this(perimeter, attributes)
        {
            _Voids = voids;
        }

        #endregion

        #region Methods

        protected override void InvalidateCachedGeometry()
        {
            base.InvalidateCachedGeometry();
            _Plane = null;
        }

        /// <summary>
        /// Calculate the area of this region
        /// </summary>
        /// <param name="centroid"></param>
        /// <returns></returns>
        public override double CalculateArea(out Vector centroid)
        {
            if (_Perimeter != null)
            {
                Plane plane = Plane;
                if (plane != null)
                {
                    double area = _Perimeter.CalculateEnclosedArea(out centroid, _Voids, plane);
                    centroid = plane.LocalToGlobal(centroid);
                    return area;
                }
            }

            centroid = Vector.Unset;
            return 0;
        }

        #endregion
    }
}