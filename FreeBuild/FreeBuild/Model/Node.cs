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

using FreeBuild.Base;
using FreeBuild.Geometry;
using FreeBuild.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeBuild.Model
{
    /// <summary>
    /// A singular point which represents a shared connection point
    /// between multiple vertices within different objects.
    /// </summary>
    [Serializable]
    public class Node : DataOwner<NodeDataStore, INodeDataComponent, Node>, IPosition
    {
        #region Properties

        /// <summary>
        /// Internal backing member for Position property
        /// </summary>
        private Vector _Position = Vector.Unset;

        /// <summary>
        /// The spatial position of this node
        /// </summary>
        [AutoUI(300)]
        public Vector Position
        {
            get { return _Position; }
            set { _Position = value; NotifyPropertyChanged("Position"); }
        }

        /// <summary>
        /// Private backing field for Vertices property.
        /// </summary>
        private VertexCollection _Vertices = null;

        /// <summary>
        /// The collection of vertices to which this node is connected
        /// </summary>
        public VertexCollection Vertices
        {
            get
            {
                if (_Vertices == null)
                {
                    _Vertices = new VertexCollection();
                }
                return _Vertices;
            }
        }

        ///// <summary>
        ///// Private backing field for Fixtity property
        ///// </summary>
        //private Bool6D _Fixity;

        ///// <summary>
        ///// The lateral and rotational directions in which this node is
        ///// fixed for the purpose of structural and physics-based analysis.
        ///// Represented by a set of six booleans, one each for the X, Y, Z, 
        ///// XX,YY and ZZ degrees of freedom.  If true, the node is fixed in
        ///// that direction, if false it is free to move.
        ///// </summary>
        //public Bool6D Fixity
        //{
        //    get { return _Fixity; }
        //    set { _Fixity = value;  NotifyPropertyChanged("Fixity"); }
        //}

        /// <summary>
        /// Get a description of this node.
        /// Will be the node's name if it has one or will return "Node {ID}"
        /// if not.
        /// </summary>
        public override string Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name) && NumericID > 0) return "Node " + NumericID;
                else return Name;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// For use in factory methods only.
        /// </summary>
        internal Node()
        {

        }

        /// <summary>
        /// Position constructor.
        /// Initialises a new node at the specified position.
        /// </summary>
        /// <param name="position"></param>
        public Node(Vector position)
        {
            _Position = position;
        }

        /// <summary>
        /// X, Y, Z position constructor.
        /// Initialises a new node at the specified position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Node(double x, double y, double z)
        {
            _Position = new Vector(x, y, z);
        }

        #endregion

        #region Methods

        protected override NodeDataStore NewDataStore()
        {
            return new NodeDataStore(this);
        }

        /// <summary>
        /// Get a collection of all elements connected to this node
        /// </summary>
        /// <param name="undeletedOnly">If true, only elements that are not marked
        /// as deleted will be returned</param>
        /// <returns></returns>
        public ElementCollection GetConnectedElements(bool undeletedOnly = true)
        {
            var result = new ElementCollection();

            foreach (Vertex v in Vertices)
            {
                if (v.Element != null &&
                    (!undeletedOnly || !v.Element.IsDeleted))
                    result.Add(v.Element);
            }

            return result;
        }

        /// <summary>
        /// Get the number of elements connected to this node
        /// </summary>
        /// <param name="undeletedOnly">If true, only elements not marked as deleted will
        /// be counted</param>
        /// <returns></returns>
        public int ConnectionCount(bool undeletedOnly = true)
        {
            int result = 0;

            foreach (Vertex v in Vertices)
            {
                if (v.Element != null && !v.Element.IsDeleted) result++;
            }

            return result;
        }

        /// <summary>
        /// Change the position of this node, optionally dragging any
        /// attached vertices through the same transformation.
        /// </summary>
        /// <param name="newPosition"></param>
        /// <param name="dragVertices"></param>
        public void MoveTo(Vector newPosition, bool dragVertices = true, ElementCollection excludeElements = null)
        {
            Vector move = newPosition - Position;
            if (dragVertices)
            {
                foreach (Vertex v in Vertices)
                {
                    if (excludeElements == null || v.Element == null || !excludeElements.Contains(v.Element))
                        v.Position += move;
                }
            }
            Position = newPosition;
        }

        /// <summary>
        /// Calculate the average of the vectors from this node position to the centroid of connected elements.
        /// </summary>
        /// <returns></returns>
        public Vector AverageConnectionDirection()
        {
            Vector result = Vector.Zero;
            int count = 0;
            foreach (Vertex v in Vertices)
            {
                if (v.Owner != null)
                {
                    VertexGeometry vG = v.Owner;
                    if (vG is Curve)
                    {
                        Curve crv = (Curve)vG;
                        Vector midPt = crv.PointAt(0.5);
                        if (midPt.IsValid())
                        {
                            result += (midPt - Position).Unitize();
                            count += 1;
                        }
                    }
                    //TODO: To surface centroid?
                }
            }
            if (count > 1) result /= count;
            return result;
        }

        /// <summary>
        /// Merge the properties of another node with this one.
        /// </summary>
        /// <param name="other">The node to merge into this one</param>
        /// <param name="averagePositions">If true, the node position will
        /// be set to the average of the original value and the position of
        /// the other.</param>
        public void Merge(Node other, bool averagePositions = false)
        {
            if (HasData() || other.HasData())
            {
                Data.Merge(other.Data);
            }
            if (other.Vertices != null)
            {
                // Replace vertex node references
                foreach (Vertex v in other.Vertices)
                {
                    if (v.Node == other) v.Node = this;
                }
            }
            if (averagePositions)
            {
                Position = (Position + other.Position) / 2;
            }
        }

        #endregion

    }

    /// <summary>
    /// Static extension methods for collections of nodes
    /// </summary>
    public static class NodeExtensions
    {
        /// <summary>
        /// Merge a collection of nodes into one node.
        /// The lowest-numbered node will be retained, the others
        /// will have their data merged into that one and be deleted.
        /// </summary>
        /// <param name="nodes">The collection of nodes to merge.  Note that any
        /// deleted nodes which you do not want to include in this merge should be
        /// removed prior to running this operation.</param>
        /// <param name="averagePositions">If true, the resultant node position will
        /// be set to the average of the node positions.  Otherwise, the position of the
        /// original node will be retained.</param>
        /// <returns></returns>
        public static Node Merge(this IList<Node> nodes, bool averagePositions = false)
        {
            int i = nodes.IndexOfLowestNumericID();
            if (i < 0) return null;
            Node result = nodes[i];
            Vector average = new Vector();
            for (int j = 0; j < nodes.Count; j++)
            {
                Node node = nodes[j];
                average += node.Position;
                if (j != i)
                {
                    result.Merge(node);
                    node.Delete();
                }
            }
            if (averagePositions)
            {
                average /= nodes.Count;
                result.Position = average;
            }
            return result;
        }
    }
}
