using System.Collections;
using System.Collections.Generic;
using GetImageFromBroswer;
using UnityEngine;
using UnityEngine.UI;

public class ImageView : MonoBehaviour
{
    public RawImage Icon;
    public ImageProvider ImageProvider;

    void Start()
    {
        ImageProvider.LoadedImage += LoadedImage;
    }

    private void OnDestroy()
    {
        if(ImageProvider != null)
            ImageProvider.LoadedImage -= LoadedImage;
    }

    private void LoadedImage(Texture2D texture)
    {
        SetTexture(texture);
    }

    private void SetTexture(Texture2D texture) =>
        Icon.texture = texture;
}
