using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XboxController
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private readonly double windowWidth = SystemParameters.WorkArea.Width;
        private readonly double windowHeight = SystemParameters.WorkArea.Height;

        int POLYS;
        double SIZE;
        double polyAngle;
        double angleGap;
        Polygon selectedPoly = null;

        public Window1()
        {
            Init();
            InitializeComponent();
        }

        private void Init()
        {
            POLYS = 25;
            SIZE = windowHeight / 4;
            polyAngle = (2.0 * Math.PI) / POLYS;
            angleGap = 0.5 * (Math.PI / 180.0);
        }

        private void CreatePolygons()
        {
            Console.WriteLine(windowWidth + ", " + windowHeight);

            for (int i = 0; i < POLYS; i++)
            {
                Polygon p = new Polygon();

                Point point1 = new Point(windowWidth / 2, windowHeight / 2); // always centered
                Point point2 = new Point(windowWidth / 2 + Math.Cos(polyAngle * i) * SIZE, windowHeight / 2 + Math.Sin(polyAngle * i) * SIZE);
                Point point3 = new Point(windowWidth / 2 + Math.Cos(polyAngle * (i + 1) - angleGap) * SIZE, windowHeight / 2 + Math.Sin(polyAngle * (i + 1) - angleGap) * SIZE);

                PointCollection polygonPoints = new PointCollection();
                polygonPoints.Add(point1);
                polygonPoints.Add(point2);
                polygonPoints.Add(point3);
                
                p.Points = polygonPoints;


                /*// Create a linear gradient brush with five stops 
                LinearGradientBrush fiveColorLGB = new LinearGradientBrush();
                fiveColorLGB.StartPoint = new Point(0.5 + ((point2.X + point3.X - (windowWidth - SIZE)) / 2 - SIZE) / (SIZE * 2), 0.5 + ((point2.Y + point3.Y) / 2 - SIZE) / (SIZE * 2));
                fiveColorLGB.EndPoint = new Point(0.5, 0.5);

                // Create and add Gradient stops
                GradientStop blueGS = new GradientStop();
                blueGS.Color = Colors.White;
                blueGS.Offset = 1.0;
                fiveColorLGB.GradientStops.Add(blueGS);*/


                p.Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                p.Fill.Opacity = .2;

                m_Grid.Children.Add(p);
            }
        }

        public void SelectPoly(double angle)
        {
            if (angle < 0.0) angle = 2.0 * Math.PI + angle;
            int index = (int)(angle / polyAngle);
            if (selectedPoly != null) selectedPoly.Fill.Opacity = .2;
            Polygon p = m_Grid.Children[POLYS - index - 1] as Polygon;
            p.Fill.Opacity = .6;
            selectedPoly = p;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // resize and center window
            m_Window.Width = windowWidth;
            m_Window.Height = windowHeight;
            m_Window.Left = 0;
            m_Window.Top = 0;

            CreatePolygons();
        }
    }
}
