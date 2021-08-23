using MudBlazor;
using ScanApp.Common.Extensions;
using ScanApp.Components.Table.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ScanApp.Components.Table.Utilities
{
    public class DialogFacade<T>
    {
        private List<ColumnConfig<T>> _configs;
        private IDialogService DialogService { get; }
        private CultureInfo CultureInfo { get; }

        public DialogFacade(IDialogService dialogService, IEnumerable<ColumnConfig<T>> configs, CultureInfo cultureInfo)
        {
            GetNewConfigs(configs);
            DialogService = dialogService;
            CultureInfo = cultureInfo;
        }

        public void GetNewConfigs(IEnumerable<ColumnConfig<T>> configs) => _configs = configs?.Where(c => c.IsPresenter is false).ToList();

        public Task<DialogResult> ShowFilterDialog(bool startExpanded, int maxContentHeight)
        {
            var dialog = DialogService.Show<FilterDialog<T>>("Filter by...",
                new DialogParameters
                {
                    ["Configs"] = _configs,
                    ["CultureInfo"] = CultureInfo,
                    ["StartExpanded"] = startExpanded,
                    ["DialogContentHeight"] = maxContentHeight,
                },
                Globals.Gui.DefaultDialogOptions);
            return dialog.Result;
        }

        public async Task<DialogResult> ShowEditDialog(bool startExpanded, int maxContentHeight, bool expandInvalidFieldsOnStart, T item, object copier = null)
        {
            var dialog = DialogService.Show<EditDialog<T>>("Edit item",
                new DialogParameters
                {
                    ["Configs"] = _configs,
                    ["SourceItem"] = await CreateCopy(item, copier),
                    ["StartExpanded"] = startExpanded,
                    ["DialogContentHeight"] = maxContentHeight,
                    ["CultureInfo"] = CultureInfo,
                    ["ExpandInvalidPanelsOnStart"] = expandInvalidFieldsOnStart
                },
                Globals.Gui.DefaultDialogOptions);

            return await dialog.Result;
        }

        private static async Task<T> CreateCopy(T source, object copier)
        {
            try
            {
                return copier switch
                {
                    null => source.Copy(),
                    Func<T, T> factory => factory.Invoke(source),
                    Func<T, Task<T>> factory => await factory.Invoke(source).ConfigureAwait(false),
                    _ => throw new ArgumentOutOfRangeException(nameof(copier), "Provided factory type is not compatible with allowed delegate types.")
                };
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                throw new Exception($"Something went wrong when trying to create item copy for editing using provided {nameof(copier)}: {ex.Message}", ex);
            }
        }

        public async Task<DialogResult> ShowAddDialog(int maxContentHeight, object itemFactory)
        {
            var dialog = DialogService.Show<AddDialog<T>>("Create new item",
                new DialogParameters
                {
                    ["Configs"] = _configs,
                    ["SourceItem"] = await CreateNewItem(itemFactory),
                    ["CultureInfo"] = CultureInfo,
                    ["DialogContentHeight"] = maxContentHeight
                },
                Globals.Gui.DefaultDialogOptions);

            return await dialog.Result;
        }

        private static async Task<T> CreateNewItem(object factory)
        {
            try
            {
                return factory switch
                {
                    Func<T> f => f.Invoke(),
                    Func<Task<T>> f => await f.Invoke().ConfigureAwait(false),
                    null => throw new ArgumentNullException(nameof(factory), "No factory delegate was provided"),
                    _ => throw new ArgumentOutOfRangeException(nameof(factory), "Provided factory type is not compatible with allowed delegate types.")
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Something went wrong when trying to create new item using provided {nameof(factory)}: {ex.Message}", ex);
            }
        }
    }
}