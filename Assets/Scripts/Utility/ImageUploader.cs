using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ImageType
{
    PNG,
    JPG
}

public class ImageUploader
{
    private readonly ICoroutineRunner _coroutineRunner;

    private Texture2D texture;
    private string url;
    private string fieldName;
    private string fileName = "defaultFileName";
    ImageType imageType = ImageType.PNG;

    Action<string> OnErrorAction;
    Action<string> OnCompleteAction;

    public ImageUploader(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public ImageUploader SetUrl(string url)
    {
        this.url = url;
        return this;
    }

    public ImageUploader SetFieldName(string fieldName)
    {
        this.fieldName = fieldName;
        return this;
    }

    public ImageUploader SetFileName(string fileName)
    {
        this.fileName = fileName;
        return this;
    }

    public ImageUploader SetTexture(Texture2D texture)
    {
        this.texture = texture;
        return this;
    }

    public ImageUploader SetImageType(ImageType imageType)
    {
        this.imageType = imageType;
        return this;
    }

    public ImageUploader OnError(Action<string> errorCallback)
    {
        this.OnErrorAction = errorCallback;
        return this;
    }

    public ImageUploader OnComplete(Action<string> completeCallback)
    {
        this.OnCompleteAction = completeCallback;
        return this;
    }

    public void Upload()
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("ImageUploader::assign url using SetUrl ( url )");
        }
        if (string.IsNullOrEmpty(fieldName))
        {
            Debug.LogError("ImageUploader::assign fieldName using SetFieldName ( fieldName )");
        }

        if(texture == null)
        {
            Debug.LogError("ImageUploader::assign texture using SetTexture ( texture )");
        }
        _coroutineRunner.StartCoroutine(StartUploading());
    }


    IEnumerator StartUploading()
    {
        Debug.Log("before GetCopyTexture() size: width: " + texture.width + " height: " + texture.height);
        Texture2D tex = GetCopyTexture(texture);
        Debug.Log("after GetCopyTexture() size: width: " + tex.width + " height: " + tex.height);
        byte[] textureBytes = null;
        WWWForm form = new WWWForm();

        switch (imageType)
        {
            case ImageType.PNG:
                textureBytes = tex.EncodeToPNG();
                break;
            case ImageType.JPG:
                textureBytes = tex.EncodeToJPG();
                break;
        }

        string extension = imageType.ToString().ToLower();

        form.AddBinaryData(fieldName, textureBytes, fileName + "." + extension, "image/" + extension);

         GameObject.Destroy(tex);

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                OnErrorAction?.Invoke(www.error);
            }
            else
            {
                OnCompleteAction?.Invoke(www.downloadHandler.text);
            }
        }

        yield return null;
    }

    private Texture2D GetCopyTexture(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
}
