// (c) Copyright Andrey Torchinskiy, 2019. All rights reserved.
using UnityEngine;
using UnityEngine.Rendering;
using System;

[ExecuteInEditMode]
public class ScreenTransitionSingleFocusSoft : MonoBehaviour
{
    public static ScreenTransitionSingleFocusSoft Instance; 
    #region Variables

    /// <summary>
    /// Background color that will be used during transition.
    /// </summary>
    [Tooltip("Background color that will be used during transition.")]
    public Color backgroundColor = Color.black;

    /// <summary>
    /// Texture that will be used as background during transition.
    /// Render Texture also allowed.
    /// </summary>
    [Tooltip("Texture that will be used as background during transition. Render Texture also allowed.")]
    public Texture backgroundTexture;

    public enum BackgroundType
    {
        COLOR,
        TEXTURE
    }
    /// <summary>
    /// Defines what type background will be used during transition.
    /// </summary>
    [Tooltip("Defines what type background will be used during transition.")]
    public BackgroundType backgroundType;

    /// <summary>
    /// Represents current progress of the transition.
    /// 0 - no transition
    /// from 1.5 to 2.5 - full transition to background color (depends on the falloff).
    /// </summary>
    [Range(0f, 2.5f), Tooltip("Represents current progress of the transition.")]
    public float cutoff = 0f;


    /// <summary>
    /// Smooth blend between rendered texture and background color.
    /// 0 - no blend (sharp border)
    /// 1 - max blend 
    /// </summary>
    [Range(0f, 1f), Tooltip("Smooth blend between rendered texture and background color.")]
    public float falloff = 0f;

    /// <summary>
    /// Flag that tells Unity to process transition. 
    /// Set this flag at the beginning of the transition and unset at the end 
    /// to avoid unnecessary calculations and save some performance.
    /// </summary>
    [Tooltip("Flag that tells Unity to process transition. Set this flag at the beginning of the transition and unset it at the end to avoid unnecessary calculations and to save some performance.")]
    public bool transitioning;

    /// <summary>
    /// Position of this Transform will be the center of the transition's circle.
    /// When Transform is Null or placed behind the camera, center of the screen will be used as the target instead.
    /// </summary>
    [Tooltip("Position of this Transform will be the center of the transition's circle. When Transform is Null or placed behind the camera, center of the screen will be used as the target instead.")]
    public Transform focus;

    /// <summary>
    /// Reference to the camera component.
    /// </summary>
    private Camera _cam;

    private Animator _animator;

    private Action _onCloseCallback;

    private Action _onOpenCallback;

    private bool _isWorking = false;


    public void OnCloseEnd() 
    {
        if (_onCloseCallback != null) {
            _onCloseCallback();
            _onCloseCallback = null;
        }
        if (_animator)
        {
            _animator.enabled = false;
        }
        _isWorking = false;
    }

    public void OnOpenEnd() {

        if (_onOpenCallback != null)
        {
            _onOpenCallback();
            _onOpenCallback = null;
        }
        if (_animator)
        {
            _animator.enabled = false;
        }
        _isWorking = false;
    }
    #endregion

    private void OnEnable()
    {
        Instance = this;
        Init();
    }

    private void OnDisable()
    {
        if (Instance == this) 
        {
            Instance = null;
        }
        Cleanup();
    }

    public bool IsWorking()
    {
        return _isWorking;
    }

    private void Awake()
    {
        _cam = GetComponentInParent<Camera>();
        _animator= GetComponent<Animator>();
        if (_animator) 
        {
            _animator.enabled = false;
        }
    }

    public void Close(Action calllback= null) 
    {
        _isWorking = true;
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.Play("Close");
        }
        this._onCloseCallback= calllback;   
    }

    public void Open(Action calllback=null) 
    {
        _isWorking= true;
        if (_animator != null)
        {
            _animator.enabled = true;
            _animator.Play("Open");
        }
        this._onOpenCallback= calllback; 
    }

    private Material m_BlitMaterial = null;

    public  string GetShaderName()
    {
        return "Hidden/PostProcessing/TransitionSingleFocusSoft";
    }
    public  void  Init()
    {
        if (m_BlitMaterial == null) 
        {
            string shaderName = GetShaderName();
            if (!string.IsNullOrEmpty(shaderName))
            {
                var shader = Shader.Find(shaderName);
                m_BlitMaterial = CoreUtils.CreateEngineMaterial(shader);
            }
        }
    }

    public void Cleanup()
    {
        if (m_BlitMaterial != null)
        {
            CoreUtils.Destroy(m_BlitMaterial);
            m_BlitMaterial = null;
        }
        _onCloseCallback = null;
        _onOpenCallback= null;
        if (_animator)
        {
            _animator.enabled =false;
        }
        _isWorking = false;
    }

   

    public void Draw(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
    {
        if (_cam != null &&enabled&& Instance== this)
        {
            if (m_BlitMaterial == null) return;

            m_BlitMaterial.SetFloat("_Cutoff", cutoff);
            m_BlitMaterial.SetFloat("_Falloff",falloff);

            switch (backgroundType)
            {
                case BackgroundType.COLOR:
                    m_BlitMaterial.DisableKeyword("USE_TEXTURE");
                    m_BlitMaterial.SetColor("_Color", backgroundColor);
                    break;
                case BackgroundType.TEXTURE:
                    m_BlitMaterial.EnableKeyword("USE_TEXTURE");
                    m_BlitMaterial.SetTexture("_Texture", backgroundTexture);
                    break;
            }
            if (focus != null)
            {
                var camera = _cam;
                var transform = camera.transform;
                // Check if the focus Transform is behind the camera
                Vector3 dir = (focus.position - transform.position).normalized;
                float dot = Vector3.Dot(transform.forward, dir);

                if (dot > 0)
                {
                    Vector2 screenPos = camera.WorldToViewportPoint(focus.position);
                    m_BlitMaterial.SetFloat("_FocusX", screenPos.x);
                    m_BlitMaterial.SetFloat("_FocusY", screenPos.y);
                }
            }
            else
            {
                m_BlitMaterial.SetFloat("_FocusX", 0.5f);
                m_BlitMaterial.SetFloat("_FocusY", 0.5f);
            }
            cmd.Blit(source, target, m_BlitMaterial);

        }
    }
  
}
