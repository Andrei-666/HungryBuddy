using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

public class LocationStuff : MonoBehaviour
{
    [SerializeField]
    private char unit = 'K';

    public TMP_Text debugTxt;
    public bool gps_ok = false;

    GPSLoc startLoc = new GPSLoc();
    GPSLoc currLoc = new GPSLoc();

    bool measureDistance = false;

    [Header("Map Settings")]
    public RawImage mapImage; // Assign in inspector
    public int mapWidth = 512;
    public int mapHeight = 512;
    public int mapZoom = 15;
    [Tooltip("Set your Google Maps Static API key here")]
    public string googleMapsApiKey = "AIzaSyDj41OK_EDVYC8EUyhsRyM1TqjAfXEkGNw";

    private string lastMapUrl = "";

    IEnumerator Start()
    {
        // Check if the user has location service enabled.
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location not enabled on device or app does not have permission to access location");
            debugTxt.text = "Location not enabled on device or app does not have permission to access location";
            yield break;
        }
        // Starts the location service.
        Input.location.Start();

        // Waits until the location service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // If the service didn't initialize in 20 seconds this cancels location service use.
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            debugTxt.text += "\nTimed Out";
            yield break;
        }

        // If the connection failed this cancels location service use.
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            debugTxt.text += ("\nUnable to determine device location");
            yield break;
        }
        else
        {
            // If the connection succeeded, this retrieves the device's current location and displays it in the Console window.
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            debugTxt.text
               = "\nLocation: \nLat: " + Input.location.lastData.latitude
                + " \nLon: " + Input.location.lastData.longitude
                + " \nAlt: " + Input.location.lastData.altitude
                + " \nH_Acc: " + Input.location.lastData.horizontalAccuracy
                + " \nTime: " + Input.location.lastData.timestamp;

            gps_ok = true;

            // Fetch and display map the first time
            currLoc.lat = (float)Input.location.lastData.latitude;
            currLoc.lon = (float)Input.location.lastData.longitude;
            StartCoroutine(UpdateMapImage(currLoc.lat, currLoc.lon));
        }
    }

    void Update()
    {
        if (gps_ok)
        {
            currLoc.lat = (float)Input.location.lastData.latitude;
            currLoc.lon = (float)Input.location.lastData.longitude;

            debugTxt.text = $"\nLocation:\nLat: {currLoc.lat}\nLon: {currLoc.lon}\nH_Acc: {Input.location.lastData.horizontalAccuracy}";

            debugTxt.text += "\nStored: " + startLoc.getLocData();

            if (measureDistance)
            {
                double distanceBetween = distance(currLoc.lat, currLoc.lon, startLoc.lat, startLoc.lon, 'K');
                debugTxt.text += "\nDistance: " + distanceBetween + " km";
            }

            // Optionally, update map only if location changes significantly to avoid excessive requests
            /*if (mapImage != null)
            {
                string newMapUrl = BuildMapUrl(currLoc.lat, currLoc.lon);
                if (newMapUrl != lastMapUrl)
                {
                    lastMapUrl = newMapUrl;
                    StartCoroutine(UpdateMapImage(currLoc.lat, currLoc.lon));
                }
            }*/
        }
    }

    public void StopGPS()
    {
        Input.location.Stop();
    }

    public void StoreCurrentGPS()
    {
        startLoc = new GPSLoc(currLoc.lon, currLoc.lat);
        measureDistance = true;
    }

    //https://www.geodatasource.com/resources/tutorials/how-to-calculate-the-distance-between-2-locations-using-c/
    private double distance(double lat1, double lon1, double lat2, double lon2, char unit)
    {
        if ((lat1 == lat2) && (lon1 == lon2))
        {
            return 0;
        }
        else
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }
    }

    private double deg2rad(double deg) { return (deg * Math.PI / 180.0); }
    private double rad2deg(double rad) { return (rad / Math.PI * 180.0); }

    // ==================== GOOGLE MAPS STATIC API FUNCTIONALITY ========================
    private string BuildMapUrl(double lat, double lon)
    {
        // Documentation: https://developers.google.com/maps/documentation/maps-static/overview
        string url = $"https://maps.googleapis.com/maps/api/staticmap?center={lat},{lon}&zoom={mapZoom}&size={mapWidth}x{mapHeight}&maptype=roadmap&markers=color:red%7C{lat},{lon}&key={googleMapsApiKey}";
        return url;
    }

    private IEnumerator UpdateMapImage(double lat, double lon)
    {
        if (string.IsNullOrEmpty(googleMapsApiKey))
        {
            Debug.LogWarning("No Google Maps API Key set!");
            debugTxt.text += "\nNo Google Maps API Key set!";
            yield break;
        }

        string url = BuildMapUrl(lat, lon);

        Debug.Log("Downloading map from: " + url);

        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url);
        yield return uwr.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
        if (uwr.result != UnityWebRequest.Result.Success)
#else
        if (uwr.isNetworkError || uwr.isHttpError)
#endif
        {
            Debug.LogError("Map download error: " + uwr.error);
            debugTxt.text += "\nMap error: " + uwr.error;
        }
        else
        {
            Texture2D mapTexture = DownloadHandlerTexture.GetContent(uwr);

            // Debug: If the image is tiny, it is probably an error image from Google
            if (mapTexture.width < 100 || mapTexture.height < 100)
            {
                Debug.LogWarning("Downloaded map image is very small. This may be an error image from Google (API key, quota, or other issue).");
                debugTxt.text += $"\nDownloaded map image is very small ({mapTexture.width}x{mapTexture.height}). Check API key and Google Cloud Console restrictions.";
            }

            if (mapImage != null)
            {
                mapImage.texture = mapTexture;
                mapImage.SetNativeSize();
            }
        }
    }
}

// ================================

public class GPSLoc
{
    public float lon;
    public float lat;

    public GPSLoc()
    {
        lon = 0;
        lat = 0;
    }
    public GPSLoc(float lon, float lat)
    {
        this.lon = lon;
        this.lat = lat;
    }

    public string getLocData()
    {
        return "Lat: " + lat + " \nLon: " + lon;
    }
}