using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using Microsoft.VisualBasic;


namespace gmap1._2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool lblFirstTimeMem = true; // used for server_Only/ cache_Only and Server_And_Cache
        private bool lblFirstTimeMap = true;// Used for type of map Gmap/ Street Map/ Google Map etc
        GMapMarker marker2;// USed to add markers via Click and Buttons
        GMapPolygon gpol;// Used to draw and Delete Cricle
        public static Socket sender;
        private bool firstTimeMoveMap = true;
        List<PointLatLng> PolygonPoints = new List<PointLatLng>();
        private bool DMIC_turn= true;
        //private Socket sender;

        public MainWindow()
        {
            InitializeComponent();
        }
        public struct GeoLocation
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
        private void ClearAddMarkerBoxes()
        {
            tbklate.Text = "";
            tbklongi.Text = "";
            tbkName.Text = "";
            tbkDescrip.Text = "";
        }
        private void ClearCircleBoxes()
        {
            circleColor.Text = "";
            CircleLongi.Text = "";
            circleLati.Text = "";
            circleRadius.Text = "";
        }
        private void PolygonExist() {
            try
            {
                int markers_count = mapView.Markers.Count();


                for (int j = markers_count - 1; j >= 0; j--)
                {
                    GMapMarker tempMarker = mapView.Markers[j];
                    if (tempMarker.Tag.Equals(tbxPolygonID.Text))
                    {
                        mapView.Markers.Remove(tempMarker);                       
                    }
                }
                
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Fill Text box properly");
            }
        }
        private void cbxMap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblFirstTimeMem)
            {
                lblFirstTimeMem = false;
            }
            else
            {
                ComboBoxItem typeItem = (ComboBoxItem)cbxMap.SelectedItem;
                string value = typeItem.Content.ToString();
                if (value.Equals("Server And Cache"))
                {
                    GMaps.Instance.Mode = AccessMode.ServerAndCache;
                    if (!lblFirstTimeMem)
                    {
                    }

                }

                if (value.Equals("Server"))
                {
                    GMaps.Instance.Mode = AccessMode.ServerOnly;

                }

                if (value.Equals("Cache"))
                {
                    mapView.CacheLocation = @"C:\Users\Computer\AppData\Local\GMap.NET\";
                    GMaps.Instance.Mode = AccessMode.CacheOnly;        
                    
                    circleLati.Text = mapView.CacheLocation.ToString();
                }
                //MessageBox.Show(value);

            }

        } //event handler for the memoery Combo box
        private void mapView_Loaded(object sender, RoutedEventArgs e)
        {
            loadMap();
            
        }// loading MAP on the front end
        private void loadMap()
        {
            ComboBoxItem typeItem = (ComboBoxItem)cbxMap.SelectedItem;
            string value = typeItem.Content.ToString();

            if (value.Equals("Server And Cache"))
            {
                GMaps.Instance.Mode = AccessMode.ServerAndCache;                

            }

            if (value.Equals("Server"))
            {
                GMaps.Instance.Mode = AccessMode.ServerOnly;
            }

            if (value.Equals("Cache"))
            {
                GMaps.Instance.Mode = AccessMode.CacheOnly;

                mapView.CacheLocation = @"C:\Users\Computer\AppData\Local\GMap.NET\";
            }


            // choose your provider here
            ComboBoxItem typeItems = (ComboBoxItem)cbxMaps.SelectedItem;
            string values = typeItems.Content.ToString();
            if (values.Equals("Street Maps"))
            {

                mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;

            }
            if (values.Equals("Google"))
            {
                mapView.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;

            }

            if (values.Equals("Satellite"))
            {
                mapView.MapProvider = GMap.NET.MapProviders.BingSatelliteMapProvider.Instance;
            }

            mapView.MinZoom = 2;
            mapView.MaxZoom = 17;
            // whole world zoom
            mapView.Zoom = 6;
            // lets the map use the mousewheel to zoom
            mapView.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
            // lets the user drag the map

            // lets the user drag the map with the left mouse button

        }// main function for loading map
        private void mapView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {               
                if (cbxAllowMark.IsChecked == true)
                {
                    Double latetude, longitude;
                    var mousePosition = e.GetPosition(mapView);

                    //latetude = mapView.Position.Lat;
                    //longitude = mapView.Position.Lng;

                    //MessageBox.Show("latetude is: " + latetude.ToString() + "and longitude is: " + longitude.ToString());

                    var ptr = mapView.FromLocalToLatLng(Convert.ToInt32(mousePosition.X), Convert.ToInt32(mousePosition.Y));
                    latetude = ptr.Lat;
                    longitude = ptr.Lng;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri("C:\\Users\\Computer\\OneDrive\\Desktop\\images\\mark.png");
                    bitmapImage.EndInit();
                    marker2 = new GMapMarker(new PointLatLng(latetude, longitude));
                    Image image = new Image();
                    image.Width = 30;
                    image.Height = 30;
                    image.Source = bitmapImage;
                    marker2.Tag = "Drop Marker";
                    marker2.Shape = image;
                    //marker2.ZIndex = markerCount;
                    //markerCount+= 1;
                    mapView.Markers.Add(marker2);
                    mapView.ShowCenter = true;
                }                           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }// mouse click even for adding markers
        private void cbxDrag_Checked(object sender, RoutedEventArgs e)
        {
            mapView.CanDragMap = true;
            mapView.DragButton = MouseButton.Left;

        }// Allow Drag
        private void cbxDrag_Unchecked(object sender, RoutedEventArgs e)
        {
            mapView.CanDragMap = false;
        }// don't Allow Drag
        private void cbxMaps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lblFirstTimeMap)
            {
                lblFirstTimeMap = false;
            }
            else {
                ComboBoxItem typeItem = (ComboBoxItem)cbxMaps.SelectedItem;
                string value = typeItem.Content.ToString();
                if (value.Equals("Street Maps"))
                {                 
                    mapView.MapProvider = GMap.NET.MapProviders.OpenStreetMapProvider.Instance;                  
                }
                if (value.Equals("Google"))
                {
                    mapView.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
                }
                if (value.Equals("Satellite"))
                {
                    mapView.MapProvider = GMap.NET.MapProviders.BingSatelliteMapProvider.Instance;
                }
            }

        } // event handler for types of Maps
        private void cbxGrids_Checked(object sender, RoutedEventArgs e)
        {
            mapView.ShowTileGridLines = true;
        }// Displying Grid on the Map
        private void cbxGrids_Unchecked(object sender, RoutedEventArgs e)
        {
            mapView.ShowTileGridLines = false;
        }// Undisplay grid 
        private void waysToAddMarkersInGmap()
        {
            ////////////////////////////////////////////////Method one

            //MessageBox.Show("Type conversion done");
            //GMapMarker marker = new GMarkerGoogle(new PointLatLng(latetude, longitude), GMarkerGoogleType.blue_pushpin);

            //mapView.Position = marker.Position;

            ////////////////////////////////////////////////Method two

            //mapView.Position = new GMap.NET.PointLatLng(latetude, longitude);
            //mapView.ShowCenter = false;

            //GMap.NET.WindowsPresentation.GMapMarker marker = new GMap.NET.WindowsPresentation.GMapMarker(new GMap.NET.PointLatLng(latetude, longitude));
            //marker.Shape = new Ellipse
            //{
            //    Width = 10,
            //    Height = 10,
            //    Stroke = Brushes.Red,
            //    StrokeThickness = 1.5,
            //    ToolTip = "This is tooltip",
            //    Visibility = Visibility.Visible,
            //    Fill = Brushes.Red,
            //    Source = new BitmapImage(new System.Uri("pack://application:,,,/assets/marker.png")),
            //};
            //mapView.Markers.Add(marker);

            //GMap.NET.WindowsPresentation.GMapMarker marker = new GMap.NET.WindowsPresentation.GMapMarker(new GMap.NET.PointLatLng(latetude, longitude));
            //marker.Shape = new Ellipse
            //{
            //    Width = 10,
            //    Height = 10,
            //    Stroke = Brushes.Red,
            //    StrokeThickness = 1.5,
            //    ToolTip = "This is tooltip",
            //    Visibility = Visibility.Visible,
            //    Fill = Brushes.Red,

            //};
            //mapView.Markers.Add(marker);

            //////////////////////////////////////////////Main mthod

            //System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            //bitmapImage.BeginInit();
            //bitmapImage.UriSource = new Uri("C:\\Users\\Computer\\Desktop\\images\\mark.png");
            //bitmapImage.EndInit();
            //GMap.NET.WindowsPresentation.GMapMarker marker2 = new GMap.NET.WindowsPresentation.GMapMarker(new GMap.NET.PointLatLng(latetude, longitude));
            //System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            //image.Width = 30;
            //image.Height = 30;
            //image.Source = bitmapImage;
            //marker2.Shape = image;
            //mapView.Markers.Add(marker2);
            //mapView.ShowCenter = true;
            //mapView.Position = marker2.Position;
        }// methods to handle maps for self use
        public void CityBlockdsitance (Double circle_X, Double circle_Y, Double Marker_X, Double Marker_Y, Double Radius)
        {
            Double x = Math.Abs(circle_X - Marker_X) + Math.Abs(circle_Y - Marker_Y);
            MessageBox.Show("Value of x is " + x);
            MessageBox.Show("Value of Radius is " + Radius / 6378.1);
            if (x <= Radius / 6378.1)
            {
                MessageBox.Show("Marker lies in the Circle");
            }
            else
            {
                MessageBox.Show("It don*t");
            }
        }
        private void btnAddMark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Double latetude, longitude;
                latetude = Double.Parse(tbklate.Text);
                longitude = Double.Parse(tbklongi.Text);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("C:\\Users\\Computer\\OneDrive\\Desktop\\images\\mark.png");
                bitmapImage.EndInit();

                marker2 = new GMapMarker(new PointLatLng(latetude, longitude));
                Image image = new Image();
                image.Width = 30;
                image.Height = 30;
                image.Source = bitmapImage;
                ToolTip tp = new ToolTip();
                tp.Background = Brushes.DarkBlue;
                tp.Foreground = Brushes.White;
                tp.Content = tbkDescrip.Text;
                image.ToolTip = tp;
                marker2.Tag = "marker@" + tbkName.Text;
                marker2.Shape = image;
                //marker2.ZIndex = markerCount;
                //markerCount+=1;
                mapView.Markers.Add(marker2);
                mapView.ShowCenter = true;
                mapView.Position = marker2.Position;

                //mapView.SetPositionByKeywords("niteroi");
                ClearAddMarkerBoxes();

                //MessageBox.Show("No Area Chosen", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                MessageBox.Show("marker added");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }// button Click functions used to add marker with 
        private void btnClearMarker_Click(object sender, RoutedEventArgs e)
        {

            int markers_count = mapView.Markers.Count();
            MessageBox.Show("marker count " + markers_count);

            int dropMarkCount = 0, btnDropCount = 0;

            foreach (var m in mapView.Markers)
            {
                if (m.Tag.Equals("Drop Marker"))
                {
                    dropMarkCount++;

                }
                else
                {
                    btnDropCount++;
                }
            }// count tag and Drop markers

            MessageBox.Show("Drop Marker: " + dropMarkCount + "Button Drop Count: " + btnDropCount);

            if (tbxRemovingMarkerName.Text.Equals("Drop Marker") || tbxRemovingMarkerName.Text.Equals(""))
            {
                for (int j = markers_count - 1; j >= 0; j--)
                {
                    GMapMarker tempMarker = mapView.Markers[j];
                    if (tempMarker.Tag.Equals("Drop Marker"))
                    {
                        mapView.Markers.Remove(tempMarker);
                        //MessageBox.Show("remove Marker" + tempMarker.Tag.ToString());
                    }
                }
            }
            else
            {
                for (int j = markers_count - 1; j >= 0; j--)
                {
                    GMapMarker tempMarker = mapView.Markers[j];
                    String removeMarkerBoxStr = "marker@" + tbxRemovingMarkerName.Text;
                    if (tempMarker.Tag.Equals(removeMarkerBoxStr))
                    {
                        mapView.Markers.Remove(tempMarker);
                        //MessageBox.Show("remnove Marker" + tempMarker.Tag.ToString());

                    }
                }
            }

            tbxRemovingMarkerName.Text = "";

            // removes all markers
            //for (int j = btnDropCount - 1; j >= 0; j--)
            //{

            //    MessageBox.Show(j.ToString());
            //    mapView.Markers.RemoveAt(j);
            //}
        } //RemoVing marker with tag

        private void btnDrawCircle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                CreateCircle(
                    Convert.ToDouble(circleLati.Text),
                    Convert.ToDouble(CircleLongi.Text),
                    Convert.ToDouble(circleRadius.Text),
                    100
                    );
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message.ToString()); 
            }
        } // Button CLick Event to Draw cirlce
        
        public static PointLatLng FindPointAtDistanceFrom(PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad + Math.Atan2(
                          Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                          distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }  
        // Funcations to calculate circle path of cirlce with radius, latuiutude and logitude 

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        } //Degree to Radina

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }// Radian to Degree

        private void CreateCircle(Double lat, Double lon, double radius, int segments)
        {
            PointLatLng point = new PointLatLng(lat, lon);

            List<PointLatLng> gpollist = new List<PointLatLng>();
            for (int i = 0; i < segments; i++)
                gpollist.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            List<PointLatLng> gpollistR = new List<PointLatLng>();
            List<PointLatLng> gpollistL = new List<PointLatLng>();
            foreach (var gp in gpollist)
            {
                if (gp.Lng > lon)
                {
                    gpollistR.Add(gp);
                }
                else
                {
                    gpollistL.Add(gp);
                }
            }
            gpollist.Clear();

            List<PointLatLng> gpollistRT = new List<PointLatLng>();
            List<PointLatLng> gpollistRB = new List<PointLatLng>();
            foreach (var gp in gpollistR)
            {
                if (gp.Lat > lat)
                {
                    gpollistRT.Add(gp);
                }
                else
                {
                    gpollistRB.Add(gp);
                }
            }
            gpollistRT.Sort(new LngComparer());
            gpollistRB.Sort(new Lng2Comparer());
            gpollistR.Clear();
            List<PointLatLng> gpollistLT = new List<PointLatLng>();
            List<PointLatLng> gpollistLB = new List<PointLatLng>();
            foreach (var gp in gpollistL)
            {
                if (gp.Lat > lat)
                {
                    gpollistLT.Add(gp);
                }
                else
                {
                    gpollistLB.Add(gp);
                }
            }
            gpollistLT.Sort(new LngComparer());
            gpollistLB.Sort(new Lng2Comparer());
            gpollistLT.Sort(new LngComparer());
            gpollistL.Clear();


            gpollist.AddRange(gpollistRT);
            gpollist.AddRange(gpollistRB);
            gpollist.AddRange(gpollistLB);
            gpollist.AddRange(gpollistLT);

            gpol = new GMapPolygon(gpollist);
            mapView.RegenerateShape(gpol);

            SolidColorBrush redBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(circleColor.Text);
            (gpol.Shape as Path).Stroke = redBrush;
            (gpol.Shape as Path).StrokeThickness = 1.5;
            (gpol.Shape as Path).Effect = null;
            (gpol.Shape as Path).Fill = null;

            if (cbxFillColor.IsChecked == true)
            {
                redBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(circleColor.Text);
                (gpol.Shape as Path).Fill = redBrush;
            }
            else {
                (gpol.Shape as Path).StrokeThickness = 1.5;
            }
            if (rbtnSold.IsChecked==true)
            {
                (gpol.Shape as Path).StrokeDashArray = null;
                redBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(circleColor.Text);
                (gpol.Shape as Path).Stroke = redBrush;
                (gpol.Shape as Path).StrokeThickness = 1.5;
            }

            if (rbtnDashes.IsChecked==true) {
                redBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(circleColor.Text);
                (gpol.Shape as Path).Stroke = redBrush;

                var foo = new double[] { 1 };
                var bar = new DoubleCollection(foo);
                (gpol.Shape as Path).StrokeDashArray = bar;
                (gpol.Shape as Path).StrokeThickness = 1.5;
            }
            
            gpol.Tag = "circle";
            mapView.Markers.Add(gpol);
            mapView.Zoom = 10;
            mapView.ShowCenter = true;
            mapView.Position = gpol.Position;
           

            
        }// Main function to Create circle

        private void RemoveCircle()
        {
            int markers_count = mapView.Markers.Count();

            if (!tbxCricleTag.Text.Equals(""))
            {
                for (int j = markers_count - 1; j >= 0; j--)
                {
                    GMapMarker tempMarker = mapView.Markers[j];
                    if (tempMarker.Tag.Equals("circle"))
                    {
                        mapView.Markers.Remove(tempMarker);
                        //MessageBox.Show("remove Marker" + tempMarker.Tag.ToString());
                    }
                }
            }
            else {
                MessageBox.Show("Enter Circle tag");
            }
           
        }//Remove circle with Some ID
        private void btnDeleteCircle_Click(object sender, RoutedEventArgs e)
        {
            RemoveCircle();

        }// ON CLick lister to Delete Circle as 
        public void addPolygon() {
            List <PointLatLng> points= new List<PointLatLng>();
            points.Add(new PointLatLng(-25.969562, 32.585789));
            points.Add(new PointLatLng(-25.966205, 32.588171));
            points.Add(new PointLatLng(-25.968134, 32.591647));
            points.Add(new PointLatLng(-25.971684, 32.589759));
            points.Add(new PointLatLng(-25.981684, 32.589741));
            points.Add(new PointLatLng(-25.971634, 32.589735));
            GMapPolygon polyOverlay = new GMapPolygon(points);
            mapView.RegenerateShape(polyOverlay);

            (polyOverlay.Shape as Path).Stroke = Brushes.Red;
            (polyOverlay.Shape as Path).StrokeThickness = 1.5;
            (polyOverlay.Shape as Path).Effect = null;
            (polyOverlay.Shape as Path).Fill = null;



            mapView.Markers.Add(polyOverlay);
            
            mapView.Zoom = 10;
            mapView.ShowCenter = true;
            mapView.Position = polyOverlay.Position;
        }

        public void addPolygonPoints(GeoLocation temp)
        {
            PolygonPoints.Add(new PointLatLng(temp.Latitude, temp.Longitude));
        }// it only add marks markers in Polygon
        public void createPolygon()
        {
            try
            {
                GMapPolygon polyOverlay = new GMapPolygon(PolygonPoints);
                mapView.RegenerateShape(polyOverlay);

                (polyOverlay.Shape as Path).Stroke = Brushes.Red;
                (polyOverlay.Shape as Path).StrokeThickness = 1.5;
                (polyOverlay.Shape as Path).Effect = null;
                (polyOverlay.Shape as Path).Fill = Brushes.Red ;
                polyOverlay.Tag = tbxPolygonID.Text;

                mapView.Markers.Add(polyOverlay);

                mapView.Zoom = 10;
                mapView.ShowCenter = true;
                mapView.Position = polyOverlay.Position;
                PolygonPoints.Clear();


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            
        }// Create Polygon 
        public void PolygonWithFixPoints()
        {
            List<PointLatLng> points = new List<PointLatLng>();
            points.Add(new PointLatLng(-25.969562, 32.585789));
            points.Add(new PointLatLng(-25.966205, 32.588171));
            points.Add(new PointLatLng(-25.968134, 32.591647));

            GMapPolygon polyOverlay = new GMapPolygon(points);
            mapView.RegenerateShape(polyOverlay);

            (polyOverlay.Shape as Path).Stroke = Brushes.Red;
            (polyOverlay.Shape as Path).StrokeThickness = 1.5;
            (polyOverlay.Shape as Path).Effect = null;
            (polyOverlay.Shape as Path).Fill = null;


            mapView.Markers.Add(polyOverlay);

            mapView.Zoom = 10;
            mapView.ShowCenter = true;
            mapView.Position = polyOverlay.Position;
        }// Fix data Polygon
        public static GeoLocation FindPointAtDistanceFrom(GeoLocation startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Latitude);
            var startLonRad = DegreesToRadians(startPoint.Longitude);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad
                + Math.Atan2(
                    Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                    distRatioCosine - startLatSin * Math.Sin(endLatRads));

            MessageBox.Show("EndlatRadian " + endLatRads+ "EndlatRadian " + endLonRads);

            return new GeoLocation
            {
                Latitude = RadiansToDegrees(endLatRads),
                Longitude = RadiansToDegrees(endLonRads)
            };
        }// it gives points at hight level

        private void btnAddPts_Click(object sender, RoutedEventArgs e)
        {
            PolygonExist();     
            Double Marker_X = Convert.ToDouble(tbxMarkerX.Text);
            Double Marker_Y = Convert.ToDouble(tbxMarkerY.Text);
            Double Radius = Convert.ToDouble(tbxRadius.Text);
            Double Angel = Convert.ToDouble(tbxAngel.Text);
            Double confidenceLevel = 10;

            PointLatLng pts = new PointLatLng();
            pts.Lat = Marker_X;
            pts.Lng = Marker_Y;
            PolygonPoints.Add(new PointLatLng(Marker_X, Marker_Y));
            GeoLocation geoLoc = new GeoLocation();
            geoLoc.Latitude = Marker_X;
            geoLoc.Latitude = Marker_Y;

            //geoLoc = FindPointAtDistanceFrom(geoLoc, confidenceLevel, Radius / 1000);
            //PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));

            //MessageBox.Show("lat " + geoLoc.Longitude + " lng " + geoLoc.Latitude);

            //geoLoc = FindPointAtDistanceFrom(geoLoc, Angel - confidenceLevel, Radius / 1000);
            //PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));



            geoLoc = LocWithBearingAndDistance1(Marker_X, Marker_Y, Angel + confidenceLevel, Radius);
            PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));
            //MessageBox.Show("lat " + geoLoc.Longitude + " lng " + geoLoc.Latitude);

            geoLoc = LocWithBearingAndDistance1(Marker_X, Marker_Y, Angel - confidenceLevel, Radius);
            PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));

            //MessageBox.Show("lat " + geoLoc.Longitude + " lng " + geoLoc.Latitude);
            createPolygon();

        } // it adds Pollygon

        public GeoLocation LocWithBearingAndDistance1(double lat, double lon, double bearing, double beamLength)
        {
            // 3450
            double angdistance = beamLength / 6371000.0;


            double φ1 = lat * (Math.PI / 180);
            double λ1 = lon * (Math.PI / 180);
            double brng = bearing * (Math.PI / 180);
            double φ2 = Math.Asin(Math.Sin(φ1) * Math.Cos(angdistance) + Math.Cos(φ1) * Math.Sin(angdistance) * Math.Cos(brng));
            double λ2 = λ1 + Math.Atan2(Math.Sin(brng) * Math.Sin(angdistance) * Math.Cos(φ1), Math.Cos(angdistance) - Math.Sin(φ1) * Math.Sin(φ2));
            double Lat3 = (φ2 * (180 / Math.PI));
            double Lng3 = λ2 * (180 / Math.PI);

            //MessageBox.Show("Lat 3 and Lng3  "+ Lat3+ Lng3);
            return new GeoLocation
            {
                Latitude = Lat3,
                Longitude = Lng3,
            };

        }//It accepts polgygon X,Y and gives point according to that

        private void btnDeletePolygon_Click(object sender, RoutedEventArgs e)
        {
            try {
                int markers_count = mapView.Markers.Count();


                for (int j = markers_count - 1; j >= 0; j--)
                {
                    GMapMarker tempMarker = mapView.Markers[j];
                    if (tempMarker.Tag.Equals(tbx_PolygonID.Text))
                    {
                        mapView.Markers.Remove(tempMarker);
                    }
                }
            } catch (Exception ex) {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Fill Text box properly"); 
            }
            
        } //It deletes Polygon with some specfic ID

        private void btnPlot_Click(object sender, RoutedEventArgs e)
        {
            liveLocationCircle();
        }// button to start TCP Client
        private void TcpConnectionStart() {
            IPAddress ipAddre = IPAddress.Parse("127.0.0.1");// our IP adress
            IPEndPoint endPoint = new IPEndPoint(ipAddre, 3001);// our port
            sender = new Socket(ipAddre.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(endPoint);
            // We print EndPoint information  // that we are connected
            Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());


            Thread accept = new Thread(new ThreadStart(tcpAcceptData));
            accept.Start();
        }// take IP and port and start TCP Connection
        public void tcpAcceptData()
        {
            while (true)
            {
                byte[] messageReceived = new byte[1024];

                int byteRecv = sender.Receive(messageReceived);
                Console.WriteLine("Message from Server -> {0}", Encoding.ASCII.GetString(messageReceived, 0, byteRecv));

                
                String msgRec= Encoding.ASCII.GetString(messageReceived);
                String[] authorsList = msgRec.Split(',');
                String ID = authorsList[0];
                String angle= authorsList[1];
                String radius = authorsList[2];

                livePolygon(ID, angle,radius);
            }

        }// Accept data from usinf server
        private void liveLocationCircle() {
            double circleMinLen=700, circleMediumLen=2700, circleMaxLen=5000;
            double AUlat = 33.7138, AUlug = 73.0247;

            BoundryCircle(AUlat, AUlug, circleMinLen, "circle", 100);
            BoundryCircle(AUlat, AUlug, circleMediumLen, "circle", 100);
            BoundryCircle(AUlat, AUlug, circleMaxLen, "circle", 100);

            TcpConnectionStart();
           
        } // Passing Circle 
        private void BoundryCircle(Double lat, Double lon, double radius, string id, int segments)
        {
            this.Dispatcher.Invoke((Action)delegate {
                // your code

                PointLatLng point = new PointLatLng(lat, lon);

                List<PointLatLng> gpollist = new List<PointLatLng>();
                for (int i = 0; i < segments; i++)
                    gpollist.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

                List<PointLatLng> gpollistR = new List<PointLatLng>();
                List<PointLatLng> gpollistL = new List<PointLatLng>();
                foreach (var gp in gpollist)
                {
                    if (gp.Lng > lon)
                    {
                        gpollistR.Add(gp);
                    }
                    else
                    {
                        gpollistL.Add(gp);
                    }
                }
                gpollist.Clear();

                List<PointLatLng> gpollistRT = new List<PointLatLng>();
                List<PointLatLng> gpollistRB = new List<PointLatLng>();
                foreach (var gp in gpollistR)
                {
                    if (gp.Lat > lat)
                    {
                        gpollistRT.Add(gp);
                    }
                    else
                    {
                        gpollistRB.Add(gp);
                    }
                }
                gpollistRT.Sort(new LngComparer());
                gpollistRB.Sort(new Lng2Comparer());
                gpollistR.Clear();
                List<PointLatLng> gpollistLT = new List<PointLatLng>();
                List<PointLatLng> gpollistLB = new List<PointLatLng>();
                foreach (var gp in gpollistL)
                {
                    if (gp.Lat > lat)
                    {
                        gpollistLT.Add(gp);
                    }
                    else
                    {
                        gpollistLB.Add(gp);
                    }
                }
                gpollistLT.Sort(new LngComparer());
                gpollistLB.Sort(new Lng2Comparer());
                gpollistLT.Sort(new LngComparer());
                gpollistL.Clear();


                gpollist.AddRange(gpollistRT);
                gpollist.AddRange(gpollistRB);
                gpollist.AddRange(gpollistLB);
                gpollist.AddRange(gpollistLT);

                gpol = new GMapPolygon(gpollist);
                mapView.RegenerateShape(gpol);

                (gpol.Shape as Path).Stroke = Brushes.DeepPink;
                (gpol.Shape as Path).StrokeThickness = 1.5;
                (gpol.Shape as Path).Effect = null;
                (gpol.Shape as Path).Fill = null;

                gpol.Tag = id;
                mapView.Markers.Add(gpol);
                mapView.Zoom = 12;
                mapView.ShowCenter = true;
                mapView.Position = gpol.Position;

            });




        }// Main function to Create circle boundry
        private void livePolygon(String ID, String angle, String radius) {

            LivePolygonExist(ID);
            double AUlat = 33.7138, AUlug = 73.0247;
            Double Marker_X = AUlat;
            Double Marker_Y = AUlug;
            Double Radius = Convert.ToDouble(radius);
            Double Angel = Convert.ToDouble(angle);
            if (Radius > 5000)
            {
                Radius = 5000;
            }
            if (Radius < 700) {
                Radius = 700;
            }
            Double confidenceLevel = 10;

            PointLatLng pts = new PointLatLng();
            pts.Lat = Marker_X;
            pts.Lng = Marker_Y;
            PolygonPoints.Add(new PointLatLng(Marker_X, Marker_Y));
            GeoLocation geoLoc = new GeoLocation();
            geoLoc.Latitude = Marker_X;
            geoLoc.Latitude = Marker_Y;

            //geoLoc = FindPointAtDistanceFrom(geoLoc, confidenceLevel, Radius / 1000);
            //PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));

            //MessageBox.Show("lat " + geoLoc.Longitude + " lng " + geoLoc.Latitude);

            //geoLoc = FindPointAtDistanceFrom(geoLoc, Angel - confidenceLevel, Radius / 1000);
            //PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));



            geoLoc = LocWithBearingAndDistance1(Marker_X, Marker_Y, Angel + confidenceLevel, Radius);
            PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));
            //MessageBox.Show("lat " + geoLoc.Longitude + " lng " + geoLoc.Latitude);

            geoLoc = LocWithBearingAndDistance1(Marker_X, Marker_Y, Angel - confidenceLevel, Radius);
            PolygonPoints.Add(new PointLatLng(geoLoc.Latitude, geoLoc.Longitude));

            //MessageBox.Show("lat " + geoLoc.Longitude + " lng " + geoLoc.Latitude);
            createLivePolygon(ID);
        } // create Points of Polygon w.r.t tcp Accept ID,Angel, Radius (In sequence)
        public void createLivePolygon(String ID)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                try
                {
                    Random r = new Random();
                    GMapPolygon polyOverlay = new GMapPolygon(PolygonPoints);
                    mapView.RegenerateShape(polyOverlay);
                    //Brush brush = new SolidColorBrush(Color.FromRgb((byte)r.Next(1, 255), (byte)r.Next(1, 255), (byte)r.Next(1, 233)));

                    (polyOverlay.Shape as Path).Stroke = Brushes.Red;
                    (polyOverlay.Shape as Path).StrokeThickness = 1.5;
                    (polyOverlay.Shape as Path).Effect = null;
                    (polyOverlay.Shape as Path).Fill = Brushes.Red;
                    polyOverlay.Tag = ID;

                    mapView.Markers.Add(polyOverlay);

                    //mapView.Zoom = 10;
                    //if (firstTimeMoveMap)
                    //{
                    //    mapView.Position = polyOverlay.Position;
                    //    firstTimeMoveMap = false;
                    //}
                    PolygonPoints.Clear();

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            });
           

        }// Collect all markers and give colors and plot
        private void LivePolygonExist(String ID)
        {
            Application.Current.Dispatcher.Invoke((Action)delegate {
                try
                {
                    int markers_count = mapView.Markers.Count();


                    for (int j = markers_count - 1; j >= 0; j--)
                    {
                        GMapMarker tempMarker = mapView.Markers[j];
                        if (tempMarker.Tag.Equals(ID))
                        {
                            mapView.Markers.Remove(tempMarker);
                        }
                    }

                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    MessageBox.Show("Fill Text box properly");
                }
            });
           
        }// check for Polygon with same ID 
    }
}

