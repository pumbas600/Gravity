using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class OrbitRendererManager : MonoBehaviour
{
    [SerializeField] private bool displayOrbit = true;
    [SerializeField] private bool displayOrbitInPlayMode = false;
    [SerializeField] private int steps = 1000;
    [SerializeField] private float timeStep = 0.1F;

    // Update is called once per frame
    private void Update()
    {
        if (displayOrbit && (displayOrbitInPlayMode || !Application.isPlaying)) {
            DisplayOrbit();
        }
    }

    private void DisplayOrbit()
    {
        if (steps <= 0) return;

        CelestialBody[] celestialBodies = CelestialManager.CelestialBodies;

        RelativeBody[] relativeBodies = new RelativeBody[celestialBodies.Length];
        VirtualBody[] virtualBodies = new VirtualBody[celestialBodies.Length];
        Vector3[][] positions = new Vector3[celestialBodies.Length][];

        //Initialise virtual bodies and starting positions.
        for (int bodyIndex = 0; bodyIndex < celestialBodies.Length; bodyIndex++)
        {
            virtualBodies[bodyIndex] = new VirtualBody(celestialBodies[bodyIndex]);
            positions[bodyIndex] = new Vector3[steps];
            positions[bodyIndex][0] = virtualBodies[bodyIndex].position;

            if (celestialBodies[bodyIndex].TryGetComponent(out OrbitRenderer renderer))
            {
                int relativeBodyIndex = FindRelativeBodyIndex(renderer.RelvativeBody, celestialBodies);
                Vector3 previousPosition = Vector3.zero;

                if (relativeBodyIndex != -1)
                {
                    previousPosition = renderer.RelvativeBody.transform.position;
                }
                relativeBodies[bodyIndex] = new RelativeBody(previousPosition, relativeBodyIndex);
            }
        }

        //Simulate their positions over time.
        for (int step = 1; step < steps; step++)
        {
            for (int bodyIndex = 0; bodyIndex < celestialBodies.Length; bodyIndex++)
            {
                var virtualBody = virtualBodies[bodyIndex];
                Vector3 position = UpdatePosition(relativeBodies[bodyIndex], virtualBody, virtualBodies);

                RelativeBody[] bodiesRelativeToCurrentBody = FindRelativeBodies(bodyIndex, relativeBodies);
                for (int relativeIndex = 0; relativeIndex < bodiesRelativeToCurrentBody.Length; relativeIndex++) 
                {
                    bodiesRelativeToCurrentBody[relativeIndex].previousPosition = position;
                }

                positions[bodyIndex][step] = position;
            }
        }

        //Draw their orbits if they have an orbit renderer.
        for (int bodyIndex = 0; bodyIndex < celestialBodies.Length; bodyIndex++)
        {
            if (celestialBodies[bodyIndex].TryGetComponent(out OrbitRenderer renderer))
            {
                renderer.DisplayOrbit(positions[bodyIndex]);
            }
        }
    }

    private Vector3 UpdatePosition(RelativeBody relativeBody, VirtualBody virtualBody, VirtualBody[] bodies)
    {
        if (virtualBody.isFixed)
        {
            return virtualBody.position;
        }

        Vector3 referenceBodyPosition = (relativeBody.IsRelative)
            ? bodies[relativeBody.referenceBodyIndex].position
            : Vector3.zero;

        foreach (var body in bodies)
        {
            if (body != virtualBody)
            {
                if (body == null) return Vector3.zero;

                float sqrDistance = (body.position - virtualBody.position).sqrMagnitude;

                Vector3 direction = (body.position - virtualBody.position).normalized;
                Vector3 acceleration = direction * (CelestialManager.GravitationalConstant
                    .GetAs(new Metre(3), new Kilogram(-1), new Second(-2)) * body.mass / sqrDistance);

                virtualBody.velocity += acceleration * timeStep;
            }
        }

        Vector3 newPosition = virtualBody.position + virtualBody.velocity * timeStep;
        virtualBody.position = newPosition;

        if (relativeBody.IsRelative)
        {
            var offset = referenceBodyPosition - relativeBody.previousPosition;
            newPosition -= offset;
        }

        return newPosition;
    }

    private int FindRelativeBodyIndex(CelestialBody body, CelestialBody[] bodies)
    {
        if (body == null) return -1;

        for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
        {
            if (bodies[bodyIndex] == body) return bodyIndex;
        }
        return -1;
    }

    private RelativeBody[] FindRelativeBodies(int bodyIndex, RelativeBody[] relativeBodies)
    {
        return relativeBodies.Where(b => b.IsRelative && b.referenceBodyIndex == bodyIndex).ToArray();
    }

    struct RelativeBody
    {
        public Vector3 previousPosition;
        public int referenceBodyIndex;
        public bool IsRelative => referenceBodyIndex != -1;
        public RelativeBody(Vector3 previousPosition, int referenceBodyIndex)
        {
            this.previousPosition = previousPosition;
            this.referenceBodyIndex = referenceBodyIndex;
        }
    }

    class VirtualBody
    {
        public float mass;
        public Vector3 position;
        public Vector3 velocity;
        public bool isFixed;

        public VirtualBody(CelestialBody body)
        {
            mass = body.Mass.GetAs(new Kilogram());
            position = body.transform.position;
            velocity = body.Velocity.GetAs(new Metre(), new Second(-1));
            isFixed = body.IsFixed;
        }
    }
}


