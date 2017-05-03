using UnityEngine;
using System.Collections;

public class CameraFocus : MonoBehaviour {

    public int giveX;
    public int giveY;
    public float translateSpeed;
    
    // Update is called once per frame
    void Start()
    {
        gameObject.transform.position = new Vector3(getPlayerPos().x, getPlayerPos().y,-10);
    }

    void Update () {
       updateCamera();
	}

    private void updateCamera()
    {
        var camera = transform;
        var player = getPlayerPos();
        int xDisplacement = (int)(player.x - Mathf.Round(camera.position.x));
        int yDisplacement = (int)(player.y - Mathf.Round(camera.position.y));
        if (xDisplacement > giveX)
            camera.Translate(new Vector3(translateSpeed, 0, 0));
        else if (xDisplacement < -giveX)
            camera.Translate(new Vector3(-translateSpeed, 0, 0));
        if (yDisplacement > giveY)
            camera.Translate(new Vector3(0, translateSpeed, 0));
        else if (yDisplacement < -giveY)
            camera.Translate(new Vector3(0, -translateSpeed, 0));
    }

    private Vector2 getPlayerPos()
    {
        var player = GameObject.Find("Player").transform;
        return new Vector2(player.position.x, player.position.y);
    }
}
