using UnityEngine;

public class ResourcesAssetProvider : IAssetsProvider
{
    public GameObject Instantiate(string path)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Object.Instantiate(prefab);
    }

    public GameObject Instantiate(string path, Vector3 at)
    {
        var prefab = Resources.Load<GameObject>(path);
        return Object.Instantiate(prefab, at, Quaternion.identity);
    }
}