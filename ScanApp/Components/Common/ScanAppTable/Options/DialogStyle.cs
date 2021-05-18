namespace ScanApp.Components.Common.ScanAppTable.Options
{
    public static class DialogStyle
    {
        public static string GetDialogStyle(int length)
        {
            if (length >= 5)
            {
                return "max-height: 500px; overflow-y: scroll;";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}