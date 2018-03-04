using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoemBehavior : MonoBehaviour {

    [SerializeField] private GameObject poem_canvas;

    private static bool oscilating;

    #region oscilation parameters
    private float degree = 0;
    private Vector3 default_position;
    public float oscillate_speed = 15.0f;
    public float oscillate_range = 0.75f;
    #endregion

    void Start () {
        //StartCoroutine(lift(transform.position + new Vector3(0, 10f, 0), transform.localScale * 2, 10));
    }
	
	void Update () {
        if (oscilating) {
            degree = Mathf.Repeat(degree + Time.deltaTime * oscillate_speed, 360.0f);
            float radian = degree * Mathf.Deg2Rad;
            Vector3 pos_temp = transform.position;
            pos_temp.y = default_position.y + oscillate_range * Mathf.Sin(10.0f * radian);
            transform.position = pos_temp;
        }
	}

    public void OnLift() {
        StartCoroutine(lift(transform.position + new Vector3(0, 10f, 0), transform.localScale * 2, 2));
    }

    private IEnumerator lift(Vector3 target_pos, Vector3 target_scale, float duration) {
        //yield return new WaitForSeconds(10);
        float t = 0;
        while (t < 0.35f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(transform.position, target_pos, t);
            transform.localScale = Vector3.Lerp(transform.localScale, target_scale, t);
            yield return new WaitForSeconds(0.01f);
        }
        
        default_position = transform.position;
        oscilating = true;
        poem_canvas.SetActive(true);
    }
}
