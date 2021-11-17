using System.Collections.Generic;
using UnityEngine;

public interface IPathfinder
{
    List<Vector2> TryFindPath(Vector2Int start, Vector2Int destination);
}
