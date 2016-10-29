﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Freebuild.WPF
{
    /// <summary>
    /// Base class for user controls with a label dependency property
    /// </summary>
    public abstract class LabelledControl : UserControl
    {
        /// <summary>
        /// Label dependency property
        /// </summary>
        public static readonly DependencyProperty LabelProperty
            = DependencyProperty.Register("Label", typeof(string), typeof(LabelledControl), new PropertyMetadata(null));

        /// <summary>
        /// The label text of the field
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Extra content dependency property
        /// </summary>
        public static readonly DependencyProperty ExtraContentProperty
            = DependencyProperty.Register("ExtraContent", typeof(object), typeof(LabelledControl));

        /// <summary>
        /// Extra content hosted as part of the field
        /// </summary>
        public object ExtraContent
        {
            get { return GetValue(ExtraContentProperty); }
            set { SetValue(ExtraContentProperty, value); }
        }

        /// <summary>
        /// Units dependency property
        /// </summary>
        public static readonly DependencyProperty UnitsProperty
            = DependencyProperty.Register("Units", typeof(string), typeof(LabelledControl));

        /// <summary>
        /// The units the displayed quantity is in
        /// </summary>
        public string Units
        {
            get { return (string)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

    }
}
