using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PictureController : MonoBehaviour
{
    [SerializeField]
    private Kakera.Unimgpicker imagePicker;

    [SerializeField]
    private RawImage image;

    private int[] sizes = { 1024, 256, 16 };

    void Awake()
    {
        imagePicker.Completed += (string path) =>
        { StartCoroutine(LoadImage(path, image)); };
    }

    public void OnPressShowPicker()
    {
        imagePicker.Show("Select Image", "image picker");
    }

    private IEnumerator LoadImage(string path, RawImage output)
    {
        var url = "file://" + path;
        var unityWebRequestTexture = UnityWebRequestTexture.GetTexture(url);
        yield return unityWebRequestTexture.SendWebRequest();

        var texture = ((DownloadHandlerTexture)unityWebRequestTexture.downloadHandler).texture;
        if (texture == null)
        {
            Debug.LogError("Failed to load texture url:" + url);
        }

        output.texture = texture;

        StartCoroutine(UploadImages(texture));
    }

    private IEnumerator UploadImages(Texture2D texture) 
    {
        var bytes = texture.EncodeToPNG();
        var form = new WWWForm();
        form.AddField("id", "upload");
        form.AddBinaryData("image", bytes, "person.png", "image/png");
        
        using(var unityWebRequest = UnityWebRequest.Post("http://localhost:3000/uploads", form))
        {
            unityWebRequest.SetRequestHeader("Authorization", "Token codemaker2015");
        
            yield return unityWebRequest.SendWebRequest();
        
            if (unityWebRequest.result != UnityWebRequest.Result.Success) 
            {
                print($"Failed to upload image: {unityWebRequest.result} - {unityWebRequest.error}");
            }
            else 
            {
                print($"Finished Uploading");
            }
        }
    }
}