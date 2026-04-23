using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class ToasterAnimator : MonoBehaviour
{
    [Header("Partes del lever")]
    public Transform activador;

    [Header("Panes")]
    public Transform panMenu;
    public Transform panOpciones;

    [Header("Posiciones del activador (local Y)")]
    public float leverPosArriba = 0f;
    public float leverPosAbajo = -0.5f;

    [Header("Posiciones del pan (local Y)")]
    public float panOcultoY = -3f;
    public float panVisibleY = 0f;

    [Header("Duraciones")]
    public float duracionBajar = 0.25f;
    public float pausaEnFondo = 0.3f;
    public float duracionSubir = 0.45f;
    public AnimationCurve curvaSubida;

    [Header("Evento al terminar")]
    public UnityEvent onMenuListo;

    void Awake()
    {
        SetActivadorY(leverPosArriba);
        SetTransformY(panMenu, panOcultoY);
        SetTransformY(panOpciones, panOcultoY);
    }

    public void PlayToastSequence()
    {
        StartCoroutine(SecuenciaTostadora());
    }

    IEnumerator SecuenciaTostadora()
    {
        yield return StartCoroutine(AnimarActivador(leverPosAbajo, duracionBajar));
        yield return new WaitForSeconds(pausaEnFondo);
        yield return StartCoroutine(AnimarActivador(leverPosArriba, duracionBajar));
        yield return StartCoroutine(AnimarTransform(panMenu, panVisibleY, duracionSubir));
        onMenuListo?.Invoke();
    }

    public IEnumerator BajarPan()
    {
        yield return StartCoroutine(AnimarTransform(panMenu, panOcultoY, duracionBajar));
    }

    public IEnumerator SubirPanOpciones()
    {
        SetTransformY(panOpciones, panOcultoY);
        yield return StartCoroutine(AnimarActivador(leverPosAbajo, duracionBajar));
        yield return new WaitForSeconds(pausaEnFondo);
        yield return StartCoroutine(AnimarActivador(leverPosArriba, duracionBajar));
        yield return StartCoroutine(AnimarTransform(panOpciones, panVisibleY, duracionSubir));
    }

    IEnumerator AnimarActivador(float targetY, float duracion)
    {
        float inicioY = activador.localPosition.y;
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            var p = activador.localPosition;
            p.y = Mathf.Lerp(inicioY, targetY, elapsed / duracion);
            activador.localPosition = p;
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetActivadorY(targetY);
    }

    IEnumerator AnimarTransform(Transform t, float targetY, float duracion)
    {
        float inicioY = t.localPosition.y;
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            float curva = curvaSubida != null && curvaSubida.length > 0
                ? curvaSubida.Evaluate(elapsed / duracion)
                : elapsed / duracion;

            var p = t.localPosition;
            p.y = Mathf.Lerp(inicioY, targetY, curva);
            t.localPosition = p;
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetTransformY(t, targetY);
    }

    public IEnumerator BajarPanOpciones()
    {
        yield return StartCoroutine(AnimarTransform(panOpciones, panOcultoY, duracionBajar));
    }

    public IEnumerator SubirPanMenu()
    {
        SetTransformY(panMenu, panOcultoY);
        yield return StartCoroutine(AnimarActivador(leverPosAbajo, duracionBajar));
        yield return new WaitForSeconds(pausaEnFondo);
        yield return StartCoroutine(AnimarActivador(leverPosArriba, duracionBajar));
        yield return StartCoroutine(AnimarTransform(panMenu, panVisibleY, duracionSubir));
    }

    void SetActivadorY(float y)
    {
        if (activador == null) return;
        var p = activador.localPosition;
        p.y = y;
        activador.localPosition = p;
    }

    void SetTransformY(Transform t, float y)
    {
        if (t == null) return;
        var p = t.localPosition;
        p.y = y;
        t.localPosition = p;
    }
}