using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteerCtrl : MonoBehaviour {

    private Rigidbody steerRB;

    private void Awake()
    {
        steerRB = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (steerRB.angularVelocity.sqrMagnitude > 16f)
        {
            if (collision.gameObject.layer == BackgroundSetting.BodyPartLayer)
            {
                collision.transform.GetComponent<BodyPartCtrl>().Separated(5f); //max is 6
            }
            else
            {
                if (collision.transform.CompareTag("Head"))
                {
                    collision.transform.GetComponent<BodyPartCtrl>().Blooded();
                    collision.transform.root.GetComponent<CharacterCtrl>().Death();
                }
            }
        }
    }
}
