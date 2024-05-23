using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//
public class VelocityVisualizer: MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer LineR;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private Transform Player;

    void Start()
    {
        LineR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 velo = rb.velocity;
        velo.Normalize();
        velo *= (rb.velocity.magnitude/25);
        velo += Player.position;
        Vector3 projected = new Vector3(velo.x, Player.position.y, velo.z);

        LineR.SetPosition(0, Player.position);
        LineR.SetPosition(1, projected);

    }
}
