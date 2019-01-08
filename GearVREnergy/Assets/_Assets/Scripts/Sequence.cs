using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Sequence {
	public List<Objective> objectiveQueue = new List<Objective>();
	

	public bool IsSequenceComplete()
	{
		for (int i = 0; i < objectiveQueue.Count; i++)
		{
			if (!objectiveQueue[i].IsObjectiveCleared())
			{
				return false;
			}
		}

		return true;
	}
}
