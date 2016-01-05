using UnityEngine;
using System.Collections;
//using UnityEditor;
using System;
using System.IO;

public class WaypointManager : MonoBehaviour
{

    public PathManager[] PathWp;
    ///<summary>
    /// Gizmo color for path ends.
    /// <summary>
    public Color color1 = new Color(1, 0, 1, 0.5f);

    /// <summary>
    /// Gizmo color for lines and waypoints.
    /// <summary>
    public Color color2 = new Color(1, 235 / 255f, 4 / 255f, 0.5f);

    /// <summary>
    /// Gizmo size for path ends.
    /// <summary>
    public Vector3 size = new Vector3(.7f, .7f, .7f);

    /// <summary>
    /// Gizmo radius for waypoints.
    /// <summary>
    public float radius = .4f;

    public Vector3 temp = Vector3.zero;
    public void Start()
    {

    }
    private void OnDrawGizmos()
    {
        if (PathWp != null)
        {
            DrawCurved();
        }
    }
    /// <summary>
    ///Lấy ra các điểm trên đường cong
    /// <summary>
    public Vector3 GetPoint(Vector3[] gizmoPoints, float t)
    {
        int numSections = gizmoPoints.Length - 3;
        int tSec = (int)Mathf.Floor(t * numSections);
        int currPt = numSections - 1;
        if (currPt > tSec)
        {
            currPt = tSec;
        }
        float u = t * numSections - currPt;

        Vector3 a = gizmoPoints[currPt];
        Vector3 b = gizmoPoints[currPt + 1];
        Vector3 c = gizmoPoints[currPt + 2];
        Vector3 d = gizmoPoints[currPt + 3];

        return .5f * (
                       (-a + 3f * b - 3f * c + d) * (u * u * u)
                       + (2f * a - 5f * b + 4f * c - d) * (u * u)
                       + (-a + c) * u
                       + 2f * b
                   );
    }
    //xử lý kích cỡ tùy thuộc vào góc nhìn
    private float GetHandleSize(Vector3 pos)
    {
        float handleSize = 1f;
#if UNITY_EDITOR
        handleSize = UnityEditor.HandleUtility.GetHandleSize(pos) * 0.5f;
#endif
        return handleSize;
    }
    /// <summary>
    /// Vẽ đường line thông qua mảng PathWps[] vừa tìm thấy
    /// </summary>
    public void DrawCurved()
    {
        if (PathWp.Length <= 0) return;

        for (int k = 0; k < PathWp.Length; k++)
        {
            //get positions
            Vector3[] waypoints = PathWp[k].GetPathPoints();

            //tô màu cho điểm đầu và cuối
            Vector3 start = waypoints[0];
            Vector3 end = waypoints[waypoints.Length - 1];
            Gizmos.color = color1;
            Gizmos.DrawWireCube(start, size * GetHandleSize(start));
            Gizmos.DrawWireCube(end, size * GetHandleSize(end));

            //tô màu cho đường đi và các điểm point trên đường
            Gizmos.color = color2;
            for (int i = 1; i < waypoints.Length - 1; i++)
                Gizmos.DrawWireSphere(waypoints[i], radius * GetHandleSize(waypoints[i]));


            //helper array for curved paths, includes control points for waypoint array
            Vector3[] gizmoPoints = new Vector3[waypoints.Length + 2];
            waypoints.CopyTo(gizmoPoints, 1);
            gizmoPoints[0] = waypoints[1];
            gizmoPoints[gizmoPoints.Length - 1] = gizmoPoints[gizmoPoints.Length - 2];

            Vector3[] drawPs;
            Vector3 currPt;

            //store draw points
            int subdivisions = gizmoPoints.Length * 20;
            drawPs = new Vector3[subdivisions + 1];
            for (int i = 0; i <= subdivisions; ++i)
            {
                float pm = i / (float)subdivisions;
                currPt = GetPoint(gizmoPoints, pm);
                drawPs[i] = currPt;
            }

            //draw path
            Vector3 prevPt = drawPs[0];
            temp = drawPs[1];
            for (int i = 1; i < drawPs.Length; ++i)
            {
                currPt = drawPs[i];
                Gizmos.DrawLine(currPt, prevPt);
                prevPt = currPt;
            }
        }

    }
    public PathManager[] GetPathWp()
    {
        return PathWp;
    }
    public Vector3 PathWaypoint(int k)
    {

        Vector3[] waypoints = PathWp[k].GetPathPoints();
        //helper array for curved paths, includes control points for waypoint array
        Vector3[] gizmoPoints = new Vector3[waypoints.Length + 2];
        waypoints.CopyTo(gizmoPoints, 1);
        gizmoPoints[0] = waypoints[1];
        gizmoPoints[gizmoPoints.Length - 1] = gizmoPoints[gizmoPoints.Length - 2];

        Vector3[] drawPs;
        Vector3 currPt;

        //store draw points
        int subdivisions = gizmoPoints.Length * 20;
        drawPs = new Vector3[subdivisions + 1];
        for (int i = 0; i <= subdivisions; ++i)
        {
            float pm = i / (float)subdivisions;
            currPt = GetPoint(gizmoPoints, pm);
            drawPs[i] = currPt;
        }
        return drawPs[1];
    }
}