using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerBehavior : MonoBehaviour {

    [SerializeField]
    private Material selected_mat;
    [SerializeField]
    private Material general_mat;

    private void OnTriggerEnter(Collider col)
    {
        //print("Trigger enter: " + col.gameObject.name);
        if (col.gameObject.name.Equals("Single_text(Clone)"))
        {
            //print("Trigger entered by Single_text(Clone)");
            GetComponent<Renderer>().sharedMaterial = selected_mat;
           // col.gameObject.transform.localScale = new Vector3(col.gameObject.transform.localScale.x, col.gameObject.transform.localScale.y, 0.5f);
            //col.gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        GetComponent<Renderer>().sharedMaterial = general_mat;
    }
}
