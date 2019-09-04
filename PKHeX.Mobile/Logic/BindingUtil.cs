using System.Collections.Generic;
using System.Linq;

using PKHeX.Core;

namespace PKHeX.Mobile.Logic
{
    public static class BindingUtil
    {
        /// <summary>
        /// Binding with a struct-based item works fine on Windows Forms, but others require an actual object (class) reference.
        /// </summary>
        /// <param name="list">Input list</param>
        /// <returns>Wrapped list</returns>
        public static ComboObject[] Convert(this IEnumerable<ComboItem> list) => list.Select(z => new ComboObject(z)).ToArray();

        public static int GetValue(object val)
        {
            if (val is int i)
                return i;
            return int.TryParse(val as string, out int v) ? v : 0;
        }
    }

    public class ComboObject
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboObject(ComboItem item)
        {
            Text = item.Text;
            Value = item.Value;
        }
    }
}
