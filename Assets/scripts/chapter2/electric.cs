using System.Collections;
using UnityEngine;

public class electric : MonoBehaviour
{
    public enum ElectricMode
    {
        Normal,
        Random
    }

    [Header("Mode")]
    [SerializeField] private ElectricMode mode = ElectricMode.Random;

    [Header("Lights")]
    [SerializeField] private Light[] lights;

    [Header("Materials")]
    [SerializeField] private Material[] light_material;

    [Header("Colors")]
    [SerializeField] private Color color0 = Color.black;
    [SerializeField] private Color color1 = Color.white;

    [Header("Emission")]
    [SerializeField] private float emissionIntensity = 2f;

    [Header("Normal Timing")]
    [SerializeField] private float onTime = 2f;
    [SerializeField] private float offTime = 0.5f;

    [Header("Random Transition Time")]
    [SerializeField] private float minOnTransition = 0.05f;
    [SerializeField] private float maxOnTransition = 0.3f;

    [SerializeField] private float minOffTransition = 0.05f;
    [SerializeField] private float maxOffTransition = 0.3f;

    [Header("Random Hold Time")]
    [SerializeField] private float minOnTime = 0.5f;
    [SerializeField] private float maxOnTime = 3f;

    [SerializeField] private float minOffTime = 0.2f;
    [SerializeField] private float maxOffTime = 2f;

    [Header("Random Pattern Control")]
    [SerializeField] private int minimumLightsOn = 1;

    [SerializeField] private int maximumLightsOn = -1;

    [SerializeField] private float patternChangeDelay = 0.1f;

    private Coroutine electricCoroutine;

    private void Start()
    {
        electricCoroutine = StartCoroutine(Electric());
    }

    private IEnumerator Electric()
    {
        while (true)
        {
            switch (mode)
            {
                case ElectricMode.Normal:
                    yield return StartCoroutine(NormalPattern());
                    break;

                case ElectricMode.Random:
                    yield return StartCoroutine(RandomPattern());
                    break;
            }
        }
    }

    private IEnumerator NormalPattern()
    {
        yield return StartCoroutine(
            ChangeAllLights(color0, color1, 0.2f)
        );

        yield return new WaitForSeconds(onTime);

        yield return StartCoroutine(
            ChangeAllLights(color1, color0, 0.2f)
        );

        yield return new WaitForSeconds(offTime);
    }

    private IEnumerator RandomPattern()
    {
        if (lights == null || lights.Length == 0)
            yield break;

        int lightCount = lights.Length;

        int maxOn = maximumLightsOn;

        if (maxOn < 0 || maxOn > lightCount)
            maxOn = lightCount;

        int minOn = Mathf.Clamp(
            minimumLightsOn,
            0,
            maxOn
        );

        int targetOnCount = Random.Range(
            minOn,
            maxOn + 1
        );

        int[] indexes = new int[lightCount];

        for (int i = 0; i < lightCount; i++)
        {
            indexes[i] = i;
        }

        for (int i = lightCount - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            int temp = indexes[i];
            indexes[i] = indexes[randomIndex];
            indexes[randomIndex] = temp;
        }

        for (int i = 0; i < lightCount; i++)
        {
            int lightIndex = indexes[i];

            Light currentLight = lights[lightIndex];

            if (currentLight == null)
                continue;

            bool shouldBeOn = i < targetOnCount;

            StartCoroutine(
                RandomLightBehaviour(
                    lightIndex,
                    shouldBeOn
                )
            );
        }

        yield return new WaitForSeconds(patternChangeDelay);
    }

    private IEnumerator RandomLightBehaviour(
        int index,
        bool shouldBeOn
    )
    {
        if (index < 0 || index >= lights.Length)
            yield break;

        Light currentLight = lights[index];

        if (currentLight == null)
            yield break;

        if (shouldBeOn)
        {
            float transitionTime = Random.Range(
                minOnTransition,
                maxOnTransition
            );

            yield return StartCoroutine(
                ChangeLight(
                    index,
                    color0,
                    color1,
                    transitionTime
                )
            );

            float holdTime = Random.Range(
                minOnTime,
                maxOnTime
            );

            yield return new WaitForSeconds(holdTime);
        }
        else
        {
            float transitionTime = Random.Range(
                minOffTransition,
                maxOffTransition
            );

            yield return StartCoroutine(
                ChangeLight(
                    index,
                    color1,
                    color0,
                    transitionTime
                )
            );

            float holdTime = Random.Range(
                minOffTime,
                maxOffTime
            );

            yield return new WaitForSeconds(holdTime);
        }
    }

    private IEnumerator ChangeLight(
        int index,
        Color from,
        Color to,
        float duration
    )
    {
        if (index < 0 || index >= lights.Length)
            yield break;

        Light currentLight = lights[index];

        if (currentLight == null)
            yield break;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            Color currentColor = Color.Lerp(
                from,
                to,
                t
            );

            SetLight(index, currentColor);

            yield return null;
        }

        SetLight(index, to);
    }

    private void SetLight(int index, Color color)
    {
        // Light
        if (index >= 0 && index < lights.Length)
        {
            if (lights[index] != null)
                lights[index].color = color;
        }

        // Material
        if (index >= 0 && index < light_material.Length)
        {
            if (light_material[index] != null)
            {
                Material material = light_material[index];

                if (material.HasProperty("_EmissionColor"))
                {
                    material.SetColor(
                        "_EmissionColor",
                        color * emissionIntensity
                    );
                }
            }
        }
    }

    private IEnumerator ChangeAllLights(
        Color from,
        Color to,
        float duration
    )
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = Mathf.Clamp01(time / duration);

            Color currentColor = Color.Lerp(
                from,
                to,
                t
            );

            for (int i = 0; i < lights.Length; i++)
            {
                SetLight(i, currentColor);
            }

            yield return null;
        }

        for (int i = 0; i < lights.Length; i++)
        {
            SetLight(i, to);
        }
    }
}