using UnityEngine;
using UnityEditor;

// aparece en el menú Tools > Quitar Movimiento de Hijos
// selecciona el padre del chunk y ejecuta esto para borrar Movimiento de todos sus hijos
public static class RemoverMovimiento
{
    [MenuItem("Tools/Quitar Movimiento de Hijos")]
    static void Quitar()
    {
        GameObject seleccionado = Selection.activeGameObject;
        if (seleccionado == null)
        {
            Debug.LogWarning("Selecciona el GameObject padre primero.");
            return;
        }

        Movimiento[] componentes = seleccionado.GetComponentsInChildren<Movimiento>();
        if (componentes.Length == 0)
        {
            Debug.Log("Ningún hijo tiene Movimiento.cs.");
            return;
        }

        Undo.SetCurrentGroupName("Quitar Movimiento de Hijos");
        foreach (Movimiento m in componentes)
        {
            Undo.DestroyObjectImmediate(m);
        }

        Debug.Log($"Movimiento.cs eliminado de {componentes.Length} objetos.");
    }
}
