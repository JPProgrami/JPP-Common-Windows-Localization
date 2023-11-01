using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Common.Windows.Localization
{
    public class DictionaryItem
    {
        string form;
        string control;
        string text;

        public string Form { get => form; set => form = value; }
        public string Control { get => control; set => control = value; }
        public string Text { get => text; set => text = value; }

        public DictionaryItem()
        {

        }
        public DictionaryItem(string form, string control, string text)
        {
            this.form = form;
            this.control = control;
            this.text = text;
        }
    }
}
