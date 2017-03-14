// A half-plane described by the equation: ax + by <= c.
namespace ExodosGame {
    class HalfPlaneCollider : Collider {
        public float A { get; private set; }
        public float B { get; private set; }
        public float C { get; private set; }

        public HalfPlaneCollider(float a, float b, float c) {
            A = a;
            B = b;
            C = c;
        }
    }
}
