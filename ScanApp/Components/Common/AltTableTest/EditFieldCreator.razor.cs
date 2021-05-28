using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Components.Common.ScanAppTable.Options;
using System;
using System.Threading.Tasks;
using ScanApp.Common.Helpers;

namespace ScanApp.Components.Common.AltTableTest
{
    public partial class EditFieldCreator<T> : FieldCreatorBase<T>
    {
        [Parameter]
        public T TargetItem { get; set; }

        [Parameter]
        public EventCallback<T> TargetItemChanged { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (TargetItem is null)
                throw new ArgumentNullException(nameof(TargetItem), "Target Item must be set (You can use @bind-TargetItem)");
        }

        private RenderFragment CreateEditField(ColumnConfig<T> config)
        {
            return builder =>
            {
                if ((config.PropertyType == typeof(DateTime?) && config.FieldType == FieldType.AutoDetect) ||
                    config.FieldType is FieldType.Date or FieldType.DateAndTime)
                    CreateDateFields(builder, config);
                if ((config.PropertyType == typeof(DateTime?) && config.FieldType == FieldType.AutoDetect) ||
                    (config.PropertyType == typeof(TimeSpan?) && config.FieldType == FieldType.AutoDetect) ||
                    config.FieldType is FieldType.Time or FieldType.DateAndTime)
                    CreateTimeFields(builder, config);
                else if (config.FieldType == FieldType.PlainText || (config.PropertyType != typeof(DateTime?) && config.PropertyType != typeof(TimeSpan?)))
                    CreateTextField(builder, config);
            };
        }

        private void CreateTextField(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            // Type of mud blazor text field (int, string, etc)
            var textFieldType = typeof(MudTextField<>).MakeGenericType(config.PropertyType);

            // Start creating text field
            builder.OpenComponent(LineNumber.Get(), textFieldType);
            builder.AddAttribute(LineNumber.Get(), "Value", config.GetValueFrom(TargetItem) as object);

            // Set callback for edit action
            var callbackType = typeof(EventCallback<>).MakeGenericType(config.PropertyType);
            async Task EditDelegate(dynamic obj)
            {
                ColumnConfigExtensions.SetValue(config, TargetItem, obj);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }

            dynamic callback = Activator.CreateInstance(callbackType, this, (Func<dynamic, Task>)EditDelegate);
            builder.AddAttribute(LineNumber.Get(), "ValueChanged", callback);

            // Set up corresponding validator
            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), "Validation", validatorDelegate);

            // Set common options
            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Disabled", !config.IsEditable);

            // Finish component
            builder.CloseComponent();
        }

        private void CreateDateFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            builder.OpenComponent(LineNumber.Get(), typeof(MudDatePicker));
            builder.AddAttribute(LineNumber.Get(), "Date", config.GetValueFrom(TargetItem) as DateTime?);

            var callback = CallbackFactory.Create<DateTime?>(this, d => EditDate(d, config));
            builder.AddAttribute(LineNumber.Get(), "DateChanged", callback);

            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), "Validation", validatorDelegate);

            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Disabled", !config.IsEditable);

            builder.CloseComponent();
        }

        private void CreateTimeFields(RenderTreeBuilder builder, ColumnConfig<T> config)
        {
            builder.OpenComponent(LineNumber.Get(), typeof(MudTimePicker));

            if (config.PropertyType == typeof(DateTime?))
            {
                var data = config.GetValueFrom(TargetItem) as DateTime?;
                TimeSpan? time = data.HasValue switch
                {
                    true => data.Value.TimeOfDay,
                    false => null
                };

                builder.AddAttribute(LineNumber.Get(), "Time", time);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTimeInDate(d, TargetItem, config));
                builder.AddAttribute(LineNumber.Get(), "TimeChanged", callback);
            }

            if (config.PropertyType == typeof(TimeSpan?))
            {
                var time = config.GetValueFrom(TargetItem) as TimeSpan?;
                builder.AddAttribute(LineNumber.Get(), "Time", time);
                var callback = CallbackFactory.Create<TimeSpan?>(this, d => EditTime(d, config));
                builder.AddAttribute(LineNumber.Get(), "TimeChanged", callback);
            }

            if (Validators.TryGetValue(config, out var validatorDelegate))
                builder.AddAttribute(LineNumber.Get(), "Validation", validatorDelegate);

            builder.AddAttribute(LineNumber.Get(), "Immediate", true);
            builder.AddAttribute(LineNumber.Get(), "Disabled", !config.IsEditable);

            builder.CloseComponent();
        }

        private async Task EditDate(DateTime? date, ColumnConfig<T> config)
        {
            var oldDate = config.GetValueFrom(TargetItem) as DateTime?;
            if (date != oldDate && oldDate.HasValue)
            {
                date += oldDate.Value.TimeOfDay;
            }
            config.SetValue(TargetItem, date);
            await TargetItemChanged.InvokeAsync(TargetItem);
        }

        private async Task EditTime(TimeSpan? time, ColumnConfig<T> config)
        {
            var oldTime = config.GetValueFrom(TargetItem) as TimeSpan?;

            if (time != oldTime)
            {
                config.SetValue(TargetItem, time);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
        }

        private async Task EditTimeInDate(TimeSpan? newTime, T target, ColumnConfig<T> config)
        {
            if (config.GetValueFrom(TargetItem) is DateTime oldDate)
            {
                var newDate = oldDate.Date + newTime;
                if (newDate != oldDate)
                {
                    config.SetValue(TargetItem, newDate);
                    await TargetItemChanged.InvokeAsync(TargetItem);
                }
            }
            else if (newTime.HasValue)
            {
                config.SetValue(TargetItem, DateTime.MinValue + newTime.Value);
                await TargetItemChanged.InvokeAsync(TargetItem);
            }
        }
    }
}