using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CelestialBody))]
public class OrbitRenderer : MonoBehaviour
{
    [SerializeField] private bool displayOrbit = true;
    [SerializeField] private CelestialBody relativeBody;
    private CelestialBody body;
    private CelestialBody Body
    {
        get {
            if (body == null)
            {
                body = GetComponent<CelestialBody>();
            }
            return body;
        }
    }
    public CelestialBody RelvativeBody => relativeBody;

    public void DisplayOrbit(Vector3[] positions)
    {
        if (relativeBody == Body) return;

        if (displayOrbit)
        {
            for (int i = 1; i < positions.Length; i++)
            {
                Debug.DrawLine(positions[i -1], positions[i], Body.Colour);
            }
        }
    }
}