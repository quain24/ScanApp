using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace ScanApp.Components.Table.Dialogs
{
    public abstract partial class FieldCreatorBase<T> : ComponentBase, IDisposable
    {
        /// <summary>
        /// Gets or sets collection of configuration objects which will be used to get informations needed to create fields.
        /// </summary>
        /// <value>Collection of <see cref="ColumnConfig{T}"/> objects if set, otherwise <see langword="null"/>.</value>
        [Parameter] public IEnumerable<ColumnConfig<T>> Configs { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether fields created by this creator should be displayed as open right after start.
        /// </summary>
        /// <value><see langword="true"/> if fields created by this creator are to be displayed as open initially, otherwise <see langword="false"/>.</value>
        [Parameter] public bool StartExpanded { get; set; }

        /// <summary>
        /// Gets or sets date / time picker button label for 'OK' option.
        /// </summary>
        /// <value><see cref="string"/> representing picker 'OK' label if set. Default value is 'Ok'.</value>
        [Parameter] public string PickerOKLabel { get; set; } = "Ok";

        /// <summary>
        /// Gets or sets date / time picker button label for 'Cancel' option.
        /// </summary>
        /// <value><see cref="string"/> representing picker 'OK' label if set. Default value is 'Cancel'.</value>
        [Parameter] public string PickerCancelLabel { get; set; } = "Cancel";

        /// <summary>
        /// Gets or sets date / time picker button label for 'Clear' option.
        /// </summary>
        /// <value><see cref="string"/> representing picker 'Clear' label if set. Default value is 'Clear'.</value>
        [Parameter] public string PickerClearLabel { get; set; } = "Clear";

        /// <summary>
        /// Gets or sets maximum height of displayed set of fields in pixels.
        /// </summary>
        /// <value>An <see cref="int"/> representing field set height in pixels. Default value is '500'.</value>
        [Parameter] public int MaxFieldSetHeight { get; set; }

        /// <summary>
        /// Gets or sets <see cref="CultureInfo"/> used in all text and picker fields.
        /// </summary>
        /// <value><see cref="CultureInfo"/> set by user, otherwise <see langword="null"/>.</value>
        [Parameter] public CultureInfo CultureInfo { get; set; }

        /// <summary>
        /// Fired when a key is pressed in an active text / numeric field generated
        /// </summary>
        [Parameter] public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        /// <summary>
        /// Gets a dictionary in which keys are <see cref="ColumnConfig{T}"/> objects and values are corresponding <see cref="MudExpansionPanel"/> references used by this creator.
        /// </summary>
        /// <value><see cref="Dictionary{TKey,TValue}"/> of <see cref="ColumnConfig{T}"/> and <see cref="MudExpansionPanel"/> if set, otherwise empty collection.</value>
        public Dictionary<ColumnConfig<T>, MudExpansionPanel> Panels { get; } = new();

        protected Dictionary<ColumnConfig<T>, Delegate> Validators { get; } = new();
        protected EventCallbackFactory CallbackFactory { get; } = new();

        protected abstract RenderFragment CreateField(ColumnConfig<T> config);

        private bool _shouldRender = true;

        protected override bool ShouldRender() => _shouldRender;

        protected override void OnInitialized()
        {
            CacheValidators();
            CreateEmptyReferencesToPanels();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (StartExpanded && firstRender)
            {
                foreach (var panel in Panels)
                {
                    panel.Value.Expand(false);
                }
                StateHasChanged();
            }
        }

        private void CacheValidators()
        {
            foreach (var config in Configs)
            {
                if (config.IsValidatable() is false)
                    continue;

                var methodType = config.GetType().GetMethod(nameof(config.Validate))?.MakeGenericMethod(config.PropertyType)
                                 ?? throw new ArgumentException("Method for validator func creation is not accessible / not existing" +
                                                                " - check source class of this method.");
                var validatorFuncType = Expression.GetDelegateType(config.PropertyType, typeof(IEnumerable<string>));
                var validationDelegate = Delegate.CreateDelegate(validatorFuncType, config, methodType);

                Validators.Add(config, validationDelegate);
            }
        }

        private void CreateEmptyReferencesToPanels()
        {
            foreach (var config in Configs)
            {
                Panels.TryAdd(config, null);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _shouldRender = false;
                OnKeyDown = EventCallback<KeyboardEventArgs>.Empty;
                foreach (var panel in Panels)
                {
                    panel.Value?.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}