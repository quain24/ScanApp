using System.Transactions;

namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public class ScanAppTableOptions
    {
        private readonly bool _allowGrouping;
        private readonly bool _allowFiltering;

        public ScanAppTableOptions()
        {
            ShowToolBar = IsToolBarEnabled();
        }

        public bool ShowToolBar { get; private set; }

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

        private bool IsToolBarEnabled()
        {
            if (AllowGrouping is false && AllowFiltering is false)
            {
                return false;
            }
            return true;
        }
    }
}