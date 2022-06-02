using System.Collections.Generic;
using Elements.Geometry;
using Newtonsoft.Json;

namespace Elements
{
    /// <summary>
    /// A dimension.
    /// </summary>
    public abstract class Dimension : Element
    {
        /// <summary>
        /// The plane in which the dimension is drawn.
        /// </summary>
        public Plane Plane { get; set; }

        /// <summary>
        /// Create a dimension.
        /// </summary>
        /// <param name="plane">The plane in which the dimension is drawn.</param>
        public Dimension(Plane plane)
        {
            this.Plane = plane;
        }
    }

    /// <summary>
    /// A linear dimension.
    /// Linear dimensions can represent aligned and projected dimensions.
    /// </summary>
    public class LinearDimension : Dimension
    {
        /// <summary>
        /// The start of the dimension.
        /// </summary>
        public Vector3 Start { get; private set; }

        /// <summary>
        /// The end of the dimension.
        /// </summary>
        public Vector3 End { get; private set; }

        /// <summary>
        /// The direction in which the dimension is offset 
        /// from the reference line.
        /// </summary>
        public Vector3 OffsetDirection { get; private set; }

        /// <summary>
        /// The line on which the start and end points are projected.
        /// </summary>
        public Plane ReferencePlane { get; private set; }

        /// <summary>
        /// Create a linear dimension with a reference plane.
        /// </summary>
        /// <param name="plane">The plane in which the dimension is measured.</param>
        /// <param name="start">The start of the dimension.</param>
        /// <param name="end">The end of the dimension.</param>
        /// <param name="referencePlane">The plane on which the start and end
        /// points will be projected.</param>
        /// <returns></returns>
        [JsonConstructor]
        public LinearDimension(Vector3 start, Vector3 end, Plane referencePlane, Plane plane = null) : base(plane)
        {
            this.Plane = plane ?? new Plane(Vector3.Origin, Vector3.ZAxis);
            this.Start = start;
            this.End = end;
            this.ReferencePlane = referencePlane;
        }

        /// <summary>
        /// Create a linear dimension with an optional reference line.
        /// If a reference line is provided, the start and end points will
        /// be projected onto the reference line.
        /// </summary>
        /// <param name="plane">The plane in which the dimension is measured.</param>
        /// <param name="start">The start of the dimension.</param>
        /// <param name="end">The end of the dimension.</param>
        /// <param name="referenceLine">A line on which the start and end points 
        /// will be projected.</param>
        public LinearDimension(Vector3 start, Vector3 end, Plane plane = null, Line referenceLine = null) : base(plane)
        {
            this.Plane = plane ?? new Plane(Vector3.Origin, Vector3.ZAxis);
            this.Start = start.Project(this.Plane);
            this.End = end.Project(this.Plane);
            Vector3 vRef;
            if (referenceLine != null)
            {
                vRef = (referenceLine.End.Project(this.Plane) - referenceLine.Start.Project(this.Plane)).Unitized();
            }
            else
            {
                vRef = (this.End - this.Start).Unitized();
            }
            this.ReferencePlane = new Plane(referenceLine.Start, this.Plane.Normal.Cross(vRef));
        }

        /// <summary>
        /// Create a linear dimension where the reference line is created
        /// by offsetting from the line created between start and end 
        /// by the provided value.
        /// </summary>
        /// <param name="plane">The plane in which the dimension is created.</param>
        /// <param name="start">The start of the dimension.</param>
        /// <param name="end">The end of the dimension.</param>
        /// <param name="offset">The offset of the reference line.</param>
        public LinearDimension(Vector3 start, Vector3 end, Plane plane = null, double offset = 0.0) : base(plane)
        {
            this.Plane = plane ?? new Plane(Vector3.Origin, Vector3.ZAxis);
            this.Start = start.Project(this.Plane);
            this.End = end.Project(this.Plane);
            var vRef = (this.End - this.Start).Unitized();
            var offsetDirection = this.Plane.Normal.Cross(vRef);
            this.ReferencePlane = new Plane(this.Start + offsetDirection * offset, offsetDirection);
        }

        /// <summary>
        /// Draw the dimension.
        /// </summary>
        /// <returns></returns>
        public List<Element> ToModelArrowsAndText()
        {
            var dimStart = this.Start.Project(this.ReferencePlane);
            var dimEnd = this.End.Project(this.ReferencePlane);
            var dimDirection = (dimEnd - dimStart).Unitized();
            var ma = new ModelArrows(new (Vector3, Vector3, double, Color?)[] { (dimStart, dimDirection, dimStart.DistanceTo(dimEnd), null) }, true, true);
            var elements = new List<Element>
            {
                ma
            };

            var c = new Material("Red", Colors.Red);

            if (dimStart.DistanceTo(this.Start) > 0)
            {
                elements.Add(new ModelCurve(new Line(this.Start, dimStart), c));
            }
            if (dimEnd.DistanceTo(this.End) > 0)
            {
                elements.Add(new ModelCurve(new Line(this.End, dimEnd), c));
            }

            // Always try to make the direction vector point in positive x, y, and z.
            var lineDirection = dimDirection.Dot(new Vector3(1, 1, 1)) > 0 ? dimDirection : dimDirection.Negate();

            var texts = new List<(Vector3, Vector3, Vector3, string, Color?)>
            {
                (dimStart.Average(dimEnd), this.Plane.Normal, lineDirection, dimStart.DistanceTo(dimEnd).ToString("0.00"), Colors.Black)
            };
            var mt = new ModelText(texts, FontSize.PT36);
            elements.Add(mt);

            return elements;
        }
    }
}