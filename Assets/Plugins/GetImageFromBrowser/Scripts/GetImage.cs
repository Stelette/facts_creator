using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace GetImageFromBroswer
{

public class GetImage
{

#if UNITY_WEBGL

        [DllImport("__Internal")]
        private static extern void getImageFromBrowser(string objectName, string callbackFuncName);

#endif

    static public void GetImageFromUserAsync(string objectName, string callbackFuncName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR

            getImageFromBrowser(objectName, callbackFuncName);
#else

        Debug.LogError("Not implemented in this platform");

#endif
    }
}

}