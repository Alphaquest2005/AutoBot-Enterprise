using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows;

namespace WpfSchduleControl
{
    /// <summary>
    /// Represents a clickable element of <see cref="MonthView"/> control
    /// </summary>
    public interface ISelectableElement
    {

        /// <summary>
        /// Gets the bounds of the element
        /// </summary>
        Rect Bounds { get; }

        /// <summary>
        /// Gets if the element is currently selected
        /// </summary>
        bool Selected { get; }
    }
}
