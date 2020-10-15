using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody), typeof(LineRenderer))]
public class CelestialBody : MonoBehaviour
{
    [SerializeField] private UnitFloat mass = new UnitFloat(0, new Kilogram());
    [SerializeField] private UnitFloat radius = new UnitFloat(0, new Metre());
    [SerializeField] private UnitVector3 velocity = new UnitVector3(Vector3.zero, new Metre(), new Second(-1));
    [SerializeField] private UnitFloat surfaceGravity = new UnitFloat(0, new Metre(), new Second(-2));
    [SerializeField] private UnitFloat velocityMagnitude = new UnitFloat(0, new Metre(), new Second(-1));
    [SerializeField] private Color colour;
    [SerializeField] private bool showPath = true;
    [SerializeField] private int pathLength = 3000;
    [SerializeField] private bool showDistanceToNearestBody = false;
    [SerializeField] private bool isFixed = false;

    public Color Colour => colour;
    public UnitVector3 Velocity { get { return velocity; } private set { velocity = value; } }
    public UnitFloat Mass { get { return mass; } private set { mass = value; } }
    public Vector3 Position => rb.position;
    public UnitFloat Radius => radius;
    public UnitFloat SurfaceGravity => surfaceGravity;
    public UnitFloat VelocityMagnitude => velocityMagnitude;
    public bool ShowPath => showPath;
    public bool ShowDistanceToNearestBody => showDistanceToNearestBody;
    public UnitFloat DistanceToNearestBody { get; private set; } = new UnitFloat(float.MaxValue, new Metre());
    public bool IsFixed => isFixed;

    private Rigidbody rb;
    private LineRenderer lineRenderer;
    private List<Vector3> path = new List<Vector3>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = Mass.GetAs(new Kilogram());

        lineRenderer = GetComponent<LineRenderer>();

        if (showPath) {

            float pathWidth = 2F * Radius.GetAs(CelestialManager.UnityUnit);
            if (pathWidth < CelestialManager.MinPathWidth) pathWidth = CelestialManager.MinPathWidth;

            lineRenderer.widthCurve = CelestialManager.PathWidthCurve;
            lineRenderer.widthMultiplier = pathWidth;
            lineRenderer.startColor = Colour;
            lineRenderer.endColor = Colour;
        }
    }

    [InspectorButton]
    private void CreateNewMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material material = new Material(renderer.sharedMaterial)
        {
            name = name,
            color = Colour
        };

        renderer.sharedMaterial = material;
    }

    [InspectorButton]
    private void UpdateMaterial()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.sharedMaterial.color = Colour;
        renderer.sharedMaterial.name = name;
    }

    private void OnValidate()
    {
        if (Application.isEditor)
        {
            surfaceGravity = CelestialManager.GravitationalConstant * Mass / (Radius * Radius);
            //Mass = surfaceGravity * Radius * Radius / CelestialManager.GravitationalConstant;
            transform.localScale = Vector3.one * 2 * Radius.GetAs(CelestialManager.UnityUnit);
        }
        else
        {
            EnableLineRenderer(showPath);
        }
    }

    public void UpdateVelocity(CelestialBody[] bodies, UnitFloat timeStep)
    {
        if (showDistanceToNearestBody) DistanceToNearestBody.value = float.MaxValue;

        foreach (var body in bodies)
        {
            if (body != this)
            {
                Vector3 distance = body.Position - Position;
                UnitFloat sqrDistance = new UnitFloat(distance.sqrMagnitude, new Metre(2));

                Vector3 direction = (body.Position - Position).normalized;
                UnitVector3 acceleration = direction * (CelestialManager.GravitationalConstant * body.Mass / sqrDistance);

                //print($"{name}: {acceleration}");

                Velocity += acceleration * timeStep;
                if (showDistanceToNearestBody && distance.magnitude < DistanceToNearestBody)
                {
                    DistanceToNearestBody = new UnitFloat(distance.magnitude, new Metre());
                }
            }
        }
        velocityMagnitude.value = Velocity.GetAs(VelocityMagnitude.GetUnits()).magnitude;
    }

    public void UpdatePosition(UnitFloat timeStep)
    {
        if (isFixed) return;

        rb.MovePosition((Position + Velocity * timeStep).value);

        UpdatePath();
    }

    public void OnClick()
    {
        //Only display the information of this body if you are already targetted to it.
        if (!FindObjectOfType<CameraController>().SetTarget(this)) {
            CelestialBodyInformation.ToggleInformation(this);
        }

        //Otherwise hide any information about other bodies.
        else
        {
            CelestialBodyInformation.HideInformation();
        }
    }

    public void EnableLineRenderer(bool isEnabled)
    {
        if (lineRenderer != null)
        {
            lineRenderer.forceRenderingOff = !isEnabled;
        }
    }

    private void UpdatePath()
    {
        path.Add(transform.position);

        while (path.Count > pathLength)
        {
            path.RemoveAt(0);
        }

        if (showPath && CelestialManager.ShowPaths)
        {
            lineRenderer.positionCount = path.Count;
            lineRenderer.SetPositions(path.ToArray());
        }
    }
}
