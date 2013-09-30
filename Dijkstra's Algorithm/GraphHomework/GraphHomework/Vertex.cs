#region Using Statements
using System;

using Microsoft.Xna.Framework;
#endregion
//@author: Rebecca Vessal and Professor Cascioli
//Instructor: Professor Cascioli
//Date: 5/7/11 - 5/8/11
//
//Vertex.cs
//
//Vertex.cs represents a tower in the graph

namespace GraphHomework
{
    public class Vertex
    {
        #region Attributes
        // Private attributes
        private String name;
        private Vector2 position;
        private Boolean visited;
        private Boolean permanent;
        private Vertex adjacent;
        private int weight;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of this vertex
        /// </summary>
        public String Name { get { return name; } set { name = value; } }

        /// <summary>
        /// Gets or sets the position of this vertex
        /// </summary>
        public Vector2 Position { get { return position; } set { position = value; } }

        /// <summary>
        /// Gets or sets the visited state of this vertex
        /// </summary>
        public Boolean Visited { get { return visited; } set { visited = value; } }

        //Property for getting and setting permanent attribute of vertex
        public Boolean Permanent
        {
            get { return permanent; }
            set
            {
                permanent = value;
            }
        }

        //Property of getting and setting adjacent attribute of vertex
        public Vertex Adjacent
        {
            get { return adjacent; }
            set
            {
                adjacent = value;
            }
        }

        //Property for getting and setting the weight of vertex
        public int Weight
        {
            get { return weight; }
            set
            {
                weight = value;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a new vertex
        /// </summary>
        /// <param name="name">The name of this vertex</param>
        /// <param name="position">The position of this vertex</param>
        public Vertex(String name, Vector2 position)
        {
            // Save the data
            this.name = name;
            this.position = position;

            // Defaults
            visited = false;
            permanent = false;
            adjacent = null;
            weight = 0;
        }
        #endregion

    }
}
