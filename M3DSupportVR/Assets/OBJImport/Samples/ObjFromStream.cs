//Original was writen by Dummiesman, Edited by Dr David Randall and David Sibrina
using Dummiesman;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]

public class ObjFromStream : MonoBehaviour
{
	Bounds bounds;
	public static GameObject externalObj;
	public static GameObject externalObjReversedNormals;
	public Material MyMaterial;

	//public Renderer thisRenderer;
	public string TextureURL = "https://medsupportvr.000webhostapp.com/M3DSupportVR/texture.png";

	void Start () {

        //linking URL string of OBJ and MTL to new variables
        var OBJwww = new WWW("https://medsupportvr.000webhostapp.com/M3DSupportVR/object.obj");
		//var MTLwww = new WWW("https://medsupportvr.000webhostapp.com/M3DSupportVR/mtl.mtl");

		while (!OBJwww.isDone)
            System.Threading.Thread.Sleep(1);
		//while (!MTLwww.isDone)
			//System.Threading.Thread.Sleep(1);

		//create stream and loading a model into variable externalObj
		var OBJStream = new MemoryStream(Encoding.UTF8.GetBytes(OBJwww.text));
		//var MTLStream = new MemoryStream(Encoding.UTF8.GetBytes(MTLwww.text));
		externalObj = new OBJLoader().Load(OBJStream);

		Debug.Log("I am here");

		//externalObj.GetComponent<Renderer>().material = MTL;

		StartCoroutine(LoadFromLikeCoroutine());
		externalObj.GetComponentInChildren<Renderer>().material = MyMaterial;

	


		//Duplicate the loaded object and reverse the normals of the duplicate to visualise the inside surface
		externalObjReversedNormals = Instantiate(externalObj, Vector3.zero, Quaternion.identity);
		ReverseNormals(externalObjReversedNormals);


		GameObject MainCube = GameObject.Find("MainCube");

		externalObj.transform.position = MainCube.transform.position;
		externalObj.transform.eulerAngles = new Vector3(-90, 0, 0);
		externalObjReversedNormals.transform.position = MainCube.transform.position;
		externalObjReversedNormals.transform.eulerAngles = new Vector3(-90, 0, 0);

		//Change material colour properties to be more visually pleasing and get extremities of mesh to adjust 
		//position and size of the cube's collider
		boundAndChangeColour(externalObj);
		boundAndChangeColour(externalObjReversedNormals);

		//Scale object to a reasonable size based on the bounding box dimensions
		//Get value of largest dimension and use to make approx. 1m
		float MaxDim = 2 * Mathf.Max(bounds.extents.x, bounds.extents.y, bounds.extents.z); //*2 becuase extents are relative to centre coords
		float ScaleFactor = 1 / MaxDim; //Scales to largest dimension to 1m
		externalObj.transform.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);
		externalObjReversedNormals.transform.localScale = new Vector3(ScaleFactor, ScaleFactor, ScaleFactor);


		//Recalculate the new bounds based on the new scaling and use this to adjust the size of the MainCube collider
		boundAndChangeColour(externalObj); //only calculate with one of the objects as they are the same
		MainCube.transform.position = bounds.center;
		MainCube.GetComponent<BoxCollider>().size = 2.3f * bounds.extents;

		//Make loaded Object a child of the MainCube so the interactions work
		externalObj.transform.parent = MainCube.transform;
		externalObjReversedNormals.transform.parent = MainCube.transform;
	}


	private IEnumerator LoadFromLikeCoroutine()
	{
		Debug.Log("Loading ....");
		WWW wwwLoader = new WWW(TextureURL);
		yield return wwwLoader;

		Debug.Log("Loaded");
		externalObj.GetComponentInChildren<Renderer>().material.color = new Color(1, 0.87f, 0.59f, 1);
		externalObj.GetComponentInChildren<Renderer>().material.mainTexture = wwwLoader.texture;

		externalObjReversedNormals.GetComponentInChildren<Renderer>().material.color = new Color(1, 0.87f, 0.59f, 1);
		externalObjReversedNormals.GetComponentInChildren<Renderer>().material.mainTexture = wwwLoader.texture;

	}


	private Bounds boundAndChangeColour(GameObject Obj)
	{
		bounds = new Bounds(externalObj.transform.position, Vector3.zero);
		Renderer[] Materials = Obj.GetComponentsInChildren<Renderer>();
		Color SpecCol = new Color(0, 0, 0);
		foreach (Renderer Mat in Materials)
		{
			//Mat.SetColor("_SpecularColor", new Color(133, 133, 133));
			//Mat.material.color. = SpecCol;
			Mat.material.SetColor("_SpecColor", SpecCol);
			bounds.Encapsulate(Mat.bounds);
		}
		return bounds;
	}


	void ReverseNormals(GameObject Object)
	{
		MeshFilter[] filters = Object.GetComponentsInChildren<MeshFilter>();
		foreach (MeshFilter filter in filters)
		{
			if (filter != null)
			{
				Mesh mesh = filter.mesh;

				Vector3[] normals = mesh.normals;
				for (int i = 0; i < normals.Length; i++)
				{
					normals[i] = -normals[i];
				}

				mesh.normals = normals;

				for (int m = 0; m < mesh.subMeshCount; m++)
				{
					int[] triangles = mesh.GetTriangles(m);
					for (int i = 0; i < triangles.Length; i += 3)
					{
						int temp = triangles[i + 0];
						triangles[i + 0] = triangles[i + 1];
						triangles[i + 1] = temp;
					}
					mesh.SetTriangles(triangles, m);
				}
			}
		}
	}


	void Update()
	{
	}

}
