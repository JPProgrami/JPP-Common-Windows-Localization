using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JPP.Common.Windows.Localization
{
    public class Langset
    {
        List<DictionaryItem> dict;
        List<DictionaryString> strings;
        string name;
        public List<DictionaryItem> Dictionary { get { return this.dict; } set { this.dict = value; } }
        public List<DictionaryString> Strings { get { return this.strings; } set { this.strings = value; } }
        public string Name { get { return this.name; } set { this.name = value; } }
        public Langset(string name, List<DictionaryItem> dct, List<DictionaryString> strings)
        {
            this.dict = dct;
            this.strings = strings;
            this.name = name;
        }

        public Langset()
        {

        }

        /// <summary>
        /// Read a langset from a .lcl file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Langset</returns>
        /// <exception cref="LclFileException"></exception>
        public static Langset FromFile(string path)
        {
            StreamReader myreader = new StreamReader(path);
            List<DictionaryItem> temp_dict = new List<DictionaryItem>();
            List<DictionaryString> temp_strings = new List<DictionaryString>();
            bool reading_strings = false;
            bool reading_dict = false;
            string temp_name = "";
            int line_count = 1;
            while (!myreader.EndOfStream)
            {
                string line = myreader.ReadLine();
                if (string.IsNullOrWhiteSpace(line) == true)
                {
                    continue;
                }
                else if (line[0] == '#')
                {
                    continue;
                }
                string[] lines = line.Split('=');
                for (int i = 0; i < lines.Length; i++)
                {
                    lines[i] = lines[i].Trim();
                }
                if (line[0] == '$')
                {
                    if (lines[0] == "$NAME")
                    {
                        temp_name = lines[1];
                        continue;
                    }
                    else if (lines[0] == "$BEGIN-DICT")
                    {
                        if (reading_strings || reading_dict)
                        {
                            throw new LclFileException("Bad file structure at " + line_count + ".");
                        }
                        reading_dict = true;
                        continue;
                    }
                    else if (lines[0] == "$END-DICT")
                    {
                        if (reading_strings || !reading_dict)
                        {
                            throw new LclFileException("Bad file structure at " + line_count + ".");
                        }
                        reading_dict = false;
                        continue;
                    }
                    else if (lines[0] == "$BEGIN-STRINGS")
                    {
                        if (reading_dict || reading_strings)
                        {
                            throw new LclFileException("Bad file structure at " + line_count + ".");
                        }
                        reading_strings = true;
                        continue;
                    }
                    else if (lines[0] == "$END-STRINGS")
                    {
                        if (!reading_strings || reading_dict)
                        {
                            throw new LclFileException("Bad file structure at " + line_count + ".");
                        }
                        reading_strings = false;
                        continue;
                    }
                }
                if (reading_dict)
                {
                    string beforeequals = line.Substring(0, line.IndexOf('=')).Trim();
                    string afterequals = line.Substring(line.IndexOf('=') + 1).Trim();
                    string[] liness = beforeequals.Split('.');

                    if (liness.Length > 1)
                    {
                        string temp = "";
                        for(int i=1;i<liness.Length;i++)
                        {
                            temp += liness[i];
                            if(i<liness.Length-1)
                            {
                                temp += ".";
                            }
                        }
                        temp_dict.Add(new DictionaryItem(liness[0], temp, afterequals));
                    }
                    else
                    {
                        temp_dict.Add(new DictionaryItem(liness[0], "", afterequals));
                    }
                }
                else if (reading_strings)
                {
                    string beforeequals = line.Substring(0, line.IndexOf('=')).Trim();
                    string afterequals = line.Substring(line.IndexOf('=')+1).Trim();
                    DictionaryString strnew = new DictionaryString(beforeequals, afterequals);
                    if (!temp_strings.Contains(strnew))
                    {
                        temp_strings.Add(strnew);
                    }
                    else
                    {
                        throw new LclFileException("Redefinition of existing string at line " + line_count + ".");
                    }

                }
                line_count++;
            }
            myreader.Close();
            return new Langset(temp_name, temp_dict, temp_strings);
        }

        /// <summary>
        /// Finds a dictionary item for the form and control name given.
        /// </summary>
        /// <param name="form_name">Name of the control's parent form.</param>
        /// <param name="control_name">Name of the control.</param>
        /// <returns>DictionaryItem</returns>
        public DictionaryItem GetDictionaryForItem(string form_name, string control_name)
        {
            return dict.Find(x => x.Form == form_name && x.Control == control_name);
        }

        /// <summary>
        /// Finds a dictionary string for the string name given.
        /// </summary>
        /// <param name="string_name">Name of the string.</param>
        /// <returns>DictionaryString</returns>
        public DictionaryString GetDictionaryString(string string_name)
        {
            return strings.Find(x => x.Name == string_name);
        }
    }
}
