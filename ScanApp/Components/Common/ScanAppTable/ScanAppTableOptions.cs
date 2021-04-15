using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.ScanAppTable
{
    public class ScanAppTableOptions
    {
        public bool ShowToolBar { get; set; } = true;
        public bool AllowGrouping { get; set; } = true;
        public bool AllowFiltering { get; set; } = true;
        public bool AllowMultiSelection { get; set; } = true;
    }
}
