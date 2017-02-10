using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class remove : MonoBehaviour {

    private GameObject bg;
	// Use this for initialization
	void Start () {
        bg = GameObject.Find("bg");



    }
	
	// Update is called once per frame
	void Update () {

        for(int i=1;i<8;i++)
        {
            string s = "bg" + i;

            Transform tf = bg.gameObject.transform.FindChild(s);

            if (tf == null)
                continue;

            GameObject c=tf.gameObject;
            
            

            if (c.transform.position.y < transform.position.y - 5f || c.transform.position.y > transform.position.y + 5f)
            {
                c.GetComponent<SpriteRenderer>().enabled = false;
                continue;
            }
            c.GetComponent<SpriteRenderer>().enabled = true;
        }
		


	}
}
