using MudBlazor;

namespace ScanApp.Components.YesNoDialog
{
    public static class IDialogServiceExtensions
    {
        public static IDialogReference AskForConfirmation(this IDialogService dialogService, string title, string content, DialogOptions options = default)
        {
            options ??= new DialogOptions { CloseButton = false, DisableBackdropClick = true, FullWidth = true };
            return dialogService.Show<YesNoDialog>(title, new DialogParameters { ["Content"] = content }, options);
        }
    }
}