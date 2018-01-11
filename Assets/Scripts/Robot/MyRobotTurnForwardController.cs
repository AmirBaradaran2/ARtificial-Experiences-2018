using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRobotTurnForwardController : MonoBehaviour {

    private MyRobotTurnForwardCharacter character;

    public float offset = 15f;
    public GameObject offsetTarget;
    public Vector3 targetRotation;

    private Vector3 targetPos = new Vector3(-100, -100, -100);
    private Vector3 m_move;

    private static bool sitting = false;
    private static bool lying = false;
    private static bool pickingup = false;

    private static bool do_sitting = true;
    private static bool do_lying = false;
    private static bool do_pickingup = false;

    private GameObject targetObj;
    private GameObject prevParent;
    [SerializeField] private GameObject rightHand;

    // Use this for initialization
    void Start () {
        character = GetComponent<MyRobotTurnForwardCharacter>();
    }

    IEnumerator Pickup()
    {
        yield return new WaitForSeconds(3);
        print("Pick up " + targetObj.name);
        print("transform.position = " + transform.position);
        moveToward(targetObj, offsetTarget == null ? targetObj.transform.position : offsetTarget.transform.position, targetRotation, true, false, false);
    }

    public void moveToward(GameObject obj, Vector3 targetPos, Vector3 targetRotation, bool pickup, bool lay, bool sit)
    {
        targetObj = obj;

        do_pickingup = pickup ? true : false;
        do_lying = lay ? true : false;
        do_sitting = sit ? true : false;

        this.targetPos = targetPos;
        print(Time.time + ": " + targetPos);
        this.targetRotation = targetRotation;
        m_move = targetPos - character.transform.position;
        print(Time.time + "] m_Move: " + m_move);
    }

    public IEnumerator putDown(GameObject obj)
    {
        pickingup = false;
        character.Move(m_move, sitting, lying, pickingup);
        print("[RobotController]: Putting down.");
        yield return new WaitForSeconds(1.7f);
        obj.transform.parent = prevParent.transform;
        obj.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void StandUp()
    {
        if (do_sitting) sitting = false;
        if (do_lying) lying = false;
        print("Stand up.");
    }

    private void FixedUpdate()
    {
        Vector3 c = character.transform.position;
        Vector3 t = new Vector3(0, 0, 0);
        if (targetObj != null)
            t = targetObj.transform.position;
        float o = do_pickingup ? offset : 0.1f;

        if (targetObj != null && (Mathf.Abs((c.x - t.x) * (c.x - t.x)) > o || Mathf.Abs((c.z - t.z) * (c.z - t.z)) > o))
        {
            print("Move");
            if (!pickingup && !sitting && !lying)
                m_move = targetObj.transform.position - character.transform.position;
        }
        else
        {
            if (do_pickingup && !m_move.Equals(new Vector3(0, 0, 0)) && !pickingup)
            {
                print("Picking up.");
                m_move = new Vector3(0, 0, 0);
                pickingup = true;
                StartCoroutine(switchTargetParent());
            }

            if (do_sitting && !m_move.Equals(new Vector3(0, 0, 0)) && !sitting)
            {
                print("Sitting");
                m_move = new Vector3(0, 0, 0);
                transform.rotation = Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z);

                sitting = true;

                //StartCoroutine(standUpFromSitting());
            }

            if (do_lying && !m_move.Equals(new Vector3(0, 0, 0)) && !lying)
            {
                print("Lying");
                m_move = new Vector3(0, 0, 0);
                transform.rotation = Quaternion.Euler(targetRotation.x, targetRotation.y, targetRotation.z);
                transform.position += new Vector3(0, 0, 7);
                lying = true;

                //StartCoroutine(standUpFromLying());                
            }
        }
        character.Move(m_move, sitting, lying, pickingup);
    }

    IEnumerator switchTargetParent()
    {
        yield return new WaitForSeconds(0.5f);
        targetObj.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(1.5f);

        prevParent = targetObj.transform.parent.gameObject;
        targetObj.transform.parent = rightHand.transform;

        //yield return putDown(targetObj);

        /*float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 2;
            targetObj.transform.position = Vector3.Lerp(targetObj.transform.position, targetObj.transform.position + new Vector3(0, 0.05f, 0), t);
            yield return null;
        }*/

        targetObj = null;
    }

    IEnumerator standUpFromSitting()
    {
        yield return new WaitForSeconds(5);
        sitting = false;
        print("Stand up.");
    }

    IEnumerator standUpFromLying()
    {
        yield return new WaitForSeconds(5);
        lying = false;
        transform.position += new Vector3(0, 0, 7);
        print("Stand up.");
    }

    private IEnumerator move(GameObject obj, Vector3 target_pos, float duration)
    {
        yield return new WaitForSeconds(5);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            obj.transform.position = Vector3.Lerp(obj.transform.position, target_pos, t);
            yield return null;
        }
    }
}
