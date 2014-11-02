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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MapMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point mousePoint;
        Point mousePointClicked; // when he started pressing and then moving..
        bool isPrinting = false;
        Point spriteDimensions;
        List<MapImage> mapImages;

        private int width;
        private int height;
        public MainWindow()
        {
            InitializeComponent();

            isPrinting = false;
            mapImages = new List<MapImage>();
            spriteDimensions = new Point();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPrinting = true;
            mousePointClicked = mousePoint;
        }
        private void imageMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPrinting = true;
            mousePointClicked = mousePoint;
            changeSourceForImage((Image)sender);
        }

        private void drawer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isPrinting = false;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {

            processMouseInput();

            if (isPrinting)
            {
                //get the current position

            }
        }

        private void imageMouseMove(object sender, MouseEventArgs e)
        {
            if(isPrinting == true)
            {
                changeSourceForImage((Image)sender);
            }
        }


        private void changeSourceForImage(Image image)
        {
            int id = int.Parse((string)LocationIdComboBox.SelectionBoxItem);

            int zindex = Canvas.GetZIndex(image);
            image.Source = new BitmapImage(new Uri("Images/"+id+".png", UriKind.Relative));


            mapImages[zindex].Location = "Images/" + id + ".png";
            mapImages[zindex].LocationId = id;
        }













        private void processMouseInput()
        {
            mousePoint = Point.ConvertPoint(Mouse.GetPosition((Grid)drawer));
            mousePoint.X += (int)MapScrollViewer.HorizontalOffset;
            mousePoint.Y += (int)MapScrollViewer.VerticalOffset;

            CursorCoordinateMapX.Text = ((int)mousePoint.X).ToString();
            CursorCoordinateMapY.Text = ((int)mousePoint.Y).ToString();
        }

        private void RedimensionateButton_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse((string)LocationIdComboBox.SelectionBoxItem);

            width = int.Parse(BoxesWidthTextBox.Text);
            height = int.Parse(BoxesHeightTextBox.Text);

            spriteDimensions.X = 1280 / width;
            spriteDimensions.Y = 720 / height;

            mapImages.Clear();
            drawer.Children.Clear();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    MapImage mapImage = new MapImage();
                    mapImage.MapDimensions = spriteDimensions;
                    mapImage.Location = "Images/"+id+".png";
                    mapImage.pos = new Point(spriteDimensions.X * i, spriteDimensions.Y * j);
                    mapImage.isStretched = true;
                    mapImage.Zindex = mapImages.Count;
                    //mapImage.Id = mapImages.Count;
                    mapImage.LocationId = id;
                    mapImages.Add(mapImage);
                    addMapImageToDrawer(mapImage);
                }
            }




            foreach (FrameworkElement frameworkElement in drawer.Children)
            {
                Image image = (Image)frameworkElement;

                image.Width = spriteDimensions.X;
                image.Height = spriteDimensions.Y;
            }

        }


        private Image addMapImageToDrawer(MapImage mapImage)
        {
            Image image = new Image();

            image.Source = new BitmapImage(new Uri(mapImage.Location, UriKind.Relative));
            image.Margin = new Thickness(mapImage.pos.X, mapImage.pos.Y, 0, 0);
            image.Width = mapImage.MapDimensions.X;
            image.Height = mapImage.MapDimensions.Y;
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.VerticalAlignment = VerticalAlignment.Top;

            image.MouseDown += imageMouseButtonDown;
            image.MouseUp += drawer_MouseLeftButtonUp;
            image.MouseMove += imageMouseMove;

            Canvas.SetZIndex(image, mapImage.Zindex);

            if (mapImage.isStretched)
            {
                //image.Stretch = Stretch.UniformToFill;
                image.Stretch = Stretch.Fill;
            }

            drawer.Children.Add(image);

            return image;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("output.txt", false))
            {
                file.WriteLine(width + " " + height);

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        file.Write(mapImages[j * height + i].LocationId);
                        if(j != width -1)
                            file.Write(" ");
                    }
                    file.WriteLine();
                }
            }

        }
    }
}
