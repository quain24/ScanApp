using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Globals
{
    public static class Gui
    {
        public static DialogOptions DefaultDialogOptions => new() { CloseButton = false, FullWidth = true, DisableBackdropClick = true };
    }
}