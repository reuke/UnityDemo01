using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class PicsumApi
{
    const string UserAgent = "My-User-Agent";
    const int RetryCount = int.MaxValue;
    const float RetryWait = 0.1f;
    const int Width = 640;
    const int Height = 480;


    public static IEnumerator LoadCard(Card card)
    {
        yield return new WaitForSeconds(0.05f);

        var request = UnityWebRequestTexture.GetTexture($"https://picsum.photos/{Width}/{Height}");

        var retry = -1;

        yield return request.SendWebRequest();

        while (request.result != UnityWebRequest.Result.Success && retry < RetryCount)
        {
            yield return new WaitForSeconds(RetryWait);
            retry++;
            yield return request.SendWebRequest();
        }

        card.SetTexture(((DownloadHandlerTexture)request.downloadHandler).texture);
    }
}
