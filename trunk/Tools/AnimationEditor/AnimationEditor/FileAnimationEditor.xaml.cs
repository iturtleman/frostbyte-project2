using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Win32;
using System.IO;
using System.Collections.ObjectModel;

namespace AnimationEditor
{
    /// <summary>
    /// Interaction logic for FileAnimationEditor.xaml
    /// </summary>
    public partial class FileAnimationEditor : UserControl
    {
        #region Variables
        XDocument doc;
        PngBitmapEncoder png;
        double DpiX;
        double DpiY;
        int pow;
        #endregion Variables

        public FileAnimationEditor()
        {
            this.InitializeComponent();

            SaveSpriteSheetButton.MouseUp += new MouseButtonEventHandler(SaveSpriteSheetButton_MouseUp);
            ShowSpriteSheetButton.MouseUp += new MouseButtonEventHandler(ShowSpriteSheetButton_MouseUp);
            CancelButton.MouseUp += new MouseButtonEventHandler(CancelButton_MouseUp);
            EditFrame.MouseUp += new MouseButtonEventHandler(EditFrame_MouseUp);
        }



        #region EventHandlers
        void EditFrame_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SpriteFrames.SelectedIndex > -1)
                {
                    Frame f = SpriteFrames.SelectedValue as Frame;
                    if (f != null)
                    {
                        Workspace.Children.Add(new EditFrame(f) { HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Edit frame Exception:{0}",ex.GetBaseException().Message));
            }
        }

        void ShowSpriteSheetButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //try
            {
                if (SpriteFrames.Items != null)
                {
                    //if (DataContext == null)
                    //{
                    //    var Files = storage.mainWindow.Files;
                    //    ItemCollection items = SpriteFrames.Items;
                    //    if (Files.HasItems && Files.Items.Count > 0)
                    //    {
                    //        (Files.SelectedItem as TabItem).DataContext = new AnimFile(items);
                    //    }
                    //    else
                    //    {
                    //        Files.Items.Clear();
                    //        FileAnimationEditor fae = new FileAnimationEditor();
                    //        fae.DataContext = new AnimFile(items);
                    //        Files.ItemsSource = new List<FileAnimationEditor>() { fae };
                    //        Files.SelectedIndex = 0;
                    //    }
                    //}
                    ObservableCollection<Frame> frames = (DataContext as AnimFile).Frames;

                    if (frames != null && frames.Count>0)
                    {
                        //find maxheight and width
                        int maxHeight = 0;
                        int maxWidth = 0;
                        foreach (Frame frame in frames)
                        {
                            if (maxHeight < frame.Height)
                                maxHeight = frame.Height;
                            if (maxWidth < frame.Width)
                                maxWidth = frame.Width;
                        }

                        int horizTileCount = (int)Math.Ceiling(Math.Sqrt(frames.Count));

                        int max = maxHeight < maxWidth ? maxWidth : maxHeight;

                        //calculate the best box for the maxsize tiles (square powers of 2)
                        pow = 2;
                        while (pow < max * horizTileCount)
                        {
                            pow *= 2;
                        }

                        //make sure it tiles across before going down
                        horizTileCount = pow / maxWidth;

                        //create Xdoc to store data about frames
                        doc = new XDocument();
                        doc.Add(new XElement("SpriteSheet"));

                        //create an object to draw the images onto
                        //Sheet.MaxHeight = pow;
                        //Sheet.MaxWidth = pow;
                        Sheet.Children.Clear();
                        Sheet.ColumnDefinitions.Clear();
                        Sheet.RowDefinitions.Clear();
                        for (int i = 0; i < horizTileCount; i++)
                        {
                            Sheet.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            Sheet.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                        }

                        //add images to the grid
                        for (int i = 0, count = frames.Count; i < count; i++)
                        {
                            Frame f = (Frame)frames[i];
                            Image img = new Image() { Source = f.Image };
                            int row = i / horizTileCount;
                            int col = i % horizTileCount;
                            Point TLpos = new Point(col * f.Width, row * f.Height);
                            f.StartPos = TLpos;
                            Grid.SetRow(img, row);
                            Grid.SetColumn(img, col);
                            Sheet.Children.Add(img);
                            XElement elem = new XElement("Frame");
                            elem.SetAttributeValue("Height", f.Height);
                            elem.SetAttributeValue("Width", f.Width);
                            elem.SetAttributeValue("TLPos", TLpos);
                            doc.Root.Add(elem);
                        }
                        DpiX = ((Frame)frames[0]).Image.DpiX;
                        DpiY = ((Frame)frames[0]).Image.DpiY;
                        ToggleSpriteSheetVisible();
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(string.Format("Show Sprite Sheet Exception:{0}",ex.GetBaseException().Message));
            //}
        }

        void SaveSpriteSheetButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //try
            {
                UIElement element = Sheet as UIElement;

                //get sheet's render size
                Size size = element.RenderSize;

                //render grid into a bitmap
                RenderTargetBitmap rtb = new RenderTargetBitmap(pow, pow, DpiX, DpiY, PixelFormats.Pbgra32);
                element.Measure(size);
                element.Arrange(new Rect(size));
                rtb.Render(element);
                png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(rtb));

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Png Files(*.png)|*.png";
                if (sfd.ShowDialog() == true)
                {
                    using (Stream s = File.Create(sfd.FileName))
                    {
                        png.Save(s);
                    }

                    doc.Save(string.Format("{0}.spsh", sfd.FileName));

                    //pass around new filename
                    AnimFile anim = (DataContext as AnimFile);
                    anim.Filename = sfd.SafeFileName;
                    foreach (var f in anim.Frames)
                    {
                        f.File = sfd.SafeFileName;
                    }
                }
                ToggleSpriteSheetVisible();
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(string.Format("Save Spritesheet Exception:{0}",ex.GetBaseException().Message));
            //}
        }

        void CancelButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ToggleSpriteSheetVisible();
        }
        #endregion EventHandlers

        #region Methods
        private void ToggleSpriteSheetVisible()
        {
            ShowSpriteSheetButton.Visibility = ShowSpriteSheetButton.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion Methods
    }

    public class VisibilityInverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}