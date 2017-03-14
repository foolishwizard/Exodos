using System;
using Microsoft.Xna.Framework;

// An Entity component that represents its capacity to collide with other Entities.
// Inherit from this for creating different collider types for different methods of collision detection
// (Axis-Aligned Bounding Box, Oriented Bounding Box, Circle, Convex Polygon, Half-plane).
namespace ExodosGame {
    public abstract class Collider {
        public Vector2 position; // It needs to reside somewhere in space.

        public virtual void UpdatePosition(Vector2 newPosition) { }
    }
}
