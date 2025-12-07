using UnityEngine;

[ExecuteAlways]
public class ShowBoxColliderGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.green;

    void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.matrix = box.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
        }
    }
}