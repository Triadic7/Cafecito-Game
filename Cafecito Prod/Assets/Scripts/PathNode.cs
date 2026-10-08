using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    /// <summary>
    /// The name of the path node.
    /// </summary>
    [SerializeField]
    private string pathNodeName;

    /// <summary>
    /// Gets or sets the name of the node.
    /// </summary>
    public string PathNodeName { get => pathNodeName;  private set => pathNodeName = value; }
}
