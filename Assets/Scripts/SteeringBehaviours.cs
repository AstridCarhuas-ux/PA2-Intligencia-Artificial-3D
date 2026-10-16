using System.Collections.Generic;
using UnityEngine;

namespace AI.SteeringBehaviours
{
    public static class SteeringBehaviours
    {
        public static Vector3 Seek(IAgent agent, Vector3 target)
        {
            Vector3 desired = (target - agent.Position);
            desired.y = 0;
            if (desired == Vector3.zero) return Vector3.zero;

            desired = desired.normalized * agent.MaxSpeed;
            Vector3 steer = Vector3.ClampMagnitude(desired - agent.Velocity, agent.MaxForce);
            return steer;
        }

        public static Vector3 Flee(IAgent agent, Vector3 threat)
        {
            Vector3 desired = (agent.Position - threat);
            desired.y = 0;
            if (desired == Vector3.zero) return Vector3.zero;

            desired = desired.normalized * agent.MaxSpeed;
            Vector3 steer = Vector3.ClampMagnitude(desired - agent.Velocity, agent.MaxForce);
            return steer;
        }

        public static Vector3 Arrive(IAgent agent, Vector3 target, float slowingRadius)
        {
            Vector3 targetOffset = target - agent.Position;
            targetOffset.y = 0;
            float distance = targetOffset.magnitude;

            if (distance < 0.01f) return -agent.Velocity;

            float rampedSpeed = agent.MaxSpeed * (distance / slowingRadius);
            float limitedSpeed = Mathf.Min(rampedSpeed, agent.MaxSpeed);
            Vector3 desired = (targetOffset / distance) * limitedSpeed;

            Vector3 steer = Vector3.ClampMagnitude(desired - agent.Velocity, agent.MaxForce);
            return steer;
        }

        public static Vector3 InsideCircle(IAgent agent, Vector3 center, float radius)
        {
            Vector3 offset = agent.Position - center;
            offset.y = 0;
            if (offset.magnitude > radius)
            {
                return Seek(agent, center);
            }
            return Vector3.zero;
        }

        public static Vector3 Separate(IAgent agent, List<IAgent> flock, float neighborRadius)
        {
            Vector3 steer = Vector3.zero;
            int count = 0;

            foreach (IAgent other in flock)
            {
                if (other == agent) continue;
                Vector3 diff = agent.Position - other.Position;
                diff.y = 0;
                float d = diff.magnitude;

                if (d > 0 && d < neighborRadius)
                {
                    diff = diff.normalized / d;
                    steer += diff;
                    count++;
                }
            }

            if (count > 0)
            {
                steer /= count;
                if (steer != Vector3.zero)
                {
                    steer = steer.normalized * agent.MaxSpeed;
                    steer = Vector3.ClampMagnitude(steer - agent.Velocity, agent.MaxForce);
                }
            }
            return steer;
        }

        public static Vector3 Cohesion(IAgent agent, List<IAgent> flock, float neighborRadius)
        {
            Vector3 centerOfMass = Vector3.zero;
            int count = 0;

            foreach (IAgent other in flock)
            {
                if (other == agent) continue;
                Vector3 diff = agent.Position - other.Position;
                diff.y = 0;
                if (diff.magnitude < neighborRadius)
                {
                    centerOfMass += other.Position;
                    count++;
                }
            }

            if (count > 0)
            {
                centerOfMass /= count;
                return Seek(agent, centerOfMass);
            }
            return Vector3.zero;
        }

        public static Vector3 Alignment(IAgent agent, List<IAgent> flock, float neighborRadius)
        {
            Vector3 avgVelocity = Vector3.zero;
            int count = 0;

            foreach (IAgent other in flock)
            {
                if (other == agent) continue;
                Vector3 diff = agent.Position - other.Position;
                diff.y = 0;
                if (diff.magnitude < neighborRadius)
                {
                    avgVelocity += other.Velocity;
                    count++;
                }
            }

            if (count > 0)
            {
                avgVelocity /= count;
                avgVelocity.y = 0;
                avgVelocity = avgVelocity.normalized * agent.MaxSpeed;
                Vector3 steer = Vector3.ClampMagnitude(avgVelocity - agent.Velocity, agent.MaxForce);
                return steer;
            }
            return Vector3.zero;
        }
    }
}