namespace ChilledWindows
{
    using ChilledWindows.Properties;
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    public class MainWindow : Window, IComponentConnector
    {
        private TransformGroup fTransformGroup = new TransformGroup();
        private TransformGroup gTransGroup = new TransformGroup();
        private TranslateTransform gTransTransform = new TranslateTransform();
        private ScaleTransform gScaleTransform = new ScaleTransform();
        private ScaleTransform fFlipTrans = new ScaleTransform();
        private ScaleTransform FlipTrans1 = new ScaleTransform();
        private ScaleTransform FlipTrans2 = new ScaleTransform();
        private RotateTransform fRotateTrans = new RotateTransform();
        private int screenWidth;
        private int screenHeight;
        private DispatcherTimer dt = new DispatcherTimer();
        private int[] flipTimes = new int[] { 
            0x7e, 0x81, 0x85, 0x88, 140, 0x8f, 0x93, 150, 0x9b, 0x9e, 0xa2, 0xa5, 0xa9, 0xac, 0xb0, 0xb3,
            0xb8, 0xbb, 0xbf, 0xc2, 0xc6, 0xc9, 0xcd, 0xd0, 0xd5, 0xd8, 220, 0xdf, 0xe3, 230, 0xea, 0xed,
            0xf2, 0xf5, 0xf9, 0xfc, 0x100, 0x103, 0x107, 0x10a, 0x10f, 0x112, 0x116, 0x119, 0x11d, 0x11e, 0x120, 0x124,
            0x127, 0x129, 300, 0x12f, 0x133, 310, 0x13a, 0x13d, 0x141, 0x144, 0x149, 0x14c, 0x150, 0x153, 0x157, 0x15a,
            350, 0x161, 0x163, 0x166, 0x169, 0x16d, 0x170, 0x174, 0x177, 0x17b, 0x17e, 0x183, 390, 0x18a, 0x18d, 0x191,
            0x194, 0x198, 0x19b, 0x1a0, 0x1a3, 0x1a7, 0x1aa, 430, 0x1b1, 0x1b5, 0, 0, 0, 0, 0, 0
        };
        private int[] flipTimes2 = new int[] { 
            440, 0x1bb, 0x1c1, 0x1c6, 0x1c8, 460, 0x1d4, 0x1dc, 0x1e1, 0x1e6, 0x1f1, 0x1f5, 0x1f8, 0x1fa, 0x200, 0x202,
            0x206, 0x20a, 0x20f, 0x213, 0x215, 0x218, 540, 0x224, 0x228, 0x22d, 0x231, 0x233, 0x239, 0x23c, 0x240, 580,
            0x246, 0x249, 0, 0, 0, 0, 0, 0
        };
        private int[] flipTimes1 = new int[] { 0x1c6, 0x1dc, 0x1f1, 0x200, 0x213, 0x224, 0x239, 0x249, 0, 0, 0, 0, 0, 0 };
        private int flipIndex;
        private int flipIndex1;
        private int flipIndex2;
        private int frameIndex;
        private bool refreshFirstFlips = true;
        private bool refreshSecondFlips = true;
        internal MediaElement mediaElement;
        internal System.Windows.Shapes.Rectangle bg;
        internal System.Windows.Controls.Image firstBg;
        internal Label label;
        internal Grid twoGrid;
        internal System.Windows.Controls.Image bg2;
        internal System.Windows.Controls.Image bg3;
        private bool _contentLoaded;

        public MainWindow()
        {
            Type typeFromProgID = Type.GetTypeFromProgID("Shell.Application");
            object target = Activator.CreateInstance(typeFromProgID);
            typeFromProgID.InvokeMember("MinimizeAll", BindingFlags.InvokeMethod, null, target, null);
            Thread.Sleep(300);
            this.screenWidth = (int) SystemParameters.PrimaryScreenWidth;
            this.screenHeight = (int) SystemParameters.PrimaryScreenHeight;
            Bitmap image = new Bitmap(this.screenWidth, this.screenHeight);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.CopyFromScreen(0, 0, 0, 0, image.Size);
            }
            this.InitializeComponent();
            base.WindowState = WindowState.Normal;
            base.WindowStyle = WindowStyle.None;
            base.Topmost = true;
            base.WindowState = WindowState.Maximized;
            ImageSource source = this.BitmapToImageSource(image);
            this.firstBg.Source = source;
            this.bg2.Source = source;
            this.bg3.Source = source;
            this.firstBg.RenderTransformOrigin = new Point(0.5, 0.5);
            this.fTransformGroup.Children.Add(this.fFlipTrans);
            this.fTransformGroup.Children.Add(this.fRotateTrans);
            this.firstBg.RenderTransform = this.fTransformGroup;
            this.bg2.RenderTransformOrigin = new Point(0.5, 0.5);
            this.bg2.RenderTransform = this.FlipTrans1;
            this.bg3.RenderTransformOrigin = new Point(0.5, 0.5);
            this.bg3.RenderTransform = this.FlipTrans2;
            this.twoGrid.RenderTransformOrigin = new Point(0.0, 0.0);
            this.gTransGroup.Children.Add(this.gTransTransform);
            this.gTransGroup.Children.Add(this.gScaleTransform);
            this.twoGrid.RenderTransform = this.gTransGroup;
            File.WriteAllBytes("chilledwindows.mp4", Resources.Chilled_Windows);
            this.mediaElement.Source = new Uri("chilledwindows.mp4", UriKind.Relative);
            this.dt.add_Tick(new EventHandler(this.Dt_Tick));
            this.dt.set_Interval(new TimeSpan(0, 0, 0, 0, 10));
            this.dt.Start();
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Bmp);
                stream.Position = 0L;
                BitmapImage image1 = new BitmapImage();
                image1.BeginInit();
                image1.StreamSource = stream;
                image1.CacheOption = BitmapCacheOption.OnLoad;
                image1.EndInit();
                return image1;
            }
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            this.frameIndex = (int) Math.Floor((double) (this.mediaElement.Position.TotalMilliseconds / 33.33333));
            this.label.Content = "Frame:" + this.frameIndex;
            if (this.frameIndex == 0x1b6)
            {
                this.refreshFirstFlips = false;
                this.firstBg.Visibility = Visibility.Hidden;
                this.twoGrid.Visibility = Visibility.Visible;
            }
            if (this.frameIndex == 0x249)
            {
                this.refreshSecondFlips = false;
            }
            if (this.frameIndex == 0x26e)
            {
                this.bg.Visibility = Visibility.Hidden;
                DoubleAnimation animation = new DoubleAnimation(0.0, (this.screenWidth * 0.13817330210772832) * 3.0, TimeSpan.FromMilliseconds(500.0));
                DoubleAnimation animation2 = new DoubleAnimation(0.0, (this.screenHeight * 0.35416666666666669) * 3.0, TimeSpan.FromMilliseconds(500.0));
                DoubleAnimation animation3 = new DoubleAnimation(1.0, 0.3, TimeSpan.FromMilliseconds(500.0));
                DoubleAnimation animation4 = new DoubleAnimation(1.0, 0.3, TimeSpan.FromMilliseconds(500.0));
                this.gTransTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                this.gTransTransform.BeginAnimation(TranslateTransform.YProperty, animation2);
                this.gScaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animation3);
                this.gScaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animation4);
            }
            if (this.frameIndex == 0x299)
            {
                this.twoGrid.Visibility = Visibility.Hidden;
            }
            if (this.frameIndex == 0x4ec)
            {
                File.Delete("chilledwindows.mp4");
                Application.Current.Shutdown();
            }
            if (this.refreshFirstFlips)
            {
                if (this.flipTimes[this.flipIndex] <= this.frameIndex)
                {
                    this.flipIndex++;
                    this.fFlipTrans.ScaleX = (this.fFlipTrans.ScaleX == -1.0) ? ((double) 1) : ((double) (-1));
                }
                if (this.frameIndex == 0x11e)
                {
                    this.fRotateTrans.Angle = -20.0;
                }
            }
            else if (this.refreshSecondFlips)
            {
                if (this.flipTimes1[this.flipIndex1] <= this.frameIndex)
                {
                    this.flipIndex1++;
                    this.FlipTrans1.ScaleX = (this.FlipTrans1.ScaleX == -1.0) ? ((double) 1) : ((double) (-1));
                }
                if (this.flipTimes2[this.flipIndex2] <= this.frameIndex)
                {
                    this.flipIndex2++;
                    this.FlipTrans2.ScaleX = (this.FlipTrans2.ScaleX == -1.0) ? ((double) 1) : ((double) (-1));
                }
            }
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (!this._contentLoaded)
            {
                this._contentLoaded = true;
                Uri resourceLocator = new Uri("/ChilledWindows;component/mainwindow.xaml", UriKind.Relative);
                Application.LoadComponent(this, resourceLocator);
            }
        }

        [DebuggerNonUserCode, GeneratedCode("PresentationBuildTasks", "4.0.0.0"), EditorBrowsable(EditorBrowsableState.Never)]
        void IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    ((MainWindow) target).KeyDown += new KeyEventHandler(this.Window_KeyDown);
                    return;

                case 2:
                    this.mediaElement = (MediaElement) target;
                    return;

                case 3:
                    this.bg = (System.Windows.Shapes.Rectangle) target;
                    return;

                case 4:
                    this.firstBg = (System.Windows.Controls.Image) target;
                    return;

                case 5:
                    this.label = (Label) target;
                    return;

                case 6:
                    this.twoGrid = (Grid) target;
                    return;

                case 7:
                    this.bg2 = (System.Windows.Controls.Image) target;
                    return;

                case 8:
                    this.bg3 = (System.Windows.Controls.Image) target;
                    return;
            }
            this._contentLoaded = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == 0x12)
            {
                this.fFlipTrans.ScaleX = (this.fFlipTrans.ScaleX == -1.0) ? ((double) 1) : ((double) (-1));
            }
        }
    }
}

