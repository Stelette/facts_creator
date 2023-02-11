using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class NetworkService : INetworkService
{
    private readonly ICoroutineRunner _coroutineRunner;

    public NetworkService(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void SendGetRequest(string url, Action<string> responseCallback)
    {
        _coroutineRunner.StartCoroutine(SendGetRequestI(url, responseCallback));
    }

    private IEnumerator SendGetRequestI(string url, Action<string> callback)
    {
        using (UnityWebRequest send = UnityWebRequest.Get(url))
        {
            yield return send.SendWebRequest();
            if (send.isDone)
            {
                switch (send.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        callback?.Invoke("");
                        break;

                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError("Error: " + send.error);
                        callback?.Invoke("");
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError("HTTP Error: " + send.error);
                        callback?.Invoke("");
                        break;

                    case UnityWebRequest.Result.Success:
                        callback?.Invoke(send.downloadHandler.text);
                        break;
                }
            }
            else
                callback?.Invoke("");
        }
    }

}

