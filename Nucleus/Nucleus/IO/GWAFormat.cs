﻿using Nucleus.Model;
using Nucleus.Model.Loading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nucleus.IO
{
    /// <summary>
    /// A class which represents the textual format of Oasys GSA GWA files
    /// </summary>
    [Serializable]
    public class GWAFormat : TextFormat
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public GWAFormat() : base()
        {
            Add(typeof(Node),
                "NODE.1\t{" + CONTEXT + ".GetID()}\t{Name}\tNO_RGB\t{Position.X}\t{Position.Y}\t{Position.Z}\tNO_GRID\tGLOBAL\tREST\t{Data.Item[NodeSupport].Fixity.X."
                + CONTEXT + ".ToInt()}\t{Data.Item[NodeSupport].Fixity.Y." + CONTEXT + ".ToInt()}\t{Data.Item[NodeSupport].Fixity.Z." + CONTEXT + 
                ".ToInt()}\t{Data.Item[NodeSupport].Fixity.XX." + CONTEXT + ".ToInt()}\t{Data.Item[NodeSupport].Fixity.YY." + CONTEXT + 
                ".ToInt()}\t{Data.Item[NodeSupport].Fixity.ZZ." + CONTEXT + ".ToInt()}");

            Add(typeof(LinearElement), 
                "EL.1\t{" + CONTEXT + ".GetID()}\t{Name}\tNO_RGB\t{" + CONTEXT + ".ElementType()}\t{Family." + CONTEXT + ".GetID()}\t{" + 
                CONTEXT + ".ElementGroup()}\t{StartNode." + CONTEXT + ".GetID()}\t{EndNode." + CONTEXT + ".GetID()}\t\t{Orientation.Degrees}\tRLS\t{Start.Releases." +
                CONTEXT + ".ToReleaseString()}\t{End.Releases." + CONTEXT + 
                ".ToReleaseString()}\tOFFSET\t{Start.CombinedOffset.X}\t{Start.CombinedOffset.Y}\t{Start.CombinedOffset.Z}\t{End.CombinedOffset.X}\t{End.CombinedOffset.Y}\t{End.CombinedOffset.Z}");

            Add(typeof(PanelElement),
                IF + " *.HasMeshRepresentation()" + THEN + 
                "EL.1\t{" + CONTEXT + ".GetID()}\t{Name}\tNO_RGB\t{" + CONTEXT + ".ElementType()}\t{Family." + CONTEXT + ".GetID()}\t{" + 
                CONTEXT + ".ElementGroup()}\t{" + CONTEXT + ".ElementTopo()}"); //TODO

            Add(typeof(SectionFamily), 
                "PROP_SEC.1\t{" + CONTEXT + ".GetID()}\t{Name}\tNO_RGB\t{" + CONTEXT + ".FamilyMaterial()}\t{" + CONTEXT + ".SectionDescription()}");

            Add(typeof(BuildUpFamily),
                "PROP_2D.1\t{" + CONTEXT + ".GetID()}\t{Name}\tNO_RGB\tGLOBAL\t{" + CONTEXT + ".FamilyMaterial()}\tPLATE\t{Layers.TotalThickness}"); //TODO

            Add(typeof(ElementSet),
                "LIST.1\t{" + CONTEXT + ".GetID()}\t{Name}\tELEMENT\t{" + CONTEXT + ".ListDefinition()}");

            Add(typeof(NodeSet),
                "LIST.1\t{" + CONTEXT + ".GetID()}\t{Name}\tNODE\t{" + CONTEXT + ".ListDefinition()}");

            Add(typeof(LoadCase),
                "LOAD_TITLE.1\t{*.GetID()}\t{Name}");

            Add(typeof(NodeLoad),
                "NODE_LOAD.2\t{Name}\t{AppliedTo.*.ListDefinition()}\t{Case.*.GetID()}\t{Axes.*.GetID()}\t{Direction}\t{Value.Evaluate()}");

            Add(typeof(LinearElementLoad),
                "LOAD_BEAM_UDL.2\t{Name}\t{AppliedTo.*.ListDefinition()}\t{Case.*.GetID()}\t{Axes.*.GetID()}\tNO\t{Direction}\t{Value.Evaluate()}");

            //LOAD_BEAM_POINT.2 | name | list | case | axis | proj | dir | pos | value
            Add(typeof(LinearElementPointLoad),
                "LOAD_BEAM_POINT.2\t{Name}\t{AppliedTo.*.ListDefinition()}\t{Case.*.GetID()}\t{Axes.*.GetID()}\tNO\t{*.PointLoadPosition()}\t{Value.Evaluate()}");

            Add(typeof(PanelLoad),
                "LOAD_2D_FACE.2\t{Name}\t{AppliedTo.*.ListDefinition()}\t{Case.*.GetID()}\t{Axes.*.GetID()}\tCONS\tNO\t{Direction}\t{Value.Evaluate()}");

            Add(typeof(GravityLoad),
                "LOAD_GRAVITY.2\t{Name}\t{AppliedTo.*.ListDefinition()}\t{Case.*.GetID()}\t{GravityVector().X}\t{GravityVector().Y}\t{GravityVector().Z}");

            Add(typeof(ThermalLoad),
                "TEMP_BEAM.2\t{Name}\t{AppliedTo.*.ListDefinition()}\t{Case.*.GetID()}\tCONS\t{Value.Evaluate()}"); //TODO: TEMP_2D

            Add(typeof(Level),
                "GRID_PLANE.1\t{*.GetID()}\t{Name}\tZ\tELEV\t{Z}");
        }

#endregion
    }
}
