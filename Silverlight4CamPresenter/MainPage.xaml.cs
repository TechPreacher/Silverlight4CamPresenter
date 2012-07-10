using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;


namespace Silverlight4CamPresenter
{
    public partial class MainPage : UserControl
    {
        CaptureSource capSource;
        VideoBrush vidBrush;
        bool isFlipped = false;

        public MainPage()
        {
            InitializeComponent();
            Application.Current.Host.Content.FullScreenChanged += new EventHandler(Content_FullScreenChanged);
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            listboxVideo.ItemsSource = CaptureDeviceConfiguration.GetAvailableVideoCaptureDevices();
            if (listboxVideo.Items.Count > 0)
            {
                listboxVideo.SelectedIndex = 0;
            }

            capSource = new CaptureSource();
            vidBrush = new VideoBrush();
        }

        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                rectVideo.Margin = new Thickness(0, 0, 0, 0);
                stackPanelCam.Visibility = Visibility.Collapsed;
            }
            else
            {
                rectVideo.Margin = new Thickness(0, 0, 141, 0);
                stackPanelCam.Visibility = Visibility.Visible;
            }
        }

        private void buttonStart_Click(object sender, RoutedEventArgs e)
        {
            if (listboxVideo.SelectedIndex >= 0)
            {

                capSource.Stop();
                capSource.VideoCaptureDevice = (VideoCaptureDevice)listboxVideo.SelectedItem;
                capSource.VideoCaptureDevice.DesiredFormat = new VideoFormat(PixelFormatType.Format32bppArgb, 1280, 720, 25);
                
                vidBrush.SetSource(capSource);
                
                //vidBrush.Stretch = Stretch.Uniform; 
                SetStretch();

                rectVideo.Fill = vidBrush;
                
                if (CaptureDeviceConfiguration.AllowedDeviceAccess || CaptureDeviceConfiguration.RequestDeviceAccess())
                    capSource.Start();
            }
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            if (capSource != null)
            {
                capSource.Stop();
                //capSource = null;
                //vidBrush = null;

                WriteableBitmap wb = new WriteableBitmap(rectVideo, null);
                ImageBrush i = new ImageBrush();
                i.ImageSource = wb;
                rectVideo.Fill = i;
            }
        }

        private void buttonFullscreen_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = Application.Current.Host.Content.IsFullScreen ? false : true;
        }


        private void radioButton_Click(object sender, RoutedEventArgs e)
        {
            SetStretch();
        }

        private void SetStretch()
        {
            if (radioButtonUniform.IsChecked == true)
                vidBrush.Stretch = Stretch.Uniform;

            if (radioButtonUniformToFill.IsChecked == true)
                vidBrush.Stretch = Stretch.UniformToFill;

            if (radioButtonFill.IsChecked == true)
                vidBrush.Stretch = Stretch.Fill;

            if (radioButtonNone.IsChecked == true)
                vidBrush.Stretch = Stretch.None;
        }


        private void buttonFlip_Click(object sender, RoutedEventArgs e)
        {
            if (isFlipped)
            {
                rectVideo_Flip2.Begin();
                isFlipped = false;
            }
            else
            {
                rectVideo_Flip.Begin();
                isFlipped = true;
            }
        }


    }
}
