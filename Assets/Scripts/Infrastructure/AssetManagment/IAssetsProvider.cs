using UnityEngine;

public interface IAssetsProvider : IService
{
	public GameObject Instantiate(string path);
	public GameObject Instantiate(string path, Vector3 at);
}

