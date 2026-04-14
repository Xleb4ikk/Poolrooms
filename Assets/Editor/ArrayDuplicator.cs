using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class ArrayDuplicator : MonoBehaviour
{
    public GameObject prefab;
    public int count = 5;
    public Vector3 offset = new Vector3(2f, 0f, 0f);

    [ContextMenu("Generate Array")]
    void GenerateArray()
    {
        // Удалить старые копии
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        // Создать новые
        for (int i = 0; i < count; i++)
        {
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, transform);
            obj.transform.localPosition = offset * i;
        }
    }
}
#endif