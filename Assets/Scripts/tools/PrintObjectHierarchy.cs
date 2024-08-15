using UnityEngine;

public class PrintHierarchy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PrintChildren(transform, "");
    }

    // Recursive method to print the hierarchy
    void PrintChildren(Transform parent, string indent)
    {
        // Print the name of the current GameObject
        Debug.Log(indent + parent.name);

        // Increase the indent for child objects
        indent += "  ";

        // Recursively print each child
        foreach (Transform child in parent)
        {
            PrintChildren(child, indent);
        }
    }
}