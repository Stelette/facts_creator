using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CategoryTypeProvider : MonoBehaviour, ICategoryTypeProvider
{
    public event Action<CategoryType> OnChanged;

    [SerializeField]
    private Toggle educationalTypeToggle;

    [SerializeField]
    private Toggle customTypeToggle;

    [SerializeField]
    private Toggle entertainmentTypeToggle;

    private CategoryType _categoryType = CategoryType.Educational;

    public CategoryType GetCategoryType()
    {
        return _categoryType;
    }

    public void Init()
    {
        SubscribeToggles();
    }

    public void Cleanup()
    {
        UnSubscrube(educationalTypeToggle);
        UnSubscrube(customTypeToggle);
        UnSubscrube(entertainmentTypeToggle);
    }

    private void SubscribeToggles()
    {
        Subscribe(educationalTypeToggle, CategoryType.Educational);
        Subscribe(customTypeToggle, CategoryType.Custom);
        Subscribe(entertainmentTypeToggle, CategoryType.Entertainment);
    }

    private void Subscribe(Toggle toggle, CategoryType categoryType)
    {
        toggle.onValueChanged.AddListener((isOn) =>
        {
            if (isOn)
            {
                _categoryType = categoryType;
                OnChanged?.Invoke(_categoryType);
            }
        });
    }

    private void UnSubscrube(Toggle toggle) =>
        toggle.onValueChanged.RemoveAllListeners();
}

