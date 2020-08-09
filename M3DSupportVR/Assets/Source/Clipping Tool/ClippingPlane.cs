using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingPlane : MonoBehaviour
{

	//material we pass the values to
	//public Material MyMaterial;
	Material MyMaterial;
	Material MyMaterialReversedNormals;

	private void Start()
	{
		//MyMaterial = GameObject.Find("InnerSideobject").GetComponent<MeshRenderer>().material;
		MyMaterial = ObjFromStream.externalObj.GetComponentInChildren<MeshRenderer>().material;
		MyMaterialReversedNormals = ObjFromStream.externalObjReversedNormals.GetComponentInChildren<MeshRenderer>().material;
	}

	//execute every frame
	void Update()
	{
		//create plane
		Plane plane = new Plane(transform.up, transform.position);
		//transfer values from plane to vector4
		Vector4 planeRepresentation = new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
		//pass vector to shader

		MyMaterial.SetVector("_Plane", planeRepresentation);
		MyMaterialReversedNormals.SetVector("_Plane", planeRepresentation);
	}
}