using System.Linq;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CelestialManager : MonoBehaviour
{
    #region Singleton

    private static CelestialManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    [SerializeField] private UnitFloat gravitationalConstant = new UnitFloat(0.001F, new Metre(3), new Kilogram(-1), new Second(-2));
    [SerializeField] private UnitFloat fixedDeltaTime = new UnitFloat(0.01F, new Second());
    [SerializeField] private UnitFloat timeStep = new UnitFloat(0.1F, new Second());
    [SerializeField] private bool showPaths = true;
    [SerializeField] private UnitFloat maxAllowedDistance = new UnitFloat(1e5F, new Metre());
    [SerializeField, EditorOnly] private float minOutlineRadius = 5F;
    [SerializeField] private CelestialBody centralBody;
    [SerializeField] private AnimationCurve pathWidthCurve;
    [SerializeField] private float minPathWidth = 3F;

    //Mass is scaled at e-16
    //Radius is scaled at e-8
    //Average Velocitys e-4
    //This means acceleration will be the same in this scaled model.
    //[SerializeField] private UnitFloat gravitationalConst = new UnitFloat(6.67408e-1F, new Metre(3), new Kilogram(-1), new Second(-2));
    public static UnitFloat GravitationalConstant => Instance.gravitationalConstant;
    public static UnitFloat TimeStep => Instance.timeStep;
    public static bool ShowPaths => Instance.showPaths;
    public static UnitFloat SqrMaxAllowedDistance => MaxAllowedDistance * MaxAllowedDistance;
    public static UnitFloat MaxAllowedDistance => Instance.maxAllowedDistance;
    public static float MinOutlineRadius => Instance.minOutlineRadius;
    public static float MinPathWidth => Instance.minPathWidth;
    public static AnimationCurve PathWidthCurve => Instance.pathWidthCurve;
    public static Unit UnityUnit = new Metre();
    public static CelestialBody CentralBody {
        get {
            if (Instance.centralBody == null)
            {
                Instance.centralBody = CelestialBodies.Aggregate((largest, next) => (largest.Mass < next.Mass) ? next : largest);
            }
            return Instance.centralBody;
        }
    }

    private static CelestialBody[] celestialBodies;

    private void Start()
    {
        UpdateCelestialBodies();
        Time.fixedDeltaTime = fixedDeltaTime.GetAs(new Second());

        //var velocityms = new UnitFloat(2, new Metre(), new Second(-1));
        //var velocitykmhr = velocityms.GetAsNew(new Kilometre(), new Hour(-1));
        //var time = new UnitFloat(2, new Second());
        //var acceleration = new UnitFloat(3F, new Metre(), new Second(-2));

        //Debug.Log(velocityms / time);
        //Debug.Log(time.GetAsNew(time.GetUnits()));
        //Debug.Log(time - time);

        //int numTests = 10;
        //long totalTime = 0;
        //Stopwatch sw = new Stopwatch();
        //for (int j = 0; j < numTests; j++)
        //{
        //    sw.Start();

        //    for (int i = 0; i < 500; i++)
        //    {
        //        /*
        //         *8-9ms - Initially
        //         *7-8ms - Changed clone constructor to manually add the units to the dictionary
        //         *        rather than calling another constructor to form it.
        //         *3ms   - Implemented deep copy method for units so that UnitHelper.Of(unit.GetType(), unit.Power)
        //         *        didn't need to be called instead, as this is not very efficient.
        //         *2.5ms - Added an override to CheckUnits for units that can't be modified in the inspector.
        //         */
        //        var distance = velocityms * time;
        //    }
        //    sw.Stop();
        //    //Debug.Log($"Elapsed Time: {sw.ElapsedMilliseconds} ms");
        //    totalTime += sw.ElapsedMilliseconds;
        //    sw.Reset();
        //}
        //Debug.Log($"Average Time: {totalTime / (float)numTests} ms");
    }
    

    private void FixedUpdate()
    {
        foreach (var body in CelestialBodies)
        {
            body.UpdateVelocity(CelestialBodies, TimeStep);
        }

        foreach (var body in CelestialBodies)
        {
            body.UpdatePosition(TimeStep);

            Vector3 distance = CentralBody.Position - body.Position;
            if (distance.sqrMagnitude > SqrMaxAllowedDistance.GetAs(UnityUnit))
            {
                Debug.Log($"<color=red>{body.name}</color> is currently <color=red>{distance.magnitude} m</color> from the central body " +
                          $"<color=red>{CentralBody.name}</color>. \nThis exceedes the allowed max distance of " +
                          $"<color=red>{MaxAllowedDistance.GetAs(new Metre())} </color> and so it has been destroyed");
                Destroy(body.gameObject);
                UpdateCelestialBodies();
            }
        }
        
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            foreach (var body in CelestialBodies)
            {
                if (!showPaths || showPaths && body.ShowPath)
                {
                    body.EnableLineRenderer(showPaths);
                }
            }
        }
    }

    public static CelestialBody[] CelestialBodies
    {
        get {
            if (Application.isEditor)
            {
                return FindObjectsOfType<CelestialBody>();
            }

            if (celestialBodies == null || celestialBodies[0] == null)
            {
                UpdateCelestialBodies();
            }
            return celestialBodies;
        }
    }

    public static void UpdateCelestialBodies()
    {
        celestialBodies = FindObjectsOfType<CelestialBody>();
    }

    public static CelestialManager Instance
    {
        get {
            if (instance == null)
            {
                instance = FindObjectOfType<CelestialManager>();
            }
            return instance;
        }
    }
}
