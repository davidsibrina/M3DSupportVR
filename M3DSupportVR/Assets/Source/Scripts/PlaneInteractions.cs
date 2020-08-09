using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneInteractions : MonoBehaviour
{

    public GameObject LeftHand;
    public GameObject RightHand;
    Collider MainCollider;

    private Vector3 StartScale;
    private float Scale;
    private float dist1;


    //edited by david
    private float Sensitivity = 0.15f;

    void Start()
    {
        //Load model using ObjImporter Script (also attached to this gameobject
        


        //Load model from resources folder
        //Model = Resources.Load("geometry_partitioned_o", typeof(GameObject)) as GameObject;
        //Instantiate(Model, Vector3.zero, Quaternion.identity);

        //Get start scale of grabbable cube
        StartScale = this.gameObject.transform.localScale;

        //Get start distance between hands
        dist1 = Vector3.Distance(LeftHand.transform.position, RightHand.transform.position);

        //Get objects to assign to cubes to switch the renderers on and off
        MainCollider = GameObject.Find("ClippingPlaneCube").GetComponent<Collider>();

    }

    void Update()
    {
        //Detect when grab trigger pulled and scale object if both pulled
        float LeftTriggerVal = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);
        float RightTriggerVal = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger);
        if (LeftTriggerVal > 0 && RightTriggerVal > 0)
        {
            //intialise float array to store distance between hands in two consecutive frames
            float dist2 = Vector3.Distance(LeftHand.transform.position, RightHand.transform.position);
			// David Sibrina Edit
            Scale = 4 * (dist2 - dist1);
            this.gameObject.transform.localScale = new Vector3(StartScale.x + Scale, StartScale.y, StartScale.z + Scale);
        }

        dist1 = Vector3.Distance(LeftHand.transform.position, RightHand.transform.position);

        //Update StartScale at the end of each frame to feed back into the next frame
        StartScale = this.gameObject.transform.localScale;

    }

}
