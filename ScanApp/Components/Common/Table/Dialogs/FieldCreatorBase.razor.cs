using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using ScanApp.Components.Common.ScanAppTable.Options;

namespace ScanApp.Components.Common.Table.Dialogs
{
    public abstract partial class FieldCreatorBase<T> : ComponentBase
    {
        [Parameter]
        public IEnumerable<ColumnConfig<T>> Configs { get; set; }

        [Parameter] public bool StartExpanded { get; set; }
        [Parameter] public string PickerOKLabel { get; set; } = "Ok";
        [Parameter] public string PickerCancelLabel { get; set; } = "Cancel";
        [Parameter] public string PickerClearLabel { get; set; } = "Clear";

        protected Dictionary<ColumnConfig<T>, Delegate> Validators { get; } = new();
        protected Dictionary<ColumnConfig<T>, MudExpansionPanel> Panels { get; } = new();
        protected EventCallbackFactory CallbackFactory { get; } = new();

        public abstract RenderFragment CreateField(ColumnConfig<T> config);

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
                    return;

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
    }
}