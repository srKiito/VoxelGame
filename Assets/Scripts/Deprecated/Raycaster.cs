using UnityEngine;

public class Raycaster : MonoBehaviour
{
    public GameObject terrain;
    private MeshCreator tScript;
    public GameObject target;
    private LayerMask layerMask = (1 << 0);

    void Start () 
    {
        tScript=terrain.GetComponent<MeshCreator>();  
    }

    void FixedUpdate () 
    {
        RaycastHit hit;
        
        float distance=Vector3.Distance(transform.position,target.transform.position);

        if( Physics.Raycast(transform.position, (target.transform.position -
            transform.position).normalized, out hit, distance , layerMask)){
        
            

            Vector2 point= new Vector2(hit.point.x, hit.point.y); 
            point+=(new Vector2(hit.normal.x,hit.normal.y))*-0.5f;
            Debug.DrawLine(transform.position,point,Color.red);
            tScript.blocks[Mathf.RoundToInt(point.x-.5f),Mathf.RoundToInt(point.y+.5f)]=0;
            tScript.UpdateTerrain();

        } else {
            Debug.DrawLine(transform.position,target.transform.position,Color.blue);
        }
    }
}
