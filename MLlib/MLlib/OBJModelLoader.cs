using MLlib.Vectors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MLlib
{
    public class OBJModelLoader : ModelLoader
    {
        public OBJModelLoader()
        {

        }

        public override Model LoadModel(string filename)
        {
            if (File.Exists(filename))
            {
                string[] fileContent = File.ReadAllLines(filename);

                List<Vector3D> vertices = new List<Vector3D>();
                List<Vector2D> uvCoordinates = new List<Vector2D>();
                List<Vector3D> normals = new List<Vector3D>();
                List<TriangleFace> triangles = new List<TriangleFace>();
                Model model = new Model();

                foreach (string line in fileContent)
                {
                    string newLine = line.Replace(".", ",");
                    string[] lineSplit = newLine.Split(' ');

                    if (line.StartsWith("v "))
                    {
                        vertices.Add(new Vector3D(float.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
                    }
                    if (line.StartsWith("vt "))
                    {
                        uvCoordinates.Add(new Vector2D(float.Parse(lineSplit[1]), 1 - float.Parse(lineSplit[2])));
                    }
                    else if (line.StartsWith("vn "))
                    {
                        normals.Add(new Vector3D(float.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
                    }
                    else if (line.StartsWith("f"))
                    {
                        Vertex[] vertexes = new Vertex[3];

                        for (int i = 1; i < 4; i++)
                        {
                            string[] vertexData = lineSplit[i].Split('/');

                            vertexes[i - 1] = new Vertex(vertices[int.Parse(vertexData[0]) - 1], uvCoordinates[int.Parse(vertexData[1]) - 1].Clone(), normals[int.Parse(vertexData[2]) - 1].Clone());
                        }
                        triangles.Add(new TriangleFace(vertexes[0], vertexes[1], vertexes[2]));
                    }
                }

                int indexCounter = 0;
                foreach(TriangleFace triangleFace in triangles)
                {
                    Vertex[] vertz = triangleFace.ToArray();

                    for(int i = 0; i < 3; i++)
                    {
                        model.Indices.Add(indexCounter);
                        model.Vertices.Add(vertz[i].Vertice);
                        model.UVCoordinates.Add(vertz[i].UVCoordinate);
                        model.Normals.Add(vertz[i].Normal);
                        indexCounter++;
                    }
                }

                return model;
            }
            return null;
        }

        //public override Model LoadModel(string filename)
        //{
        //    if (File.Exists(filename))
        //    {
        //        string[] fileContent = File.ReadAllLines(filename);

        //        List<Vector2D> uvCoordinates = new List<Vector2D>();
        //        List<Vector3D> normals = new List<Vector3D>();
        //        Model model = new Model();

        //        Vector2D[] uvs = null;

        //        foreach (string line in fileContent)
        //        {
        //            string newLine = line.Replace(".", ",");
        //            string[] lineSplit = newLine.Split(' ');

        //            if (line.StartsWith("v "))
        //            {
        //                model.Vertices.Add(new Vector3D(float.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
        //            }
        //            if (line.StartsWith("vt "))
        //            {
        //                uvCoordinates.Add(new Vector2D(float.Parse(lineSplit[1]), float.Parse(lineSplit[2])));
        //            }
        //            else if (line.StartsWith("vn "))
        //            {
        //                normals.Add(new Vector3D(float.Parse(lineSplit[1]), float.Parse(lineSplit[2]), float.Parse(lineSplit[3])));
        //            }
        //            else if (line.StartsWith("f"))
        //            {
        //                if (uvs == null)
        //                    uvs = new Vector2D[model.Vertices.Count];

        //                for (int i = 1; i < lineSplit.Length; i++)
        //                {
        //                    string[] vertex = lineSplit[i].Split('/');

        //                    model.Indices.Add(int.Parse(vertex[0]) - 1);
        //                    //model.UVCoordinates.Add(uvCoordinates[int.Parse(vertex[1]) - 1].Clone());
        //                    uvs[int.Parse(vertex[0]) - 1] = uvCoordinates[int.Parse(vertex[1]) - 1].Clone();
        //                    model.Normals.Add(normals[int.Parse(vertex[2]) - 1].Clone());
        //                }
        //            }
        //        }

        //        model.UVCoordinates = new List<Vector2D>(uvs);

        //        return model;
        //    }
        //    return null;
        //}

        //public override Model LoadModel(string filename)
        //{
        //    if (File.Exists(filename))
        //    {
        //        string[] fileContent = File.ReadAllLines(filename);

        //        List<Vector3D> vertices = new List<Vector3D>();
        //        List<Vector2D> textures = new List<Vector2D>();
        //        List<Vector3D> normals = new List<Vector3D>();
        //        List<int> indices = new List<int>();
        //        float[] verticesArray = null;
        //        float[] normalsArray = null;
        //        float[] texturesArray = null;
        //        int[] indicesArray = null;

        //        foreach (string line in fileContent)
        //        {
        //            string[] currentLine = line.Split(' ');

        //            if (line.StartsWith("v "))
        //            {
        //                Vector3D vertex = new Vector3D(float.Parse(currentLine[1]), float.Parse(currentLine[2]), float.Parse(currentLine[3]));
        //                vertices.Add(vertex);
        //            }
        //            else if (line.StartsWith("vt "))
        //            {
        //                Vector2D texture = new Vector2D(float.Parse(currentLine[1]), float.Parse(currentLine[2]));
        //                textures.Add(texture);
        //            }
        //            else if (line.StartsWith("vn "))
        //            {
        //                Vector3D normal = new Vector3D(float.Parse(currentLine[1]), float.Parse(currentLine[2]), float.Parse(currentLine[3]));
        //                normals.Add(normal);
        //            }
        //            else if (line.StartsWith("f "))
        //            {
        //                texturesArray = new float[vertices.Count * 2];
        //                normalsArray = new float[vertices.Count * 3];
        //                break;
        //            }
        //        }

        //        foreach (string line in fileContent)
        //        {
        //            if (!line.StartsWith("f "))
        //                continue;

        //            string[] currentLine = line.Split(' ');
        //            string[] vertex1 = currentLine[1].Split('/');
        //            string[] vertex2 = currentLine[2].Split('/');
        //            string[] vertex3 = currentLine[3].Split('/');

        //            ProcessVertex(vertex1, indices, textures, normals, texturesArray, normalsArray);
        //            ProcessVertex(vertex2, indices, textures, normals, texturesArray, normalsArray);
        //            ProcessVertex(vertex3, indices, textures, normals, texturesArray, normalsArray);
        //        }

        //        verticesArray = new float[vertices.Count * 3];
        //        indicesArray = new int[indices.Count];

        //        int vertexPointer = 0;
        //        foreach(Vector3D vertex in vertices)
        //        {
        //            verticesArray[vertexPointer++] = vertex.X;
        //            verticesArray[vertexPointer++] = vertex.Y;
        //            verticesArray[vertexPointer++] = vertex.Z;
        //        }

        //        for (int i = 0; i < indices.Count; i++)
        //            indicesArray[i] = indices[i];

        //        Model model = new Model();

        //        List<Vector3D> verts = new List<Vector3D>();
        //        for (int i = 0; i < verticesArray.Length; i += 3)
        //            verts.Add(new Vector3D(verticesArray[i], verticesArray[i + 1], verticesArray[i + 2]));

        //        List<Vector2D> texs = new List<Vector2D>();
        //        for (int i = 0; i < texturesArray.Length; i += 2)
        //            texs.Add(new Vector2D(texturesArray[i], texturesArray[i + 1]));

        //        List<Vector3D> norms = new List<Vector3D>();
        //        for (int i = 0; i < normalsArray.Length; i += 3)
        //            norms.Add(new Vector3D(normalsArray[i], normalsArray[i + 1], normalsArray[i + 2]));

        //        model.Vertices = verts;
        //        model.UVCoordinates = texs;
        //        model.Normals = norms;
        //        model.Indices = indices;
        //        return model;
        //    }
        //    return null;
        //}

        //private void ProcessVertex(string[] vertexData, List<int> indices, List<Vector2D> textures, List<Vector3D> normals, float[] textureArray, float[] normalsArray)
        //{
        //    int currentVertexPointer = int.Parse(vertexData[0]) - 1;
        //    indices.Add(currentVertexPointer);
        //    Vector2D currentTex = textures[int.Parse(vertexData[1]) - 1];
        //    textureArray[currentVertexPointer * 2] = currentTex.X;
        //    textureArray[currentVertexPointer * 2 + 1] = 1 - currentTex.Y;
        //    Vector3D currentNorm = normals[int.Parse(vertexData[2]) - 1];
        //    normalsArray[currentVertexPointer * 3] = currentNorm.X;
        //    normalsArray[currentVertexPointer * 3 + 1] = currentNorm.Y;
        //    normalsArray[currentVertexPointer * 3 + 2] = currentNorm.Z;
        //}
    }
}
