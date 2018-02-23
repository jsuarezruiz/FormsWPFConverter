using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace FormsWPFLive.Helpers
{
    public static class XamlConverter
    {
        public static string ReplaceList = "StackPanel,StackLayout;TextBlock,Label;TextBox,Entry;ListBox,ListView;Foreground,TextColor;Background,BackgroundColor;HorizontalAlignment,HorizontalOptions;VerticalAlignment,VerticalOptions;SelectionChanged,ItemSelected;ItemClick,ItemSelected;FontWeight,FontAttributes;TextAlignment,XAlign;ProgressRing,ActivityIndicator;ScrollViewer,ScrollView;Rectangle,BoxView;\r\nHorizontalTextAlignment,XAlign;Height,HeightRequest;Width,WidthRequest;RowDefinition HeightRequest,RowDefinition Height;ColumnDefinition WidthRequest,ColumnDefinition Width;DataContext,BindingContext;ThemeResource,StaticResource;";
        public static string RemoveList = "RenderTransformOrigin,Tag,HorizontalTextAlignment";

        public static string Convert(string wpfXaml)
        {
            string FormsXaml = string.Empty;
            Debug.WriteLine("Start process");
            StringReader reader = new StringReader(wpfXaml);

            try
            {
                while (true)
                {
                    string line = reader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    if (string.IsNullOrEmpty(line))
                        FormsXaml += Environment.NewLine;

                    line = ProcessReadEndLine(line, reader);
                    line = ProcessReplaceList(line);
                    line = ProcessRemoveList(line);
                    line = ProcessRemoveProperties(line);
                    line = ProcessAlignment(line);
                    line = ProcessButton(line);
                    line = ProcessImage(line);
                    line = ProcessCheckBox(line);

                    FormsXaml += line + Environment.NewLine;
                }
            }
            catch (Exception)
            {
                FormsXaml = string.Empty;
            }
            finally
            {
                reader.Close();
                Debug.WriteLine("End process");
            }

            return FormsXaml;
        }

        private static string ProcessReadEndLine(string line, StringReader reader)
        {
            if (line.TrimEnd(new char[0]).EndsWith(">") == false)
            {
                line = string.Concat(line, ReadEndLine(reader));
            }

            return line;
        }

        private static string ReadEndLine(StringReader reader)
        {
            string result = string.Empty;
            Debug.WriteLine("Read until the end of line");

            string str = string.Empty;

            try
            {
                while (true)
                {
                    string line = reader.ReadLine();

                    if (line == null)
                    {
                        break;
                    }

                    str = string.Concat(str, string.Empty, line);

                    if (line.TrimEnd(new char[0]).EndsWith(">"))
                    {
                        result = str;

                        return result;
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.Message);
            }

            result = string.Empty;

            return result;
        }

        private static string ProcessReplaceList(string line)
        {
            Debug.WriteLine(string.Format("Processing - {0}", "Replacing", line));

            char[] separatorArray = new char[] { ';' };
            var replaceItem = ReplaceList.Split(separatorArray);
            var replaceItems = new List<string>();

            for (int i = 0; i < replaceItem.Count(); i++)
            {
                replaceItems.Add(replaceItem[i]);
            }

            for (int j = 0; j < replaceItems.Count; j++)
            {
                if (replaceItems[j].Length != 0)
                {
                    string item = replaceItems[j];
                    separatorArray = new char[] { ',' };
                    var fromto = item.Split(separatorArray);

                    if (line.Contains(fromto[0]))
                    {
                        string[] strArrays = new string[] { "Record: ", line, " From: ", fromto[0], " To: ", fromto[1] };
                        Debug.WriteLine(string.Concat(strArrays));

                        line = line.Replace(fromto[0], fromto[1]);
                    }
                }
            }

            return line;
        }

        private static string ProcessAlignment(string line)
        {
            line = line.Replace(string.Concat('\"', "Top", '\"'), string.Concat('\"', "Start", '\"'));
            line = line.Replace(string.Concat('\"', "Bottom", '\"'), string.Concat('\"', "End", '\"'));
            line = line.Replace(string.Concat('\"', "Stretch", '\"'), string.Concat('\"', "CenterAndExpand", '\"'));
            line = line.Replace(string.Concat('\"', "Left", '\"'), string.Concat('\"', "Start", '\"'));
            line = line.Replace(string.Concat('\"', "Right", '\"'), string.Concat('\"', "End", '\"'));

            return line;
        }

        private static string ProcessRemoveList(string line)
        {
            var separatorArray = new char[] { ',' };
            var removeItem = RemoveList.Split(separatorArray);
            var removeList = new List<string>();

            for (int i = 0; i < removeItem.Count(); i++)
            {
                removeList.Add(removeItem[i]);
            }

            for (int j = 0; j < removeList.Count() - 1; j++)
            {
                var toRemove = removeList[j];
                line = RemoveRecord(line, toRemove);
            }

            return line;
        }

        private static string ProcessButton(string line)
        {
            if (line.Trim().StartsWith("<Button"))
            {
                Debug.WriteLine(string.Format("Processing 'Button' - {0}", line));

                line = line.Replace("Content", "Text");
                line = line.Replace("Click=", "Clicked=");
            }

            return line;
        }

        private static string ProcessImage(string line)
        {
            if (line.Trim().StartsWith("<Image"))
            {
                Debug.WriteLine(string.Format("Processing 'Image' - {0}", line));

                line = line.Replace("Stretch=", "Aspect=");
                line = line.Replace("UniformToFill", "AspectFill");
                line = line.Replace("Fill", "Fill");
                line = line.Replace("None", "AspectFill");
                line = line.Replace("Uniform", "AspectFit");
            }

            return line;
        }

        private static string ProcessCheckBox(string line)
        {
            if (line.Trim().StartsWith("<CheckBox"))
            {
                Debug.WriteLine(string.Format("Processing 'CheckBox' - {0}", line));

                line = line.Replace("CheckBox", "Switch").Replace("IsChecked", "IsToggled").Replace("Checked=", "Toggled=");
            }

            return line;
        }

        private static string ProcessVisibility(string line)
        {
            bool change = true;

            if (!line.Contains("Visibility"))
            {
                change = false;
            }

            if (!change)
            {
                Debug.WriteLine(string.Format("Processing 'Visibility' - {0}", line));

                line = line.Replace("Visibility", "IsVisible").Replace("Collapsed", "false").Replace(string.Concat('\"', "Visible"), string.Concat('\"', "true"));
            }

            return line;
        }

        private static string ProcessRemoveProperties(string line)
        {
            if (line.Trim().StartsWith("<Entry") || line.Trim().StartsWith("<Label"))
            {
                Debug.WriteLine(string.Format("Remove Properties - {0}", line));

                line = RemoveRecord(line, "TextWrapping");
                line = RemoveRecord(line, "MaxLength");
                line = RemoveRecord(line, "InputScope");
            }

            return line;
        }

        private static string RemoveRecord(string record, string stringtoremove)
        {
            string str;

            if (!(stringtoremove != "Height" ? true : !record.ToLower().Contains("rowdefinition")))
            {
                str = record;
            }
            else if ((stringtoremove != "Width" ? true : !record.ToLower().Contains("columndefinition")))
            {
                try
                {
                    int num = record.IndexOf(stringtoremove);

                    if (num != -1)
                    {
                        int num1 = record.IndexOf('\"', num);
                        num1 = record.IndexOf('\"', num1 + 1);
                        Debug.WriteLine(string.Format("Remove", stringtoremove));
                        record = record.Replace(record.Substring(num, num1 + 1 - num), string.Empty);
                    }
                    else
                    {
                        str = record;
                        return str;
                    }
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }

                str = record;
            }
            else
            {
                str = record;
            }

            return str;
        }
    }
}