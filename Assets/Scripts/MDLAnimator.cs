using UnityEngine;

using System;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class MDLAnimator : MonoBehaviour
{
    [SerializeField]
    MDL m_model;

    [SerializeField]
    MDLAnimation m_animation;

    float m_frameTime = 1.0f / 10;

    MeshRenderer m_meshRenderer;
    MeshFilter m_meshFilter;

    Mesh m_mesh;

    Vector3[] m_initialVertices;
    Vector3[] m_frameBlendVertices;
    float m_elaspedTime;

    int m_frameIndex;
    bool m_animationFinished;

    Action m_animationFinishCallback;

    void Awake()
    {
        m_meshFilter = GetComponent<MeshFilter>();
        m_meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        this.model = m_model;

        m_animationFinished = true;
        if (m_animation != null)
        {   
            SetFrameIndex(0); // rewind to the first frame
        }
    }

    void Update()
    {
        if (isAnimationPlaying) {
            m_elaspedTime += Time.deltaTime;
            if (m_elaspedTime > m_frameTime)
            {
                int nextFrame = m_frameIndex + 1;
                if (nextFrame < m_animation.frameCount) {
                    SetFrameIndex(nextFrame);
                } else {
                    if (m_animation.type == MDLAnimationType.Looped) {
                            SetFrameIndex(0);
                    } else {
                        if (m_animation.type == MDLAnimationType.Rewind) {
                            SetFrameIndex(0);
                        }

                        m_animationFinished = true;
                        if (m_animationFinishCallback != null) {
                            var callback = m_animationFinishCallback;
                            m_animationFinishCallback = null;
                            callback();
                        }
                    }
                }
            }
        }
    }

    void Blend(int frameIndex1, int frameIndex2, float alpha) {
        var v1 = m_animation.frames[frameIndex1].vertices;
        var v2 = m_animation.frames[frameIndex2].vertices;

        for (int i = 0; i < m_frameBlendVertices.Length; i++) {
            m_frameBlendVertices[i] = Vector3.Lerp(v1[i], v2[i], alpha);
        }

        m_mesh.vertices = m_frameBlendVertices;
    }

    void SetFrameIndex(int index) {
        m_elaspedTime = 0.0f;

        // indices
        m_frameIndex = index;

        var vertices = m_animation.frames[index].vertices;
        m_mesh.vertices = vertices;

        if (m_frameBlendVertices == null || m_frameBlendVertices.Length != vertices.Length)
        {
            m_frameBlendVertices = new Vector3[vertices.Length];
        }
        Array.Copy(vertices, m_frameBlendVertices, vertices.Length);
    }

    #region Skins

    void SetSkin(Material skin)
    {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = skin;
    }

    #endregion

    #region Animations
    
    public void StopAnimation()
    {
        m_animation = null;
        m_animationFinishCallback = null;
        m_mesh.vertices = m_initialVertices;
    }

    public void PlayAnimation(MDLAnimation animation, Action finishCallback = null)
    {
        m_animation = animation;
        m_animationFinished = false;
        m_animationFinishCallback = finishCallback;
        SetFrameIndex(0);
    }
    
    #endregion

    #region Editor helpers

    public Material sharedSkin
    {
        get
        {
            if (Application.isPlaying)
            {
                return this.skin;
            }

            var meshRenderer = GetComponent<MeshRenderer>();
            return meshRenderer.sharedMaterial;
        }
        set
        {
            if (Application.isPlaying)
            {
                this.skin = value;
            }
            else
            {
                var meshRenderer = GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = value;
            }
        }
    }

    #endregion

    #region Properties

    public MDL model
    {
        get { return m_model; }
        set
        {
            m_model = value;
            m_animation = null;
            m_animationFinishCallback = null;

            if (m_model != null)
            {
                m_meshFilter.mesh = m_model.mesh;
                m_meshFilter.mesh.MarkDynamic();
                m_mesh = m_meshFilter.mesh;
                m_initialVertices = m_mesh.vertices;

                m_meshRenderer.material = m_model.material;
            }
        }
    }

    public MDL sharedModel
    {
        get { return m_model; }
        set
        {
            if (value != m_model)
            {
                m_animation = null;
            }

            m_model = value;

            var meshFilter = GetComponent<MeshFilter>();
            var meshRenderer = GetComponent<MeshRenderer>();

            if (m_model != null)
            {   
                meshFilter.sharedMesh = m_model.mesh;
                meshRenderer.sharedMaterial = m_model.material;
            }
            else
            {
                meshFilter.sharedMesh = null;
                meshRenderer.sharedMaterial = null;
            }
        }
    }

    public new MDLAnimation animation
    {
        get { return m_animation; }
        set { m_animation = value; }
    }

    public bool isAnimationPlaying
    {
        get { return m_animation != null && !m_animationFinished; }
    }

    public string animationName
    {
        get { return m_animation != null ? m_animation.name : null; }
    }

    public Material skin
    {
        get { return m_meshRenderer.material; }
        set { m_meshRenderer.material = value; }
    }

    #endregion
}
