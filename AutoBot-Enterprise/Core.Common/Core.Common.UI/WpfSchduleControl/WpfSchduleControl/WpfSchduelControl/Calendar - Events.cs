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
        /// <summary>
        /// Delegate that supports <see cref="LoadItems"/> event
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event Data</param>
        public delegate void CalendarLoadEventHandler(object sender, CalendarLoadEventArgs e);

        /// <summary>
        /// Delegate that supports item-related events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CalendarItemEventHandler(object sender, CalendarItemEventArgs e);

        /// <summary>
        /// Delegate that supports cancelable item-related events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CalendarItemCancelEventHandler(object sender, CalendarItemCancelEventArgs e);

        /// <summary>
        /// Delegate that supports <see cref="CalendarDay"/>-related events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void CalendarDayEventHandler(object sender, CalendarDayEventArgs e);

        /// <summary>
        /// Occurs when items are load into view
        /// </summary>
        [Description("Occurs when items are load into view")]
        public event CalendarLoadEventHandler LoadItems;

        /// <summary>
        /// Occurs when a day header is clicked
        /// </summary>
        [Description("Occurs when a day header is clicked")]
        public event CalendarDayEventHandler DayHeaderClick;

        /// <summary>
        /// Occurs when an item is about to be created.
        /// </summary>
        /// <remarks>
        /// Event can be cancelled
        /// </remarks>
        [Description("Occurs when an item is about to be created.")]
        public event CalendarItemCancelEventHandler ItemCreating;

        /// <summary>
        /// Occurs when an item has been created.
        /// </summary>
        [Description("Occurs when an item has been created.")]
        public event CalendarItemCancelEventHandler ItemCreated;

        /// <summary>
        /// Occurs before an item is deleted
        /// </summary>
        [Description("Occurs before an item is deleted")]
        public event CalendarItemCancelEventHandler ItemDeleting;

        /// <summary>
        /// Occurs when an item has been deleted
        /// </summary>
        [Description("Occurs when an item has been deleted")]
        public event CalendarItemEventHandler ItemDeleted;

        /// <summary>
        /// Occurs when an item text is about to be edited
        /// </summary>
        [Description("Occurs when an item text is about to be edited")]
        public event CalendarItemCancelEventHandler ItemTextEditing;

        /// <summary>
        /// Occurs when an item text is edited
        /// </summary>
        [Description("Occurs when an item text is edited")]
        public event CalendarItemCancelEventHandler ItemTextEdited;

        /// <summary>
        /// Occurs when an item time range has changed
        /// </summary>
        [Description("Occurs when an item time range has changed")]
        public event CalendarItemEventHandler ItemDatesChanged;

        /// <summary>
        /// Occurs when an item is clicked
        /// </summary>
        [Description("Occurs when an item is clicked")]
        public event CalendarItemEventHandler ItemClick;

        /// <summary>
        /// Occurs when an item is double-clicked
        /// </summary>
        [Description("Occurs when an item is double-clicked")]
        public event CalendarItemEventHandler ItemDoubleClick;

        /// <summary>
        /// Occurs when an item is selected
        /// </summary>
        [Description("Occurs when an item is selected")]
        public event CalendarItemEventHandler ItemSelected;

        /// <summary>
        /// Occurs after the items are positioned
        /// </summary>
        /// <remarks>
        /// Items bounds can be altered using the <see cref="CalendarItem.SetBounds"/> method.
        /// </remarks>
        [Description("Occurs after the items are positioned")]
        public event EventHandler ItemsPositioned;

        /// <summary>
        /// Occurs when the mouse is moved over an item
        /// </summary>
        [Description("Occurs when the mouse is moved over an item")]
        public event CalendarItemEventHandler ItemMouseHover;


        #region Overrided Events and Raisers

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            Renderer.OnInitialize(new CalendarRendererEventArgs(new CalendarRendererEventArgs(this, null, new Rect())));
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            FocusManager.SetFocusedElement(this, this);
            //Select();
        }

        //protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        //{
        //    base.OnMouseDoubleClick(e);

        //    CreateItemOnSelection(string.Empty, true);
        //}

        protected virtual void OnDayHeaderClick(CalendarDayEventArgs e)
        {
            if (DayHeaderClick != null)
            {
                DayHeaderClick(this, e);
            }
        }

        protected virtual void OnItemClick(CalendarItemEventArgs e)
        {
            if (ItemClick != null)
            {
                ItemClick(this, e);
            }
        }

        protected virtual void OnItemCreating(CalendarItemCancelEventArgs e)
        {
            if (ItemCreating != null)
            {
                ItemCreating(this, e);
            }
        }

        protected virtual void OnItemCreated(CalendarItemCancelEventArgs e)
        {
            if (ItemCreated != null)
            {
                ItemCreated(this, e);
            }
        }

        protected virtual void OnItemDeleting(CalendarItemCancelEventArgs e)
        {
            if (ItemDeleting != null)
            {
                ItemDeleting(this, e);
            }
        }

        protected virtual void OnItemDeleted(CalendarItemEventArgs e)
        {
            if (ItemDeleted != null)
            {
                ItemDeleted(this, e);
            }
        }

        protected virtual void OnItemDoubleClick(CalendarItemEventArgs e)
        {
            if (ItemDoubleClick != null)
            {
                ItemDoubleClick(this, e);
            }
        }

        protected virtual void OnItemEditing(CalendarItemCancelEventArgs e)
        {
            if (ItemTextEditing != null)
            {
                ItemTextEditing(this, e);
            }
        }

        protected virtual void OnItemEdited(CalendarItemCancelEventArgs e)
        {
            if (ItemTextEdited != null)
            {
                ItemTextEdited(this, e);
            }
        }

        protected virtual void OnItemSelected(CalendarItemEventArgs e)
        {
            if (ItemSelected != null)
            {
                ItemSelected(this, e);
            }
        }

        protected virtual void OnItemsPositioned(EventArgs e)
        {
            if (ItemsPositioned != null)
            {
                ItemsPositioned(this, e);
            }
        }

        protected virtual void OnItemDatesChanged(CalendarItemEventArgs e)
        {
            if (ItemDatesChanged != null)
            {
                ItemDatesChanged(this, e);
            }
        }

        protected virtual void OnItemMouseHover(CalendarItemEventArgs e)
        {
            if (ItemMouseHover != null)
            {
                ItemMouseHover(this, e);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (AllowNew)
                CreateItemOnSelection(e.Key.ToString(), true);

            bool shiftPressed = (e.Key.GetTypeCode()) == ModifierKeys.Shift.GetTypeCode();
            int jump = (int)TimeScale;
            ICalendarSelectableElement sStart = null;
            ICalendarSelectableElement sEnd = null;

            if (e.Key == Key.F2)
            {
                ActivateEditMode();
            }
            else if (e.Key == Key.Delete)
            {
                DeleteSelectedItems();
            }
            else if (e.Key == Key.Insert)
            {
                if (AllowNew)
                    CreateItemOnSelection(string.Empty, true);
            }
            else if (e.Key == Key.Down)
            {
                if (e.Key.GetTypeCode() == ModifierKeys.Shift.GetTypeCode())
                    sStart = SelectedElementStart;

                sEnd = GetTimeUnit(SelectedElementEnd.Date.Add(new TimeSpan(0, (int)TimeScale, 0)));
            }
            else if (e.Key == Key.Up)
            {
                if (e.Key.GetTypeCode() == ModifierKeys.Shift.GetTypeCode())
                    sStart = SelectedElementStart;

                sEnd = GetTimeUnit(SelectedElementEnd.Date.Add(new TimeSpan(0, -(int)TimeScale, 0)));
            }
            else if (e.Key == Key.Right)
            {
                sEnd = GetTimeUnit(SelectedElementEnd.Date.Add(new TimeSpan(24, 0, 0)));
            }
            else if (e.Key == Key.Left)
            {
                sEnd = GetTimeUnit(SelectedElementEnd.Date.Add(new TimeSpan(-24, 0, 0)));
            }
            else if (e.Key == Key.PageDown)
            {

            }
            else if (e.Key == Key.PageUp)
            {

            }


            if (sStart != null)
            {
                SetSelectionRange(sStart, sEnd);
            }
            else if (sEnd != null)
            {
                SetSelectionRange(sEnd, sEnd);

                if (sEnd is CalendarTimeScaleUnit)
                    EnsureVisible(sEnd as CalendarTimeScaleUnit);
            }
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);

        //    if (AllowNew)
        //        CreateItemOnSelection(e.Key.ToString(), true);
        //}

        protected virtual void OnLoadItems(CalendarLoadEventArgs e)
        {
            if (LoadItems != null)
            {
                LoadItems(this, e);
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            CalendarItem item = ItemAt(e.GetPosition(this));

            if (item != null)
            {
                OnItemDoubleClick(new CalendarItemEventArgs(item));
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            ICalendarSelectableElement hitted = HitTest(e.GetPosition(this));
            CalendarItem hittedItem = hitted as CalendarItem;
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (!Focused)
            {
                Focus();
            }

            switch (State)
            {
                case CalendarState.Idle:
                    if (hittedItem != null)
                    {
                        if (!shiftPressed)
                            ClearSelectedItems();

                        hittedItem.SetSelected(true);
                        Invalidate(hittedItem);
                        OnItemSelected(new CalendarItemEventArgs(hittedItem));

                        itemOnState = hittedItem;
                        itemOnStateChanged = false;

                        if (AllowItemEdit)
                        {
                            if (itemOnState.ResizeStartDateZone(e.GetPosition(this)) && AllowItemResize)
                            {
                                SetState(CalendarState.ResizingItem);
                                itemOnState.SetIsResizingStartDate(true);
                            }
                            else if (itemOnState.ResizeEndDateZone(e.GetPosition(this)) && AllowItemResize)
                            {
                                SetState(CalendarState.ResizingItem);
                                itemOnState.SetIsResizingEndDate(true);
                            }
                            else
                            {
                                SetState(CalendarState.DraggingItem);
                            }
                        }

                        SetSelectionRange(null, null);
                    }
                    else
                    {
                        ClearSelectedItems();

                        if (shiftPressed)
                        {
                            if (hitted != null && SelectedElementEnd == null && !SelectedElementEnd.Equals(hitted))
                                SelectedElementEnd = hitted;
                        }
                        else
                        {
                            if (SelectedElementStart == null || (hitted != null && !SelectedElementStart.Equals(hitted)))
                            {
                                SetSelectionRange(hitted, hitted);
                            }
                        }

                        SetState(CalendarState.DraggingTimeSelection);
                    }
                    break;
                case CalendarState.DraggingTimeSelection:
                    break;
                case CalendarState.DraggingItem:
                    break;
                case CalendarState.ResizingItem:
                    break;
                case CalendarState.EditingItemText:
                    break;

            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            ICalendarSelectableElement hitted = HitTest(e.GetPosition(this), State != CalendarState.Idle);
            CalendarItem hittedItem = hitted as CalendarItem;
            CalendarDayTop hittedTop = hitted as CalendarDayTop;
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            if (hitted != null)
            {
                switch (State)
                {
                    case CalendarState.Idle:
                        Cursor should = Cursors.Arrow;

                        if (hittedItem != null)
                        {
                            if ((hittedItem.ResizeEndDateZone(e.GetPosition(this)) || hittedItem.ResizeStartDateZone(e.GetPosition(this))) && AllowItemResize)
                            {
                                should = hittedItem.IsOnDayTop || DaysMode == CalendarDaysMode.Short ? Cursors.SizeWE : Cursors.SizeNS;
                            }

                            OnItemMouseHover(new CalendarItemEventArgs(hittedItem));

                        }
                        if (!Cursor.Equals(should)) Cursor = should;
                        break;
                    case CalendarState.DraggingTimeSelection:
                        if (SelectedElementStart != null && !SelectedElementEnd.Equals(hitted))
                            SelectedElementEnd = hitted;
                        break;
                    case CalendarState.DraggingItem:
                        TimeSpan duration = itemOnState.Duration;
                        itemOnState.SetIsDragging(true);
                        itemOnState.StartDate = hitted.Date;
                        itemOnState.EndDate = itemOnState.StartDate.Add(duration);
                        Renderer.PerformItemsLayout();
                        Invalidate();
                        itemOnStateChanged = true;
                        break;
                    case CalendarState.ResizingItem:
                        if (itemOnState.IsResizingEndDate && hitted.Date.CompareTo(itemOnState.StartDate) >= 0)
                        {
                            itemOnState.EndDate = hitted.Date.Add(hittedTop != null || DaysMode == CalendarDaysMode.Short ? new TimeSpan(23, 59, 59) : Days[0].TimeUnits[0].Duration);
                        }
                        else if (itemOnState.IsResizingStartDate && hitted.Date.CompareTo(itemOnState.EndDate) <= 0)
                        {
                            itemOnState.StartDate = hitted.Date;
                        }
                        Renderer.PerformItemsLayout();
                        Invalidate();
                        itemOnStateChanged = true;
                        break;
                    case CalendarState.EditingItemText:
                        break;
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            ICalendarSelectableElement hitted = HitTest(e.GetPosition(this), State == CalendarState.DraggingTimeSelection);
            CalendarItem hittedItem = hitted as CalendarItem;
            CalendarDay hittedDay = hitted as CalendarDay;
            bool shiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);

            switch (State)
            {
                case CalendarState.Idle:

                    break;
                case CalendarState.DraggingTimeSelection:
                    if (SelectedElementStart == null || (hitted != null && !SelectedElementEnd.Equals(hitted)))
                    {
                        SelectedElementEnd = hitted;
                    }
                    if (hittedDay != null)
                    {
                        if (hittedDay.HeaderBounds.Contains(e.GetPosition(this)))
                        {
                            OnDayHeaderClick(new CalendarDayEventArgs(hittedDay));
                        }
                    }
                    break;
                case CalendarState.DraggingItem:
                    if (itemOnStateChanged)
                        OnItemDatesChanged(new CalendarItemEventArgs(itemOnState));
                    break;
                case CalendarState.ResizingItem:
                    if (itemOnStateChanged)
                        OnItemDatesChanged(new CalendarItemEventArgs(itemOnState));
                    break;
                case CalendarState.EditingItemText:
                    break;
            }

            if (itemOnState != null)
            {
                itemOnState.SetIsDragging(false);
                itemOnState.SetIsResizingEndDate(false);
                itemOnState.SetIsResizingStartDate(false);
                Invalidate(itemOnState);
                OnItemClick(new CalendarItemEventArgs(itemOnState));
                itemOnState = null;
            }
            SetState(CalendarState.Idle);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            if (DaysMode == CalendarDaysMode.Expanded)
            {
                ScrollTimeUnits(e.Delta);
            }
            else if (DaysMode == CalendarDaysMode.Short)
            {
                ScrollCalendar(e.Delta);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            CalendarRendererEventArgs evt = new CalendarRendererEventArgs(this, e.DrawingContext, e.ClipRect);

            ///Calendar background
            Renderer.OnDrawBackground(evt);

            /// Headers / Timescale
            switch (DaysMode)
            {
                case CalendarDaysMode.Short:
                    Renderer.OnDrawDayNameHeaders(evt);
                    Renderer.OnDrawWeekHeaders(evt);
                    break;
                case CalendarDaysMode.Expanded:
                    Renderer.OnDrawTimeScale(evt);
                    break;
                default:
                    throw new NotImplementedException("Current DaysMode not implemented");
            }

            ///Days on view
            Renderer.OnDrawDays(evt);

            ///Items
            Renderer.OnDrawItems(evt);

            ///Overflow marks
            Renderer.OnDrawOverflows(evt);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo e)
        {
            base.OnRenderSizeChanged(e);

            TimeUnitsOffset = TimeUnitsOffset;
            Renderer.PerformLayout();
        }

        #endregion

    }
}
