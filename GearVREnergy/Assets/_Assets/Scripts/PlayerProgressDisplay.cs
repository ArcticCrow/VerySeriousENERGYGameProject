using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProgressDisplay : MonoBehaviour {

	[Serializable]
	public struct Objective
	{
		public Sequence sequence;
		public Image imageDisplay;
	}

	public List<Objective> objectives = new List<Objective>();

	public Color finished;
	public Color inProgress;
	public Color locked;

	private void Update()
	{
		if (objectives == null) enabled = false;

		for (int i = 0; i < objectives.Count; i++)
		{
			Objective o = objectives[i];
			if (o.imageDisplay == null || o.sequence == null)
				continue;

			if (o.sequence.isSequenceFinished)
			{
				o.imageDisplay.color = finished;
			}
			else if (o.sequence == GameManager.instance.activeSequence)
			{
				o.imageDisplay.color = inProgress;
			}
			else
			{
				o.imageDisplay.color = locked;
			}
		}

	}
}
