using System.Transactions;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ScanAppTableOptions
    {
        private readonly bool _allowGrouping;
        private readonly bool _allowFiltering;
        private readonly bool _allowAdding;
        private readonly bool _allowDeleting;

        public ScanAppTableOptions() { }

        public bool ShowToolBar { get; private set; }
        public bool ToolBarAlwaysOpened { get; set; }

        public bool AllowGrouping
        {
            get => _allowGrouping;
            init
            {
                if (value is true) ShowToolBar = true;
                _allowGrouping = value;
            }
        }

        public bool AllowFiltering
        {
            get => _allowFiltering;
            init
            {
                if (value is true) ShowToolBar = true;
                _allowFiltering = value;
            }
        }

        public bool AllowAdding
        {
            get => _allowAdding;
            init
            {
                if (value is true) ShowToolBar = true;
                _allowAdding = value;
            }
        }

        public bool AllowDeleting
        {
            get => _allowDeleting;
            init
            {
                if (value is true) ShowToolBar = true;
                _allowDeleting = value;
            }
        }
    }
}