using System.Collections;
using System.Collections.Generic;

namespace Tree
{
    /// <summary>
    /// A generic class for representing trees
    /// </summary>
    public class TreeNode<T>
    {
        /// <summary>
        /// The value stored in the tree node
        /// </summary>
        public T Value;
        /// <summary>
        /// The children of the node
        /// </summary>
        public List<TreeNode<T>> Children = new List<TreeNode<T>>();
        
        public TreeNode(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Adds a child to the tree node with the given value
        /// </summary>
        /// <param name="value">The value to add as a child</param>
        public void AddChild(T value)
        {
            var child = new TreeNode<T>(value);
            Children.Add(child);
        }

        /// <summary>
        /// Adds the given tree node as a child of this tree node
        /// </summary>
        /// <param name="child">The child tree node</param>
        public void AddChild(TreeNode<T> child)
        {
            Children.Add(child);
        }
    }
}