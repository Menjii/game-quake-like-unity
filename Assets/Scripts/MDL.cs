using UnityEngine;
using System.Collections;

public class MDL : ScriptableObject
{
    [SerializeField]
    Mesh m_mesh;

    [SerializeField]
    Material[] m_materials;

    #region Properties

    public Mesh mesh {
        get { return m_mesh; }
        set { m_mesh = value; }
    }

    public Material[] materials {
        get { return m_materials; }
        set { m_materials = value; }
    }

    public Material material {
        get { return this.materialCount > 0 ? m_materials[0] : null; }
    }

    public int materialCount {
        get { return m_materials != null ? m_materials.Length : 0; }
    }
    
    #endregion
}