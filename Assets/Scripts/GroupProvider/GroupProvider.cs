using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class GroupProvider : IGroupProvider
{
    private List<string> _groups = new List<string>();

    private readonly INetworkService _networkService;

    public GroupProvider(INetworkService networkService)
    {
        _networkService = networkService;
    }

    public List<string> GetGroups()
    {
        return _groups;
    }

    public void Cleanup()
    {
        _groups.Clear();
    }

    public async Task LoadAsync(string category)
    {
        _groups.Clear();
        _groups.AddRange(await DownloadGroups(category));
    }

    private Task<string[]> DownloadGroups(string category)
    {
        var tcs = new TaskCompletionSource<string[]>();

        string url = AntCore.Social.RequestBuilder.GetGroups(category);

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

