using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JPP.Common.Windows.Localization
{
    public class Translator
    {
        Langset langset;

        public Langset Langset { get { return this.langset; } set { this.langset = langset; } }

        public Translator(Langset langset)
        {
            this.langset = langset;
        }

        void set_assumed_property(Control c,string text)
        {
            if (c is TextBox)
            {
                ((TextBox)c).Text = text;
            }
            else if (c is Label)
            {
                ((Label)c).Text = text;
            }
            else if (c is Button)
            {
                ((Button)c).Text = text;
            }
        }

        public void TranslateForm(Form form)
        {
            var Form_dict = this.langset.Dictionary.FindAll(x=>x.Form==form.Name);
            foreach(DictionaryItem di in this.langset.Dictionary)
            {
                if (string.IsNullOrEmpty(di.Control))
                {
                    form.Text = di.Text;
                }
                else
                {
                    Control[] c = form.Controls.Find(di.Control, true);
                    if (c.Length > 0)
                    {
                        set_assumed_property(c[0], di.Text);
                    }
                }
            }
        }

        public void TranslateApplication(List<Form> forms)
        {
            foreach(Form name in forms)
            {
                TranslateForm(name);
            }
        }

    }
}
