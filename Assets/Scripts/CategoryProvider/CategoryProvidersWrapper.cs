using System;

public class CategoryInfoWrapper
{
    public string Category;
    public CategoryType CategoryType;


    public CategoryInfoWrapper(string Category, CategoryType CategoryType)
    {
        this.Category = Category;
        this.CategoryType = CategoryType;
    }
}
