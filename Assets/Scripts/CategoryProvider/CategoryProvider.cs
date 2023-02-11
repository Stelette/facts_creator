using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class CategoryProvider : ICategoryProvider
{
    private List<string> _categories = new List<string>();

    private readonly INetworkService _networkService;

    public CategoryProvider(INetworkService networkService)
    {
        _networkService = networkService;
    }

    public List<string> GetCategories()
    {
        return _categories;
    }

    public async Task LoadAsync(CategoryType categoryType)
    {
        _categories.Clear();
        _categories.AddRange(await DownloadCategories(categoryType));
    }

    public void Cleanup()
    {
        _categories.Clear();
    }

    private Task<string[]> DownloadCategories(CategoryType categoryType)
    {
        var tcs = new TaskCompletionSource<string[]>();

        string url = AntCore.Social.RequestBuilder.GetCategories(categoryType);

        Debug.Log("url: " + url);

        Action<string> ResultCallback = (info) =>
        {
            if (!string.IsNullOrEmpty(info))
            {
                try
                {
                    string[] categories = JsonConvert.DeserializeObject<string[]>(info);
                    tcs.TrySetResult(categories);
                }
                catch (Exception ex)
                {
                    Debug.Log("GetCategories parse error: " + ex);
                    tcs.TrySetResult(null);
                }
            }
            else
            {
                tcs.TrySetResult(null);
            }
        };

        _networkService.SendGetRequest(url, ResultCallback);
        return tcs.Task;
    }
}