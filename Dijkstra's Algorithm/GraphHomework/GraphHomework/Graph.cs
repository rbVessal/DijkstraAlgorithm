#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion
//@author: Rebecca Vessal and Professor Cascioli
//Instructor: Professor Cascioli
//Date: 5/7/11 - 5/8/11
//
//Graph.cs
//
//Graph.cs contains methods for highlighting the shortest path based on Dijkstra's Algorithm

namespace GraphHomework
{
    /// <summary>
    /// A graph for pathfinding
    /// </summary>
    public class Graph
    {
        #region Attributes
        // Assets
        private Texture2D pixel;
        private Texture2D vertexTexture;
        private SpriteFont font;

        // The list of vertices in the graph
        // Note that the index in this list is also the vertex's row/col in the matrix.
        List<Vertex> vertices;
        //Declare a list to be used to store the visited vertices
        List<Vertex> visitedVerticesList;

        //Create a current vertex that is the working node
        Vertex current;
        //Create a source vertex
        Vertex source;
        
        // The dictionary of vertices, to look up their indices quickly
        Dictionary<String, int> vertNameToIndex;

        // Adjacency matrix
        int[,] adjMatrix;

        // A matrix of colors for the edges.
        // We'll use this for drawing the result of Dijkstra's algorithm.
        Color[,] edgeColor;

        #endregion

        #region Constants
        // The maximum number of vertices
        public const int MAX_VERTICES = 10;

        // The "width" of an edge when drawn
        public const int EDGE_WIDTH = 2;

        // Colors
        public Color NORMAL_EDGE_COLOR = Color.Brown;
        public Color HIGHLIGHT_EDGE_COLOR = Color.Orange;
        public Color VERTEX_NAME_COLOR = Color.CornflowerBlue;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the sprite font for drawing text
        /// </summary>
        public SpriteFont Font { get { return font; } set { font = value; } }

        /// <summary>
        /// Gets or sets the texture for the vertices
        /// </summary>
        public Texture2D VertexTexture { get { return vertexTexture; } set { vertexTexture = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new graph
        /// </summary>
        /// <param name="device">The graphics device for this XNA game</param>
        public Graph(GraphicsDevice device)
        {
            // Set up the vertex list & dictionary
            vertices = new List<Vertex>();
            vertNameToIndex = new Dictionary<String, int>();

            //Initialize the visited vertices list
            visitedVerticesList = new List<Vertex>();

            // Set up the adjacency matrix
            adjMatrix = new int[MAX_VERTICES, MAX_VERTICES];
            edgeColor = new Color[MAX_VERTICES, MAX_VERTICES];

            // Create a 1x1 white pixel texture
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
        }
        #endregion

        #region Add Methods
        /// <summary>
        /// Adds a vertex to the graph
        /// </summary>
        /// <param name="vert">The vert to add</param>
        public void AddVertex(Vertex vert)
        {
            // Add the vertex if we're below the maximum
            if (vertices.Count < MAX_VERTICES)
            {
                vertNameToIndex.Add(vert.Name, vertices.Count);
                vertices.Add(vert);
            }
        }

        /// <summary>
        /// Adds a directed (one-way) edge to the graph
        /// </summary>
        /// <param name="vert1">The starting vert</param>
        /// <param name="vert2">The ending vert</param>
        /// <param name="weight">The weight of this edge</param>
        public void AddDirectedEdge(int vert1, int vert2, int weight)
        {
            // Add the edge
            adjMatrix[vert1, vert2] = weight;
            // Set a default color
            edgeColor[vert1, vert2] = NORMAL_EDGE_COLOR;
        }

        /// <summary>
        /// Adds a directed (one-way) edge to the graph
        /// </summary>
        /// <param name="vert1">The starting vert</param>
        /// <param name="vert2">The ending vert</param>
        /// <param name="weight">The weight of this edge</param>
        public void AddDirectedEdge(String vert1, String vert2, int weight)
        {
            // Add the edge if the verts exist
            if (vertNameToIndex.ContainsKey(vert1) && vertNameToIndex.ContainsKey(vert2))
            {
                adjMatrix[vertNameToIndex[vert1], vertNameToIndex[vert2]] = weight;
                edgeColor[vertNameToIndex[vert1], vertNameToIndex[vert2]] = NORMAL_EDGE_COLOR;
            }
        }

        /// <summary>
        /// Adds an undirected (two-way) edge to the graph
        /// </summary>
        /// <param name="vert1">One of the verts</param>
        /// <param name="vert2">The other vert</param>
        /// <param name="weight">The weight of this edge</param>
        public void AddUndirectedEdge(int vert1, int vert2, int weight)
        {
            // Add the edge in both directions
            AddDirectedEdge(vert1, vert2, weight);
            AddDirectedEdge(vert2, vert1, weight);
        }

        /// <summary>
        /// Adds an undirected (two-way) edge to the graph
        /// </summary>
        /// <param name="vert1">One of the verts</param>
        /// <param name="vert2">The other vert</param>
        /// <param name="weight">The weight of this edge</param>
        public void AddUndirectedEdge(String vert1, String vert2, int weight)
        {
            // Add the edge in both directions
            AddDirectedEdge(vert1, vert2, weight);
            AddDirectedEdge(vert2, vert1, weight);
        }
        #endregion

        #region Draw Methods
        /// <summary>
        /// Draws the graph's edges to the screen
        /// </summary>
        /// <param name="sb">
        /// The sprite batch to use when drawing.  This method assumes that
        /// the sprite batch's Begin() has already been called.
        /// </param>
        public void DrawEdges(SpriteBatch sb)
        {
            // Loop through the adjacency matrix and draw any edges.
            // Note: Since the vertices list can never have more vertices than
            //       MAX_VERTICES, but it could have fewer, we'll just loop
            //       enough times to cover all the verts, not necessarily through
            //       the entire adjacency matrix
            for (int row = 0; row < vertices.Count; row++)
            {
                for (int col = 0; col < vertices.Count; col++)
                {
                    // Check for an edge
                    if (adjMatrix[row, col] > 0)
                    {
                        // Found an edge, so draw it
                        DrawOneEdge(vertices[row], vertices[col], edgeColor[row, col], sb);
                    }
                }
            }
        }

        /// <summary>
        /// Helper method for drawing a single edge
        /// </summary>
        /// <param name="v1">The first vertex</param>
        /// <param name="v2">The second vertex</param>
        /// <param name="color">The color of the edge & weight</param>
        /// <param name="sb">The spritebatch to use when drawing</param>
        private void DrawOneEdge(Vertex v1, Vertex v2, Color color, SpriteBatch sb)
        {
            // Calculate the scale of the edge in pixels
            Vector2 scale = new Vector2(Vector2.Distance(v2.Position, v1.Position), EDGE_WIDTH);

            // Calculate the rotation
            float rotation = (float)Math.Atan2(v2.Position.Y - v1.Position.Y, v2.Position.X - v1.Position.X);

            // Draw
            sb.Draw(
                pixel,
                v1.Position,
                null,
                color,
                rotation,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0.0f
            );

            // Get the edge's weight
            int weight = adjMatrix[vertNameToIndex[v1.Name], vertNameToIndex[v2.Name]];

            // Draw above the center
            Vector2 pos = v1.Position;
            pos.X += (v2.Position.X - v1.Position.X) / 2.0f;
            pos.Y += (v2.Position.Y - v1.Position.Y) / 2.0f;

            // Draw the text
            sb.DrawString(font, weight.ToString(), pos, Color.White);
        }

        /// <summary>
        /// Draws the graph's vertices to the screen
        /// </summary>
        /// <param name="sb">
        /// The sprite batch to use when drawing.  This method assumes that
        /// the sprite batch's Begin() has already been called.
        /// </param>
        public void DrawVertices(SpriteBatch sb)
        {
            // Loop through the vertices and draw them to the screen
            foreach (Vertex vert in vertices)
            {
                // Offset the position (centering the graphic)
                Vector2 pos = vert.Position;
                pos.X -= vertexTexture.Width / 2.0f;
                pos.Y -= vertexTexture.Height / 2.0f;

                // Draw this vert
                if (GetVertexUnderMouse() == vert)
                {
                    sb.Draw(vertexTexture, pos, Color.Yellow);
                }
                else
                {
                    sb.Draw(vertexTexture, pos, Color.White);
                }

                // Draw the name
                Vector2 namePos = vert.Position;
                namePos.Y += (int)(vertexTexture.Height / 2.0f);
                namePos.X -= (int)(font.MeasureString(vert.Name).X / 2.0f);
                sb.DrawString(font, vert.Name, namePos, VERTEX_NAME_COLOR);
            }
        }

        // ClearHighlightedPaths:
        // Resets each vertex's visited property and resets the edge colors,
        // preparing for another Dijkstra's.
        public void ClearHighlightedPaths()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].Visited = false;
                for (int j = 0; j < vertices.Count; j++)
                {
                    if (adjMatrix[i, j] > 0)
                    {
                        edgeColor[i, j] = NORMAL_EDGE_COLOR;
                    }
                }
            }
        }
        #endregion

        #region Mouse Methods
        /// <summary>
        /// Returns the vertex under the mouse, if there is one
        /// </summary>
        /// <returns>A vertex, or null</returns>
        public Vertex GetVertexUnderMouse()
        {
            // Get the mouse state
            MouseState ms = Mouse.GetState();

            // Loop through the vertices
            foreach (Vertex v in vertices)
            {
                // Figure out this vert's rectangle on the screen
                Rectangle vertRect = new Rectangle();
                vertRect.X = (int)(v.Position.X - (vertexTexture.Width / 2.0f));
                vertRect.Y = (int)(v.Position.Y - (vertexTexture.Height / 2.0f));
                vertRect.Width = vertexTexture.Width;
                vertRect.Height = vertexTexture.Height;

                // Test the mouse pos
                if (ms.X >= vertRect.Left && ms.X <= vertRect.Right &&
                    ms.Y >= vertRect.Top && ms.Y <= vertRect.Bottom)
                {
                    // The mouse is over this vertex
                    return v;
                }
            }

            // Nothing found
            return null;
        }
        #endregion

        #region Student Methods
        /// <summary>
        /// This should highlight the tree of shortest paths from the vertex to all other vertices,
        /// using Dijkstra's algorithm.
        /// </summary>
        #region HighlightShortestPaths method
        public void HighlightShortestPaths(Vertex v)
        {
            //Call Reset method to set all of the vertices weight to the largest int max value
            if (visitedVerticesList.Count == 0)
            {
                Reset();
            }
            //Keep doing the algorithm until it is finished
            while (Finished() != true)
            {
                //Check to see if the room is in the vertex list
                if (vertices.Contains(v))
                {
                    if (visitedVerticesList.Count == 0)
                    {
                        //Set the source vertex to the source node
                        source = v;
                        //Make source node permanent
                        source.Permanent = true;
                        source.Weight = 0;
                        //Call the label method
                        Label(source);
                    }
                    else
                    {
                        Label(current);
                    }

                }
                if (visitedVerticesList.Count == 0)
                {
                    visitedVerticesList.Add(source);
                }
                current = SmallestLabel();
            }
            //Call the clear list to clear the vertices in vistedVerticesList 
            //so that the algorithm can be runned again
            visitedVerticesList.Clear();

        }
        #endregion 
        #region Helper Method - Reset
        //Create a method that will reset all of the vertices to false
        public void Reset()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i].Permanent = false;
                vertices[i].Weight = int.MaxValue;
            }
        }
        #endregion
        #region Helper Method - Finished
        //Create a helper method that will determine if the algorithm is done
        public bool Finished()
        {
            //Create a temp counter
            int numOfPermanentVert = 0;

            //Loop through the vertex list
            foreach (Vertex vert in vertices)
            {
                //Check to see if vertex is permanent
                if (vert.Permanent)
                {
                    //Increment number of permanent vertices
                    numOfPermanentVert++;
                }
                //If number of permanent vertices equals the vertex list count
                //then the algorithm is finished
                if (numOfPermanentVert == vertices.Count)
                {
                    return true;
                }
            }
            return false;

        }
        #endregion
        #region Helper Method - SmallestLabel
        //Create a method that will search through the vertices list and find out which non-permanent node
        //contains the smallest label and has been visited
        public Vertex SmallestLabel()
        {
            //Create a local variable for smallest vertex
            Vertex smallest = new Vertex("foo", new Vector2(0, 0));
            smallest.Weight = int.MaxValue;
            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].Weight < smallest.Weight && vertices[i].Permanent == false && vertices[i].Visited)
                {
                    smallest = vertices[i];
                }
            }
            //Make smallest node permanent
            smallest.Permanent = true;
            //Highlight the shortest path
            edgeColor[vertNameToIndex[smallest.Name], vertNameToIndex[smallest.Adjacent.Name]] = HIGHLIGHT_EDGE_COLOR;
            //Add to the visitedlist
            visitedVerticesList.Add(smallest);
            return smallest;
        }
        #endregion
        #region Helper Method - Label
        //Create a method that will examine each non-permanent node adjacent to the working node
        public void Label(Vertex current)
        {
            //Loop through the vertices in vertexList
            foreach (Vertex vert in vertices)
            {
                //If a vertex in the list matches the given room 
                if (vert == current)
                {
                    //Go through the 2d array
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        //If there is a connection and permanent is false
                        if (adjMatrix[vertNameToIndex[vert.Name], i] != 0 && vertices[i].Permanent == false && vertices[i].Adjacent == null)
                        {
                            //Label with total distance from the source and the name of the working node
                            //Total distance from source
                            vertices[i].Weight = adjMatrix[vertNameToIndex[vert.Name], i] + vert.Weight;

                            //Set adjacent vertex's adjacent vertex to working node
                            vertices[i].Adjacent = vert;
                            vertices[i].Visited = true;
                        }

                        //If there is a connection and if it has already been labeled
                        if (adjMatrix[vertNameToIndex[vert.Name], i] != 0 && vertices[i].Adjacent != null)
                        {
                            //Check to see if the cost computed using the working node is better
                            //than the current cost in the label
                            int newCost = adjMatrix[vertNameToIndex[vert.Name], i] + vert.Weight;
                            if (newCost < vertices[i].Weight)
                            {
                                //If so, change the label
                                //Change the total distance and current working node
                                vertices[i].Weight = newCost;
                                vertices[i].Visited = true;
                                vertices[i].Adjacent = vert;
                            }


                        }
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
