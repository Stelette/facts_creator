using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntCore.Social;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GroupCreator : MonoBehaviour
{
    private const string FieldName = "thumbnail";
    private const string FileName = "thumbnail";
    private const int SizeThumbnailGroup = 256;
    private const int SizeThumbnailPreview = 1024;

    [Header("Name inputs")]
    [SerializeField]
    private TMP_InputField NameRuInputText;

    [SerializeField]
    private TMP_InputField NameEnInputText;

    [Header("Author inputs")]
    [SerializeField]
    private TMP_InputField AuthorRuInputText;

    [SerializeField]
    private TMP_InputField AuthorEnInputText;

    [Header("Category zone")]
    [SerializeField]
    private TextMeshProUGUI CategoryTypeT;

    [SerializeField]
    private TextMeshProUGUI CategoryT;

    [SerializeField]
    private TMP_Dropdown SubCategoryDropdown;


    private IGameStateMachine _gameStateMachine;
    private IImageProvider _imageProvider;
    private INetworkService _networkService;
    private ICoroutineRunner _coroutineRunner;

    private bool _lockInput = false;
    private string _category;
    private CategoryType _categoryType;

    public void Construct(IGameStateMachine stateMachine,ICoroutineRunner coroutineRunner, IImageProvider imageProvider, INetworkService networkService, CategoryInfoWrapper categoryWrapper)
    {
        _gameStateMachine = stateMachine;
        _coroutineRunner = coroutineRunner;
        _imageProvider = imageProvider;
        _networkService = networkService;

        _category = categoryWrapper.Category;
        _categoryType = categoryWrapper.CategoryType;
    }

    public void Init()
    {
        UpdateUI();
    }

    public async void Save()
    {
        if (_lockInput)
            return;
        if (IsValidInput())
        {
            _lockInput = true;
            string group = GetGroup();
            bool isExist = await IsExistGroup(_category, group);
            if (isExist)
            {
                Debug.Log("Such a group with the same name already exists, create another one!");
                _lockInput = false;
                return;
            }
            else
            {
                bool result = false;
                result = await UploadThumbnails();
                if (result)
                    Debug.Log("UploadThumbnails succesfull");

                result = await CreateGroup(_category, group);
                if (result)
                    Debug.Log("CreateGroup succesfull");

                result = await CreateTranslateInfo(GetTranslateGroupNameBody(), _category, group);
                if (result)
                    Debug.Log("CreateTranslateInfo succesfull");

                result = await CreateAuthorsInfo(GetTranslateAuthorsGroupBody(), _category, group);
                if (result)
                    Debug.Log("CreateAuthorsInfo succesfull");

                string subcategory = GetCurrentSubCategory();
                result = await AddGroupInSubCategory(_category, subcategory, group);
                if (result)
                    Debug.Log("AddGroupInSubCategory succesfull");

                _lockInput = false;
                
                Exit();
            }

        }
    }

    public void Exit()
    {
        if (_lockInput)
            return;

        _gameStateMachine.Enter<MainMenuState>();
    }

    private void UpdateUI()
    {
        CategoryTypeT.text = _categoryType.ToString();
        CategoryT.text = _category;
        RefreshDropDown();
    }

    private async void RefreshDropDown()
    {
        ClearDropDown();
        UpdateDropDown(await LoadSubCategories(_category));
    }

    private void ClearThumbnail(Texture2D thumbnail)
    {
        if (thumbnail != null)
            Destroy(thumbnail);
    }

    private Texture2D CreateThumbnail(int sizeThumbnail)
    {
        Texture2D thumbnail = _imageProvider.GetTexture();
        return thumbnail.ManualResize(sizeThumbnail, sizeThumbnail);
    }


    private async Task<bool> UploadThumbnails()
    {
        Texture2D thumbnail = CreateThumbnail(SizeThumbnailGroup);

        bool result = await UploadThumbnail(thumbnail, GetGroupPath());
        Debug.Log("UploadThumbnail result: " + result);
        ClearThumbnail(thumbnail);

        thumbnail = CreateThumbnail(SizeThumbnailPreview);

        result = await UploadThumbnail(thumbnail, GetGroupPreviewPath());
        Debug.Log("UploadThumbnail result: " + result);
        ClearThumbnail(thumbnail);
        return result;
    }

    private string GetGroupPath()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(_category)
          .Append("/")
          .Append(GetGroup())
          .Append("/");

        return UnityWebRequest.EscapeURL(sb.ToString());
    }

    private string GetGroupPreviewPath()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(_category)
          .Append("/")
          .Append(GetGroup())
          .Append("/Preview/");

        return UnityWebRequest.EscapeURL(sb.ToString());
    }

    private Task<bool> UploadThumbnail(Texture2D _thumbnail,string innerPath)
    {
        var tcs = new TaskCompletionSource<bool>();


        ImageUploader imageUploader =
                new ImageUploader(_coroutineRunner)
                .SetUrl(RequestBuilder.UploadThumbnail(innerPath))
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

    #region validInput

    private bool IsValidInput()
    {
        return IsValidNames() &&
            IsValidAuthors() &&
            _imageProvider.GetTexture() != null &&
            !string.IsNullOrEmpty(GetCurrentSubCategory());
    }

    private bool IsValidNames()
    {
        return !string.IsNullOrEmpty(NameRuInputText.text) &&
            !string.IsNullOrEmpty(NameEnInputText.text);
    }

    private bool IsValidAuthors()
    {
        return !string.IsNullOrEmpty(AuthorRuInputText.text) &&
            !string.IsNullOrEmpty(AuthorEnInputText.text);
    }

    #endregion

    private Task<bool> IsExistGroup(string category,string group)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_group = UnityWebRequest.EscapeURL(group);
        string url = RequestBuilder.IsExistGroup(escape_category,escape_group);
        //Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = request.Equals("1") ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<bool> CreateGroup(string category, string group)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_group = UnityWebRequest.EscapeURL(group);
        string url = RequestBuilder.CreateGroup(escape_category, escape_group);
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<bool> CreateTranslateInfo(string body, string category, string group)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_group = UnityWebRequest.EscapeURL(group);
        string url = RequestBuilder.CreateTranslateInfoGroup(body, escape_category, escape_group);
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<bool> CreateAuthorsInfo(string body, string category, string group)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_group = UnityWebRequest.EscapeURL(group);
        string url = RequestBuilder.CreateAuthorsGroup(body, escape_category, escape_group);
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<List<string>> LoadSubCategories(string category)
    {
        var tcs = new TaskCompletionSource<List<string>>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string url = RequestBuilder.GetSubCategories(escape_category);
        //Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            List<string> subcategories = JsonConvert.DeserializeObject<string[]>(request).ToList();
            tcs.TrySetResult(subcategories);
        });
        return tcs.Task;
    }

    private Task<bool> AddGroupInSubCategory(string category,string subcategory, string group)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_subcategory = UnityWebRequest.EscapeURL(subcategory);
        string escape_group = UnityWebRequest.EscapeURL(group);
        string url = RequestBuilder.AddGroupInSubCategory(escape_category, escape_subcategory, escape_group);
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private string GetTranslateGroupNameBody()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("en=")
          .Append(NameEnInputText.text)
          .Append("&ru=")
          .Append(NameRuInputText.text);

        return UnityWebRequest.EscapeURL(sb.ToString());
    }

    private string GetTranslateAuthorsGroupBody()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("en=")
          .Append(AuthorEnInputText.text)
          .Append("&ru=")
          .Append(AuthorRuInputText.text);

        return UnityWebRequest.EscapeURL(sb.ToString());
    }


    private string GetCurrentSubCategory() =>
    SubCategoryDropdown.options[SubCategoryDropdown.value].text;

    private void ClearDropDown() =>
        SubCategoryDropdown.ClearOptions();

    private void UpdateDropDown(List<string> subgroups) =>
        SubCategoryDropdown.AddOptions(subgroups);

    private string GetGroup() =>
        NameEnInputText.text;
}
