using UnityEngine;
using System.Collections;
using TMPro;
using System.Threading.Tasks;
using UnityEngine.Networking;
using AntCore.Social;

public class SubCategoryCreator : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField NameRuInputText;

    [SerializeField]
    private TMP_InputField NameEnInputText;

    [SerializeField]
    private TextMeshProUGUI CategoryT;

    private IGameStateMachine _gameStateMachine;
    private INetworkService _networkService;
    private ICoroutineRunner _coroutineRunner;
    private string _category;

    private bool _lockInput = false;

    public void Construct(IGameStateMachine gameStateMachine,ICoroutineRunner coroutineRunner,INetworkService networkService)
    {
        _gameStateMachine = gameStateMachine;
        _coroutineRunner = coroutineRunner;
        _networkService = networkService;
    }

    public void Init(string category)
    {
        _category = category;
        CategoryT.text = _category;
    }

    public async void Save()
    {
        if (_lockInput)
            return;

        if (IsValidInput())
        {
            _lockInput = true;
            string subCategory = NameEnInputText.text;
            bool exist = await IsExistSubCategory(_category, subCategory);
            if (exist)
            {
                Debug.Log("Such a subcategory with the same name already exists, create another one!");
                _lockInput = false;
                return;
            }

            bool result = await AddSubCategory(_category);
            _lockInput = false;
            if (result)
                Exit();
        }
    }

    public void Exit()
    {
        if (_lockInput)
            return;

        _gameStateMachine.Enter<MainMenuState>();
    }

    private bool IsValidInput()
    {
        return !string.IsNullOrEmpty(NameRuInputText.text) &&
            !string.IsNullOrEmpty(NameEnInputText.text);
    }

    private Task<bool> IsExistSubCategory(string category,string subCategory)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_subCategory = UnityWebRequest.EscapeURL(subCategory);
        string url = RequestBuilder.IsExistSubCategory(escape_category, escape_subCategory);
        //Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = request.Equals("1") ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }

    private Task<bool> AddSubCategory(string category)
    {
        var tcs = new TaskCompletionSource<bool>();

        string escape_category = UnityWebRequest.EscapeURL(category);
        string escape_name = UnityWebRequest.EscapeURL(NameEnInputText.text);
        string escape_translateName = UnityWebRequest.EscapeURL(NameRuInputText.text);
        string url = RequestBuilder.AddSubCategory(escape_name, escape_translateName,escape_category);
        Debug.Log("url: " + url);
        _networkService.SendGetRequest(url, (request) =>
        {
            bool result = string.IsNullOrEmpty(request) ? true : false;
            tcs.SetResult(result);
        });
        return tcs.Task;
    }
}

