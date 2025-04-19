using System;
using System.Collections.Generic;
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
        public static bool DateIntersects(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
        {
            return startB < endA && startA < endB;
        }

        /// <summary>
        /// Activates the edit mode on the first selected item
        /// </summary>
        /// <param name="item"></param>
        public void ActivateEditMode()
        {
            foreach (CalendarItem item in Items)
            {
                if (item.Selected)
                {
                    ActivateEditMode(item);
                    return;
                }
            }
        }

        /// <summary>
        /// Activates the edit mode on the specified item
        /// </summary>
        /// <param name="item"></param>
        public void ActivateEditMode(CalendarItem item)
        {
            CalendarItemCancelEventArgs evt = new CalendarItemCancelEventArgs(item);

            if (!_creatingItem)
            {
                OnItemEditing(evt);
            }

            if (evt.Cancel)
            {
                return;
            }

            _editModeItem = item;
            TextBox = new CalendarTextBox(this);
            TextBox.KeyDown += new KeyEventHandler(TextBox_KeyDown);
            TextBox.LostFocus += new RoutedEventHandler(TextBox_LostFocus);
            Rect r = item.Bounds;
            r.Inflate(-2, -2);
            TextBox.Margin = new Thickness(r.Left,r.Top,r.Right, r.Bottom);
           // TextBox.BorderBrush = new Brush();
            TextBox.Text = item.Text;
            TextBox.TextWrapping = TextWrapping.Wrap;

            AddLogicalChild(TextBox);
            TextBox.Visibility = Visibility.Visible;
            TextBox.Focus();
            TextBox.SelectionStart = TextBox.Text.Length;

            SetState(CalendarState.EditingItemText);
        }

        /// <summary>
        /// Creates a new item on the current selection. 
        /// If there's no selection, this will be ignored.
        /// </summary>
        /// <param name="itemText">Text of the item</param>
        /// <param name="editMode">If <c>true</c> activates the edit mode so user can edit the text of the item.</param>
        public void CreateItemOnSelection(string itemText, bool editMode)
        {
            if (SelectedElementEnd == null || SelectedElementStart == null) return;

            CalendarTimeScaleUnit unitEnd = SelectedElementEnd as CalendarTimeScaleUnit;
            CalendarDayTop dayTop = SelectedElementEnd as CalendarDayTop;
            CalendarDay day = SelectedElementEnd as CalendarDay;
            TimeSpan duration = unitEnd != null ? unitEnd.Duration : new TimeSpan(23, 59, 59);
            CalendarItem item = new CalendarItem(this);

            DateTime dstart = SelectedElementStart.Date;
            DateTime dend = SelectedElementEnd.Date;

            if (dend.CompareTo(dstart) < 0)
            {
                DateTime dtmp = dend;
                dend = dstart;
                dstart = dtmp;
            }

            item.StartDate = dstart;
            item.EndDate = dend.Add(duration);
            item.Text = itemText;

            CalendarItemCancelEventArgs evtA = new CalendarItemCancelEventArgs(item);

            OnItemCreating(evtA);

            if (!evtA.Cancel)
            {
                Items.Add(item);

                if (editMode)
                {
                    _creatingItem = true;
                    ActivateEditMode(item);
                }
            }


        }

        /// <summary>
        /// Ensures the scrolling shows the specified time unit. It doesn't affect View date ranges.
        /// </summary>
        /// <param name="unit">Unit to ensure visibility</param>
        public void EnsureVisible(CalendarTimeScaleUnit unit)
        {
            if (Days == null || Days.Length == 0) return;

            Rect view = Days[0].BodyBounds;

            if (unit.Bounds.Bottom > view.Bottom)
            {
                TimeUnitsOffset = -Convert.ToInt32(Math.Ceiling(unit.Date.TimeOfDay.TotalMinutes / (double)TimeScale))
                     + Renderer.GetVisibleTimeUnits();
            }
            else if (unit.Bounds.Top < view.Top)
            {
                TimeUnitsOffset = -Convert.ToInt32(Math.Ceiling(unit.Date.TimeOfDay.TotalMinutes / (double)TimeScale));
            }
        }

        /// <summary>
        /// Finalizes editing the <see cref="EditModeItem"/>.
        /// </summary>
        /// <param name="cancel">Value indicating if edition of item should be canceled.</param>
        public void FinalizeEditMode(bool cancel)
        {
            if (!EditMode || EditModeItem == null || _finalizingEdition) return;

            _finalizingEdition = true;

            string cancelText = _editModeItem.Text;
            CalendarItem itemBuffer = _editModeItem;
            _editModeItem = null;
            CalendarItemCancelEventArgs evt = new CalendarItemCancelEventArgs(itemBuffer);

            if (!cancel)
                itemBuffer.Text = TextBox.Text.Trim();

            if (TextBox != null)
            {
                TextBox.Visibility = Visibility.Collapsed;
                RemoveLogicalChild(TextBox);
                TextBox.Dispose();
            }

            if (_editModeItem != null)
                Invalidate(itemBuffer);

            _textBox = null;

            if (_creatingItem)
            {
                OnItemCreated(evt);
            }
            else
            {
                OnItemEdited(evt);
            }

            if (evt.Cancel)
            {
                itemBuffer.Text = cancelText;
            }


            _creatingItem = false;
            _finalizingEdition = false;

            if (State == CalendarState.EditingItemText)
            {
                SetState(CalendarState.Idle);
            }
        }

        /// <summary>
        /// Finds the <see cref="CalendarDay"/> for the specified date, if in the view.
        /// </summary>
        /// <param name="d">Date to find day</param>
        /// <returns><see cref="CalendarDay"/> object that matches the date, <c>null</c> if day was not found.</returns>
        public CalendarDay FindDay(DateTime d)
        {
            if (Days == null) return null;

            for (int i = 0; i < Days.Length; i++)
            {
                if (Days[i].Date.Date.Equals(d.Date.Date))
                {
                    return Days[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the items that are currently selected
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CalendarItem> GetSelectedItems()
        {
            List<CalendarItem> items = new List<CalendarItem>();

            foreach (CalendarItem item in Items)
            {
                if (item.Selected)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the time unit that starts with the specified date
        /// </summary>
        /// <param name="dateTime">Date and time of the unit you want to extract</param>
        /// <returns>Matching time unit. <c>null</c> If out of range.</returns>
        public CalendarTimeScaleUnit GetTimeUnit(DateTime d)
        {
            if (Days != null)
            {
                foreach (CalendarDay day in Days)
                {
                    if (day.Date.Equals(d.Date))
                    {
                        double duration = Convert.ToDouble((int)TimeScale);
                        int index =
                            Convert.ToInt32(
                                Math.Floor(
                                    d.TimeOfDay.TotalMinutes / duration
                                )
                            );

                        return day.TimeUnits[index];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Searches for the first hitted <see cref="ICalendarSelectableElement"/>
        /// </summary>
        /// <param name="p">Point to check for hit test</param>
        /// <returns></returns>
        public ICalendarSelectableElement HitTest(Point p)
        {
            return HitTest(p, false);
        }

        /// <summary>
        /// Searches for the first hitted <see cref="ICalendarSelectableElement"/>
        /// </summary>
        /// <param name="p">Point to check for hit test</param>
        /// <returns></returns>
        public ICalendarSelectableElement HitTest(Point p, bool ignoreItems)
        {
            if (!ignoreItems)
                foreach (CalendarItem item in Items)
                {
                    foreach (Rect r in item.GetAllBounds())
                    {
                        if (r.Contains(p))
                        {
                            return item;
                        }
                    }
                }

            for (int i = 0; i < Days.Length; i++)
            {
                if (Days[i].Bounds.Contains(p))
                {
                    if (DaysMode == CalendarDaysMode.Expanded)
                    {
                        if (Days[i].DayTop.Bounds.Contains(p))
                        {
                            return Days[i].DayTop;
                        }
                        else
                        {
                            for (int j = 0; j < Days[i].TimeUnits.Length; j++)
                            {
                                if (Days[i].TimeUnits[j].Visible &&
                                    Days[i].TimeUnits[j].Bounds.Contains(p))
                                {
                                    return Days[i].TimeUnits[j];
                                }
                            }
                        }

                        return Days[i];
                    }
                    else if (DaysMode == CalendarDaysMode.Short)
                    {
                        return Days[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the item hitted at the specified location. Null if no item hitted.
        /// </summary>
        /// <param name="p">Location to serach for items</param>
        /// <returns>Hitted item at the location. Null if no item hitted.</returns>
        public CalendarItem ItemAt(Point p)
        {
            return HitTest(p) as CalendarItem;
        }

        /// <summary>
        /// Invalidates the bounds of the specified day
        /// </summary>
        /// <param name="day"></param>
        public void Invalidate(CalendarDay day)
        {
            InvalidateVisual(); // Invalidate(day.Bounds);
        }

        /// <summary>
        /// Ivalidates the bounds of the specified unit
        /// </summary>
        /// <param name="unit"></param>
        public void Invalidate(CalendarTimeScaleUnit unit)
        {
            InvalidateVisual(); //Invalidate(unit.Bounds);
        }

        /// <summary>
        /// Invalidates the area of the specified item
        /// </summary>
        /// <param name="item"></param>
        public void Invalidate(CalendarItem item)
        {
            Rect r = item.Bounds;

            foreach (Rect bounds in item.GetAllBounds())
            {
                r = Rect.Union(r, bounds);
            }

            r.Inflate(Renderer.ItemShadowPadding + Renderer.ItemInvalidateMargin, Renderer.ItemShadowPadding + Renderer.ItemInvalidateMargin);
            InvalidateVisual(); //Invalidate(r);
        }

        /// <summary>
        /// Establishes the selection range with only one graphical update.
        /// </summary>
        /// <param name="selectionStart">Fisrt selected element</param>
        /// <param name="selectionEnd">Last selection element</param>
        public void SetSelectionRange(ICalendarSelectableElement selectionStart, ICalendarSelectableElement selectionEnd)
        {
            _selectedElementStart = selectionStart;
            SelectedElementEnd = selectionEnd;
        }

        /// <summary>
        /// Sets the value of <see cref="ViewStart"/> and <see cref="ViewEnd"/> properties
        /// triggering only one repaint process
        /// </summary>
        /// <param name="dateStart">Start date of view</param>
        /// <param name="dateEnd">End date of view</param>
        public void SetViewRange(DateTime dateStart, DateTime dateEnd)
        {
            _viewStart = dateStart.Date;
            ViewEnd = dateEnd;
        }

        /// <summary>
        /// Returns a value indicating if the view range intersects the specified date range.
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        public bool ViewIntersects(DateTime dateStart, DateTime dateEnd)
        {
            return DateIntersects(ViewStart, ViewEnd, dateStart, dateEnd);
        }

        /// <summary>
        /// Returns a value indicating if the view range intersect the date range of the specified item
        /// </summary>
        /// <param name="item"></param>
        public bool ViewIntersects(CalendarItem item)
        {
            return ViewIntersects(item.StartDate, item.EndDate);
        }


        #region Private Methods

        protected bool IsInputKey(KeyEventArgs keyData)
        {
            if (
                keyData.Key == Key.Down ||
                keyData.Key == Key.Up ||
                keyData.Key == Key.Right ||
                keyData.Key == Key.Left)
            {
                return true;
            }
            else
            {

                return Keyboard.IsKeyDown(keyData.Key); //base.OnPreviewKeyDown(keyData);
            }
        }

        /// <summary>
        /// Removes all the items currently on the calendar
        /// </summary>
        private void ClearItems()
        {
            Items.Clear();
            Renderer.DayTopHeight = Renderer.DayTopMinHeight;
        }

        /// <summary>
        /// Unselects the selected items
        /// </summary>
        private void ClearSelectedItems()
        {
            Rect r = Rect.Empty;

            foreach (CalendarItem item in Items)
            {
                if (item.Selected)
                {
                    if (r.IsEmpty)
                    {
                        r = item.Bounds;
                    }
                    else
                    {
                        r = Rect.Union(r, item.Bounds);
                    }
                }

                item.SetSelected(false);
            }

            InvalidateVisual();
        }

        /// <summary>
        /// Deletes the currently selected item
        /// </summary>
        private void DeleteSelectedItems()
        {
            Stack<CalendarItem> toDelete = new Stack<CalendarItem>();

            foreach (CalendarItem item in Items)
            {
                if (item.Selected)
                {
                    CalendarItemCancelEventArgs evt = new CalendarItemCancelEventArgs(item);

                    OnItemDeleting(evt);

                    if (!evt.Cancel)
                    {
                        toDelete.Push(item);
                    }
                }
            }

            if (toDelete.Count > 0)
            {
                while (toDelete.Count > 0)
                {
                    CalendarItem item = toDelete.Pop();

                    Items.Remove(item);

                    OnItemDeleted(new CalendarItemEventArgs(item));
                }

                Renderer.PerformItemsLayout();
            }
        }

        /// <summary>
        /// Clears current items and reloads for specified view
        /// </summary>
        private void ReloadItems()
        {
            OnLoadItems(new CalendarLoadEventArgs(this, ViewStart, ViewEnd));
        }

        /// <summary>
        /// Grows the Rect to repaint currently selected elements
        /// </summary>
        /// <param name="rect"></param>
        private void GrowSquare(Rect rect)
        {
            if (_selectedElementSquare.IsEmpty)
            {
                _selectedElementSquare = rect;
            }
            else
            {
                _selectedElementSquare = Rect.Union(_selectedElementSquare, rect);
            }
        }

        /// <summary>
        /// Clears selection of currently selected components (As quick as possible)
        /// </summary>
        private void ClearSelectedComponents()
        {
            foreach (CalendarSelectableElement element in _selectedElements)
            {
                element.SetSelected(false);
            }

            _selectedElements.Clear();

            InvalidateVisual();//Invalidate(_selectedElementSquare);
            _selectedElementSquare = Rect.Empty;

        }

        /// <summary>
        /// Scrolls the calendar using the specified delta
        /// </summary>
        /// <param name="delta"></param>
        private void ScrollCalendar(int delta)
        {
            if (delta < 0)
            {
                SetViewRange(ViewStart.AddDays(7), ViewEnd.AddDays(7));
            }
            else
            {
                SetViewRange(ViewStart.AddDays(-7), ViewEnd.AddDays(-7));
            }
        }

        /// <summary>
        /// Raises the <see cref="ItemsPositioned"/> event
        /// </summary>
        internal void RaiseItemsPositioned()
        {
            OnItemsPositioned(EventArgs.Empty);
        }

        /// <summary>
        /// Scrolls the time units using the specified delta
        /// </summary>
        /// <param name="delta"></param>
        private void ScrollTimeUnits(int delta)
        {
            int possible = TimeUnitsOffset;
            int visible = Renderer.GetVisibleTimeUnits();

            if (delta < 0)
            {
                possible--;
            }
            else
            {
                possible++;
            }

            if (possible > 0)
            {
                possible = 0;
            }
            else if (
                Days != null
                && Days.Length > 0
                && Days[0].TimeUnits != null
                && possible * -1 >= Days[0].TimeUnits.Length)
            {
                possible = Days[0].TimeUnits.Length - 1;
                possible *= -1;
            }
            else if (Days != null
               && Days.Length > 0
               && Days[0].TimeUnits != null)
            {
                int max = Days[0].TimeUnits.Length - visible;
                max *= -1;
                if (possible < max) possible = max;
            }

            if (possible != TimeUnitsOffset)
            {
                TimeUnitsOffset = possible;
            }
        }

        /// <summary>
        /// Sets the value of the <see cref="DaysMode"/> property.
        /// </summary>
        /// <param name="mode">Mode in which days will be rendered</param>
        private void SetDaysMode(CalendarDaysMode mode)
        {
            _daysMode = mode;
        }

        /// <summary>
        /// Sets the value of the <see cref="State"/> property
        /// </summary>
        /// <param name="state">Current state of the calendar</param>
        private void SetState(CalendarState state)
        {
            _state = state;
        }

        /// <summary>
        /// Handles the LostFocus event of the TextBox that edit items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            FinalizeEditMode(false);
        }

        /// <summary>
        /// Handles the Keydown event of the TextBox that edit items
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                FinalizeEditMode(true);
            }
            else if (e.Key == Key.Enter)
            {
                FinalizeEditMode(false);
            }
        }

        /// <summary>
        /// Updates the 
        /// </summary>
        private void UpdateDaysAndWeeks()
        {
            TimeSpan span = (new DateTime(ViewEnd.Year, ViewEnd.Month, ViewEnd.Day, 23, 59, 59)).Subtract(ViewStart.Date);
            int preDays = 0;
            span = span.Add(new TimeSpan(0, 0, 0, 1, 0));

            if (span.Days < 1 || span.Days > MaximumViewDays)
            {
                throw new Exception("Days between ViewStart and ViewEnd should be between 1 and MaximumViewDays");
            }

            if (span.Days > MaximumFullDays)
            {
                SetDaysMode(CalendarDaysMode.Short);
                preDays = (new int[] { 0, 1, 2, 3, 4, 5, 6 })[(int)ViewStart.DayOfWeek] - (int)FirstDayOfWeek;
                span = span.Add(new TimeSpan(preDays, 0, 0, 0));

                while (span.Days % 7 != 0)
                    span = span.Add(new TimeSpan(1, 0, 0, 0));
            }
            else
            {
                SetDaysMode(CalendarDaysMode.Expanded);
            }

            _days = new CalendarDay[span.Days];

            for (int i = 0; i < Days.Length; i++)
                Days[i] = new CalendarDay(this, ViewStart.AddDays(-preDays + i), i);


            //Weeks
            if (DaysMode == CalendarDaysMode.Short)
            {
                List<CalendarWeek> weeks = new List<CalendarWeek>();

                for (int i = 0; i < Days.Length; i++)
                {
                    if (Days[i].Date.DayOfWeek == FirstDayOfWeek)
                    {
                        weeks.Add(new CalendarWeek(this, Days[i].Date));
                    }
                }

                _weeks = weeks.ToArray();
            }
            else
            {
                _weeks = new CalendarWeek[] { };
            }

            UpdateHighlights();

        }

        /// <summary>
        /// Updates the value of the <see cref="CalendarTimeScaleUnit.Highlighted"/> property on the time units of days.
        /// </summary>
        internal void UpdateHighlights()
        {
            if (Days == null) return;

            for (int i = 0; i < Days.Length; i++)
            {
                Days[i].UpdateHighlights();
            }
        }

        /// <summary>
        /// Informs elements who's selected and who's not, and repaints <see cref="_selectedElementSquare"/>
        /// </summary>
        private void UpdateSelectionElements()
        {
            CalendarTimeScaleUnit unitStart = _selectedElementStart as CalendarTimeScaleUnit;
            CalendarDayTop topStart = _selectedElementStart as CalendarDayTop;
            CalendarDay dayStart = _selectedElementStart as CalendarDay;
            CalendarTimeScaleUnit unitEnd = _selectedElementEnd as CalendarTimeScaleUnit;
            CalendarDayTop topEnd = _selectedElementEnd as CalendarDayTop;
            CalendarDay dayEnd = _selectedElementEnd as CalendarDay;

            ClearSelectedComponents();

            if (_selectedElementEnd == null || _selectedElementStart == null) return;

            if (_selectedElementEnd.CompareTo(SelectedElementStart) < 0)
            {
                //swap
                unitStart = _selectedElementEnd as CalendarTimeScaleUnit;
                topStart = _selectedElementEnd as CalendarDayTop;
                dayStart = _selectedElementEnd as CalendarDay;
                unitEnd = SelectedElementStart as CalendarTimeScaleUnit;
                topEnd = SelectedElementStart as CalendarDayTop;
                dayEnd = _selectedElementStart as CalendarDay;
            }

            if (unitStart != null && unitEnd != null)
            {
                bool reached = false;
                for (int i = unitStart.Day.Index; !reached; i++)
                {
                    for (int j = (i == unitStart.Day.Index ? unitStart.Index : 0); i < Days.Length && j < Days[i].TimeUnits.Length; j++)
                    {
                        CalendarTimeScaleUnit unit = Days[i].TimeUnits[j];
                        unit.SetSelected(true);
                        GrowSquare(unit.Bounds);
                        _selectedElements.Add(unit);

                        if (unit.Equals(unitEnd))
                        {
                            reached = true;
                            break;
                        }
                    }
                }
            }
            else if (topStart != null && topEnd != null)
            {
                for (int i = topStart.Day.Index; i <= topEnd.Day.Index; i++)
                {
                    CalendarDayTop top = Days[i].DayTop;

                    top.SetSelected(true);
                    GrowSquare(top.Bounds);
                    _selectedElements.Add(top);
                }
            }
            else if (dayStart != null && dayEnd != null)
            {
                for (int i = dayStart.Index; i <= dayEnd.Index; i++)
                {
                    CalendarDay day = Days[i];

                    day.SetSelected(true);
                    GrowSquare(day.Bounds);
                    _selectedElements.Add(day);
                }
            }

            InvalidateVisual(); // Invalidate(_selectedElementSquare);
        }

        #endregion
    }
}
