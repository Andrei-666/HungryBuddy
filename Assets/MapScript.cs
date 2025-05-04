using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class MapScript : MonoBehaviour
{
    public RawImage mapImage;
    public string googleMapsApiKey = "AIzaSyDj41OK_EDVYC8EUyhsRyM1TqjAfXEkGNw";
    public int zoom = 16;
    public int mapWidth = 640;
    public int mapHeight = 640;

    private float latitude;
    private float longitude;

    void Start()
    {
        StartCoroutine(UpdateMapLoop());
    }

    IEnumerator UpdateMapLoop()
    {
        while (true)
        {
#if UNITY_EDITOR
            latitude = 44.4268f;
            longitude = 26.1025f;
#else
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogError("Location not enabled.");
                yield break;
            }

            Input.location.Start();

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogError("Location service failed.");
                yield break;
            }

            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
#endif

            string url = $"https://maps.googleapis.com/maps/api/staticmap?center={latitude},{longitude}&zoom={zoom}&size={mapWidth}x{mapHeight}&markers=color:red%7C{latitude},{longitude}&key={googleMapsApiKey}";
            Debug.Log("Requesting map from URL: " + url);

            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Map downloaded successfully.");
                mapImage.texture = DownloadHandlerTexture.GetContent(www);
            }
            else
            {
                Debug.LogError("Map download failed: " + www.error);
            }

            yield return new WaitForSeconds(5f);
        }
    }
}
