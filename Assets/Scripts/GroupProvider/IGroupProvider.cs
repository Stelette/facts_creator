using System.Collections.Generic;
using System.Threading.Tasks;

public interface IGroupProvider
{
    List<string> GetGroups();
    Task LoadAsync(string category);
    void Cleanup();
}

