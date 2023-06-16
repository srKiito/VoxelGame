using System.Collections;
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
        
        blocks = new byte[10, 10];
        GenTerrain();
        BuildMesh();
        UpdateMesh();

    }

    
    private void Update()
    {
        
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
        Debug.Log(mesh.vertices.Length);
        Debug.Log(mesh.triangles.Length);
    }

    private void GenSquare(int x, int y, Vector2 texture)
    {
        Vector3[] newVertex = new Vector3[4];
        newVertex[0] = new Vector3 (x  , y  ,0);
        newVertex[1] = new Vector3 (x + 1 , y  , 0);
        newVertex[2] = new Vector3 (x + 1 , y-1 , 0);
        newVertex[3] = new Vector3 (x  , y-1 , 0);
        List<int> vertexIndex = new List<int>();

        //merging vertex
        foreach(Vector3 vert in newVertex){
            var index = newVertices.IndexOf(vert);
            if(index != -1){
                vertexIndex.Add(index);
            }else {
                newVertices.Add(vert);
                vertexIndex.Add(newVertices.Count-1);
            }
        }
        
        newTriangles.Add(vertexIndex[0]);
        newTriangles.Add(vertexIndex[1]);
        newTriangles.Add(vertexIndex[3]);
        newTriangles.Add(vertexIndex[1]);
        newTriangles.Add(vertexIndex[2]);
        newTriangles.Add(vertexIndex[3]);
        
        newUV.Add(new Vector2 (tUnit *texture.x, tUnit *texture.y + tUnit));
        newUV.Add(new Vector2 (tUnit *texture.x + tUnit, tUnit *texture.y + tUnit));
        newUV.Add(new Vector2 (tUnit *texture.x + tUnit, tUnit *texture.y));
        newUV.Add(new Vector2 (tUnit *texture.x, tUnit *texture.y));

        squareCount++;
    }

    private void GenTerrain()
    {
        for(int px=0;px<blocks.GetLength(0);px++){
            for(int py=0;py<blocks.GetLength(1);py++){
                if(py==blocks.GetLength(1)){
                    blocks[px,py]=2;
                } else{
                    blocks[px,py]=1;
                }
            }
        }
    }

     
    private void BuildMesh()
    {
        for(int px=0;px<blocks.GetLength(0);px++){
            for(int py=0;py<blocks.GetLength(1);py++){
                if(blocks[px,py] == 0)return;

                GenCollider(px, py);
                if(blocks[px,py]==1){
                    GenSquare(px,py,tGrass);
                } else if(blocks[px,py]==2){
                    GenSquare(px,py,tStone);
                }    
            }
        }
    }

    private void GenCollider(int x, int y)
    {
        /*if(Block(x,y+1)==0){
            colVertices.Add( new Vector3 (x  , y  , 1));
            colVertices.Add( new Vector3 (x + 1 , y  , 1));
            colVertices.Add( new Vector3 (x + 1 , y  , 0 ));
            colVertices.Add( new Vector3 (x  , y  , 0 ));
            
            ColliderTriangles();
            
            colCount++;
        }
        
        //bot
        if(Block(x,y-1)==0){
            if(Block(x+1,y)==0 && Block(x-1,y)==0){
                colVertices.Add( new Vector3 (x  , y -1 , 0));
                colVertices.Add( new Vector3 (x + 1 , y -1 , 0));
                colVertices.Add( new Vector3 (x + 1 , y -1 , 1));
                colVertices.Add( new Vector3 (x  , y -1 , 1));
                
                ColliderTriangles();
                
                colCount++;
            }

            if(Block(x+1,y)!=0 && Block(x-1,y)==0){
                tColVertices[0] = new Vector3 (x  , y -1 , 0);
                tColVertices[3] = new Vector3 (x  , y -1 , 1);
                Debug.Log(tColVertices.Length);

                tColVerCount+= 2;
            }

            if(Block(x+1,y)==0 && Block(x-1,y)!=0){
                tColVertices[1] = new Vector3 (x + 1 , y -1 , 0);
                tColVertices[2] = new Vector3 (x + 1 , y -1 , 1);
                Debug.Log(tColVertices.Length);

                tColVerCount+= 2;
            }
            
        }

        if(tColVerCount == 4){
            colVertices.AddRange(tColVertices);
            ColliderTriangles();
                
            colCount++;
            tColVerCount = 0;
        }
        
        //left
        if(Block(x-1,y)==0){
            if(Block(x,y+1)==0 && Block(x,y-1)==0){
                colVertices.Add( new Vector3 (x  , y -1 , 1));
                colVertices.Add( new Vector3 (x  , y  , 1));
                colVertices.Add( new Vector3 (x  , y  , 0 ));
                colVertices.Add( new Vector3 (x  , y -1 , 0 ));
                
                ColliderTriangles();
                
                colCount++;
            }

            if(Block(x,y+1)!=0 && Block(x,y-1)==0){
                rColVertices[0] = new Vector3 (x  , y -1 , 1);
                rColVertices[3] = new Vector3 (x  , y -1 , 0 );
                Debug.Log(rColVertices.Length);

                rColVerCount+= 2;
            }

            if(Block(x,y+1)==0 && Block(x,y-1)!=0){
                rColVertices[1] = new Vector3 (x  , y  , 1);
                rColVertices[2] = new Vector3 (x  , y  , 0 );
                Debug.Log(rColVertices.Length);

                rColVerCount+= 2;
            }
            
        }

        if(rColVerCount == 4){
            colVertices.AddRange(rColVertices);
            ColliderTriangles();
                
            colCount++;
            rColVerCount = 0;
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

        colVertices.Add( new Vector3 (x  , y  ,0));
        colVertices.Add( new Vector3 (x + 1 , y  , 0));
        colVertices.Add( new Vector3 (x + 1 , y-1 , 0));
        colVertices.Add( new Vector3 (x  , y-1 , 0));

        ColliderTriangles();

        colCount++;

        colVertices.Add( new Vector3 (x  , y  ,1));
        colVertices.Add( new Vector3 (x + 1 , y  , 1));
        colVertices.Add( new Vector3 (x + 1 , y-1 , 1));
        colVertices.Add( new Vector3 (x  , y-1 , 1));

        ColliderTriangles();

        colCount++;

        /*if(Block(x+1,y)==0 && Block(x-1,y)==0){
                colVertices.Add( new Vector3 (x  , y -1 , 0));
                colVertices.Add( new Vector3 (x + 1 , y -1 , 0));
                colVertices.Add( new Vector3 (x + 1 , y -1 , 1));
                colVertices.Add( new Vector3 (x  , y -1 , 1));
                
                ColliderTriangles();
                
                colCount++;
            }

            if(Block(x+1,y)!=0 && Block(x-1,y)==0){
                tColVertices[0] = new Vector3 (x  , y -1 , 0);
                tColVertices[3] = new Vector3 (x  , y -1 , 1);
                Debug.Log(tColVertices.Length);

                tColVerCount+= 2;
            }

            if(Block(x+1,y)==0 && Block(x-1,y)!=0){
                tColVertices[1] = new Vector3 (x + 1 , y -1 , 0);
                tColVertices[2] = new Vector3 (x + 1 , y -1 , 1);
                Debug.Log(tColVertices.Length);

                tColVerCount+= 2;
        }
        if(tColVerCount == 4){
            colVertices.AddRange(tColVertices);
            ColliderTriangles();
                
            colCount++;
            tColVerCount = 0;
        }*/
        

    }

    private void ColliderTriangles()
    {
        colTriangles.Add(colCount*4);
        colTriangles.Add((colCount*4)+1);
        colTriangles.Add((colCount*4)+3);
        colTriangles.Add((colCount*4)+1);
        colTriangles.Add((colCount*4)+2);
        colTriangles.Add((colCount*4)+3);
        // Debug.Log("foi");
    }

    private byte Block(int x, int y)
    {
        if(x == -1 || y == -1 || x == blocks.GetLength(0) || y == blocks.GetLength(1)) return 0;
        
        return blocks[x,y];
    }

    private int Noise (int x, int y, float scale, float mag, float exp){

        return (int) (Mathf.Pow ((Mathf.PerlinNoise(x/scale,y/scale)*mag), (exp))); 
  
    }
}
