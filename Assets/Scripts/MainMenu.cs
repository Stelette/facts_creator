using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Unity Editor assigning")]

    public TMP_Dropdown CategoryDropdown;
    public TMP_Dropdown GroupDropdown;

    private IGameStateMachine _stateMachine;
    private IGroupProvider _groupProvider;
    private ICategoryProvider _categoryProvider;
    private ICategoryTypeProvider _categoryTypeProvider;

    private bool isReady = false;

    public void Construct(IGameStateMachine stateMachine, ICategoryProvider categoryProvider,IGroupProvider groupProvider, ICategoryTypeProvider categoryTypeProvider)
    {
        _stateMachine = stateMachine;
        _categoryProvider = categoryProvider;
        _groupProvider = groupProvider;
        _categoryTypeProvider = categoryTypeProvider;
    }

    private void OnDestroy()
    {
        if(_categoryTypeProvider != null)
        {
            _categoryTypeProvider.Cleanup();
            _categoryTypeProvider.OnChanged -= ChangedCategoryType;
        }

        CategoryDropdown.onValueChanged.RemoveAllListeners();
    }

    public void Init()
    {
        Subscribers();
        ChangedCategoryType(CategoryType.Educational);
        isReady = true;
    }

    public void Refresh()
    {
        if (!isReady)
            return;

        ChangedCategoryType(_categoryTypeProvider.GetCategoryType());
    }

    public void MoveCreateGroupState()
    {
        if (!isReady)
            return;
        CategoryInfoWrapper providersWrapper = new CategoryInfoWrapper(GetCurrentCategory(), _categoryTypeProvider.GetCategoryType());
        _stateMachine.Enter<GroupCreatorState, CategoryInfoWrapper>(providersWrapper);
    }

    public void MoveCreateSubCategoryState()
    {
        if (!isReady)
            return;
        _stateMachine.Enter<SubCategoryCreatorState,string>(GetCurrentCategory());
    }

    public void MoveCreateCategoryState()
    {
        if (!isReady)
            return;
        _stateMachine.Enter<CategoryCreatorState>();
    }

    private void Subscribers()
    {
        _categoryTypeProvider.OnChanged += ChangedCategoryType;
        _categoryTypeProvider.Init();

        CategoryDropdown.onValueChanged.AddListener( (index) =>
        {
            RefreshGroups();
        });
    }

    private async void ChangedCategoryType(CategoryType categoryType)
    {
        await RefreshCategories(categoryType);
        await RefreshGroups();
    }

    private async Task RefreshCategories(CategoryType categoryType)
    {
        ClearCategoryDropDown();
        await LoadCategories(categoryType);
        UpdateCategoryDropDown();
    }

    private async Task RefreshGroups()
    {
        ClearGroupDropDown();
        await LoadGroups(GetCurrentCategory());
        UpdateGroupDropDown();
    }

    private async Task LoadCategories(CategoryType categoryType)
    {
        await _categoryProvider.LoadAsync(categoryType);
        Debug.Log(string.Join("&", _categoryProvider.GetCategories()));
    }

    private async Task LoadGroups(string category)
    {
        await _groupProvider.LoadAsync(category);
        Debug.Log(string.Join("&", _groupProvider.GetGroups()));
    }

    private string GetCurrentCategory() =>
        CategoryDropdown.options[CategoryDropdown.value].text;

    private void ClearCategoryDropDown() =>
        CategoryDropdown.ClearOptions();

    private void UpdateCategoryDropDown() =>
        CategoryDropdown.AddOptions(_categoryProvider.GetCategories());

    private string GetCurrentGroup() =>
        GroupDropdown.options[GroupDropdown.value].text;

    private void ClearGroupDropDown() =>
        GroupDropdown.ClearOptions();

    private void UpdateGroupDropDown() =>
        GroupDropdown.AddOptions(_groupProvider.GetGroups());
}
