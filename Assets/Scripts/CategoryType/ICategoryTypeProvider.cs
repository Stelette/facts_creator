using System;

public interface ICategoryTypeProvider
{
    event Action<CategoryType> OnChanged;

    CategoryType GetCategoryType();

    void Init();
    void Cleanup();
}