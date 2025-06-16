

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MapManager : MonoBehaviour
{
    [Header("UI Reference")]
    public RawImage mapImage;

    [Header("Google Maps Settings")]
    [Tooltip("Your Google Static Maps API Key")]
    public string googleMapsApiKey = "AIzaSyDj41OK_EDVYC8EUyhsRyM1TqjAfXEkGNw";
    public int zoom = 16;
    public int mapWidth = 1024;
    public int mapHeight = 1024;

    [Header("Update Settings")]
    public float updateInterval = 5f; // seconds

    private float latitude;
    private float longitude;
    private bool isWaitingForLocation = false;

    private void Start()
    {
        StartCoroutine(CheckAndRequestLocationPermission());
    }

    private IEnumerator CheckAndRequestLocationPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            // Wait a bit for the user to respond (optional: poll until permission changes)
            yield return new WaitForSeconds(1f);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.LogError("Location permission not granted by user.");
            yield break;
        }
#endif
        StartCoroutine(UpdateMapLoop());
        yield break;
    }

    private IEnumerator UpdateMapLoop()
    {
        while (true)
        {
#if UNITY_EDITOR
            // Test coordinates for Editor (Piata Unirii, Bucharest)
            latitude = 44.4268f;
            longitude = 26.1025f;
            yield return StartCoroutine(UpdateMap(latitude, longitude));
#else
            if (!Input.location.isEnabledByUser)
            {
                Debug.LogError("Location not enabled on device.");
                yield break;
            }

            if (!isWaitingForLocation)
            {
                isWaitingForLocation = true;
                Input.location.Start();
            }

            int maxWait = 20;
            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
            {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogError("Location service failed or timed out.");
                yield break;
            }

            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            yield return StartCoroutine(UpdateMap(latitude, longitude));
#endif
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private IEnumerator UpdateMap(float lat, float lon)
    {
        string url = $"https://maps.googleapis.com/maps/api/staticmap?center={lat},{lon}&zoom={zoom}&size={mapWidth}x{mapHeight}&markers=color:red%7C{lat},{lon}&key={googleMapsApiKey}";
        Debug.Log("Requesting map from URL: " + url);

        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (www.result == UnityWebRequest.Result.Success)
#else
        if (!www.isNetworkError && !www.isHttpError)
#endif
        {
            Debug.Log("Map downloaded successfully.");
            mapImage.texture = DownloadHandlerTexture.GetContent(www);
        }
        else
        {
            Debug.LogError("Map download failed: " + www.error);
        }
    }

    private void OnDisable()
    {
#if !UNITY_EDITOR
        if (Input.location.isEnabledByUser && Input.location.status == LocationServiceStatus.Running)
        {
            Input.location.Stop();
        }
#endif
    }
}