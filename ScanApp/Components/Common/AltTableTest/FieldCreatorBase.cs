using Microsoft.AspNetCore.Components;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ScanApp.Components.Common.AltTableTest
{
    public abstract class FieldCreatorBase<T> : ComponentBase
    {
        [Parameter]
        public List<ColumnConfig<T>> Configs { get; set; }

        protected Dictionary<ColumnConfig<T>, Delegate> Validators { get; } = new();
        protected EventCallbackFactory CallbackFactory { get; } = new();

        protected override void OnInitialized()
        {
            CacheValidators();
        }

        private void CacheValidators()
        {
            foreach (var config in Configs)
            {
                if (config.IsValidatable is false)
                    return;

                var methodType = config.GetType().GetMethod(nameof(config.Validate))?.MakeGenericMethod(config.PropertyType)
                                 ?? throw new ArgumentException("Method for validator func creation is not accessible / not existing" +
                                                                " - check source class of this method.");
                var validatorFuncType = Expression.GetDelegateType(config.PropertyType, typeof(IEnumerable<string>));
                var validationDelegate = Delegate.CreateDelegate(validatorFuncType, config, methodType);

                Validators.Add(config, validationDelegate);
            }
        }
    }
}