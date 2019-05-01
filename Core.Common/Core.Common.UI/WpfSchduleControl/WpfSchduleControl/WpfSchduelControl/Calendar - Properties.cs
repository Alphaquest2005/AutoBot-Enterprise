using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSchduleControl
{

    public partial class Calendar
    {
        private CalendarTextBox _textBox;
        private bool _allowNew;
        private bool _allowItemEdit;
        private bool _allowItemResize;
        private bool _creatingItem;
        private CalendarDay[] _days;
        private CalendarDaysMode _daysMode;
        private CalendarItem _editModeItem;
        private bool _finalizingEdition;
        private DayOfWeek _firstDayOfWeek;
        private CalendarHighlightRange[] _highlightRanges;
        private CalendarItemCollection _items;
        private string _itemsDateFormat;
        private string _itemsTimeFormat;
        private int _maximumFullDays;
        private int _maximumViewDays;
        private CalendarRenderer _renderer;
        private DateTime _selEnd;
        private DateTime _selStart;
        private CalendarState _state;
        private CalendarTimeScale _timeScale;
        private int _timeUnitsOffset;
        private DateTime _viewEnd;
        private DateTime _viewStart;
        private CalendarWeek[] _weeks;
        private List<CalendarSelectableElement> _selectedElements;
        private ICalendarSelectableElement _selectedElementEnd;
        private ICalendarSelectableElement _selectedElementStart;
        private Rect _selectedElementSquare;
        private CalendarItem itemOnState;
        private bool itemOnStateChanged;

        /// <summary>
        /// Gets or sets a value indicating if the control let's the user create new items.
        /// </summary>
        [DefaultValue(true)]
        [Description("Allows the user to create new items on the view")]
        public bool AllowNew
        {
            get { return _allowNew; }
            set { _allowNew = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if the user can edit the item using the mouse or keyboard
        /// </summary>
        [DefaultValue(true)]
        [Description("Allows or denies the user the edition of items text or date ranges.")]
        public bool AllowItemEdit
        {
            get { return _allowItemEdit; }
            set { _allowItemEdit = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating if calendar allows user to resize the calendar.
        /// </summary>
        [DefaultValue(true)]
        [Description("Allows or denies the user to resize items on the calendar")]
        public bool AllowItemResize
        {
            get { return _allowItemResize; }
            set { _allowItemResize = value; }
        }

        /// <summary>
        /// Gets the days visible on the ccurrent view
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarDay[] Days
        {
            get { return _days; }
        }

        /// <summary>
        /// Gets the mode in which days are drawn.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarDaysMode DaysMode
        {
            get { return _daysMode; }
        }

        /// <summary>
        /// Gets the union of day body Rects
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rect DaysBodyRect
        {
            get
            {
                Rect first = Days[0].BodyBounds;
                Rect last = Days[Days.Length - 1].BodyBounds;

                return Rect.Union(first, last);
            }
        }

        /// <summary>
        /// Gets if the calendar is currently in edit mode of some item
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EditMode
        {
            get { return TextBox != null; }
        }

        /// <summary>
        /// Gets the item being edited (if any)
        /// </summary>
        public CalendarItem EditModeItem
        {
            get
            {
                return _editModeItem;
            }
        }

        /// <summary>
        /// Gets or sets the first day of weeks
        /// </summary>
        [Description("Starting day of weeks")]
        [DefaultValue(DayOfWeek.Sunday)]
        public DayOfWeek FirstDayOfWeek
        {
            set { _firstDayOfWeek = value; }
            get { return _firstDayOfWeek; }
        }


        /// <summary>
        /// Gets or sets the time ranges that should be highlighted as work-time.
        /// This ranges are week based.
        /// </summary>
        public CalendarHighlightRange[] HighlightRanges
        {
            get { return _highlightRanges; }
            set { _highlightRanges = value; UpdateHighlights(); }
        }

        /// <summary>
        /// Gets the collection of items currently on the view.
        /// </summary>
        /// <remarks>
        /// This collection changes every time the view is changed
        /// </remarks>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarItemCollection Items
        {
            get { return _items; }
        }

        /// <summary>
        /// Gets or sets the format in which time is shown in the items, when applicable
        /// </summary>
        [DefaultValue("dd/MMM")]
        public string ItemsDateFormat
        {
            get { return _itemsDateFormat; }
            set { _itemsDateFormat = value; }
        }

        /// <summary>
        /// Gets or sets the format in which time is shown in the items, when applicable
        /// </summary>
        [DefaultValue("hh:mm tt")]
        public string ItemsTimeFormat
        {
            get { return _itemsTimeFormat; }
            set { _itemsTimeFormat = value; }
        }

        /// <summary>
        /// Gets or sets the maximum full days shown on the view. 
        /// After this amount of days, they will be shown as short days.
        /// </summary>
        [DefaultValue(8)]
        public int MaximumFullDays
        {
            get { return _maximumFullDays; }
            set { _maximumFullDays = value; }
        }

        /// <summary>
        /// Gets or sets the maximum amount of days supported by the view.
        /// Value must be multiple of 7
        /// </summary>
        [DefaultValue(35)]
        public int MaximumViewDays
        {
            get { return _maximumViewDays; }
            set
            {
                if (value % 7 != 0)
                {
                    throw new Exception("MaximumViewDays must be multiple of 7");
                }
                _maximumViewDays = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarRenderer"/> of the <see cref="Calendar"/>
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarRenderer Renderer
        {
            get { return _renderer; }
            set
            {
                _renderer = value;

                if (value != null && Created)
                {
                    value.OnInitialize(new CalendarRendererEventArgs(null, null, Rect.Empty));
                }
            }
        }

        /// <summary>
        /// Gets the last selected element
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICalendarSelectableElement SelectedElementEnd
        {
            get { return _selectedElementEnd; }
            set
            {
                _selectedElementEnd = value;

                UpdateSelectionElements();
            }
        }

        /// <summary>
        /// Gets the first selected element
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICalendarSelectableElement SelectedElementStart
        {
            get { return _selectedElementStart; }
            set
            {
                _selectedElementStart = value;

                UpdateSelectionElements();
            }
        }

        /// <summary>
        /// Gets or sets the end date-time of the view's selection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectionEnd
        {
            get { return _selEnd; }
            set { _selEnd = value; }
        }

        /// <summary>
        /// Gets or sets the start date-time of the view's selection.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime SelectionStart
        {
            get { return _selStart; }
            set { _selStart = value; }
        }

        /// <summary>
        /// Gets or sets the state of the calendar
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public CalendarState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Gets the TextBox of the edit mode
        /// </summary>
        internal CalendarTextBox TextBox
        {
            get { return _textBox; }
            set { _textBox = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="CalendarTimeScale"/> for visualization.
        /// </summary>
        [DefaultValue(CalendarTimeScale.ThirtyMinutes)]
        public CalendarTimeScale TimeScale
        {
            get { return _timeScale; }
            set
            {
                _timeScale = value;

                if (Days != null)
                {
                    for (int i = 0; i < Days.Length; i++)
                    {
                        Days[i].UpdateUnits();
                    }
                }

                Renderer.PerformLayout();
                Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the offset of scrolled units
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int TimeUnitsOffset
        {
            get { return _timeUnitsOffset; }
            set
            {
                _timeUnitsOffset = value;
                Renderer.PerformLayout();
                Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the end date-time of the current view.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime ViewEnd
        {
            get { return _viewEnd; }
            set
            {
                _viewEnd = value.Date.Add(new TimeSpan(23, 59, 59));
                ClearItems();
                UpdateDaysAndWeeks();
                Renderer.PerformLayout();
                Invalidate();
                ReloadItems();
            }
        }

        /// <summary>
        /// Gets or sets the start date-time of the current view.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime ViewStart
        {
            get { return _viewStart; }
            set
            {
                _viewStart = value.Date;
                ClearItems();
                UpdateDaysAndWeeks();
                Renderer.PerformLayout();
                Invalidate();
                ReloadItems();
            }
        }

        /// <summary>
        /// Gets the weeks currently visible on the calendar, if <see cref="DaysMode"/> is <see cref="CalendarDaysMode.Short"/>
        /// </summary>
        public CalendarWeek[] Weeks
        {
            get { return _weeks; }
        }


    }
}
