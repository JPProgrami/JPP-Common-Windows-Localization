using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JPP.Common.Windows.Localization
{
    public class DictionaryString
    {
        string name;
        string text;

        public string Name { get => name; set => name = value; }
        public string Text { get => text; set => text = value; }

        public DictionaryString()
        {

        }
        public DictionaryString(string name, string text)
        {
            this.name = name;
            this.text = text;
        }
    }
}
