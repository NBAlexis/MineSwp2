using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class APuzzleGizmo : MonoBehaviour
{
    public bool ShowOnePuzzle = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (ShowOnePuzzle)
	    {
	        ShowOnePuzzle = false;
	        CPuzzleStructure puzzle = CPuzzleCreator.CreatePuzzle(16, 9);
            ShowPuzzle(puzzle);
	    }
	}

    void OnDrawGizmosSelected()
    {
        DrawEdge();
        DrawLines();
    }

    private List<Vector2> m_pLineXs;
    private List<Vector2> m_pLineYs;

    private void ShowPuzzle(CPuzzleStructure puzzle)
    {
        #region Lines

        m_pLineXs = new List<Vector2>();
        m_pLineYs = new List<Vector2>();
        for (int i = 0; i < puzzle.m_pLines.Count; ++i)
        {
            m_pLineXs.Add(puzzle.m_pLines[i].m_vPos1);
            m_pLineYs.Add(puzzle.m_pLines[i].m_vPos2);
        }

        #endregion
    }

    private void DrawEdge()
    {
        Gizmos.color = new Color(0.5f, 0.5f, 0.5f);
        Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(CPuzzleCreator.m_fScreenWidth, 0.0f, 0.0f));
        Gizmos.DrawLine(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, CPuzzleCreator.m_fScreenHeight));
        Gizmos.DrawLine(new Vector3(CPuzzleCreator.m_fScreenWidth, 0.0f, CPuzzleCreator.m_fScreenHeight), new Vector3(CPuzzleCreator.m_fScreenWidth, 0.0f, 0.0f));
        Gizmos.DrawLine(new Vector3(CPuzzleCreator.m_fScreenWidth, 0.0f, CPuzzleCreator.m_fScreenHeight), new Vector3(0.0f, 0.0f, CPuzzleCreator.m_fScreenHeight));
    }

    private void DrawLines()
    {
        if (null != m_pLineXs && null != m_pLineYs && m_pLineXs.Count == m_pLineYs.Count)
        {
            Gizmos.color = new Color(0.0f, 0.0f, 1.0f);
            for (int i = 0; i < m_pLineXs.Count; ++i)
            {
                Gizmos.DrawLine(
                    new Vector3(m_pLineXs[i].x, 0.0f, m_pLineXs[i].y), 
                    new Vector3(m_pLineYs[i].x, 0.0f, m_pLineYs[i].y)
                    );
            }
        }
    }
}
