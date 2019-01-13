using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightColorControl : MonoBehaviour {


	public float transitionTime = 1f;

	public bool useRandomColors = false;
	public List<Color> colors;

	public bool fadeFromInitialColor = true;
	public bool fadeToInitialColor = true;

	public bool findAllLights = true;
	public List<GameObject> lights;

	Color startColor;

	bool running = false;

	private void Start()
	{
		if (findAllLights)
		{
			lights = new List<GameObject>(GameObject.FindGameObjectsWithTag("Light"));
		}
	}

	public void StartColorChange()
	{
		if (lights == null || lights.Count == 0) return;
		if (!useRandomColors && (colors == null || colors.Count == 0)) return;
		running = true;
		StartCoroutine(ChangeColor());
	}

	public void StopColorChange()
	{
		running = false;

		if (!fadeToInitialColor)
		{
			StopCoroutine(ChangeColor());
		}
	}

	private IEnumerator ChangeColor()
	{
		float timeLeft = 0f;
		int colorIndex = 0;

		Color toColor = new Color(Random.value, Random.value, Random.value);
		Color fromColor = startColor = lights[0].GetComponent<Light>().color;

		while (running)
		{
			if (timeLeft <= Time.deltaTime)
			{
				if (!fadeFromInitialColor)
				{
					if (useRandomColors)
					{
						SetColor(toColor);

						fromColor = toColor;		
					}
					else
					{
						toColor = colors[colorIndex];
						SetColor(toColor);

						fromColor = toColor;

						colorIndex = (colorIndex + 1) % colors.Count;
					}
				}
				else
				{
					fadeFromInitialColor = false;
				}

				if (useRandomColors)
				{
					toColor = new Color(Random.value, Random.value, Random.value);
				}
				else
				{
					toColor = colors[colorIndex];
				}
				timeLeft = transitionTime;
			}
			else
			{
				SetColor(Color.Lerp(fromColor, toColor, Time.deltaTime / timeLeft));
				timeLeft -= Time.deltaTime;
			}

			yield return null;
		}

		if (fadeToInitialColor)
		{
			bool complete = false;
			fromColor = lights[0].GetComponent<Light>().color;
			toColor = startColor;
			timeLeft = transitionTime;

			while (!complete)
			{
				if (timeLeft <= Time.deltaTime)
				{
					complete = true;
				}
				else
				{
					SetColor(Color.Lerp(fromColor, toColor, Time.deltaTime / timeLeft));
					timeLeft -= Time.deltaTime;
				}

				yield return null;
			}
		}
	}

	public void SetColor(Color color)
	{
		for (int i = 0; i < lights.Count; i++)
		{
			lights[i].GetComponent<Light>().color = color;
		}
	}

}
