using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject uiObject;

    void Start() {
        uiObject.SetActive(true);
    }

    void OnTriggerEnter(Collider player) {
        Debug.Log("On Trigger Enter");
        if(player.gameObject.name == "MyMan" || player.gameObject.name == "MyGal") {
            Debug.Log("Uomo");
            Transform trans = uiObject.transform;
            Transform childTrans = trans.Find("PickUpText");
            childTrans.gameObject.SetActive(true);
            // if (Input.GetKeyDown(KeyCode.F)) 
            // {/
                // Debug.Log("Input");
            player.gameObject.GetComponent<ObjectsController>().pickUp(transform);
            // }
        }
    }

    void OnTriggerExit(Collider player) {
        if(player.gameObject.name == "MyMan" || player.gameObject.name == "MyGal") {
            Transform trans = uiObject.transform;
            Transform childTrans = trans.Find("PickUpText");
            childTrans.gameObject.SetActive(true);
        }
    }
}
