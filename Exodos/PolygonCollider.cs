using System.Collections.Generic;
using Microsoft.Xna.Framework;

// Collider that represents a convex polygon.
namespace ExodosGame {
    class PolygonCollider : Collider {
        // A list of vertices describing the polygon.
        public List<Vector2> vertices;

        public PolygonCollider(float xPosition, float yPosition, List<Vector2> vertices) {
            this.vertices = vertices;
            position = new Vector2(xPosition, yPosition);
            for(int i = 0; i < vertices.Count; i++) {
                vertices[i] += position;
            }
        }

        public override void UpdatePosition(Vector2 newPosition) {
            Vector2 distance = newPosition - position;
            position = newPosition;
            
            for(int i = 0; i < vertices.Count; i++) {
                vertices[i] += distance;
            }
        }
    }
}
