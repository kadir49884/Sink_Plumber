using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoroutineHelper : MonoBehaviourSingleton<CoroutineHelper>
{
	private List<Execute> m_OnGUIObjects = new List<Execute>();
	public int ScheduledOnGUIItems
	{
		get {return m_OnGUIObjects.Count;}
	}
	public Execute Add(Execute aRun)
	{
		if (aRun != null)
			m_OnGUIObjects.Add(aRun);
		return aRun;
	}
	void OnGUI()
	{
		for(int i = 0; i < m_OnGUIObjects.Count; i++)
		{
			Execute R = m_OnGUIObjects[i];
			if (!R.abort && !R.isDone && R.onGUIaction != null)
				R.onGUIaction();
			else
				R.isDone = true;
		}
	}
	void Update()
	{
		for(int i = m_OnGUIObjects.Count-1; i >= 0; i--)
		{
			if (m_OnGUIObjects[i].isDone)
				m_OnGUIObjects.RemoveAt(i);
		}
	}
}
