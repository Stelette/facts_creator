using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using AntCore.Social;
using UnityEngine.Networking;
using System.Text;
using System;
using System.Threading.Tasks;
using UnityEngine.Events;

public class CategoryCreator : MonoBehaviour
{
    private const string FieldName = "thumbnail";
    private const string FileName = "thumbnail";

    private const int SizeThumbnail = 256;

    [SerializeField]
    private TMP_InputField NameRuInputText;

    [SerializeField]
    private TMP_InputField NameEnInputText;

    private IImageProvider _imageProvider;
    private INetworkService _networkService;
    private ICategoryTypeProvider _categoryTypeProvider;
    private IGameStateMachine _stateMachine;
    private ICoroutineRunner _coroutineRunner;

    public void Construct(IGameStateMachine stateMachine,ICoroutineRunner coroutineRunner, INetworkService networkService, IImageProvider imageProvider, ICategoryTypeProvider categoryTypeProvider)
    {
        _stateMachine = stateMachine;
        _coroutineRunner = coroutineRunner;
        _imageProvider = imageProvider;
        _networkService = networkService;
        _categoryTypeProvider = categoryTypeProvider;
    }

    public async void Save()
    {
        if (IsValidInput())
        {
            string category = NameEnInputText.text;
            bool isExistCategory = await IsExistCategory(category);
            if (isExistCategory)
            {
                Debug.Log("Such a category with the same name already exists, create another one!");
                return;
            }
            else
            {
                Texture2D thumbnail = CreateThumbnail();

                bool result = await UploadThumbnail(thumbnail);
                Debug.Log("UploadThumbnail result: " + result);
                ClearThumbnail(thumbnail);

                result = await CreateCategory(category, GetTranslateBody());
                Debug.Log("CreateCategory result: " + result);

                result = await AddCategory(category);
                Debug.Log("AddCategory result: " + result);

                _stateMachine.Enter<MainMenuState>();
            }
        }
        else
        {
            Debug.LogError("not all fields are filled in!");
        }
    }

    private void ClearThumbnail(Texture2D thumbnail)
    {
        if (thumbnail != null)
            Destroy(thumbnail);
    }

    private Texture2D CreateThumbnail()
    {
        Texture2D thumbnail = _imageProvider.GetTexture();
        return thumbnail.ManualResize(SizeThumbnail, SizeThumbnail);
    }

    private Task<bool> UploadThumbnail(Texture2D _thumbnail)
    {
        var tcs = new TaskCompletionSource<bool>();


        ImageUploader imageUploader =
                new ImageUploader(_coroutineRunner)
                .SetUrl(RequestBuilder.UploadThumbnail(GetCategoryPath()))
                .SetTexture(_thumbnail)
                .SetImageType(ImageType.PNG)
                .SetFieldName(FieldName)
                .SetFileName(FileName)
                .OnComplete((upload_url) =>
                {
                    Debug.Log("upload url: " + upload_url);
                    tcs.SetResult(true);
                })
                .OnError((error) =>
                {
                    Debug.Log("upload thumbnail error: " + error);
                    tcs.SetResult(false);
                });

        imageUploader.Upload();
        return tcs.Task;
    }

    private string GetCategoryPath()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("/")
          .Append(NameEnInputText.text)
          .Append("/");

        return UnityWebRequest.EscapeURL(sb.ToString());
    }

    private string GetTranslateBody()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("en=")
          .Append(NameEnInputText.text)
          .Append("&ru=")
          .Append(NameRuInputText.text);

        return UnityWebRequest.EscapeURL(sb.ToString());
    }

    private bool IsValidInput()
    {
        return !string.IsNullOrEmpty(NameRuInputText.text) &&
            !string.IsNullOrEmpty(NameEnInputText.text) &&
            _imageProvider.GetTexture() != null;
    }

    private Task<bool> IsExistCategory(string category)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string url = RequestBuilder.IsExistCategory(escape_category);
        //Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = request.Equals("1") ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<bool> CreateCategory(string category,string translateBody)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string url = RequestBuilder.CreateCategory(escape_category, translateBody);
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<bool> AddCategory(string category)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string url = RequestBuilder.AddCategory(escape_category, _categoryTypeProvider.GetCategoryType());
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }
}
