using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GrapherApp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScaleTransform _scale;

        public MainWindow()
        {
            InitializeComponent();

            SetupCanvas();

            var t = new TransformGroup();
            t.Children.Add(_scale = new ScaleTransform(1, 1, 275, 275));
            TheCanvas.RenderTransform = t;

            MouseWheel += TheCanvas_MouseWheel;
            Version version = Assembly.GetEntryAssembly().GetName().Version;
            this.Title = "Grapher " + version.Major + "." + version.Minor;
        }
        void TheCanvas_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var n = e.Delta > 0 ? 0.05 : -0.05;
            var candidate = _scale.ScaleX + n;
            if(candidate > 0.2 && candidate < 5) _scale.ScaleX = _scale.ScaleY = candidate;
        }

        


        
        private readonly List<Line> _lines = new List<Line>(); 

        private readonly Evaluator _evaluator = new Evaluator();
        private void DrawGraphFromSource(Color color, string source)
        {

            

            var x2 = Double.NaN;
            var y2 = Double.NaN;

            const double edge = 8.0;
            for (var i = -edge; i <= edge; i += 0.005)
            {
                double x1 = i;
                string error = null;
                double y1;
                try
                {
                    y1 = _evaluator.Evaluate(source, x1, out error);
                }
                catch(Exception ex)
                {
                    AddErrorMessage(error);
                    break;
                }
                if (Double.IsNaN(y1) || Double.IsInfinity(y1))
                {
                    if (error != null)
                    {
                        AddErrorMessage(error);
                        break;
                    }

                    //MessageBox.Show("NaN");
                    continue;
                }

                if (!Double.IsNaN(x2) && !Double.IsNaN(y2) && !Double.IsInfinity(x2) && !Double.IsInfinity(y2) && y1 < edge && y1 > -edge)
                {
                    AddLine(x1, y1, x2, y2, color);
                }
                
                x2 = x1;
                y2 = y1;
            }
        }

        private void AddLine(double x1, double y1, double x2, double y2, Color color)
        {
            const double half = 1.1;
            const double pixels = 550;
            _lines.Add(AddGraphLine(
                ((x1 + half) * pixels) / 2.2,
                ((-y1 + half) * pixels) / 2.2,
                ((x2 + half) * pixels) / 2.2,
                ((-y2 + half) * pixels) / 2.2,
                color));
        }

        private void AddErrorMessage(string error)
        {
            if (Message.Text.Length < 1024)
            {
                if (Message.Text.Length > 0) Message.Text += "; ";
                Message.Text += error;
            }
        }

        private void SetupCanvas()
        {
            const double w = 550.0;
            const int n = 22;
            const double p = w / n;
            for(var i = 0; i <= n; ++i)
            {
                AddGridLine(p * i, 0, p * i, w, i);
                AddGridLine(0, p * i, w, p * i, i);
            }
        }

        private Line AddGridLine(double x1, double y1, double x2, double y2, int i)
        {
            EnsureNoInfinity(ref x1, ref y1, ref x2, ref y2);

            var axis = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xBB, 0xBB));
            var primary = new SolidColorBrush(Color.FromArgb(0xFF, 0xAA, 0xAA, 0xAA));
            var secondary = new SolidColorBrush(Color.FromArgb(0x99, 0xCC, 0xCC, 0xCC));

            var brush = (i == 1 || i == 21)
                                ? primary
                                : (i == 11)
                                    ? axis
                                    : secondary;
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

        private Line AddGraphLine(double x1, double y1, double x2, double y2, Color color)
        {
            EnsureNoInfinity(ref x1, ref y1, ref x2, ref y2);

            var brush = new SolidColorBrush(color);

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
            _lines.ForEach(l => TheCanvas.Children.Remove(l));
            _lines.Clear();

            Message.Text = "";
            try
            {
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
//            if (code.StartsWith("TST")) { RunTestCode(color, Int32.Parse(code.Replace("TST", ""))); return; }

            if (code != "") DrawGraphFromSource(color, sourceCode.Text);
        }


//        #region Test Code
//        private void RunTestCode(Color color, int n)
//        {
//            var px = Double.NaN;
//            var py = Double.NaN;
//            for (var x = -1.0; x < 1.0; x += 0.01) // for each frame
//            {
//
//                var y = OnEachFrame(n);
//
//                if (!Double.IsNaN(px))
//                {
//                    AddLine(px, py, x, y, color);
//                }
//                px = x;
//                py = y;
//            }
//        }
//
//
//        private const double _c_max_move = 0.01;
//
//        private double _m_move;
//        private double OnEachFrame(int n)
//        {
//            var requestedSpeed = n/10.0;
//            var x = _m_move / (Math.Abs(requestedSpeed - 0) < 0.0000001 ? 0.0000001 : requestedSpeed);
//            var p = Math.Pow(5, -(Math.Pow((x - 0.5), 2) / Math.Pow(0.76, 2)))*2-1;
//    
//            _m_move += _c_max_move*p;
//
//            return (_m_move <= 0) ? 0 : _m_move;
//        }
//
//        #endregion
    }
}
