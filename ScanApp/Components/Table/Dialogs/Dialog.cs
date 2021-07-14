using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace ScanApp.Components.Table.Dialogs
{
    public abstract class Dialog<T> : ComponentBase, IDisposable
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
        /// Gets or sets <see cref="CultureInfo"/> used in all text and picker fields in this dialog.
        /// </summary>
        /// <value><see cref="CultureInfo"/> set by user, otherwise <see langword="null"/>.</value>
        [Parameter] public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Fired when a key is pressed in an active text / numeric field generated
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        protected override void OnInitialized()
        {
            OnKeyDown = OnKeyDown.HasDelegate ? OnKeyDown : EventCallback.Factory.Create<KeyboardEventArgs>(this, OnKeyDownPress);
        }

        protected abstract void Submit();

        protected abstract void Cancel();

        private void OnKeyDownPress(KeyboardEventArgs args)
        {
            switch (args.Key)
            {
                case "Enter":
                    Submit();
                    break;

                case "Escape":
                    Cancel();
                    break;
            }
        }

        private bool _shouldRender = true;

        protected override bool ShouldRender() => _shouldRender || !_disposing;

        private bool _disposing;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposing = true;
                _shouldRender = false;
                OnKeyDown = EventCallback<KeyboardEventArgs>.Empty;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}