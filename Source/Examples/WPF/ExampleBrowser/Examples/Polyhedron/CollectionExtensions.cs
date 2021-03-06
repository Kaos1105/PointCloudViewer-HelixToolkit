// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace PolyhedronDemo
{
    public static class CollectionExtensions
    {
        public static void Add(this Int32Collection c, params int[] values)
        {
            foreach (var i in values)
                c.Add(i);
        }

        public static void Add(this Point3DCollection c, double x, double y, double z)
        {
            c.Add(new Point3D(x, y, z));
        }
    }
}