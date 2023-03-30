using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsConstants : MonoBehaviour
{
    public float gravity = 9.81f;
    public float atmosphericDensity = 1.293f;

    public float areaObjectCalculation(float width1, float width2, bool _roundObject) {
        float area;

        if (_roundObject) {
            if (width1 == width2) {
                area = (Mathf.PI * Mathf.Pow((width1/2), 2));
            } else {
                area = ( Mathf.PI * width1 * width2);
            }
        } else {
            area = width1 * width2;
        }

        return area;
    }

    public float dragCoefficient(bool _roundObject) {
        //too complicated to calculate. This should be fine

        if (_roundObject) {
            return 0.5f;
        }

        return 1f;
    }

    public float terminalVelocity(float mass, float area, float dragC) {

        float termVelocity;

        termVelocity = Mathf.Sqrt((2 * mass * gravity)/(atmosphericDensity * area * dragC));

        return termVelocity;
    }

    public float dragForce(float mass, float area, float dragC, float velocity) {

        float drag;

        drag = (dragC * ((atmosphericDensity * Mathf.Pow(velocity, 2))/2) * area);

        return drag;

    }

    public float objectGravity(float mass, float currentVelocity, float width1, float width2, bool _roundObject) {
        float area = areaObjectCalculation(width1, width2, _roundObject);
        float dragC = dragCoefficient(_roundObject);
        float termVelocity = terminalVelocity(mass, area, dragC);

        float objectAcceleration;
        float newObjectVelocity;

        if (-currentVelocity < termVelocity) {

            objectAcceleration = -((((mass*gravity)-dragForce(mass, area, dragC, currentVelocity))/mass)*Time.deltaTime);
            newObjectVelocity = currentVelocity + objectAcceleration;

            return newObjectVelocity;
        }
        return currentVelocity;
    }


}
