using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Space_1
{
    /// <summary>
    /// 指定したシェーダープロパティ（Vector2）を一定速度でスクロールさせる。
    /// 例: ShaderGraph の Vector2 プロパティ名を _UVOffset にしておけばそのまま動作。
    /// </summary>
    [RequireComponent(typeof(Renderer))]

    public class ScrollUV : MonoBehaviour
    {
        [Header("Scroll Settings")]
    [Tooltip("シェーダー内の Vector2 プロパティ名 (既定: _UVOffset)")]
    public string propertyName = "_UVOffset";

    [Tooltip("1 周期あたりの X オフセット量")]
    public float xOffset = 1f;

    [Tooltip("1 周期あたりの Y オフセット量")]
    public float yOffset = 0f;

    [Tooltip("1 周期にかかる秒数 (0 なら停止)")]
    public float duration = 2f;

    /* ----------------- internal ----------------- */
    Renderer _rend;
    MaterialPropertyBlock _mpb;
    int _propID;
    float _time;

    void Awake()
    {
        _rend  = GetComponent<Renderer>();
        _mpb   = new MaterialPropertyBlock();
        _propID = Shader.PropertyToID(propertyName);
    }

    void Update()
    {
        if (duration <= 0f) return;             // 停止

        _time += Time.deltaTime;
        float t = (_time / duration) % 1f;      // 0〜1 をループ

        Vector2 offset = new Vector2(xOffset * t, yOffset * t);

        _rend.GetPropertyBlock(_mpb);
        _mpb.SetVector(_propID, offset);
        _rend.SetPropertyBlock(_mpb);
    }
    }
}
