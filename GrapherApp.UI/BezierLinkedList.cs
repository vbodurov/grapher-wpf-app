using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GrapherApp.UI.Services;

namespace GrapherApp.UI
{
    public interface IBezierCurve
    {
        IBezierCurve Previous { get; }
        IBezierCurve Next { get; }
        Path Path { get; }
        Ellipse Start { get; }
        Ellipse End { get; }
        Ellipse Control1 { get; }
        Ellipse Control2 { get; }
        Line Line1 { get; }
        Line Line2 { get; }
        PathFigure PathFigure { get; }
        BezierSegment BezierSegment { get; }
        void SetPoint(DotType type, Point pos);
    }

    public class BezierLinkedList : IEnumerable<IBezierCurve>
    {
        private const int EllipseWidth = 10;
        private const int HalfEllipseWidth = EllipseWidth/2;

        public IBezierCurve First { get; private set; }
        public IBezierCurve Last { get; private set; }

        

        public int Count { get; private set; }
        public BezierLinkedList Clear(Canvas canvas, IDrawingBoardHolder eventsHolder)
        {
            foreach (var n in this)
            {
                n.Start.MouseDown -= eventsHolder.OnStartPointMouseDown;
                n.End.MouseDown -= eventsHolder.OnEndPointMouseDown;
                n.Control1.MouseDown -= eventsHolder.OnControl1PointMouseDown;
                n.Control2.MouseDown -= eventsHolder.OnControl2PointMouseDown;

                n.Start.Tag = n.End.Tag = n.Control1.Tag = n.Control2.Tag = null;

                canvas.Children.Remove(n.Path);
                canvas.Children.Remove(n.Start);
                canvas.Children.Remove(n.End);
                canvas.Children.Remove(n.Control1);
                canvas.Children.Remove(n.Control2);
            }
            Count = 0;
            First = null;
            Last = null;
            return this;
        }
        public BezierLinkedList AddBezier(Canvas canvas, IDrawingBoardHolder eh, IBezierFragment bf)
        {
            var newNode = new BezierCurve(this, Count);

            canvas.Children.Add(newNode.Line1);
            canvas.Children.Add(newNode.Line2);
            canvas.Children.Add(newNode.Path);
            canvas.Children.Add(newNode.Start);
            canvas.Children.Add(newNode.End);
            canvas.Children.Add(newNode.Control1);
            canvas.Children.Add(newNode.Control2);
            

            newNode.Start.MouseDown += eh.OnStartPointMouseDown;
            newNode.End.MouseDown += eh.OnEndPointMouseDown;
            newNode.Control1.MouseDown += eh.OnControl1PointMouseDown;
            newNode.Control2.MouseDown += eh.OnControl2PointMouseDown;
            var a = new Point(eh.GraphToPixelX(bf.From.X), eh.GraphToPixelY(bf.From.Y));
            var b = new Point(eh.GraphToPixelX(bf.B.X), eh.GraphToPixelY(bf.B.Y));
            var c = new Point(eh.GraphToPixelX(bf.C.X), eh.GraphToPixelY(bf.C.Y));
            var d = new Point(eh.GraphToPixelX(bf.To.X), eh.GraphToPixelY(bf.To.Y));

            newNode.PathFigure.StartPoint = a;
            newNode.BezierSegment.Point1 = b;
            newNode.BezierSegment.Point2 = c;
            newNode.BezierSegment.Point3 = d;
            newNode.Start.SetValue(Canvas.LeftProperty, a.X-HalfEllipseWidth);
            newNode.Start.SetValue(Canvas.TopProperty, a.Y-HalfEllipseWidth);
            newNode.End.SetValue(Canvas.LeftProperty, d.X-HalfEllipseWidth);
            newNode.End.SetValue(Canvas.TopProperty, d.Y-HalfEllipseWidth);
            newNode.Control1.SetValue(Canvas.LeftProperty, b.X-HalfEllipseWidth);
            newNode.Control1.SetValue(Canvas.TopProperty, b.Y-HalfEllipseWidth);
            newNode.Control2.SetValue(Canvas.LeftProperty, c.X-HalfEllipseWidth);
            newNode.Control2.SetValue(Canvas.TopProperty, c.Y-HalfEllipseWidth);

            newNode.SetLines();

            if (Last == null)
            {
                First = Last = newNode;
            }
            else
            {
                var last = (BezierCurve)Last;
                newNode.Previous = last;
                last.Next = newNode;
                Last = newNode;
            }
            ++Count;
            return this;
        }
        class BezierCurve : IBezierCurve
        {
            private static readonly Color[] AllColors = 
                {
                    Color.FromArgb(0xFF, 0xFF, 0x99, 0x33),
                    Color.FromArgb(0xFF, 0x55, 0xAA, 0xFF),
                    Color.FromArgb(0xFF, 0x33, 0xCC, 0x11),
                    Color.FromArgb(0xFF, 0xBB, 0x55, 0xBB)
                };

            private readonly BezierLinkedList _list;
            internal BezierCurve(BezierLinkedList list, int index)
            {
                _list = list;
                Start = CreateEllipse(index);
                End = CreateEllipse(index);
                Control1 = CreateEllipse(index, true);
                Control2 = CreateEllipse(index, true);
                Line1 = CreateLine();
                Line2 = CreateLine();
                var bezierSegment = new BezierSegment();
                var pathFigure = 
                    new PathFigure
                    {
                        Segments = new PathSegmentCollection
                                   {
                                       bezierSegment
                                   },
                        IsClosed = false
                    };
                Path = new Path
                       {
                           Data = new PathGeometry
                                  {
                                      Figures = new PathFigureCollection
                                                {
                                                    pathFigure
                                                }
                                  },
                           Stroke = new SolidColorBrush(AllColors[index%AllColors.Length]),
                           StrokeThickness = 2
                       };
                PathFigure = pathFigure;
                BezierSegment = bezierSegment;
            }

            private Line CreateLine()
            {
                return new Line
                       {
                           Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xAA, 0xAA)),
                           StrokeThickness = 1
                       };
            }

            private Ellipse CreateEllipse(int index, bool isControl = false)
            {
                return new Ellipse
                       {
                           Tag = this,
                           Width = EllipseWidth,
                           Height = EllipseWidth,
                           Fill = new SolidColorBrush(AllColors[index%AllColors.Length].MultiplyBy(isControl ? 0.75 : 1))
                       };
            }

            public IBezierCurve Previous { get; internal set; }
            public IBezierCurve Next { get; internal set; }
            public Ellipse Start { get; private set; }
            public Ellipse End { get; private set; }
            public Ellipse Control1 { get; private set; }
            public Ellipse Control2 { get; private set; }
            public Line Line1 { get; private set; }
            public Line Line2 { get; private set; }
            public PathFigure PathFigure { get; private set; }
            public BezierSegment BezierSegment { get; private set; }
            public void SetPoint(DotType type, Point pos)
            {
                if (type == DotType.Control1)
                {
                    Control1.SetValue(Canvas.LeftProperty, pos.X-HalfEllipseWidth);
                    Control1.SetValue(Canvas.TopProperty, pos.Y-HalfEllipseWidth);
                    BezierSegment.Point1 = pos;
                }
                else if (type == DotType.Control2)
                {
                    Control2.SetValue(Canvas.LeftProperty, pos.X-HalfEllipseWidth);
                    Control2.SetValue(Canvas.TopProperty, pos.Y-HalfEllipseWidth);
                    BezierSegment.Point2 = pos;
                }
                else if (type == DotType.Start)
                {
                    var hasPrev = Previous != null;
                    if (hasPrev)
                    {
                        if((pos.X-HalfEllipseWidth) <= Previous.PathFigure.StartPoint.X) return;
                    }

                    if((pos.X-HalfEllipseWidth) >= BezierSegment.Point3.X) return;

                    Start.SetValue(Canvas.LeftProperty, pos.X-HalfEllipseWidth);
                    Start.SetValue(Canvas.TopProperty, pos.Y-HalfEllipseWidth);
                    PathFigure.StartPoint = pos;
                    if (hasPrev)
                    {
                        Previous.BezierSegment.Point3 = pos;
                        Previous.End.SetValue(Canvas.LeftProperty, pos.X-HalfEllipseWidth);
                        Previous.End.SetValue(Canvas.TopProperty, pos.Y-HalfEllipseWidth);
                        Previous.SetLines();
                    }
                }
                else if (type == DotType.End)
                {
                    var hasNext = Next != null;
                    if (hasNext)
                    {
                        if((pos.X-HalfEllipseWidth) >= Next.BezierSegment.Point3.X) return;
                    }

                    if((pos.X-HalfEllipseWidth) <= PathFigure.StartPoint.X) return;

                    End.SetValue(Canvas.LeftProperty, pos.X-HalfEllipseWidth);
                    End.SetValue(Canvas.TopProperty, pos.Y-HalfEllipseWidth);
                    BezierSegment.Point3 = pos;

                    if (hasNext)
                    {
                        Next.PathFigure.StartPoint = pos;
                        Next.Start.SetValue(Canvas.LeftProperty, pos.X-HalfEllipseWidth);
                        Next.Start.SetValue(Canvas.TopProperty, pos.Y-HalfEllipseWidth);
                        Next.SetLines();
                    }
                }
                this.SetLines();
            }

            public Path Path { get; private set; }
        }
        class BezierLinkedListEnumerator : IEnumerator<IBezierCurve>
        {
            private readonly BezierLinkedList _list;
            private IBezierCurve _current;
            internal BezierLinkedListEnumerator(BezierLinkedList list)
            {
                _list = list;
            }
            public void Dispose() => _current = null;
            public bool MoveNext()
            {
                if (_current == null)
                {
                    if (_list.First == null) return false;
                    _current = _list.First;
                    return true;
                }
                _current = _current.Next;
                return _current != null;
            }
            public void Reset() => _current = null;
            public IBezierCurve Current => _current;
            object IEnumerator.Current => Current;
        }

        public IEnumerator<IBezierCurve> GetEnumerator()
        {
            return new BezierLinkedListEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string ToCode(IDrawingBoardHolder holder)
        {
            if (Count == 0) return "";
            var n = First;
                Point a, b, c, d;
                n.Path.GetGraphPoints(holder, out a, out b, out c, out d);

            if (Count == 1)
            {
                const double tolerance = 0.000001;
                if (abs(a.X) < tolerance &&
                    abs(a.Y) < tolerance &&
                    abs(d.X - 1) < tolerance &&
                    abs(d.Y - 1) < tolerance)
                {
                    return $"bezier(x,{b.X:N2},{b.Y:N2},{c.X:N2},{c.Y:N2})";
                }
                return $"bezier(x,{a.X:N2},{a.Y:N2},{b.X:N2},{b.Y:N2},{c.X:N2},{c.Y:N2},{d.X:N2},{d.Y:N2})";
            }

            return 
                new StringBuilder()
                .AppendLine($"return beziers(b => b.from({a.X:N2},{a.Y:N2})")
                .AppendCurves(this, holder, (a1,b1,c1,d1) => $"    .to({d1.X:N2},{d1.Y:N2}).curve({b1.X:N2},{b1.Y:N2},{c1.X:N2},{c1.Y:N2})")
                .AppendLine(").run(x)")
                .ToString();
        }

        private static double abs(double n) => n < 0 ? n*-1 : n;
    }

}