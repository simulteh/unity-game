using UnityEngine;

public class HTTP
{
    public string Get(string url)
    {
        Debug.Log($"HTTP GET request to {url}");
        return $"Response from {url} (status: 200 OK)";
    }

    public string Post(string url, string data)
    {
        Debug.Log($"HTTP POST request to {url} with data: {data}");
        return $"Response from {url} (status: 201 Created)";
    }
}
