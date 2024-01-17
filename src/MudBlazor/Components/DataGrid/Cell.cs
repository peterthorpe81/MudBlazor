﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /*public class Cell
    {
        public static Cell<T> Create<T>(MudDataGrid<T> dataGrid, Column<T> column, T item)
        {
            return new Cell<T>(dataGrid, column, item);
        }
    }*/

    public class Cell<T>
    {
        private readonly MudDataGrid<T> _dataGrid;
        private readonly Column<T> _column;
        internal T _item;
        internal string? _valueString;
        internal double? _valueNumber;
        internal bool _isEditing;

        #region Computed Properties

        public object? ComputedValue
        {
            get
            {
                return _column.CellContent(_item);
            }
        }              
        
        public CellContext<T> CellContext { get; internal set; }

        public string ComputedClass
        {
            get
            {
                return new CssBuilder(_column.CellClassFunc?.Invoke(_item))
                    .AddClass(_column.CellClass)
                    .AddClass("mud-table-cell")
                    .AddClass("mud-table-cell-hide", _column.HideSmall)
                    .AddClass("sticky-left", _column.StickyLeft)
                    .AddClass("sticky-right", _column.StickyRight)
                    .AddClass($"edit-mode-cell", _dataGrid.EditMode == DataGridEditMode.Cell && _column.IsEditable)
                    .Build();
            }
        }

        public string ComputedStyle
        {
            get
            {
                return new StyleBuilder()
                    .AddStyle(_column.CellStyleFunc?.Invoke(_item))
                    .AddStyle(_column.CellStyle)
                    .Build();
            }
        }

        #endregion

        public Cell(MudDataGrid<T> dataGrid, Column<T> column, T item)
        {
            _dataGrid = dataGrid;
            _column = column;
            _item = item;

            OnStartedEditingItem();

            // Create the CellContext
            CellContext = new CellContext<T>(_dataGrid, _item);
        }

        public async Task StringValueChangedAsync(string value)
        {
            _column.SetProperty(_item, value);

            // If the edit mode is Cell, we update immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        public async Task NumberValueChangedAsync(double? value)
        {
            _column.SetProperty(_item, value);

            // If the edit mode is Cell, we update immediately.
            if (_dataGrid.EditMode == DataGridEditMode.Cell)
                await _dataGrid.CommitItemChangesAsync(_item);
        }

        private void OnStartedEditingItem()
        {
            if (ComputedValue is null)
            {
                return;
            }

            if (ComputedValue is JsonElement element)
            {
                if (_column.dataType == typeof(string))
                {
                    _valueString = element.GetString();
                }
                else if (_column.isNumber)
                {
                    _valueNumber = element.GetDouble();
                }
            }
            else
            {
                if (_column.dataType == typeof(string))
                {
                    _valueString = (string)ComputedValue;
                }
                else if (_column.isNumber)
                {
                    _valueNumber = Convert.ToDouble(ComputedValue);
                }
            }
        }
    }
}
