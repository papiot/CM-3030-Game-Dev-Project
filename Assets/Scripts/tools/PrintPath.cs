using UnityEngine;

public class GetGameObjectPath : MonoBehaviour
{
    // This method returns the full path of the GameObject
    public static string GetPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    // You can use this function to print the path of the selected GameObject
    [ContextMenu("Print Full Path")]
    void PrintFullPath()
    {
        Debug.Log(GetPath(transform));
    }
}