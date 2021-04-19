using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class Headers
    {
        public Headers(IEnumerable<string> names)
        {
            Names.AddRange(names);
            foreach (var name in names)
            {
                Editable.Add(true);
                Visible.Add(true);
                Groupable.Add(true);
            }
            Count = Names.Count;
        }
        public List<string> Names { get; set; } = new List<string>();
        public List<bool> Editable { get; set; } = new List<bool>();
        public List<bool> Visible { get; set; } = new List<bool>();
        public List<bool> Groupable { get; set; } = new List<bool>();
        public int Count { get; set; }

        public bool IsVisible(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (headerName == Names[i])
                {
                    return Visible[i];
                }
            }
            return true;
        }

        public bool IsEditable(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (headerName == Names[i])
                {
                    return Editable[i];
                }
            }
            return true;
        }

        public bool IsGroupable (string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (headerName == Names[i])
                {
                    return Groupable[i];
                }
            }
            return true;
        }

        public bool IsVisible(int index)
        {
            return Visible[index];
        }

        public bool IsEditable(int index)
        {
            return Editable[index];
        }

        public bool IsGroupable(int index)
        {
            return Groupable[index];
        }

        public void MakeNotEditable(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == headerName)
                {
                    Editable[i] = false;
                }
            }
        }

        public void MakeNotEditable(int index)
        {
            Editable[index] = false;
        }

        public void MakeEditable(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == headerName)
                {
                    Editable[i] = true;
                }
            }
        }

        public void MakeEditable(int index)
        {
            Editable[index] = true;
        }

        public void MakeNotVisible(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == headerName)
                {
                    Visible[i] = false;
                }
            }
        }

        public void MakeNotVisible(int index)
        {
            Visible[index] = false;
        }

        public void MakeVisible(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == headerName)
                {
                    Visible[i] = true;
                }
            }
        }

        public void MakeVisible(int index)
        {
            Visible[index] = true;
        }

        public void MakeNotGroupable(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == headerName)
                {
                    Groupable[i] = false;
                }
            }
        }

        public void MakeNotGroupable(int index)
        {
            Groupable[index] = false;
        }

        public void MakeGroupable(string headerName)
        {
            for (int i = 0; i < Names.Count; i++)
            {
                if (Names[i] == headerName)
                {
                    Groupable[i] = true;
                }
            }
        }

        public void MakeGroupable(int index)
        {
            Groupable[index] = true;
        }
    }
}
