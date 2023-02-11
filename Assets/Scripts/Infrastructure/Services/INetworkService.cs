using System;

public interface INetworkService : IService
{
    void SendGetRequest(string url, Action<string> responseCallback);
}