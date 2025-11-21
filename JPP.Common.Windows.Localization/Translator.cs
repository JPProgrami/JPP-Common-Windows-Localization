using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace JPP.Common.Windows.Localization
{
    public class Translator
    {
        Langset langset;

        public Langset Langset { get { return this.langset; } set { this.langset = value; } }

        /// <summary>
        /// Instantiates a new form translator based on a langset.
        /// </summary>
        /// <param name="langset">Langset to base the translator on.</param>
        public Translator(Langset langset)
        {
            this.langset = langset;
        }

        /// <summary>
        /// FOR INTERNAL USE: Sets the assumed property of an object.
        /// </summary>
        /// <param name="c">Object to set the property for.</param>
        /// <param name="text">Text to display.</param>
        void set_assumed_property(Object c,string text)
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
            else if(c is GroupBox)
            {
                ((GroupBox)c).Text = text;
            }
            else if(c is TabPage)
            {
                ((TabPage)c).Text = text;
            }
            else if(c is CheckBox)
            {
                ((CheckBox)c).Text = text;
            }
            else if(c is RadioButton)
            {
                ((RadioButton)c).Text = text;
            }
            else if (c is ToolStripMenuItem)
            {
                ((ToolStripItem)c).Text = text;
            }
        }

        /// <summary>
        /// FOR INTERNAL USE: Sets the assumed property of a group object.
        /// Due to objects such as TabControls, GroupBoxes etc. behaving differently.
        /// </summary>
        /// <param name="c">Group object.</param>
        /// <param name="key_index">String key or numeric index to access the element.</param>
        /// <param name="text">Text to display.</param>
        void set_group_assumed_property(Object c, Object key_index, string text)
        {
            if(key_index is int)
            {
                if(c is TabControl)
                {
                    ((TabControl)c).TabPages[(int)key_index].Text = text;
                }
                else if(c is ListBox)
                {
                    ((ListBox)c).Items[(int)key_index] = text;
                }
                else if (c is CheckedListBox)
                {
                    ((CheckedListBox)c).Items[(int)key_index] = text;
                }
                else if (c is ListView)
                {
                    ((ListView)c).Items[(int)key_index].Text = text;
                }
                else if(c is ComboBox)
                {
                    ((ComboBox)c).Items[(int)key_index] = text;
                }

            }
            else if(key_index is string)
            {
                if (c is TabControl)
                {
                    ((TabControl)c).TabPages[(string)key_index].Text = text;
                }
                else if (c is ListBox)
                {
                    ((ListBox)c).Items[((ListBox)c).FindStringExact((string)key_index)] = text;
                }
                else if (c is CheckedListBox)
                {
                    ((CheckedListBox)c).Items[((CheckedListBox)c).FindStringExact((string)key_index)] = text;
                }
                else if (c is ListView)
                {
                    ((ListView)c).Items[(string)key_index].Text = text;
                }
                else if (c is ComboBox)
                {
                    ((ComboBox)c).Items[((ComboBox)c).FindStringExact((string)key_index)] = text;
                }
            }
        }

        /// <summary>
        /// Translates the form to the given langset.
        /// </summary>
        /// <param name="form">Form to translate.</param>
        public void TranslateForm(Form form)
        {
            var Form_dict = this.langset.Dictionary.FindAll(x=>x.Form==form.Name);
            ToolTip tt = new ToolTip();
            foreach (DictionaryItem di in this.langset.Dictionary)
            {
                if (string.IsNullOrEmpty(di.Control))
                {
                    form.Text = di.Text;
                }
                else
                {
                    if (di.Control.Contains("ToolTip") || di.Control.Contains("tooltip") || di.Control.Contains("toolTip") || di.Control.Contains("TOOLTIP") || di.Control.Contains("TOOL_TIP"))
                    {
                        string control = di.Control.Substring(0, di.Control.LastIndexOf('.'));
                        object c = GetNestedObject(form, control);

                        tt.SetToolTip((Control)c, di.Text);
                        
                    }
                    else
                    {
                        object c = GetNestedObject(form, di.Control);
                        if (c is Tuple<object,string>)
                        {
                            Tuple<object, string> t = (Tuple<object, string>)c;
                            if (t.Item2[t.Item2.Length - 1] == ']')
                            {
                                set_group_assumed_property(t.Item1, Convert.ToInt32(t.Item2.Substring(0,t.Item2.Length - 1)), di.Text);
                            }
                            else
                            {
                                set_group_assumed_property(t.Item1, t.Item2, di.Text);
                            }
                        }
                        else
                        {
                            set_assumed_property(c, di.Text);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// FOR INTERNAL USE: Gets the nested object from a string of access operations.
        /// </summary>
        /// <param name="root">Root object to start descending from.</param>
        /// <param name="path">Path towards the object with access operations.</param>
        /// <returns>Given object or null.</returns>
        static object GetNestedObject(object root, string path)
        {
            string[] parts = path.Split('.','[');
            object currentObject = root;
            Type currentType = root.GetType();

            foreach (string part in parts)
            {
                if (currentObject == null)
                {
                    return null;
                }

                if (int.TryParse(part, out int index) && currentObject is IEnumerable enumerable)
                {
                    currentObject = GetElementAt(enumerable, index);
                }
                else
                {
                    FieldInfo field = currentType.GetField(part, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    PropertyInfo property = currentType.GetProperty(part, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                    if (field != null)
                    {
                        currentObject = field.GetValue(currentObject);
                    }
                    else if (property != null)
                    {
                        currentObject = property.GetValue(currentObject);
                    }
                    else
                    {

                        if (currentType == typeof(ToolStripMenuItem))
                        {
                            currentObject = ((ToolStripMenuItem)currentObject).DropDownItems[part];
                        }
                        else if (currentType == typeof(MenuStrip))
                        {
                            currentObject = ((MenuStrip)currentObject).Items[part];
                        }
                        else if (currentType == typeof(StatusStrip))
                        {
                            currentObject = ((StatusStrip)currentObject).Items[part];
                        }
                        else if (currentType == typeof(TabControl))
                        {
                            currentObject = ((TabControl)currentObject).TabPages[part];
                        }
                        else if (currentType == typeof(ListBox))
                        {
                            return new Tuple<object,string>((ListBox)currentObject,part);
                        }
                        else if (currentType == typeof(CheckedListBox))
                        {
                            return new Tuple<object, string>((CheckedListBox)currentObject,part);
                        }
                        else if (currentType == typeof(ComboBox))
                        {
                            return new Tuple<object, string>((ComboBox)currentObject,part);
                        }
                        else if (currentType == typeof(ListView))
                        {
                            return new Tuple<object, string>((ListView)currentObject,part);
                        }
                    }
                }

                currentType = currentObject?.GetType();
            }

            return currentObject;
        }


        /// <summary>
        /// FOR INTERNAL USE : Gets the element at the index of the IEnumerable.
        /// </summary>
        /// <param name="enumerable">Enumerable to index.</param>
        /// <param name="index">Index of the element.</param>
        /// <returns>The element as object or null.</returns>
        private static object GetElementAt(IEnumerable enumerable, int index)
        {
            var enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }
            return enumerator.Current;
        }

        /// <summary>
        /// Translates all forms in a given application.
        /// </summary>
        /// <param name="forms">Forms to translate.</param>
        public void TranslateApplication(List<Form> forms)
        {
            foreach(Form name in forms)
            {
                TranslateForm(name);
            }
        }

    }
}
