using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using UnityEngine.Networking;

public class DownloadingImage : MonoBehaviour
{
    private readonly string imageURL = "https://upload.wikimedia.org/wikipedia/commons/thumb/b/b3/Wikipedia-logo-v2-en.svg/1200px-Wikipedia-logo-v2-en.svg.png";
    private Texture2D texture;

    async void Start()
    {
        Image ImageComponent = GetComponent<Image>();
        Material material = ImageComponent.material;

        texture = await GetRemoteTexture(imageURL); //download image and set it to texture

        material.mainTexture = texture;     //display image via material
        ImageComponent.material = material;
    }

    public static async Task<Texture2D> GetRemoteTexture(string url)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            var asyncOp = www.SendWebRequest();     //begin requenst

            while (asyncOp.isDone == false)     //await until it's done
            {
                await Task.Delay(1000 / 30);
            }

            if (www.isNetworkError || www.isHttpError)      //read results
            {
#if DEBUG
                Debug.Log($"{ www.error }, URL:{ www.url }");
#endif

                return null;        //nothing to return on error
            }
            else
            {
#if DEBUG
                Debug.Log("Download success");
#endif

                return DownloadHandlerTexture.GetContent(www);
            }
        }
    }

    void OnDestroy() => Object.Destroy(texture);// memory released, leak otherwise
}
