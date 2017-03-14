using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

// A container for functions related to collision detection.
namespace ExodosGame {
    static class Collisions {
        // Allows for easier checking of collisions between entities.
        public static bool CheckCollision(Entity e1, Entity e2) {
            Collider a = e1.collider;
            Collider b = e2.collider;
            return CheckCollision(a, b);
        }

        // Sometimes we might want to use a collider without an entity. This method allows for that.
        public static bool CheckCollision(Collider a, Collider b) {
            Type aType = a.GetType();
            Type bType = b.GetType();
            if (aType == typeof(AABBCollider)) {
                if (bType == typeof(AABBCollider)) return AABB((AABBCollider)a, (AABBCollider)b);
                if (bType == typeof(CircleCollider)) return AABBCircle((AABBCollider)a, (CircleCollider)b);
                if (bType == typeof(PolygonCollider)) return AABBPolygon((AABBCollider)a, (PolygonCollider)b);
                if (bType == typeof(HalfPlaneCollider)) return AABBHalfPlane((AABBCollider)a, (HalfPlaneCollider)b);
            } else if (aType == typeof(CircleCollider)) {
                if (bType == typeof(CircleCollider)) return Circle((CircleCollider)a, (CircleCollider)b);
                if (bType == typeof(AABBCollider)) return AABBCircle((AABBCollider)b, (CircleCollider)a);
                if (bType == typeof(PolygonCollider)) return PolygonCircle((PolygonCollider)b, (CircleCollider)a);
                if (bType == typeof(HalfPlaneCollider)) return CircleHalfPlane((CircleCollider)a, (HalfPlaneCollider)b);
            } else if (aType == typeof(PolygonCollider)) {
                if (bType == typeof(PolygonCollider)) return Polygon((PolygonCollider)a, (PolygonCollider)b);
                if (bType == typeof(AABBCollider)) return AABBPolygon((AABBCollider)b, (PolygonCollider)a);
                if (bType == typeof(CircleCollider)) return PolygonCircle((PolygonCollider)a, (CircleCollider)b);
                if (bType == typeof(HalfPlaneCollider)) return PolygonHalfPlane((PolygonCollider)a, (HalfPlaneCollider)b);
            } else if (aType == typeof(HalfPlaneCollider)) {
                if (bType == typeof(HalfPlaneCollider)) return HalfPlane((HalfPlaneCollider)a, (HalfPlaneCollider)b);
                if (bType == typeof(PolygonCollider)) return PolygonHalfPlane((PolygonCollider)b, (HalfPlaneCollider)a);
                if (bType == typeof(AABBCollider)) return AABBHalfPlane((AABBCollider)b, (HalfPlaneCollider)a);
                if (bType == typeof(CircleCollider)) return CircleHalfPlane((CircleCollider)b, (HalfPlaneCollider)a);
            }

            throw new NotImplementedException();
        }

        // Check for collision between two Axis-Aligned Bounding Boxes.
        private static bool AABB(AABBCollider a, AABBCollider b) {
            // This is a special case of the Separating Axis Theorem for AABBs.
            if (a.position.X > b.position.X + b.Width) return false;
            if (a.position.X + a.Width < b.position.X) return false;
            if (a.position.Y > b.position.Y + b.Height) return false;
            if (a.position.Y + a.Height < b.position.Y) return false;
            return true;
        }

        // Check for collision between two convex polygon using the Separating Axis Theorem.
        private static bool Polygon(PolygonCollider a, PolygonCollider b) {
            for(int i = 0; i < a.vertices.Count; i++) {
                // Get the axis to project the polygons' points onto.
                Vector2 edge;
                if (i != a.vertices.Count - 1) edge = a.vertices[i + 1] - a.vertices[i];
                else edge = a.vertices[0] - a.vertices[i];
                Vector2 axis = new Vector2(edge.Y, -edge.X); // Rotate the vector 90 degrees to get the axis vector. Rotation direction doesn't matter.
                axis.Normalize(); // Normalize. It only needs to describe direction.

                // Project each polygon onto the axis. Remember the extents (minimum and maximum) of each projection.
                float aMin, aMax, bMin, bMax;
                GatherPolygonProjectionExtents(axis, a.vertices, out aMin, out aMax);
                GatherPolygonProjectionExtents(axis, b.vertices, out bMin, out bMax);

                // Test for overlap between the two projections.
                if (aMin > bMax) return false;
                if (aMax < bMin) return false;
            }

            // Do the same, but this time get the axes from the second polygon.
            for (int i = 0; i < b.vertices.Count; i++) {
                Vector2 edge;
                if (i != b.vertices.Count - 1) edge = b.vertices[i + 1] - b.vertices[i];
                else edge = b.vertices[0] - b.vertices[i];
                Vector2 axis = new Vector2(edge.Y, -edge.X);
                axis.Normalize();

                float aMin, aMax, bMin, bMax;
                GatherPolygonProjectionExtents(axis, a.vertices, out aMin, out aMax);
                GatherPolygonProjectionExtents(axis, b.vertices, out bMin, out bMax);

                if (aMin > bMax) return false;
                if (aMax < bMin) return false;
            }

            // We iterated through all the possible separating axes. None of them separates the polygons - they overlap!
            return true;
        }

        private static bool Circle(CircleCollider a, CircleCollider b) {
            if ((a.Radius + b.Radius) > ((a.position + a.center) - (b.position + b.center)).Length())
                return true;
            return false;
        }

        private static bool HalfPlane(HalfPlaneCollider k, HalfPlaneCollider l) {
            if((k.A * l.B) - (k.B * l.A) == 0) {
                // The two lines defining the half-planes are parallel.
                // The lines can't intersect, but the half-planes still might!
                // @todo I'm not sure about this, should make sure!
                if (k.C < l.C)
                    return true;
                return false;
            }
            // The lines are intersecting, so the half-planes are, too.
            return true;
        }

        // Uses the Separating Axis Theorem, but accounts for one of the colliders being an AABB and thus described in a different way.
        private static bool AABBPolygon(AABBCollider a, PolygonCollider p) {
            // Get the AABB's vertices in a format suitable for GatherPolygonProjectionExtents.
            List<Vector2> AABBVerts = new List<Vector2>();
            AABBVerts.Add(new Vector2(a.position.X, a.position.Y));
            AABBVerts.Add(new Vector2(a.position.X + a.Width, a.position.Y));
            AABBVerts.Add(new Vector2(a.position.X + a.Width, a.position.Y + a.Height));
            AABBVerts.Add(new Vector2(a.position.X, a.position.Y + a.Height));

            for (int i = 0; i < p.vertices.Count; i++) {
                Vector2 edge;
                if (i != p.vertices.Count - 1) edge = p.vertices[i + 1] - p.vertices[i];
                else edge = p.vertices[0] - p.vertices[i];
                Vector2 axis = new Vector2(edge.Y, -edge.X);
                axis.Normalize();
                
                float aMin, aMax, bMin, bMax;
                GatherPolygonProjectionExtents(axis, p.vertices, out aMin, out aMax);
                GatherPolygonProjectionExtents(axis, AABBVerts, out bMin, out bMax);

                if (aMin > bMax) return false;
                if (aMax < bMin) return false;
            }

            for(int i = 0; i < AABBVerts.Count; i++) {
                Vector2 edge;
                if (i != AABBVerts.Count - 1) edge = AABBVerts[i + 1] - AABBVerts[i];
                else edge = AABBVerts[0] - AABBVerts[i];
                Vector2 axis = new Vector2(edge.Y, -edge.X);
                axis.Normalize();

                float aMin, aMax, bMin, bMax;
                GatherPolygonProjectionExtents(axis, p.vertices, out aMin, out aMax);
                GatherPolygonProjectionExtents(axis, AABBVerts, out bMin, out bMax);

                if (aMin > bMax) return false;
                if (aMax < bMin) return false;
            }

            return true;
        }

        private static bool AABBCircle(AABBCollider a, CircleCollider c) {
            List<Vector2> AABBVerts = new List<Vector2>();
            AABBVerts.Add(new Vector2(0, 0));
            AABBVerts.Add(new Vector2(a.Width, 0));
            AABBVerts.Add(new Vector2(a.Width, a.Height));
            AABBVerts.Add(new Vector2(0, a.Height));
            PolygonCollider hax = new PolygonCollider(a.position.X, a.position.Y, AABBVerts);
            return PolygonCircle(hax, c);
        }

        private static bool AABBHalfPlane(AABBCollider a, HalfPlaneCollider hp) {
            // Check if any of the AABB's vertices satisfies the half-plane's equation.
            if (hp.A * a.position.X + hp.B * a.position.Y <= hp.C) return true;
            if (hp.A * a.position.X + hp.B * (a.position.Y + a.Height) <= hp.C) return true;
            if (hp.A * (a.position.X + a.Width) + hp.B * a.position.Y <= hp.C) return true;
            if (hp.A * (a.position.X + a.Width) + hp.B * (a.position.Y + a.Height) <= hp.C) return true;
            return false;
        }

        private static bool PolygonCircle(PolygonCollider p, CircleCollider c) {
            // Circle projection: project center, add and subtract radius.
            // Circle - only one axis, runs from the center of the circle to the closest vertex on the polygon.
            for (int i = 0; i < p.vertices.Count; i++) {
                // Get the axis to project the polygons' points onto.
                Vector2 edge;
                if (i != p.vertices.Count - 1) edge = p.vertices[i + 1] - p.vertices[i];
                else edge = p.vertices[0] - p.vertices[i];
                Vector2 axis = new Vector2(edge.Y, -edge.X); // Rotate the vector 90 degrees to get the axis vector. Rotation direction doesn't matter.
                axis.Normalize(); // Normalize. It only needs to describe direction.

                // Project each polygon onto the axis. Remember the extents (minimum and maximum) of each projection.
                float aMin, aMax, bMin, bMax;
                GatherPolygonProjectionExtents(axis, p.vertices, out aMin, out aMax);
                GatherCircleProjectionExtents(axis, c, out bMin, out bMax);

                // Test for overlap between the two projections.
                if (aMin > bMax) return false;
                if (aMax < bMin) return false;
            }

            // When checking against a circle we are interested in only one axis.
            // It goes from the center of the circle to the closest vertex of the polygon.
            // We need to find the closest vertex.
            Vector2 closestVertex = p.vertices[0];
            float distance = (c.position - closestVertex).LengthSquared(); // We can use squared length.
            for(int i = 0; i < p.vertices.Count; i++) {
                float dist = p.vertices[i].LengthSquared();
                if (dist < distance) {
                    distance = dist;
                    closestVertex = p.vertices[i];
                }
            }

            Vector2 circleEdge = closestVertex - c.position;
            Vector2 circleAxis = new Vector2(circleEdge.Y, -circleEdge.X);
            circleAxis.Normalize();

            float caMin, caMax, cbMin, cbMax;
            GatherPolygonProjectionExtents(circleAxis, p.vertices, out caMin, out caMax);
            GatherCircleProjectionExtents(circleAxis, c, out cbMin, out cbMax);

            if (caMin > cbMax) return false;
            if (caMax < cbMin) return false;
            return true;
        }

        private static bool PolygonHalfPlane(PolygonCollider p, HalfPlaneCollider hp) {
            // Just check if any of the polygon's vertices satisfies the half-plane's equation.
            foreach(Vector2 v in p.vertices) {
                if (hp.A * v.X + hp.B * v.Y <= hp.C) return true;
            }
            // If none does, there is no intersection.
            return false;
        }

        private static bool CircleHalfPlane(CircleCollider c, HalfPlaneCollider hp) {
            // Distance from the circle's center to the half-plane.
            float distance = Math.Abs(hp.A * (c.position.X + c.center.X) + hp.B * (c.position.Y + c.center.Y) + hp.C) / (float) Math.Sqrt(hp.A * hp.A + hp.B * hp.B);
            if (distance < c.Radius) return true;
            return false;
        }

        // Finds the maximum and minimum values of a polygon's projection onto an axis.
        private static void GatherPolygonProjectionExtents(Vector2 projectionAxis, List<Vector2> polygonVertices, out float min, out float max) {
            // We use the first vertex as a starting point.
            min = Vector2.Dot(polygonVertices[0], projectionAxis);
            max = min;
            for (int i = 1; i < polygonVertices.Count; i++) {
                // The dot product is a projection of one vector onto another.
                float projection = Vector2.Dot(polygonVertices[i], projectionAxis);
                if (projection < min) min = projection;
                else if (projection > max) max = projection;
            }
        }

        // Finds the maximum and minimum values of a circle's projection onto an axis.
        private static void GatherCircleProjectionExtents(Vector2 projectionAxis, CircleCollider circle, out float min, out float max) {
            // Project the circle's center onto the axis with a dot product.
            float centerProjection = Vector2.Dot(circle.position + circle.center, projectionAxis);
            // Add and subtract the radius to get the extents.
            min = centerProjection - circle.Radius;
            max = centerProjection + circle.Radius;
        }
    }
}
