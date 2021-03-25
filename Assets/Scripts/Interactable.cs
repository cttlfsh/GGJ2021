using UnityEngine;

public class Interactable : MonoBehaviour
{
    // public GameObject uiObject;

    void Start() {
        
        // uiObject.SetActive(true);
    }
    
    void OnCollisionEnter(Collision player) {
        Debug.Log("On Collision Enter");
        if(player.gameObject.tag == "Player") { // || player.gameObject.name == "MyGal") {
            Debug.Log("Uomo");
            // Transform uiTransform = uiObject.transform;
            // Transform childTrans = uiTransform.Find("PickUpText");
            // childTrans.gameObject.SetActive(true);
            // if (Input.GetKeyDown(KeyCode.F)) 
            // {/
                // Debug.Log("Input");
            player.gameObject.GetComponent<ObjectsController>().pickUp(transform);
            // }
        }
    }

    void OnCollisionExit(Collision player) {
        // if(player.gameObject.tag == "Player") { // || player.gameObject.name == "MyGal") {
        //     Transform trans = uiObject.transform;
        //     Transform childTrans = trans.Find("PickUpText");
        //     childTrans.gameObject.SetActive(true);
        // }
    }
}
