﻿using AntDesign.TableModels;
using CuratorMagazineBlazorApp.Data.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using ITable = AntDesign.ITable;

namespace CuratorMagazineBlazorApp.Pages.Admin.Role.Parent;

/// <summary>
/// Class MenuItemParent.
/// Implements the <see cref="ComponentBase" />
/// </summary>
/// <seealso cref="ComponentBase" />
public partial class MenuItemParent
{
    /// <summary>
    /// Gets or sets the parent service.
    /// </summary>
    /// <value>The parent service.</value>
    [Inject]
    private ParentService? ParentService { get; set; }

    /// <summary>
    /// The parents
    /// </summary>
    private List<CuratorMagazineWebAPI.Models.Entities.Domains.Parent>? _parents;

    /// <summary>
    /// The selected rows
    /// </summary>
    private IEnumerable<CuratorMagazineWebAPI.Models.Entities.Domains.Parent>? _selectedRows;

    /// <summary>
    /// The table
    /// </summary>
    private ITable? _table;

    /// <summary>
    /// The edit cache
    /// </summary>
    private IDictionary<string, (bool edit, CuratorMagazineWebAPI.Models.Entities.Domains.Parent data)> _editCache = new Dictionary<string, (bool edit, CuratorMagazineWebAPI.Models.Entities.Domains.Parent data)>();

    /// <summary>
    /// The page index
    /// </summary>
    int _pageIndex = 1;

    /// <summary>
    /// The page size
    /// </summary>
    int _pageSize = 10;

    /// <summary>
    /// The total
    /// </summary>
    int _total = 0;

    /// <summary>
    /// The i
    /// </summary>
    int i = 0;

    /// <summary>
    /// The edit identifier
    /// </summary>
    private string? _editId;

    /// <summary>
    /// On initialized as an asynchronous operation.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    protected override async Task OnInitializedAsync()
    {
        var ret = await ParentService?.PostAsync()!;
        _parents = JsonConvert.DeserializeObject<List<CuratorMagazineWebAPI.Models.Entities.Domains.Parent>>(ret.Result.Items?.ToString() ?? string.Empty);

        _parents?.ForEach(item =>
        {
            _editCache[item.Id.ToString()] = (false, item);
        });

        if (_parents != null) _total = _parents.Count;
    }

    /// <summary>
    /// Starts the edit.
    /// </summary>
    /// <param name="id">The identifier.</param>
    private void StartEdit(string id)
    {
        var data = _editCache[id];
        _editCache[id] = (true, data.data);
    }

    /// <summary>
    /// Cancels the edit.
    /// </summary>
    /// <param name="id">The identifier.</param>
    private void CancelEdit(string id)
    {
        if (_parents != null)
        {
            var data = _parents.FirstOrDefault(item => item.Id == Convert.ToInt32(id));
            _editCache[id] = (false, data)!;
        }
    }

    /// <summary>
    /// Saves the edit.
    /// </summary>
    /// <param name="id">The identifier.</param>
    private async void SaveEdit(string id)
    {
        if (_parents != null)
        {
            var index = _parents.FindIndex(item => item.Id == Convert.ToInt32(id));
            _parents[index] = _editCache[id].data;
            _editCache[id] = (false, _parents[index]);
        }

        var ret = await ParentService?.PutAsync(_editCache[id].data)!;

        Console.WriteLine(ret.Success ? $"{_editCache[id].data} is updated!" : ret.Error);
    }

    /// <summary>
    /// Called when [change].
    /// </summary>
    /// <param name="queryModel">The query model.</param>
    public async Task OnChange(QueryModel<CuratorMagazineWebAPI.Models.Entities.Domains.Parent> queryModel)
    {
        Console.WriteLine(JsonConvert.SerializeObject(queryModel));
    }

    /// <summary>
    /// Removes the selection.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void RemoveSelection(int id)
    {
        if (_selectedRows != null)
        {
            var selected = _selectedRows.Where(x => x.Id != id);
            _selectedRows = selected;
        }
    }

    /// <summary>
    /// Deletes the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    private void Delete(int id)
    {
        if (_parents != null)
        {
            _parents = _parents.Where(x => x.Id != id).ToList();
            _total = _parents.Count;
        }
    }
}