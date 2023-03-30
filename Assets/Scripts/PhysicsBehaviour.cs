using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsBehaviour : MonoBehaviour {

    [SerializeField] private float mass;
    [SerializeField] private bool round;
    private PhysicsConstants physics;
    private CharacterController objectController;   
    private Vector3 objectVelocity;

    void Start() {

        objectController = GetComponent<CharacterController>();

        physics = new PhysicsConstants();

    }

    void Update() {
        
        objectVelocity.y = physics.objectGravity(mass, objectVelocity.y, transform.localScale.x, transform.localScale.z, round);

        objectController.Move(objectVelocity * Time.deltaTime);

    }
}
