using MudBlazor;

// ReSharper disable once CheckNamespace
namespace Globals
{
    /// <summary>
    /// Provides global variables for GUI layer, to be used throughout application
    /// </summary>
    /// <remarks>This class is part of <see cref="Globals"/> namespace, which is shared with other, similar, classes that contains global variables for different aspects of this application</remarks>
    public static class Gui
    {
        /// <summary>
        /// Default dialog options for creating <see cref="MudDialog"/>
        /// </summary>
        /// <value>Default <see cref="MudDialog"/> options - no close button, no background click to close, full width enabled</value>
        public static DialogOptions DefaultDialogOptions { get; } = new() { CloseButton = false, FullWidth = true, DisableBackdropClick = true };
    }
}