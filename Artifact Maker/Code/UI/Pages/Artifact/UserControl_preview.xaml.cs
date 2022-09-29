using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;

/*
 this page used Pfim
 https://github.com/nickbabcock/Pfim

 Pfim License: MIT License

                             MIT License

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */


namespace ArtifactMaker
{
    /// <summary>
    /// Page_preview.xaml 的交互逻辑
    /// </summary>
    /// 

    public partial class usercontrolPreview : UserControl
    {
        Dictionary<string, List<BitmapSource>> imageCache = new Dictionary<string, List<BitmapSource>>();
        int iconWidth = int.Parse(Config.get("artifact_icon_width"));


        public usercontrolPreview()
        {
            InitializeComponent();
        }

        public void applyLocalisation()
        {
        }

        public void refreshIcon(string visual, int rarity, bool isUnique)
        {
            stackpanelIcon.Children.Clear();

            //bad info if game path is empty
            if (String.IsNullOrEmpty(MainWindow.gamePath))
            {
                //think think
                /*
                var labelError = new Label();
                labelError.Content = error_error + "\r\n" + error_not_find_game_path;
                stackpanelIcon.Children.Add(labelError);
                */
                return;
            }

            List<string> iconName;
            if (String.IsNullOrEmpty(visual))
            {
                iconName = new List<string>();
            }
            else
            {
                iconName = Data.get("icon_" + visual);
            }

            int imageIndex = 0;
            do
            {
                Grid gridImage = new Grid();
                gridImage.Width = stackpanelIcon.ActualWidth;
                gridImage.Height = stackpanelIcon.ActualWidth;

                var tempHandler = new MouseButtonEventHandler(gridDoubleClickExecute);
                gridImage.AddHandler(Grid.PreviewMouseDownEvent, tempHandler);

                //background
                if (rarity >= 0)
                {
                    var background = new System.Windows.Controls.Image();
                    background.Source = getImage(@"artifact_bg.dds", rarity);

                    var tempGrid = new Grid();
                    tempGrid.Width = stackpanelIcon.ActualWidth * 1.1;
                    tempGrid.Height = stackpanelIcon.ActualWidth * 1.1;
                    tempGrid.VerticalAlignment = VerticalAlignment.Center;
                    tempGrid.HorizontalAlignment = HorizontalAlignment.Center;
                    tempGrid.Children.Add(background);

                    gridImage.Children.Add(tempGrid);
                }

                //unique
                if (isUnique)
                {
                    var unique = new System.Windows.Controls.Image();
                    unique.Source = getImage(@"artifact_unique.dds", 0);

                    var tempGrid = new Grid();
                    tempGrid.Width = stackpanelIcon.ActualWidth * 1.1;
                    tempGrid.Height = stackpanelIcon.ActualWidth * 1.1;
                    tempGrid.VerticalAlignment = VerticalAlignment.Center;
                    tempGrid.HorizontalAlignment = HorizontalAlignment.Center;
                    tempGrid.Children.Add(unique);

                    gridImage.Children.Add(tempGrid);
                }

                //image
                if (iconName.Count > imageIndex)
                {
                    var image = new System.Windows.Controls.Image();
                    image.Source = getImage(iconName[imageIndex], rarity);

                    var tempGrid = new Grid();
                    tempGrid.Width = stackpanelIcon.ActualWidth;
                    tempGrid.Height = stackpanelIcon.ActualWidth;
                    tempGrid.VerticalAlignment = VerticalAlignment.Center;
                    tempGrid.HorizontalAlignment = HorizontalAlignment.Center;
                    tempGrid.Children.Add(image);

                    gridImage.Children.Add(tempGrid);
                }

                stackpanelIcon.Children.Add(gridImage);
                imageIndex++;
            } while (imageIndex < iconName.Count);
        }

        private BitmapSource? getImage(string item, int rarity)
        {
            List<BitmapSource>? target;

            //read file if not cache
            if (!imageCache.TryGetValue(item, out target))
            {
                target = new List<BitmapSource>();

                FileStream fileStream;
                try
                {
                    fileStream = new FileStream(MainWindow.gamePath + @"\game\gfx\interface\icons\artifact\" + item, FileMode.Open, FileAccess.Read);
                }
                catch
                {
                    return null;
                }


                using (var iconDds = Pfim.Pfim.FromStream(fileStream))
                {
                    PixelFormat format;

                    //ensure format
                    switch (iconDds.Format)
                    {
                        case Pfim.ImageFormat.Rgba32:
                            format = PixelFormat.Format32bppArgb;
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    //dds to Bitmap to BitmapSource
                    var handle = GCHandle.Alloc(iconDds.Data, GCHandleType.Pinned);
                    try
                    {
                        //to Bitmap
                        var data = Marshal.UnsafeAddrOfPinnedArrayElement(iconDds.Data, 0);
                        Bitmap bitmapComplete = new Bitmap(iconDds.Width, iconDds.Height, iconDds.Stride, format, data);

                        int imageNum = (bitmapComplete.Width) / iconWidth;
                        for (int i = 0; i < imageNum; i++)
                        {
                            //clip
                            Rectangle clipRectangle = new Rectangle(iconWidth * i, 0, iconWidth, bitmapComplete.Height);
                            Bitmap clipBitmap = bitmapComplete.Clone(clipRectangle, bitmapComplete.PixelFormat);

                            //to bitmapSource
                            [DllImport("gdi32")] static extern int DeleteObject(IntPtr o);
                            IntPtr tempBitmapPtr = clipBitmap.GetHbitmap();
                            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(tempBitmapPtr, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            DeleteObject(tempBitmapPtr);

                            //save
                            target.Add(bitmapSource);
                        }

                        //if imageCache is too big, clear it
                        if (imageCache.Count > 30)
                        {
                            imageCache.Clear();
                        }
                        imageCache.Add(item, target);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }

                fileStream.Close();
            }

            if (rarity >= target.Count || rarity < 0)
            {
                rarity = 0;
            }
            return target[rarity];
        }

        private void gridDoubleClickExecute(object sender, MouseButtonEventArgs args)
        {
            List<Grid> image;

            if (args.ClickCount == 2)
            {
                image = new List<Grid>();
                findImage((Grid)sender);

                Popup popup = new Popup();
                //popup.PopupAnimation = PopupAnimation.Fade;
                popup.AllowsTransparency = true;
                popup.PlacementTarget = (UIElement)sender;
                popup.Placement = PlacementMode.Center;
                popup.StaysOpen = false;

                Grid bigImage = new Grid();
                bigImage.Height = iconWidth;
                bigImage.Width = iconWidth;
                foreach (var item in image)
                {
                    bigImage.Children.Add(item);
                }

                popup.Child = bigImage;
                popup.IsOpen = true;
            }

            return;

            //search all image
            void findImage(Grid sender)
            {
                foreach (var item in sender.Children)
                {
                    string itemType = item.GetType().Name;
                    if (itemType == "Grid")
                    {
                        findImage((Grid)item);
                        continue;
                    }

                    if (itemType == "Image")
                    {
                        var tempGrid = new Grid();
                        tempGrid.Width = iconWidth * (sender.Width / stackpanelIcon.ActualWidth);
                        tempGrid.Height = tempGrid.Width;
                        tempGrid.VerticalAlignment = VerticalAlignment.Center;
                        tempGrid.HorizontalAlignment = HorizontalAlignment.Center;

                        var tempImage = new System.Windows.Controls.Image();
                        tempImage.Source = ((System.Windows.Controls.Image)item).Source;
                        tempGrid.Children.Add(tempImage);

                        image.Add(tempGrid);
                    }
                }

                return;
            }
        }
    }
}
