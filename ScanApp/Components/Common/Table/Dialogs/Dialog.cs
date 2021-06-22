using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.Table.Dialogs
{
    public abstract class Dialog<T> : ComponentBase
    {
        [CascadingParameter] protected MudDialogInstance MudDialog { get; set; }

        /// <summary>
        /// Gets or sets collection of configuration objects which will be used to get informations needed to create fields for this dialog.
        /// </summary>
        /// <value>Collection of <see cref="ColumnConfig{T}" /> objects if set, otherwise <see langword="null" />.</value>
        [Parameter] public List<ColumnConfig<T>> Configs { get; set; }

        /// <summary>
        /// Gets or sets height of dialog content (which exclude buttons and header).
        /// </summary>
        /// <value>Dialog content height in pixels.</value>
        [Parameter] public int DialogContentHeight { get; set; }

        /// <summary>
        /// Fired when a key is pressed in an active text / numeric field generated
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        protected override void OnInitialized()
        {
            OnKeyDown = OnKeyDown.HasDelegate ? OnKeyDown : EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownPress);
        }

        protected abstract Task Submit();

        protected abstract void Cancel();

        private void OnKeyDownPress(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter": Submit();
                    break;
                case "Escape": Cancel();
                    break;
            }
        }
    }
}