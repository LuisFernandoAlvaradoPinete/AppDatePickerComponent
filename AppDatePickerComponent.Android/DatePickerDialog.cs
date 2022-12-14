using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppDatePickerComponent.Droid
{
    public class DatePickerDialog : DialogFragment
    {
        public event EventHandler<DateTime> OnDateTimeChanged;
        public event EventHandler<DateTime> OnClosed;

        #region Private Fields

        private const int DefaultDay = 1;
        private const int MinNumberOfMonths = 1;
        private const int MaxNumberOfMonths = 12;
        private const int MinNumberOfDays = 1;
        private const int MaxNumberOfDays = 31;
        private const int MinNumberOfYears = 1900;
        private string Title = "";
        //private const int MaxNumberOfYears = 2022;

        private NumberPicker _monthPicker;
        private NumberPicker _yearPicker;
        private NumberPicker _dayPicker;

        #endregion

        public DatePickerDialog()
        {

        }

        public DatePickerDialog(string title)
        {
            this.Title = title;
        }

        #region Public Properties

        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public DateTime? Date { get; set; }
        public bool InfiniteScroll { get; set; }

        #endregion

        public void Hide() => base.Dialog?.Hide();

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var builder = new AlertDialog.Builder(Activity);
            var inflater = Activity.LayoutInflater;

            var selectedDate = GetSelectedDate();

            var dialog = inflater.Inflate(Resource.Layout.date_picker_dialog, null);
            _monthPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_month);
            _yearPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_year);
            _dayPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_day);

            InitializeMonthPicker(selectedDate.Month);
            InitializeYearPicker(selectedDate.Year);
            InitializeDayPicker(selectedDate);
            SetMaxMinDate(MaxDate, MinDate);

            builder.SetView(dialog)
                .SetPositiveButton("Aceptar", (sender, e) =>
                {
                    selectedDate = new DateTime(_yearPicker.Value, _monthPicker.Value, _dayPicker.Value);
                    OnDateTimeChanged?.Invoke(dialog, selectedDate);
                })
                .SetNegativeButton("Cancelar", (sender, e) =>
                {
                    Dialog.Cancel();
                    OnClosed?.Invoke(dialog, selectedDate);
                })
                .SetTitle(Title);

            var dialogResult = builder.Create();
            return dialogResult;
        }

        //public override Dialog OnCreateDialog(Bundle savedInstanceState)
        //{
        //    var builder = new AlertDialog.Builder(Activity);
        //    var inflater = Activity.LayoutInflater;

        //    var selectedDate = GetSelectedDate();

        //    var dialog = inflater.Inflate(Resource.Layout.date_picker_dialog, null);
        //    _monthPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_month);
        //    _yearPicker = (NumberPicker)dialog.FindViewById(Resource.Id.picker_year);

        //    InitializeMonthPicker(selectedDate.Month);
        //    InitializeYearPicker(selectedDate.Year);
        //    SetMaxMinDate(MaxDate, MinDate);

        //    builder.SetView(dialog)
        //        .SetPositiveButton("Aceptar", (sender, e) =>
        //        {
        //            selectedDate = new DateTime(_yearPicker.Value, _monthPicker.Value, DefaultDay);
        //            OnDateTimeChanged?.Invoke(dialog, selectedDate);
        //        })
        //        .SetNegativeButton("Cancelar", (sender, e) =>
        //        {
        //            Dialog.Cancel();
        //            OnClosed?.Invoke(dialog, selectedDate);
        //        });
        //    return builder.Create();
        //}

        protected override void Dispose(bool disposing)
        {
            if (_yearPicker != null)
            {
                _yearPicker.ScrollChange -= YearPicker_ScrollChange;
                _yearPicker.Dispose();
                _yearPicker = null;
            }

            _monthPicker?.Dispose();
            _monthPicker = null;


            base.Dispose(disposing);
        }

        #region Private Methods

        private DateTime GetSelectedDate() => Date ?? DateTime.Now;

        private void InitializeYearPicker(int year)
        {
            _yearPicker.MinValue = MinNumberOfYears;
            _yearPicker.MaxValue = DateTime.Now.Year;
            _yearPicker.Value = year;
            _yearPicker.ScrollChange += YearPicker_ScrollChange;
            if (!InfiniteScroll)
            {
                _yearPicker.WrapSelectorWheel = false;
                _yearPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            }
        }

        private void InitializeMonthPicker(int month)
        {
            _monthPicker.MinValue = MinNumberOfMonths;
            _monthPicker.MaxValue = MaxNumberOfMonths;
            _monthPicker.SetDisplayedValues(GetMonthNames());
            _monthPicker.ScrollChange += MonthPicker_ScrollChange;
            _monthPicker.Value = month;
            if (!InfiniteScroll)
            {
                _monthPicker.WrapSelectorWheel = false;
                _monthPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            }
        }

        private void InitializeDayPicker(DateTime date)
        {
            _dayPicker.MinValue = MinNumberOfDays;
            _dayPicker.MaxValue = DateTime.DaysInMonth(date.Year, date.Month);
            _dayPicker.Value = date.Day;
            if (!InfiniteScroll)
            {
                _dayPicker.WrapSelectorWheel = false;
                _dayPicker.DescendantFocusability = DescendantFocusability.BlockDescendants;
            }
        }

        private void YearPicker_ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            SetMaxMinDate(MaxDate, MinDate);
        }

        private void MonthPicker_ScrollChange(object sender, View.ScrollChangeEventArgs e)
        {
            var month = _monthPicker.Value;
            var year = _yearPicker.Value;
            SetMaxDay(month, year);
        }

        private void SetMaxDay(int month, int year)
        {
            int maxDaysMonth = DateTime.DaysInMonth(year, month);
            _dayPicker.MaxValue = maxDaysMonth;
        }

        private void SetMaxMinDate(DateTime? maxDate, DateTime? minDate)
        {
            try
            {
                int month = _monthPicker.Value;
                int year = _yearPicker.Value;
                SetMaxDay(month, year);
                //if (maxDate.HasValue)
                //{
                //    var maxYear = maxDate.Value.Year;
                //    var maxMonth = maxDate.Value.Month;

                //    if (_yearPicker.Value == maxYear)
                //    {
                //        _monthPicker.MaxValue = maxMonth;
                //    }
                //    else if (_monthPicker.MaxValue != MaxNumberOfMonths)
                //    {
                //        _monthPicker.MaxValue = MaxNumberOfMonths;
                //    }

                //    _yearPicker.MaxValue = maxYear;
                //}

                //if (minDate.HasValue)
                //{
                //    var minYear = minDate.Value.Year;
                //    var minMonth = minDate.Value.Month;

                //    if (_yearPicker.Value == minYear)
                //    {
                //        _monthPicker.MinValue = minMonth;
                //    }
                //    else if (_monthPicker.MinValue != MinNumberOfMonths)
                //    {
                //        _monthPicker.MinValue = MinNumberOfMonths;
                //    }

                //    _yearPicker.MinValue = minYear;
                //}
                //_monthPicker.SetDisplayedValues(GetMonthNames(_monthPicker.MinValue));
            }
            catch (Exception e)
            {
            }
        }

        private string[] GetMonthNames(int start = 1) =>
            System.Globalization.DateTimeFormatInfo.CurrentInfo?.MonthNames.Skip(start - 1).ToArray();  

        #endregion
    }
}