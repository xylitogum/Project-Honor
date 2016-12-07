using UnityEngine;
using System.Collections;

public class CameraLock : MonoBehaviour {
    public GameObject target;
    public float height;
	private float size;
    public float minSize;
    public float maxSize;
    public float scalingSpeed;
	private float scaleSize = 1f;

    // Use this for initialization
    void Start () {
		size = maxSize;
        GetComponent<Camera>().orthographicSize = size;
		scaleSize = 1f;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(Input.mouseScrollDelta);

        float d = Mathf.Pow(Mathf.Clamp(getMouseDistance(), 0.4f, 1f) + 0.6f, 2f) - 0.2f;
        //float d = Mathf.Pow(Mathf.Clamp(getMouseDistance(), 0f, 1f) + 1f, 1f);
        
        if (Input.mouseScrollDelta.y != 0)
        {

            size = Mathf.Clamp(size - Input.mouseScrollDelta.y * Time.deltaTime * scalingSpeed, minSize, maxSize);
			scaleSize = size / maxSize;
            
        }

        GetComponent<Camera>().orthographicSize = size * d;

        transform.position = new Vector3(
            target.transform.position.x,
            target.transform.position.y,
            -height);

    }

    float getMouseDistance()
    {
        float dX = Mathf.Abs(2f * (Mathf.Clamp(Input.mousePosition.x, 0f, Screen.width) / Screen.width - 0.5f));
        float dY = Mathf.Abs(2f * (Mathf.Clamp(Input.mousePosition.y, 0f, Screen.height) / Screen.height - 0.5f));

        //float d = 1f - ((1f - dX) * (1f - dY));
        //float d = Mathf.Max(dX , dY);
		float d = Mathf.Sqrt(dX * dX + dY * dY);
        return d;
    }

	public float getScaleSize() {
		return GetComponent<Camera>().orthographicSize / maxSize ;
	}
	
}
