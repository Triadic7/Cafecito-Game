using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Path : MonoBehaviour
{
    /// <summary>
    /// The name of the path.
    /// </summary>
    [SerializeField]
    private string pathName;

    /// <summary>
    /// Gets or sets the list of path nodes.
    /// </summary>
    [SerializeField]
    public List<PathNode> PathNodes { get; set; }

    /// <summary>
    /// Gets or sets the name of the path.
    /// </summary>
    public string PathName {  get => pathName; private set => pathName = value; }

    private void Awake()
    {
        this.PathNodes = GetComponentsInChildren<PathNode>().ToList();
    }
}
