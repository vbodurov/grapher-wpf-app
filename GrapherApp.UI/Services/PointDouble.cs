namespace GrapherApp.UI.Services
{
    public struct PointDouble
    {
        public PointDouble(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}