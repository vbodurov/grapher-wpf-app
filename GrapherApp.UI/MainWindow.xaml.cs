using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.Reflection;
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
    public partial class MainWindow : Window
    {
        private readonly ScaleTransform _scaleTransform;
        private readonly TranslateTransform _translateTransform;
        private readonly IFuncRunnerCreator _runnerCreator;
        private readonly DispatcherTimer _timer;


        private double _scale = 1;
        private Point _translate = new Point(0,0);
        private Point? _dragPosition = null;


        public MainWindow()
        {
            InitializeComponent();

            

            var t = new TransformGroup();
            t.Children.Add(_scaleTransform = new ScaleTransform(1, 1, 275, 275));
            t.Children.Add(_translateTransform = new TranslateTransform(0,0));
            TheCanvas.RenderTransform = t;

            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
            _timer.Tick += OnTimerTick;

            KeyDown += MainWindow_KeyDown;
            MouseWheel += TheCanvas_MouseWheel;
            MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            MouseMove += MainWindow_MouseMove;
            MouseLeftButtonUp += MainWindow_MouseLeftButtonUp;

            var nameSuffix = "";
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version version = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                nameSuffix = " " +version.Major + "." + version.Minor;
            }
            Title = "Grapher" + nameSuffix;

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
            var n = e.Delta > 0 ? 0.05 : -0.05;
            var candidate = _scaleTransform.ScaleX + n;
            if (candidate > 0.2 && candidate < 5)
            {
                var pre = _scaleTransform.ScaleX;

                _scaleTransform.ScaleX = _scaleTransform.ScaleY = candidate;

                _scale *= (1.0 + (candidate-pre));
            }

            _timer.Stop();
            _timer.Start();
        }
        void OnTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            ReDrawCanvas();
        }
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragPosition = e.GetPosition(OuterCanvas);
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
            var diff = position - _dragPosition.Value;
            _translate.X += diff.X;
            _translate.Y += diff.Y;
            _translateTransform.X += diff.X;
            _translateTransform.Y += diff.Y;
            _dragPosition = position;
        }
        void MainWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            OnDragEnds();
        }








        private void OnDragEnds()
        {
            _dragPosition = null;
            ReDrawCanvas();
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
                AddGridLine(GraphToPixelX(x), GraphToPixelY(-10), GraphToPixelX(x), GraphToPixelY(10), GetTypeByValue(x));
            }
            for (var y = -10; y <= 10; y += 1)
            {
                AddGridLine(GraphToPixelX(-10), GraphToPixelY(y), GraphToPixelX(10), GraphToPixelY(y), GetTypeByValue(y));
            }
            if (_scale > 0.3)
            {
                for (var x = -1.1; x <= 1.1; x += 0.1)
                {
                    if (Math.Abs(x - Math.Round(x)) > 0.0001)
                        AddGridLine(GraphToPixelX(x), GraphToPixelY(-1.1), GraphToPixelX(x), GraphToPixelY(1.1), GetTypeByValue(x));
                }
                for (var y = -1.1; y <= 1.1; y += 0.1)
                {
                    if (Math.Abs(y - Math.Round(y)) > 0.0001)
                        AddGridLine(GraphToPixelX(-1.1), GraphToPixelY(y), GraphToPixelX(1.1), GraphToPixelY(y), GetTypeByValue(y));
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
            if (Double.IsNegativeInfinity(x1)) x1 = -10000;
            if (Double.IsPositiveInfinity(x1)) x1 = 10000;
            if (Double.IsNegativeInfinity(y1)) y1 = -10000;
            if (Double.IsPositiveInfinity(y1)) y1 = 10000;
            if (Double.IsNegativeInfinity(x2)) x2 = -10000;
            if (Double.IsPositiveInfinity(x2)) x2 = 10000;
            if (Double.IsNegativeInfinity(y2)) y2 = -10000;
            if (Double.IsPositiveInfinity(y2)) y2 = 10000;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ReDrawCanvas();
        }

        private void ReDrawCanvas()
        {
            TheCanvas.Children.Clear();

            _scaleTransform.ScaleX = _scaleTransform.ScaleY = 1;
            _scaleTransform.CenterX = 275 + _translate.X;
            _scaleTransform.CenterY = 275 + _translate.Y;
            _translateTransform.X = _translateTransform.Y = 0;

            Message.Text = "";
            try
            {
                DrawLines();
                DrawIfHasCode(Colors.Red, SourceCode1);
                DrawIfHasCode(Colors.Green, SourceCode2);
                DrawIfHasCode(Colors.Blue, SourceCode3);
                DrawIfHasCode(Colors.Purple, SourceCode4);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void DrawIfHasCode(Color color, TextBox sourceCode)
        {
            var code = sourceCode.Text.Trim();

            if (code != "") DrawGraphFromSource(color, sourceCode.Text);
        }

        private void DrawGraphFromSource(Color color, string source)
        {
            BaseFuncRunner runner;
            IList<string> errors;
            if (!_runnerCreator.TryGetRunner(source, out runner, out errors))
            {
                Message.Text = String.Join("; ", errors);
                return;
            }

            var x2 = Double.NaN;
            var y2 = Double.NaN;

            var fromX = PixelToGraphX(-225);
            var toX = PixelToGraphX(1024-225);
            var step = 0.005/_scale;

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
                if (Double.IsNaN(y1) || Double.IsInfinity(y1))
                {
                    continue;
                }


                if (!Double.IsNaN(x2) && !Double.IsNaN(y2) && !Double.IsInfinity(x2) && !Double.IsInfinity(y2))
                {
                    AddLine(x1, y1, x2, y2, color);
                }

                x2 = x1;
                y2 = y1;
            }
        }

        
        const double width = 550;
        const double halfWidth = width * 0.5;
        private double GraphToPixelX(double graphCoord)
        {
            return (halfWidth + graphCoord * halfWidth * _scale + _translate.X);
        }
        private double GraphToPixelY(double graphCoord)
        {
            return (halfWidth + graphCoord * halfWidth * _scale + _translate.Y);
        }

        private double PixelToGraphX(double pixelCoord)
        {
            return (pixelCoord - _translate.X - halfWidth) / (halfWidth*_scale);
        }
        private double PixelToGraphY(double pixelCoord)
        {
            return (pixelCoord - _translate.Y - halfWidth) / (halfWidth * _scale);
        }

        private static readonly SolidColorBrush BrushAxis = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xBB, 0xBB));
        private static readonly SolidColorBrush BrushMajor = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xAA, 0xAA));
        private static readonly SolidColorBrush BrushMinor = new SolidColorBrush(Color.FromArgb(0x99, 0xCC, 0xCC, 0xCC));
        private static readonly IDictionary<Color,SolidColorBrush> Brushes = new Dictionary<Color, SolidColorBrush>();

        private Line AddGridLine(double x1, double y1, double x2, double y2, GridLineType type)
        {
            if ((y1 < -50 && y2 < -50) || (y1 > 600 && y2 > 600)) return null;
            if ((x1 < -300 && x2 < -300) || (x1 > 900 && x2 > 900)) return null;

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
            var px1 = GraphToPixelX(x1);
            var py1 = GraphToPixelY(-y1);
            var px2 = GraphToPixelX(x2);
            var py2 = GraphToPixelY(-y2);

            if ((py1 < -100 && py2 < -100) || (py1 > 600 && py2 > 600)) return;

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

        
    }
}
