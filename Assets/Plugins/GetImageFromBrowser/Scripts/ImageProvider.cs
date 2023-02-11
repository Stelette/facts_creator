using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GetImageFromBroswer
{
    public class ImageProvider : MonoBehaviour, IImageProvider
    {
        public event Action<Texture2D> LoadedImage;

        private const string CallbackFuncName = "ReceiveImage";

        private Texture2D _texture;

        public void Choose()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GetImage.GetImageFromUserAsync(gameObject.name, CallbackFuncName);
#elif UNITY_EDITOR
            string path = EditorUtility.OpenFilePanel("Choose image file", "", "png,jpg,jpeg");
            if (path.Length != 0)
            {
                if (_texture != null)
                    Destroy(_texture);
                var fileContent = File.ReadAllBytes(path);
                _texture = new Texture2D(1,1);
                _texture.LoadImage(fileContent);
                LoadedImage?.Invoke(_texture);
            }
#endif
        }

        public Texture2D GetTexture()
        {
            return _texture;
        }

        private void ReceiveImage(string dataUrl)
        {
            // Запускаем корутину для загрузки картинки
            StartCoroutine(UploadImage(dataUrl));
        }

        // Корутина для загрузки картинки
        private IEnumerator UploadImage(string path)
        {
            // Тут будет хранится текстура
            Texture2D texture;

            // using для автоматического вызова Dispose, создаем запрос по пути к файлу
            using (UnityWebRequest imageWeb = new UnityWebRequest(path, UnityWebRequest.kHttpVerbGET))
            {
                // Создаем "скачиватель" для текстур и передаем запросу
                imageWeb.downloadHandler = new DownloadHandlerTexture();

                // Отправляем запрос, выполнение продолжится после загрузки всего файла
                yield return imageWeb.SendWebRequest();

                // Получаем текстуру из "скачивателя"
                texture = ((DownloadHandlerTexture)imageWeb.downloadHandler).texture;
            }

            _texture = texture;
            LoadedImage?.Invoke(_texture);
        }

        private void OnDestroy()
        {
            if (_texture != null)
                Destroy(_texture);
        }
    }
}
