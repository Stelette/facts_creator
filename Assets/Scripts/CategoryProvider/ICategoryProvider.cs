using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICategoryProvider
{
    List<string> GetCategories();
    Task LoadAsync(CategoryType categoryType);
    void Cleanup();
}