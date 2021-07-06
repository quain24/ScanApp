using Microsoft.AspNetCore.Components;
using MudBlazor;
using ScanApp.Common.Helpers;
using ScanApp.Components.Common.Table.Enums;
using ScanApp.Components.Common.Table.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ScanApp.Components.Common.Table
{
    public partial class SCTable<TTableType>
    {
        [Inject] internal IDialogService DialogService { get; set; }

        #region Outside parameters

        /// <inheritdoc cref="MudTableBase.Loading"/>
        [Parameter] public bool Loading { get; set; }

        /// <inheritdoc cref="MudTableBase.FixedHeader" />
        [Parameter] public bool FixedHeader { get; set; }

        /// <inheritdoc cref="MudTableBase.FixedFooter" />
        [Parameter] public bool FixedFooter { get; set; }

        /// <inheritdoc cref="MudTableBase.Height" />
        [Parameter] public int Height { get; set; }

        /// <inheritdoc cref="MudTableBase.Outlined" />
        /// <remarks>Default value is <see langword="true"/>.</remarks>
        [Parameter] public bool Outlined { get; set; } = true;

        /// <inheritdoc cref="MudTableBase.Bordered" />
        [Parameter] public bool Bordered { get; set; }

        /// <inheritdoc cref="MudTableBase.Virtualize" />
        [Parameter] public bool Virtualize { get; set; }

        private CultureInfo _cultureInfo;

        /// <summary>
        /// Gets or sets variable used to display time and date in the table.
        /// </summary>
        /// <value><see cref="CultureInfo"/> set by user, or <see cref="CultureInfo.CurrentCulture"/> otherwise.</value>
        [Parameter]
        public CultureInfo CultureInfo
        {
            get => _cultureInfo ?? CultureInfo.CurrentCulture;
            set => _cultureInfo = value;
        }

        /// <inheritdoc cref="MudTable{T}.HorizontalScrollbar"/>
        [Parameter]
        public bool HorizontalScrollbar { get; set; }

        /// <summary>
        /// Gets or sets maximum height of displayed Add / edit / filter dialog in pixels.
        /// </summary>
        /// <value>Maximum height of dialogs content in pixels.</value>
        [Parameter] public int MaxDialogContentHeight { get; set; }

        ///<summary>
        /// Gets or sets number of rows per single table page.<br />
        /// If the table has more items than this number, it will break into pages of said size.
        /// </summary>
        /// <value>Number of rows per single page. If set to 0, table will disable pagination. Default value is <c>50</c></value>
        [Parameter] public int RowsPerPage { get; set; } = 50;

        ///<summary>
        /// Gets or sets available page size options for user to choose from.
        /// </summary>
        /// <value>Array of page sizes. Default values are 10, 25, 50, 100 and 250 rows per page.</value>
        [Parameter] public int[] PageSizeOptions { get; set; } = { 10, 25, 50, 100, 250 };

        /// <summary>
        /// Gets or sets default row behavior for OnClick event. If enabled, clicking the row will open edit dialog.
        /// </summary>
        /// <value><see langword="true" /> if clicking a row will open edit dialog, otherwise <see langword="false" />.</value>
        [Parameter] public bool EditOnRowClick { get; set; }

        /// <summary>
        /// <inheritdoc cref="MudComponentBase.Style" />.<br />
        /// This parameter restyles table header, when table is in non-grouped mode..
        /// </summary>
        /// <value>A <see cref="string" /> representing CSS style if set. By default <c>"padding: 10px; font-size: smaller"</c>.</value>
        [Parameter] public string HeaderStyle { get; set; } = "padding: 10px; font-size: smaller";

        /// <summary>
        /// <inheritdoc cref="MudComponentBase.Style" />.<br />
        /// This parameter restyles header of table when it is in grouped mode.
        /// </summary>
        /// <value>A <see cref="string" /> representing CSS style if set. By default <c>"padding: 10px; z-index: 10"</c>.</value>
        [Parameter] public string GroupedHeaderStyle { get; set; } = "padding: 10px; z-index: 10";

        /// <summary>
        /// <inheritdoc cref="MudComponentBase.Style" />.<br />
        /// This parameter restyles rows displaying groups when table it is in grouped mode.
        /// </summary>
        /// <value>A <see cref="string" /> representing CSS style if set. By default <c>"padding-left: 10px;"</c>.</value>
        [Parameter] public string GroupedRowStyle { get; set; } = "padding-left: 10px;";

        /// <summary>
        /// <inheritdoc cref="MudComponentBase.Style" />.<br />
        /// This parameter restyles headers of sub-tables when main table is in grouped mode.
        /// </summary>
        /// <value>A <see cref="string" /> representing CSS style if set. By default <c>"border: thin solid darkgray; padding: 2px 10px 2px 10px;"</c>.</value>
        [Parameter] public string GroupHeaderStyle { get; set; } = "border: thin solid darkgray; padding: 2px 10px 2px 10px;";

        /// <summary>
        /// Gets or sets custom CSS for table rows<br />
        /// Note that some of the CSS settings are override by table settings.
        /// </summary>
        /// <value>A <see cref="string" /> representing CSS style if set. By default <c>"font-size: smaller; padding: 10px"</c>.</value>
        [Parameter] public string RowStyle { get; set; } = "font-size: smaller; padding: 10px";

        /// <summary>
        /// Gets or sets data to be displayed in table. One item will be displayed as one row.
        /// </summary>
        [Parameter] public List<TTableType> Data { get; set; }

        ///<summary>
        ///<inheritdoc cref="MudTable{T}.SelectedItem" />
        /// <br />@bind-... notation is supported
        /// </summary>
        [Parameter] public TTableType SelectedItem { get; set; }

        ///<summary>
        ///<inheritdoc cref="MudTable{T}.SelectedItemChanged" />
        /// <br />@bind-... notation is supported
        /// </summary>
        [Parameter] public EventCallback<TTableType> SelectedItemChanged { get; set; }

        /// <summary>
        /// Called when a new item is successfully created by 'Add item' table functionality and added to <see cref="Data"/> collection.
        /// </summary>
        /// <value>Callback providing freshly created <typeparamref name="TTableType" /> item.</value>
        [Parameter] public EventCallback<TTableType> ItemCreated { get; set; }

        /// <summary>
        /// Called when a new item is successfully edited by integrated 'edit' table functionality and added to <see cref="Data"/> collection.
        /// </summary>
        /// <value>Callback providing original and edited <typeparamref name="TTableType" /> items.</value>
        [Parameter] public EventCallback<(TTableType, TTableType)> ItemHasBeenEdited { get; set; }

        ///<inheritdoc cref="MudTableBase.MultiSelection" />
        [Parameter] public bool MultiSelection { get; set; }

        ///<summary>
        ///<inheritdoc cref="MudTable{T}.SelectedItems" />
        /// <br />@bind-... notation is supported
        /// </summary>
        [Parameter] public HashSet<TTableType> SelectedItems { get; set; }

        /// <summary>
        ///<inheritdoc cref="MudTable{T}.SelectedItemsChanged" />
        /// <br />@bind-... notation is supported
        /// </summary>
        [Parameter] public EventCallback<HashSet<TTableType>> SelectedItemsChanged { get; set; }

        private HashSet<TTableType> SelectedItemsBoundChild
        {
            get => SelectedItems;
            set
            {
                SelectedItems = value;
                SelectedItemsChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Gets or sets collection of configuration objects used to set all properties and behaviors of corresponding columns in table.
        /// </summary>
        /// <value>Collection of configuration objects.</value>
        [Parameter] public List<ColumnConfig<TTableType>> Configs { get; set; }

        /// <summary>
        /// Gets or sets visibility of edit button.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="Show.Auto" /></term>
        ///     <description>Table will decide if edit button is needed (<strong>default</strong>).</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.Yes" /></term>
        ///     <description>Always show edit button.</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.No" /></term>
        ///     <description>Never show edit button.</description>
        /// </item>
        /// </list>
        /// </value>
        [Parameter] public Show ShowEditButton { get; set; } = Show.Auto;

        /// <summary>
        /// Gets or sets visibility of filter button.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="Show.Auto" /></term>
        ///     <description>Table will decide if filter button is needed (<strong>default</strong>).</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.Yes" /></term>
        ///     <description>Always show filter button.</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.No" /></term>
        ///     <description>Never show filter button.</description>
        /// </item>
        /// </list>
        /// </value>
        [Parameter] public Show ShowFilterButton { get; set; } = Show.Auto;

        /// <summary>
        /// Gets or sets visibility of Add button.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="Show.Auto" /></term>
        ///     <description>Table will decide if Add button is needed (<strong>default</strong>).</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.Yes" /></term>
        ///     <description>Always show Add button.</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.No" /></term>
        ///     <description>Never show Add button.</description>
        /// </item>
        /// </list>
        /// </value>
        [Parameter] public Show ShowAddButton { get; set; } = Show.Auto;

        /// <summary>
        /// Gets or sets visibility of group-by field.
        /// </summary>
        /// <value>
        /// <list type="bullet">
        /// <item>
        ///     <term><see cref="Show.Auto" /></term>
        ///     <description>Table will decide if group-by field is needed (<strong>default</strong>).</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.Yes" /></term>
        ///     <description>Always show group-by field.</description>
        /// </item>
        /// <item>
        ///     <term><see cref="Show.No" /></term>
        ///     <description>Never show group-by field.</description>
        /// </item>
        /// </list>
        /// </value>
        [Parameter] public Show ShowGroupsField { get; set; } = Show.Auto;

        /// <summary>
        /// Gets or sets value indicating whether table should allow editing / adding.
        /// </summary>
        /// <value>If set to <see langword="true" />, Add and edit functions will be disabled (if they are available).</value>
        [Parameter] public bool ReadOnly { get; set; }

        /// <summary>
        /// Gets or sets how the edit dialog fields will be visible when dialog is opened.
        /// </summary>
        /// <value>If set to <see langword="true" />, dialog will start with all fields expanded (<strong>default</strong>).</value>
        [Parameter] public bool EditDialogStartsExpanded { get; set; } = true;

        /// <summary>
        /// Gets or sets how the edit dialog fields that are marked by validation as invalid will be visible when dialog is opened.
        /// </summary>
        /// <value>If set to <see langword="true" />, dialog will start with all invalid fields expanded (<strong>default</strong>).</value>
        [Parameter] public bool EditDialogInvalidFieldsStartExpanded { get; set; } = true;

        /// <summary>
        /// Gets or sets how the filter dialog fields will be visible when dialog is opened.
        /// </summary>
        /// <value>If set to <see langword="true" />, dialog will start with all fields expanded.</value>
        [Parameter] public bool FilterDialogStartsExpanded { get; set; }

        /// <inheritdoc cref="MudTable{T}.RowStyleFunc" />
        /// <remarks>If not set, will be used to mark currently selected row(s) by default.</remarks>
        /// <value>A <see cref="Func{TTableType, int, string}" /> delegate used to restyle rows if set, otherwise default built-in delegate.</value>
        [Parameter] public Func<TTableType, int, string> RowStyleFunc { get; set; }

        /// <summary>
        /// Gets or sets <typeparam name="TTableType" /> object factory necessary for creating new table items for when 'Add' is enabled. Supported delegates are:
        /// <para><see cref="Func{TTableType}">Func&lt;TTableType&gt;</see> - Parameter-less delegate creating new <typeparamref name="TTableType" />.</para>
        /// <para><see cref="Func{Task{TTableType}}">Func&lt;Task&lt;TTableType&gt;&gt;</see> - Async Parameter-less delegate creating new <typeparamref name="TTableType" />.</para>
        /// </summary>
        [Parameter] public object ItemFactory { get; set; }

        /// <inheritdoc cref="EditDialog{T}.ItemCopier"/>
        [Parameter] public object ItemCopier { get; set; }

        #endregion Outside parameters

        /// <summary>
        /// Gets or sets optional columns for displaying additional <see cref="RenderFragment"/> in table.
        /// </summary>
        /// <value>One or more <see cref="SCColumn{T}"/> objects if set, otherwise <see langword="null"/>.</value>
        [Parameter] public RenderFragment ChildContent { get; set; }

        private ColumnConfig<TTableType> SelectedGroupable { get; set; }
        private MarkupString ColumnStyles { get; set; }
        private string _selectedGroupId;
        private bool _groupingEnabled;
        private bool _filteringEnabled;
        private bool _editingEnabled;
        private bool _addingEnabled;
        private SortedDictionary<string, List<TTableType>> GroupedData { get; } = new(new WordAndNumberStringComparer());
        private readonly HashSet<ColumnConfig<TTableType>> _comparables = new();
        private readonly HashSet<ColumnConfig<TTableType>> _groupables = new();
        private readonly List<IFilter<TTableType>> _filters = new();
        private readonly List<SCColumn<TTableType>> _columns = new();

        /// <summary>
        /// Ensures that only one dialog can be displayed even when there are some serious network delays.
        /// </summary>
        private readonly SemaphoreSlim _dialogGuard = new(1);

        private DialogFacade<TTableType> _dialogFacade;

        /// <summary>
        /// Registers new custom <see cref="SCColumn{T}"/> component to this table.<br/>
        /// Typically this is done by itself by column being registered.
        /// </summary>
        /// <param name="column">A column component being registered.</param>
        public void AddColumn(SCColumn<TTableType> column) => _columns.Add(column);

        /// <summary>
        /// Removes given <paramref name="column"/> component from this table.<br/>
        /// Typically this is done by itself by column being registered.
        /// </summary>
        /// <param name="column">A column component being unregistered.</param>
        public void RemoveColumn(SCColumn<TTableType> column) => _columns.Remove(column);

        protected override void OnInitialized()
        {
            AssignColumnsByProperties();
            EnableAvailableFunctionality();
            ColumnStyles = new ColumnStyleBuilder<TTableType>().BuildUsing(Configs);
            RowStyleFunc ??= DefaultRowStyleFunc;
            _dialogFacade = new DialogFacade<TTableType>(DialogService, Configs, CultureInfo);
        }

        private void AssignColumnsByProperties()
        {
            foreach (var config in Configs)
            {
                if (CanBeComparedDirectly(config))
                    _comparables.Add(config);

                if (config.IsGroupable)
                    _groupables.Add(config);
            }
        }

        private static bool CanBeComparedDirectly(ColumnConfig<TTableType> config)
        {
            if (config.IsPresenter) return false;
            if (config.PropertyType.IsValueType) return true;

            var interfaces = config.PropertyType.GetInterfaces();
            return interfaces.Any(i => i == typeof(IComparable) || i == typeof(IComparable<>).MakeGenericType(config.PropertyType));
        }

        private Func<TTableType, dynamic> ChooseSortingAlgorithm(ColumnConfig<TTableType> config)
        {
            if (config.IsPresenter) return null;
            return _comparables.Contains(config)
                ? new Func<TTableType, dynamic>(config.GetValueFrom)
                : item => config.GetValueFrom(item)?.ToString();
        }

        private void EnableAvailableFunctionality()
        {
            _filteringEnabled = (Configs?.Any(c => c.IsFilterable) ?? false) &&
                                (ShowFilterButton is Show.Auto) || ShowFilterButton is Show.Yes;
            _groupingEnabled = (Configs?.Any(c => c.IsGroupable) ?? false) &&
                               (ShowGroupsField is Show.Auto) || ShowGroupsField is Show.Yes;
            _editingEnabled = (Configs?.Any(c => c.IsEditable) ?? false) &&
                              (ShowEditButton is Show.Auto) || ShowEditButton is Show.Yes;
            _addingEnabled = (ItemFactory is not null && ShowAddButton is Show.Auto) || ShowAddButton is Show.Yes;
        }

        protected override void OnParametersSet()
        {
            CreateGroupsBasedOn(SelectedGroupable);
        }

        private void CreateGroupsBasedOn(ColumnConfig<TTableType> selectedColumn)
        {
            SelectedGroupable = selectedColumn;

            if (selectedColumn is null || Data is null || Data.Count <= 1)
            {
                GroupedData.Clear();
                return;
            }

            // Must be done without deleting groups collections.
            // Otherwise will cause sub-table to scroll up after row click or edit.
            foreach (var group in GroupedData.Values)
            {
                group.Clear();
            }

            foreach (var item in FilterDataSource(Data))
            {
                string key = selectedColumn.Converter is null
                    ? selectedColumn.GetValueFrom(item)?.ToString()
                    : selectedColumn.Converter.SetFunc(selectedColumn.GetValueFrom(item));
                key ??= "No value";

                if (GroupedData.TryGetValue(key, out var collection))
                    collection.Add(item);
                else
                    GroupedData.Add(key, new List<TTableType> { item });
            }

            var emptyKeys = GroupedData.Where(d => d.Value.Count == 0)
                .Select(d => d.Key)
                .ToList();
            emptyKeys.ForEach(k => GroupedData.Remove(k));
        }

        private IEnumerable<TTableType> FilterDataSource(IEnumerable<TTableType> data)
        {
            return _filters.Count == 0 ? data : data.Filter(_filters);
        }

        private MarkupString FormatOutput(ColumnConfig<TTableType> config, TTableType context)
        {
            return config.GetValueFrom(context) switch
            {
                var v when config.Converter is not null => new MarkupString(config.Converter.SetFunc(v)),
                null => new MarkupString(),
                var v and (DateTime or DateTimeOffset) => config.FieldType switch
                {
                    FieldType.AutoDetect => new MarkupString(v.ToString("f", CultureInfo)),
                    FieldType.Date => new MarkupString(v.ToString(CultureInfo.DateTimeFormat.ShortDatePattern)),
                    FieldType.Time => new MarkupString(v.ToString("t", CultureInfo)),
                    FieldType.DateAndTime => new MarkupString(v.ToString("g", CultureInfo)),
                    FieldType.PlainText => new MarkupString(v.ToString()),
                    _ => throw new ArgumentOutOfRangeException(nameof(FieldType), $"Unknown value of {nameof(FieldType)} was used.")
                },
                var v and TimeSpan when config.FieldType is FieldType.Time or FieldType.AutoDetect => new MarkupString(v.ToString("t", CultureInfo)),
                var v => new MarkupString(v.ToString())
            };
        }

        private void OnGroupRowClick(TableRowClickEventArgs<KeyValuePair<string, List<TTableType>>> args)
        {
            _selectedGroupId = _selectedGroupId == args.Item.Key ? null : args.Item.Key;
        }

        private async Task SelectedItemHasChangedHandler(TTableType item)
        {
            try
            {
                await _dialogGuard.WaitAsync();
                SelectedItem = item;
                await SelectedItemChanged.InvokeAsync(item);
            }
            finally
            {
                _dialogGuard.Release();
            }
        }

        private async Task OnRowClick(TableRowClickEventArgs<TTableType> args)
        {
            try
            {
                if (_dialogGuard.CurrentCount == 0) return;
                await _dialogGuard.WaitAsync();

                if (EditOnRowClick)
                {
                    await OpenEditItemDialog();
                    return;
                }

                // If not editing by row click, then another click on the same row will deselect item.
                SelectedItem = SelectedItem is null ? args.Item : default;
            }
            finally
            {
                _dialogGuard.Release();
            }
        }

        /// <summary>
        /// Opens editing dialog which context is currently selected item, if this function is enabled.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task OpenEditItemDialog()
        {
            if (_editingEnabled is false || ReadOnly) return;

            var result = await _dialogFacade.ShowEditDialog(EditDialogStartsExpanded, MaxDialogContentHeight,
                EditDialogInvalidFieldsStartExpanded, SelectedItem);
            if (result.Cancelled)
                return;

            await OnEditItemHandler((TTableType)result.Data);
        }

        private async Task OnEditItemHandler(TTableType editedItem)
        {
            var oldItemIndex = Data.FindIndex(tt => tt.Equals(SelectedItem));
            if (oldItemIndex == -1) return;

            var oldItem = Data[oldItemIndex];
            Data[oldItemIndex] = editedItem;
            await Task.WhenAll(ItemHasBeenEdited.InvokeAsync((oldItem, editedItem)), SelectedItemChanged.InvokeAsync(editedItem));
        }

        /// <summary>
        /// Opens up 'Add new item' dialog if this function is enabled.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task OpenAddItemDialog()
        {
            try
            {
                if (_addingEnabled is false || ReadOnly) return;
                if (_dialogGuard.CurrentCount == 0) return;
                await _dialogGuard.WaitAsync();

                var result = await _dialogFacade.ShowAddDialog(MaxDialogContentHeight, ItemFactory);
                if (result.Cancelled)
                    return;
                await OnAddNewItemHandler((TTableType)result.Data);
            }
            finally
            {
                _dialogGuard.Release();
            }
        }

        private Task OnAddNewItemHandler(TTableType item)
        {
            Data.Add(item);
            return ItemCreated.InvokeAsync(item);
        }

        /// <summary>
        /// Opens table filtering dialog.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        public async Task OpenFilterItemDialog()
        {
            try
            {
                if (_dialogGuard.CurrentCount == 0) return;
                await _dialogGuard.WaitAsync();
                var result = await _dialogFacade.ShowFilterDialog(FilterDialogStartsExpanded, MaxDialogContentHeight);
                if (result.Cancelled)
                    return;
                _filters.AddRange(result.Data as IEnumerable<IFilter<TTableType>> ??
                                  Enumerable.Empty<IFilter<TTableType>>());

                // This de-selects item after filters are applied and triggers regrouping
                SelectedItem = default;
                await SelectedItemChanged.InvokeAsync();
            }
            finally
            {
                _dialogGuard.Release();
            }
        }

        /// <summary>
        /// Clears applied content filters.
        /// </summary>
        public void RemoveFilters()
        {
            _filters.Clear();
            CreateGroupsBasedOn(SelectedGroupable);
        }

        private string CalculateHeight(int rows = 0, bool isNested = false)
        {
            // No height set
            if (Height < 1)
                return null;

            // non-nested table is either unlimited or always set to given MaxHeight
            if (isNested is false)
                return Height < 1 ? null : Height + "px";

            // rows in group * row height + header height and some
            var theoreticalSize = (rows * 38) + 120;

            // 70% for nested table, so user can grab below nested table and drag to other groupings
            return (theoreticalSize > Height * 0.7 ? Height * 0.7 : theoreticalSize) + "px";
        }

        private string DefaultRowStyleFunc(TTableType rowValue, int rowNumber)
        {
            return MultiSelection switch
            {
                true => SelectedItems?.Contains(rowValue) ?? false,
                false => SelectedItem?.Equals(rowValue) ?? false
            } ? Globals.Gui.SelectedTableRowStyle : string.Empty;
        }
    }
}