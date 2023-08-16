using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();

    private Mesh mesh;

    private float tUnit = 0.25f;
    private Vector2 tStone = new Vector2 (0, 1);
    private Vector2 tGrass = new Vector2 (0, 4);

    public byte[,] blocks;

    private int squareCount;

    private List<Vector3> colVertices = new List<Vector3>();
    private List<int> colTriangles = new List<int>();
    private int colCount;

    private MeshCollider col;

    private Vector3[] rColVertices = new Vector3[4];
    private int rColVerCount;

    private Vector3[] bColVertices = new Vector3[4];
    private int bColVerCount;

    private Vector3[] tColVertices = new Vector3[4];
    private int tColVerCount;

    private Vector3[] lColVertices = new Vector3[4];
    private int lColVerCount;
    
    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        col = GetComponent<MeshCollider> ();

        GenTerrain();
        BuildMesh();
        UpdateMesh();

    }

    private void UpdateMesh()
    {
        mesh.Clear ();
        mesh.vertices = newVertices.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.Optimize ();
        mesh.RecalculateNormals ();

        squareCount=0;
        newVertices.Clear();
        newTriangles.Clear();
        newUV.Clear();

        Mesh newMesh = new Mesh();
        newMesh.vertices = colVertices.ToArray();
        newMesh.triangles = colTriangles.ToArray();
        col.sharedMesh= newMesh;

        colVertices.Clear();
        colTriangles.Clear();
        colCount=0;
    }

    private void GenSquare(int x, int y, Vector2 texture)
    {
        newVertices.Add( new Vector3 (x  , y  , 0 ));
        newVertices.Add( new Vector3 (x + 1 , y  , 0 ));
        newVertices.Add( new Vector3 (x + 1 , y-1 , 0 ));
        newVertices.Add( new Vector3 (x  , y-1 , 0 ));
        
        newTriangles.Add(squareCount*4);
        newTriangles.Add((squareCount*4)+1);
        newTriangles.Add((squareCount*4)+3);
        newTriangles.Add((squareCount*4)+1);
        newTriangles.Add((squareCount*4)+2);
        newTriangles.Add((squareCount*4)+3);
        
        newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y + tUnit));
        newUV.Add(new Vector2 (tUnit*texture.x+tUnit, tUnit*texture.y+tUnit));
        newUV.Add(new Vector2 (tUnit * texture.x + tUnit, tUnit * texture.y));
        newUV.Add(new Vector2 (tUnit * texture.x, tUnit * texture.y));
        
        squareCount++;
    }

    private void GenTerrain()
    {
        blocks=new byte[96,128];
  
        for(int px=0;px<blocks.GetLength(0);px++){
            int stone= Noise(px,0, 80,15,1);
            stone+= Noise(px,0, 50,30,1);
            stone+= Noise(px,0, 10,10,1);
            stone+=75;
            
            int dirt = Noise(px,0, 100,35,1);
            dirt+= Noise(px,0, 50,30,1);
            dirt+=75;
            
            for(int py=0;py<blocks.GetLength(1);py++){
                if(py<stone){
                    blocks[px,py]=1;
                    // Debug.Log("predra" + px);

                    //The next three lines make dirt spots in random places
                    if(Noise(px,py,12,16,1)>10){
                        
                    
                    }
                    
                    //The next three lines remove dirt and rock to make caves in certain places
                    if(Noise(px,py*2,16,14,1)>10){ //Caves
                        blocks[px,py]=0;
                    }
                } else if(py<dirt) {
                    blocks[px,py]=2;
                    // Debug.Log("terra" + px);
                }
            }
        }

        /*for(int px=0;px<blocks.GetLength(0);px++){
            for(int py=0;py<blocks.GetLength(1);py++){
                if(py==blocks.GetLength(1)-1){
                    blocks[px,py]=2;
                } else if(py<blocks.GetLength(1)-1){
                    blocks[px,py]=1;
                }
            }
        }*/
    }

     
    private void BuildMesh()
    {
        for(int px=0;px<blocks.GetLength(0);px++){
            for(int py=0;py<blocks.GetLength(1);py++){
                if(blocks[px,py] != 0) GenCollider(px, py);;
         
                if(blocks[px,py]==1){
                    GenSquare(px,py,tGrass);
                    Debug.Log(blocks[px,py] + "Grama" + px);
                } else if(blocks[px,py]==2){
                    GenSquare(px,py,tStone);
                    Debug.Log(blocks[px,py] + "Pedra" + px);
                }    
            }
        }
    }

    private void GenCollider(int x, int y)
    {
        //Top
        if(Block(x,y+1)==0){
        colVertices.Add( new Vector3 (x  , y  , 1));
        colVertices.Add( new Vector3 (x + 1 , y  , 1));
        colVertices.Add( new Vector3 (x + 1 , y  , 0 ));
        colVertices.Add( new Vector3 (x  , y  , 0 ));
        
        ColliderTriangles();
        
        colCount++;
        }
        
        //bot
        if(Block(x,y-1)==0){
        colVertices.Add( new Vector3 (x  , y -1 , 0));
        colVertices.Add( new Vector3 (x + 1 , y -1 , 0));
        colVertices.Add( new Vector3 (x + 1 , y -1 , 1 ));
        colVertices.Add( new Vector3 (x  , y -1 , 1 ));
        
        ColliderTriangles();
        colCount++;
        }
        
        //left
        if(Block(x-1,y)==0){
        colVertices.Add( new Vector3 (x  , y -1 , 1));
        colVertices.Add( new Vector3 (x  , y  , 1));
        colVertices.Add( new Vector3 (x  , y  , 0 ));
        colVertices.Add( new Vector3 (x  , y -1 , 0 ));
        
        ColliderTriangles();
        
        colCount++;
        }
        
        //right
        if(Block(x+1,y)==0){
        colVertices.Add( new Vector3 (x +1 , y  , 1));
        colVertices.Add( new Vector3 (x +1 , y -1 , 1));
        colVertices.Add( new Vector3 (x +1 , y -1 , 0 ));
        colVertices.Add( new Vector3 (x +1 , y  , 0 ));
        
        ColliderTriangles();
        
        colCount++;
        }
    }

    private void ColliderTriangles()
    {
        colTriangles.Add(colCount*4);
        colTriangles.Add((colCount*4)+1);
        colTriangles.Add((colCount*4)+3);
        colTriangles.Add((colCount*4)+1);
        colTriangles.Add((colCount*4)+2);
        colTriangles.Add((colCount*4)+3);
    }

    private byte Block(int x, int y)
    {
        if(x == -1 || y == -1 || x == blocks.GetLength(0) || y == blocks.GetLength(1)) return 0;
        
        return blocks[x,y];
    }

    private int Noise (int x, int y, float scale, float mag, float exp)
    {
        return (int) (Mathf.Pow ((Mathf.PerlinNoise(x/scale,y/scale)*mag), (exp))); 
    }

    public void UpdateTerrain()
    {
        BuildMesh();
        UpdateMesh();
    }

}
