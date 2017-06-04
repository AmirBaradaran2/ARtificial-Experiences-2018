using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable_text : MonoBehaviour {

    private void OnCollisionEnter(Collision col)
    {
        //Debug.Log("On collision enter");
        //Debug.Log("Collision tag: " + col.gameObject.tag);
        // Debug.Log("Collision name: " + col.gameObject.name);
        /*if (col.gameObject.tag.Equals("Controller"))
        {
            Debug.Log("Dragged");
            // dragged
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.05f);
            GetComponent<Rigidbody>().useGravity = true;
        }*/
    }

    private void OnTriggerEnter(Collider col)
    {
        //Debug.Log("On trigger enter: " + col.name);
        if (col.gameObject.name.Equals("Container"))
        {
            StartCoroutine(onTriggerBehavior(col));
        }
    }

    IEnumerator onTriggerBehavior(Collider col) {
        if(!GetComponent<Rigidbody>().useGravity)
            GetComponent<Rigidbody>().useGravity = true;
        transform.localScale = new Vector3(2*col.gameObject.transform.localScale.x, 2*col.gameObject.transform.localScale.y, 0.5f);
        print("Use gravity");
        yield return new WaitForSeconds(0.1f);
    }
}
