using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    private static List<Vector3> occupiedNodes = new List<Vector3>();

    [SerializeField] private PatrolPathNode[] nodes;

    private void Start()
    {
        UpdatePathNode();
    }

    [ContextMenu("Update Path Nodes")]
    private void UpdatePathNode()
    {
        nodes = new PatrolPathNode[transform.childCount];

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = transform.GetChild(i).GetComponent<PatrolPathNode>();
        }
    }

    public PatrolPathNode GetRandomPathNode()
    {
        var availableNodes = nodes.Where(node => !node.IsOccupied).ToList();

        if (availableNodes.Count == 0)
            return nodes[Random.Range(0, nodes.Length)];

        var randomNode = availableNodes[Random.Range(0, availableNodes.Count)];
        randomNode.IsOccupied = true;

        return randomNode;
    }

    public PatrolPathNode GetNextPathNode(ref int index)
    {
        index = Mathf.Clamp(index, 0, nodes.Length - 1);
        index++;

        if (index >= nodes.Length) index = 0;

        if (nodes[index].IsOccupied)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                int nextIndex = (index + 1) % nodes.Length;
                if (!nodes[nextIndex].IsOccupied)
                {
                    index = nextIndex;

                    nodes[index].IsOccupied = true;
                    return nodes[index];
                }
            }

            return nodes[index];
        }

        nodes[index].IsOccupied = true;
        return nodes[index];
    }

    public void ReleaseNode(Vector3 nodePosition)
    {
        var node = nodes.FirstOrDefault(n => n.transform.position == nodePosition);

        if (node != null)
        {
            node.IsOccupied = false;
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        Gizmos.color = Color.red;
        for (int i = 0; i < nodes.Length - 1; i++)
        {
            Gizmos.DrawLine(nodes[i].transform.position + new Vector3(0, 0.5f, 0), nodes[i + 1].transform.position + new Vector3(0, 0.5f, 0));
        }

        Gizmos.DrawLine(nodes[0].transform.position + new Vector3(0, 0.5f, 0), nodes[nodes.Length - 1].transform.position + new Vector3(0, 0.5f, 0));
    }

#endif

}
