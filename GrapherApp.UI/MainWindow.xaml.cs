using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using GrapherApp.UI.Services;

namespace GrapherApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public interface IDrawingBoardHolder
    {
        void OnStartPointMouseDown(object sender, MouseButtonEventArgs e);
        void OnEndPointMouseDown(object sender, MouseButtonEventArgs e);
        void OnControl1PointMouseDown(object sender, MouseButtonEventArgs e);
        void OnControl2PointMouseDown(object sender, MouseButtonEventArgs e);
        double GraphToPixelX(double graphCoord);
        double GraphToPixelY(double graphCoord);
        double PixelToGraphX(double pixelCoord);
        double PixelToGraphY(double pixelCoord);
    }
    public partial class MainWindow : Window, IDrawingBoardHolder
    {
        private const double PixelsPerOne = 250;
        private const double CanvasWidth = 1200;
        private const double CanvasHeight = 720;
        private const double CanvasHalfWidth = CanvasWidth/2;
        private const double CanvasHalfHeight = CanvasHeight/2;

        private readonly ScaleTransform _scaleTransform;
        private readonly TranslateTransform _translateTransform;
        private readonly IFuncRunnerCreator _runnerCreator;
        private readonly DispatcherTimer _timer;
        private readonly BezierLinkedList _beziers = new BezierLinkedList();
        private readonly IDrawingBoardHolder _eh;

//        private double _scale = 1;
//        private Point _translate = new Point(0,0);
        private Point? _dragPosition = null;
        private DraggedBezierPoint _draggedBezierPoint;

        public MainWindow()
        {
            _eh = this;
            InitializeComponent();

            

            var t = new TransformGroup();

            t.Children.Add(_scaleTransform = new ScaleTransform(1, 1, CanvasHalfWidth, CanvasHalfHeight));
            t.Children.Add(_translateTransform = new TranslateTransform(0,0));
            TheCanvas.RenderTransform = t;

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(80)};
            _timer.Tick += OnTimerTick;

            KeyDown += MainWindow_KeyDown;
            MouseWheel += TheCanvas_MouseWheel;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            MouseMove += MainWindow_MouseMove;
            MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;

            var nameSuffix = "";
            //if (ApplicationDeployment.IsNetworkDeployed)
            //{
                Version version = GetType().Assembly.GetName().Version;
                nameSuffix = " " +version.Major + "." + version.Minor;
            //}
            Title = "YouVisio Grapher" + nameSuffix;

            _runnerCreator = new FuncRunnerCreator();

            ReDrawCanvas();
        }

        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Enter || 
                (!Keyboard.IsKeyDown(Key.LeftCtrl) && 
                 !Keyboard.IsKeyDown(Key.RightCtrl) &&
                 !Keyboard.IsKeyDown(Key.LeftAlt) &&
                 !Keyboard.IsKeyDown(Key.RightAlt))) return;
            ReDrawCanvas();
        }
        void TheCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            // no zoom when has bezier because there is a zoom bug I haven't figured out yet
            if(_beziers.Count > 0) return;

            var n = e.Delta > 0 ? 0.05 : -0.05;
            var candidate = _scaleTransform.ScaleX + n;
            if (candidate > 0.2 && candidate < 5)
            {
                _scaleTransform.ScaleX = _scaleTransform.ScaleY = candidate;
            }

            _timer.Stop();
            _timer.Start();
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            if (_beziers.Count == 0)
            {
                ReDrawCanvas();
            }
        }
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            OnDragStarts(e.GetPosition(OuterCanvas));
        }
        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragPosition == null) return;
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                OnDragEnds();
                return;
            }
            var position = e.GetPosition(OuterCanvas);
            // if dragging bezier point
            if (_draggedBezierPoint != null)
            {
                var p = e.GetPosition(TheCanvas);
                MoveBezierPont(_draggedBezierPoint, p);
            }
            else // if dragging the canvas
            {
                var diff = position - _dragPosition.Value;
                _translateTransform.X += diff.X;
                _translateTransform.Y += diff.Y;
            }
            _dragPosition = position;
        }

        private void MoveBezierPont(DraggedBezierPoint point, Point pos)
        {
            if(point.Type == DotType.Control1 || point.Type == DotType.Control2)
            {
                point.Curve.SetPoint(point.Type, pos);
                return;
            }
            var prev = point.Curve.GetPoint(point.Type);
            var diff = pos - prev;
            if (point.Type == DotType.Start)
            {
                point.Curve.SetPoint(point.Type, pos);
                var c1 = point.Curve.GetPoint(DotType.Control1);
                point.Curve.SetPoint(DotType.Control1, c1 + diff);
                var prevCurve = point.Curve.Previous;
                if (prevCurve != null)
                {
                    var c2 = prevCurve.GetPoint(DotType.Control2);
                    prevCurve.SetPoint(DotType.Control2, c2 + diff);
                }
            }
            else if (point.Type == DotType.End)
            {
                point.Curve.SetPoint(point.Type, pos);
                var c2 = point.Curve.GetPoint(DotType.Control2);
                point.Curve.SetPoint(DotType.Control2, c2 + diff);
                var nextCurve = point.Curve.Next;
                if (nextCurve != null)
                {
                    var c1 = nextCurve.GetPoint(DotType.Control1);
                    nextCurve.SetPoint(DotType.Control1, c1 + diff);
                }
            }

        }

        void IDrawingBoardHolder.OnStartPointMouseDown(object sender, MouseButtonEventArgs e) =>
            TryStartDragPoint(sender, DotType.Start);
        void IDrawingBoardHolder.OnEndPointMouseDown(object sender, MouseButtonEventArgs e) =>
            TryStartDragPoint(sender, DotType.End);
        void IDrawingBoardHolder.OnControl1PointMouseDown(object sender, MouseButtonEventArgs e) =>
            TryStartDragPoint(sender, DotType.Control1);
        void IDrawingBoardHolder.OnControl2PointMouseDown(object sender, MouseButtonEventArgs e) =>
            TryStartDragPoint(sender, DotType.Control2);
        private bool TryStartDragPoint(object sender, DotType dotType)
        {
            if (_beziers.Count == 0) return false;
            var ellipse = sender as Ellipse;
            var curve = ellipse?.Tag as IBezierCurve;
            if (curve == null) return false;
            _draggedBezierPoint = new DraggedBezierPoint(curve, dotType);
            return true;
        }

        void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnDragEnds();
        }

        private void OnDragStarts(Point point)
        {
            _dragPosition = point;
        }
        private void OnDragEnds()
        {
            _dragPosition = null;
            _draggedBezierPoint = null;
            if (_beziers.Count == 0)
            {
                ReDrawCanvas();
            }
        }
        private void AddErrorMessage(string error)
        {
            if (Message.Text.Length < 1024)
            {
                if (Message.Text.Length > 0) Message.Text += "; ";
                Message.Text += error;
            }
        }

        private void DrawLines()
        {
            for (var x = -10; x <= 10; x += 1)
            {
                AddGridLine(x, 10, x, -10, GetTypeByValue(x));
            }
            for (var y = -10; y <= 10; y += 1)
            {
                AddGridLine(-10, y, 10, y, GetTypeByValue(y));
            }
            if (_scaleTransform.ScaleX > 0.3)
            {
                for (var x = -1.1; x <= 1.1; x += 0.1)
                {
                    if (Math.Abs(x - Math.Round(x)) > 0.0001)
                        AddGridLine(x, 1.1, x, -1.1, GetTypeByValue(x));
                }
                for (var y = -1.1; y <= 1.1; y += 0.1)
                {
                    if (Math.Abs(y - Math.Round(y)) > 0.0001)
                        AddGridLine(-1.1, y, 1.1, y, GetTypeByValue(y));
                }
            }
            
        }


        private GridLineType GetTypeByValue(double n)
        {
            const double epsilon = 0.000001;
            var absN = Math.Abs(n);
            var r = absN < epsilon 
                    ? GridLineType.Axis 
                    : (Math.Abs(absN-Math.Round(absN)) < epsilon) 
                        ? GridLineType.Major
                        : GridLineType.Minor;

            return r;
        }


        private void EnsureNoInfinity(ref double x1, ref double y1, ref double x2, ref double y2)
        {
            if (double.IsNegativeInfinity(x1)) x1 = -10000;
            if (double.IsPositiveInfinity(x1)) x1 = 10000;
            if (double.IsNegativeInfinity(y1)) y1 = -10000;
            if (double.IsPositiveInfinity(y1)) y1 = 10000;
            if (double.IsNegativeInfinity(x2)) x2 = -10000;
            if (double.IsPositiveInfinity(x2)) x2 = 10000;
            if (double.IsNegativeInfinity(y2)) y2 = -10000;
            if (double.IsPositiveInfinity(y2)) y2 = 10000;
        }


        private void RunButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (_beziers.Count > 0)
            {
                // check if this is not another (non bezier) func the user whants to run
                var txt = Regex.Replace(SourceCode1.Text,@"([ \t\n\r;,/*]|return)","");
                if (txt.Trim() == "" || 
                    txt.StartsWith("bezier", StringComparison.CurrentCultureIgnoreCase))
                {
                    SourceCode1.Text = _beziers.ToCode(this);
                    SourceCode2.Text = SourceCode3.Text = SourceCode4.Text = "";
                }
            }
            _beziers.Clear(TheCanvas, this);
            ReDrawCanvas();
        }

        private readonly BaseFuncRunner[] _runners = new BaseFuncRunner[4];
        private void ReDrawCanvas()
        {
            TheCanvas.Children.Clear();
            if (_rectangle != null) TheCanvas.Children.Add(_rectangle);

            Message.Text = "";

            
            try
            {
                DrawLines();
                _runners[0] = DrawIfHasCode(Colors.Red, SourceCode1);
                _runners[1] = DrawIfHasCode(Colors.Green, SourceCode2);
                _runners[2] = DrawIfHasCode(Colors.Blue, SourceCode3);
                _runners[3] = DrawIfHasCode(Colors.Purple, SourceCode4);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private BaseFuncRunner DrawIfHasCode(Color color, TextBox sourceCode)
        {
            var code = sourceCode.Text.Trim();
            BaseFuncRunner r = null;
            if (code != "") r = DrawGraphFromSource(color, sourceCode.Text);
            return r;
        }

        private BaseFuncRunner DrawGraphFromSource(Color color, string source)
        {
            BaseFuncRunner runner;
            IList<string> errors;
            if (!_runnerCreator.TryGetRunner(source, out runner, out errors))
            {
                Message.Text = String.Join("; ", errors);
                return null;
            }
            runner.GraphDrawingStarts(color, source);

            var x2 = double.NaN;
            var y2 = double.NaN;

            var fromX = Math.Min(_eh.PixelToGraphX(0), -3/_scaleTransform.ScaleX + 0.5);
            var toX = Math.Max(_eh.PixelToGraphX(CanvasWidth), 3/_scaleTransform.ScaleX - 0.5);
            var step = 0.0075/_scaleTransform.ScaleX;
//SourceCode4.Text = fromX + "|" + toX+"|"+DateTime.UtcNow.Ticks;
            for (var x = fromX; x <= toX; x += step)
            {
                double x1 = x;
                double y1;
                try
                {
                    y1 = runner.Run(x1);
                }
                catch (Exception ex)
                {
                    AddErrorMessage(ex.Message);
                    break;
                }
                if (double.IsNaN(y1) || double.IsInfinity(y1))
                {
                    continue;
                }

                if (!double.IsNaN(x2) && !double.IsNaN(y2) && !double.IsInfinity(x2) && !double.IsInfinity(y2))
                {
                    AddLine(x1, y1, x2, y2, color);
                }

                x2 = x1;
                y2 = y1;
            }

            runner.GraphDrawingEnds(color, source);

            return runner;
        }



        double IDrawingBoardHolder.GraphToPixelX(double graphCoord)
        {
            return graphCoord  * PixelsPerOne * _scaleTransform.ScaleX + _scaleTransform.CenterX;
        }
        double IDrawingBoardHolder.GraphToPixelY(double graphCoord)
        {
            return graphCoord  * -PixelsPerOne * _scaleTransform.ScaleY + _scaleTransform.CenterY;
        }

        double IDrawingBoardHolder.PixelToGraphX(double pixelCoord)
        {
            return (pixelCoord - _scaleTransform.CenterX)/(PixelsPerOne * _scaleTransform.ScaleX);
        }
        double IDrawingBoardHolder.PixelToGraphY(double pixelCoord)
        {
            return (pixelCoord - _scaleTransform.CenterY)/(-PixelsPerOne*_scaleTransform.ScaleY);
        }

        private static readonly SolidColorBrush BrushAxis = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xBB, 0xFF));
        private static readonly SolidColorBrush BrushMajor = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xAA, 0xAA));
        private static readonly SolidColorBrush BrushMinor = new SolidColorBrush(Color.FromArgb(0x99, 0xCC, 0xCC, 0xCC));
        private static readonly IDictionary<Color,SolidColorBrush> Brushes = new Dictionary<Color, SolidColorBrush>();

        private Line AddGridLine(double x1, double y1, double x2, double y2, GridLineType type)
        {
            x1 = _eh.GraphToPixelX(x1);
            y1 = _eh.GraphToPixelY(y1);
            x2 = _eh.GraphToPixelX(x2);
            y2 = _eh.GraphToPixelY(y2);

            EnsureNoInfinity(ref x1, ref y1, ref x2, ref y2);

            var brush = type == GridLineType.Minor
                            ? BrushMinor
                            : type == GridLineType.Major
                                ? BrushMajor
                                : BrushAxis;
            Line line;
            TheCanvas.Children.Add(line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = brush
            });

            return line;
        }

        private void AddLine(double x1, double y1, double x2, double y2, Color color)
        {
            var px1 = _eh.GraphToPixelX(x1);
            var py1 = _eh.GraphToPixelY(y1);
            var px2 = _eh.GraphToPixelX(x2);
            var py2 = _eh.GraphToPixelY(y2);

            var minY = Math.Min(py1, py2);
            var maxY = Math.Max(py1, py2);
//            var minX = Math.Min(px1, px2);
//            var maxX = Math.Max(px1, px2);
            if(minY < -CanvasHeight || maxY > CanvasHeight+CanvasHeight) return;

            AddGraphLine(px1, py1, px2, py2, color);
        }

        private Line AddGraphLine(double x1, double y1, double x2, double y2, Color color)
        {
            EnsureNoInfinity(ref x1, ref y1, ref x2, ref y2);

            SolidColorBrush brush;
            if (!Brushes.TryGetValue(color, out brush))
            {
                brush = Brushes[color] = new SolidColorBrush(color);
            }

            Line line;
            TheCanvas.Children.Add(line = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = brush,
                StrokeThickness = 2
            });
            return line;
        }

        private Rectangle _rectangle;
        private DispatcherTimer _animationTimer;
        

        private void AnimateOnClick(object sender, RoutedEventArgs e)
        {
            double from, to, sec;
            if (!Double.TryParse(AniFrom.Text.Trim(), out from) || from < -10 || from > 10)
            {
                MessageBox.Show("'from X' must be number between -10 and 10");
                return;
            }
            if (!Double.TryParse(AniTo.Text.Trim(), out to) || from < -10 || from > 10)
            {
                MessageBox.Show("'to X' must be number between -10 and 10");
                return;
            }
            if (to <= from)
            {
                MessageBox.Show("'to X' must be larger then 'from X'");
                return;
            }
            if (!Double.TryParse(AniTime.Text.Trim(), out sec) || sec < 0.1 || sec > 10)
            {
                MessageBox.Show("'sec' must be number between 0.1 and 10");
                return;
            }
            var source = SourceCode1.Text.Trim();
            if (source == "")
            {
                MessageBox.Show("Please enter equation for the red curve");
                return;
            }
            if (_animationTimer != null)
            {
                MessageBox.Show("Animation is already running");
                return;
            }
            BaseFuncRunner runner;
            IList<string> errors;
            if (!_runnerCreator.TryGetRunner(source, out runner, out errors))
            {
                MessageBox.Show(String.Join("; ", errors));
                return;
            }

            runner.GraphDrawingStarts(Color.FromRgb(0xFF, 0, 0), source);

            var rect = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00)),
                StrokeThickness = 0,
                Width = 50,
                Height = 50
            };
            var rectHalfWidth = 25*_scaleTransform.ScaleX;

            rect.SetValue(Canvas.LeftProperty, CanvasHalfWidth - rectHalfWidth);
            var ystart = runner.Run((to - from) * 0 + from);
            rect.SetValue(Canvas.TopProperty, CanvasHalfHeight + (-PixelsPerOne*ystart) - rectHalfWidth);
            TheCanvas.Children.Add(rect);
            AnimateButton.IsEnabled = false;

     

            var sw = Stopwatch.StartNew();
            Stopwatch sw2 = null;
            _animationTimer = new DispatcherTimer(DispatcherPriority.Render)
                              {
                                  Interval = TimeSpan.FromSeconds(1/60.0)
                              };
            

            var ifFinished = false;
            _animationTimer.Tick += 
                (o, evt) =>
                {
                    var relX = (sw.ElapsedMilliseconds*0.001) / sec;
                    if (relX > 1)
                    {
                        if (!ifFinished)
                        {
                            var yend = runner.Run((to - from) * 1 + from);
                            rect.SetValue(Canvas.TopProperty, CanvasHalfHeight + (-PixelsPerOne * yend) - rectHalfWidth);
                            ifFinished = true;
                            sw2 = Stopwatch.StartNew();       
                        }
                        else if (sw2 != null && sw2.ElapsedMilliseconds > 500)
                        {
                            AnimateButton.IsEnabled = true;
                            TheCanvas.Children.Remove(rect);
                            _animationTimer.Stop();
                            _animationTimer = null;
                            _rectangle = null;
                            ReDrawCanvas();
                        }
                        return;
                    }
                    var x = (to - from)*relX + from;
                    var y = runner.Run(x);
                    rect.SetValue(Canvas.TopProperty, CanvasHalfHeight + (-PixelsPerOne * y) - rectHalfWidth);
                };
            _animationTimer.Start();
            

        }

        private void CodeToBezierUiButtonOnClick(object sender, RoutedEventArgs e)
        {
            var txt = SourceCode1.Text;
            SourceCode2.Text = SourceCode3.Text = SourceCode4.Text = "";
            ReDrawCanvas();
            IBezierGroup bg = _runners[0]?.BezierGroup;
            if (bg == null || bg.FragmentsCount == 0)
            {
                txt = txt
                    .Replace(" ","")
                    .Replace("\t","")
                    .Replace("\n", "")
                    .Replace("\r", "")
                    .Replace("return", "")
                    .ToLowerInvariant();
                if (txt.StartsWith("bezier("))
                {
                    txt = RemoveBezierNonNumbers(txt);
                    var arr = ToNumbers(txt);
                    if (arr.Length == 4)
                    {
                        IBezierGroupBuilder bgb = new BezierGroup();
                        bgb.from(0, 0);
                        bgb.to(1, 1).curve(arr[0],arr[1],arr[2],arr[3]);
                        bg = (IBezierGroup)bgb;
                    }
                    else if (arr.Length == 8)
                    {
                        IBezierGroupBuilder bgb = new BezierGroup();
                        bgb.from(arr[0],arr[1]);
                        bgb.to(arr[6],arr[7]).curve(arr[2],arr[3],arr[4],arr[5]);
                        bg = (IBezierGroup)bgb;
                    }
                }
                else if (txt.StartsWith("bezier2parts"))
                {
                    txt = RemoveBezierNonNumbers(txt);
                    var arr = ToNumbers(txt);
                    if (arr.Length == (8 * 2 - 2))
                    {
                        IBezierGroupBuilder bgb = new BezierGroup();
                        bgb.from(arr[0], arr[1]);
                        bgb.to(arr[6], arr[7]).curve(arr[2], arr[3], arr[4], arr[5]);
                        bgb.to(arr[12], arr[13]).curve(arr[8], arr[9], arr[10], arr[11]);
                        bg = (IBezierGroup)bgb;
                    }
                }
                else if (txt.StartsWith("bezier3parts"))
                {
                    txt = RemoveBezierNonNumbers(txt);
                    var arr = ToNumbers(txt);
                    if (arr.Length == (8 * 3 - 2 * 2))
                    {
                        IBezierGroupBuilder bgb = new BezierGroup();
                        bgb.from(arr[0], arr[1]);
                        bgb.to(arr[6], arr[7]).curve(arr[2], arr[3], arr[4], arr[5]);
                        bgb.to(arr[12], arr[13]).curve(arr[8], arr[9], arr[10], arr[11]);
                        bgb.to(arr[18], arr[19]).curve(arr[14], arr[15], arr[16], arr[17]);
                        bg = (IBezierGroup)bgb;
                    }
                }
                else if (txt.StartsWith("bezier4parts"))
                {
                    txt = RemoveBezierNonNumbers(txt);
                    var arr = ToNumbers(txt);
                    if (arr.Length == (8 * 4 - 3 * 2))
                    {
                        IBezierGroupBuilder bgb = new BezierGroup();
                        bgb.from(arr[0], arr[1]);
                        bgb.to(arr[6], arr[7]).curve(arr[2], arr[3], arr[4], arr[5]);
                        bgb.to(arr[12], arr[13]).curve(arr[8], arr[9], arr[10], arr[11]);
                        bgb.to(arr[18], arr[19]).curve(arr[14], arr[15], arr[16], arr[17]);
                        bgb.to(arr[24], arr[25]).curve(arr[20], arr[21], arr[22], arr[23]);
                        bg = (IBezierGroup)bgb;
                    }
                }


                if (bg == null)
                {
                    IBezierGroupBuilder bgb = new BezierGroup();
                    bgb.from(0, 0);
                    bgb.to(1, 1).curve(0.17,0.67,0.8,0.33);
                    bg = (IBezierGroup)bgb;
                }
            }

            // ReSharper disable once PossibleNullReferenceException
            GenerateBezierUi(bg, TheCanvas, this);
        }
        static string RemoveBezierNonNumbers(string str)
        {
            return new StringBuilder(str)
                .Replace("bezier4parts", "")
                .Replace("bezier3parts", "")
                .Replace("bezier2parts", "")
                .Replace("bezier", "")
                .Replace("(", "")
                .Replace("x,", "")
                .Replace("f,", "")
                .Replace("m,", "")
                .Replace(")", "")
                .Replace(";", "")
                .ToString();
        }
        static double[] ToNumbers(string str)
        {
            return str.Split(',')
                    .Select(s =>
                    {
                        double d;
                        if (double.TryParse(s, out d))
                        {
                            return d;
                        }
                        return double.NaN;
                    })
                    .Where(d => !double.IsNaN(d))
                    .ToArray();
        }
        private void GenerateBezierUi(IBezierGroup bezierGroup, Canvas canvas, IDrawingBoardHolder eventsHolder)
        {
            canvas.Children.Clear();
            DrawLines();
            _beziers.Clear(canvas, eventsHolder);
            foreach (var f in bezierGroup.Fragments)
            {
                _beziers.AddBezier(canvas, eventsHolder, f);
            }
        }


        private void SingleBezierOnClick(object sender, RoutedEventArgs e)
        {
            SourceCode1.Text = "bezier(x,.17,.67,.8,.33)";
            ReDrawCanvas();
        }
        private void DoubleBezierOnClick(object sender, RoutedEventArgs e)
        {
            SourceCode1.Text = "return bezier2parts(x, \n"+
                            "   0.00, 0.00, 0.50, 0.00, 0.30, 1.00,\n"+
                            "   0.60, 1.00, 1.00, 1.00, 1.00, 0.30, 1.00, 0.00)";
            ReDrawCanvas();
        }
        private void TrippleBezierOnClick(object sender, RoutedEventArgs e)
        {
            SourceCode1.Text = "return bezier3parts(x, \n" +
                               "   0.00, 0.00, 0.19, -1.99, 0.20, 1.76,\n" +
                               "   0.32, 0.53, 0.53, -1.84, 0.52, 1.76,\n" +
                               "   0.74, -0.53, 0.82, -1.37, 0.88, 0.53, 1.00, 0.00)";
            ReDrawCanvas();
        }
        private void QuadroBezierOnClick(object sender, RoutedEventArgs e)
        {
            SourceCode1.Text = "return bezier4parts(x, \n" +
                               "   0.00, 0.00, 0.22, -1.72, 0.17, 0.67,\n" +
                               "   0.25, 0.00, 0.36, -0.86, 0.31, 1.07,\n" +
                               "   0.50, 0.00, 0.59, -0.48, 0.68, 0.30,\n" +
                               "   0.75, 0.00, 0.84, -0.31, 0.92, 0.32, 1.00, 0.00)";
            ReDrawCanvas();
        }
        private void ClearBezierOnClick(object sender, RoutedEventArgs e)
        {
            _beziers.Clear(TheCanvas, this);
            TheCanvas.Children.Clear();
            DrawLines();
        }

        internal class DraggedBezierPoint
        {
            public DraggedBezierPoint(IBezierCurve curve, DotType type)
            {
                Curve = curve;
                Type = type;
            }
            public IBezierCurve Curve { get; private set; }
            public DotType Type { get; private set; }
        }
    }

    internal static class MainWindowsExtensions
    {
        public static Color MultiplyBy(this Color c, double factor)
        {
            factor = factor < 0 ? 0.0 : factor > 1 ? 1.0 : factor;
            if (factor > 0.9999999) return c;
            return Color.FromArgb(c.A,(byte)(c.R*factor),(byte)(c.G*factor),(byte)(c.B*factor));
        }

        public static void GetScreenPoints(this Path path, out Point a, out Point b, out Point c, out Point d)
        {
            var pg = (PathGeometry) path.Data;
            var pf = pg.Figures[0];
            var bs = (BezierSegment) pf.Segments[0];
            a = pf.StartPoint;
            b = bs.Point1;
            c = bs.Point2;
            d = bs.Point3;
        }

        public static void GetGraphPoints(this Path path, IDrawingBoardHolder holder, out Point a, out Point b, out Point c, out Point d)
        {
            path.GetScreenPoints(out a, out b, out c, out d);

            a = new Point(holder.PixelToGraphX(a.X), holder.PixelToGraphY(a.Y));
            b = new Point(holder.PixelToGraphX(b.X), holder.PixelToGraphY(b.Y));
            c = new Point(holder.PixelToGraphX(c.X), holder.PixelToGraphY(c.Y));
            d = new Point(holder.PixelToGraphX(d.X), holder.PixelToGraphY(d.Y));
        }

        public static StringBuilder AppendCurves(
            this StringBuilder sb, 
            IEnumerable<IBezierCurve> curves,
            IDrawingBoardHolder holder,
            Func<Point, Point, Point, Point, string> format)
        {
            foreach (var curve in curves)
            {
                Point a, b, c, d;
                curve.Path.GetGraphPoints(holder, out a, out b, out c, out d);
                sb.AppendLine(format(a,b,c,d));
            }
            return sb;
        }

        public static void SetLines(this IBezierCurve curve)
        {
            curve.Line1.X1 = curve.PathFigure.StartPoint.X;
            curve.Line1.Y1 = curve.PathFigure.StartPoint.Y;
            curve.Line1.X2 = curve.BezierSegment.Point1.X;
            curve.Line1.Y2 = curve.BezierSegment.Point1.Y;
            curve.Line2.X1 = curve.BezierSegment.Point3.X;
            curve.Line2.Y1 = curve.BezierSegment.Point3.Y;
            curve.Line2.X2 = curve.BezierSegment.Point2.X;
            curve.Line2.Y2 = curve.BezierSegment.Point2.Y;
        }
    }

}
