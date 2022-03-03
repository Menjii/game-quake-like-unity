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
                    if (m_animation.type == MDLAnimationType.Rewind) {
                            SetFrameIndex(0);
                    } else {
                        if (m_animation.type == MDLAnimationType.Looped) {
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
}