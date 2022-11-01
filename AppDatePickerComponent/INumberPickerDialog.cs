using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppDatePickerComponent
{
    public interface INumberPickerDialog
    {
        Task<(bool, int)> ShowPicker(string title, string okButtonText, string cancelButtonText, NumberPickerOptions options);
    }
}
