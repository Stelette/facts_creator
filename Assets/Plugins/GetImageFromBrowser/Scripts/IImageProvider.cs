using System;
using UnityEngine;

public interface IImageProvider
{
    public event Action<Texture2D> LoadedImage;
    public void Choose();
    public Texture2D GetTexture();
}

