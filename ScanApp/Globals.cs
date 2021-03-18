using MudBlazor;

namespace ScanApp
{
    public static partial class Globals
    {
        public static class Gui
        {
            public static DialogOptions DefaultDialogOptions => new() { CloseButton = false, FullWidth = true, DisableBackdropClick = true };
        }
    }
}