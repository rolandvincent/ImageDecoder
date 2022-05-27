using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageDecoder
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateCanvas();
        }

        Bitmap CurrentImage;
        string path;
        double Scale = 1;

        void UpdateCanvas()
        {
            if (CurrentImage != null)
            {
                Bitmap Result = (Bitmap)CurrentImage.Clone();
                Graphics g = Graphics.FromImage(Result);
                g.DrawImage(GetTransparentPattern(new Size((int)(10 * (1 / Scale)), (int)(10 * (1 / Scale))), CurrentImage.Size),
                    new Rectangle(0, 0, CurrentImage.Width, CurrentImage.Height));
                g.DrawImage(CurrentImage, new Rectangle(0, 0, CurrentImage.Width, CurrentImage.Height));
                pictureBox1.Image = Result;
            }
        }

        public Bitmap GetTransparentPattern(Size patternsize, Size bitmapsize)
        {
            Bitmap b = new Bitmap(bitmapsize.Width, bitmapsize.Height);
            Graphics g = Graphics.FromImage(b);
            for (int y = 0; y < Math.Ceiling((decimal)bitmapsize.Height / patternsize.Height); y++)
                for (int x = 0; x < Math.Ceiling((decimal)bitmapsize.Width / patternsize.Width); x++)
                        g.FillRectangle(new SolidBrush((x + y) % 2 == 0? Color.LightGray:Color.White), x * patternsize.Width, y * patternsize.Height, patternsize.Width, patternsize.Width);
            return b;
        }

        public void ResizeImage(ref Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            image = destImage;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog oplg = new OpenFileDialog();
            if (oplg.ShowDialog() == DialogResult.OK)
            {
                if (new System.IO.FileInfo(oplg.FileName).Extension.ToLower() == ".txtjpg")
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(oplg.FileName, Encoding.Unicode);
                    string strsize = file.ReadLine();
                    string colorhexs = file.ReadToEnd();
                    Match match = Regex.Match(strsize, @"(?<width>\d+)x(?<height>\d+)");
                    MatchCollection matches = Regex.Matches(colorhexs, @"\d{3}");
                    if (match.Success)
                    {
                        try
                        {
                            Size imgsize = new Size(int.Parse(match.Groups["width"].Value), int.Parse(match.Groups["height"].Value));
                            Bitmap openBitmap = new Bitmap(imgsize.Width, imgsize.Height);
                            for (int h = 0; h < imgsize.Height; h++)
                            {
                                for (int w = 0; w < imgsize.Width; w++)
                                {
                                    int R = int.Parse(matches[h * imgsize.Width * 3 + w * 3].Value);
                                    int G = int.Parse(matches[h * imgsize.Width * 3 + w * 3 + 1].Value);
                                    int B = int.Parse(matches[h * imgsize.Width * 3 + w * 3 + 2].Value);

                                    openBitmap.SetPixel(w, h, Color.FromArgb(R, G, B));
                                }
                            }
                            CurrentImage = (Bitmap)openBitmap;
                            UpdateCanvas();
                        }
                        catch
                        {
                            MessageBox.Show("Error while parse the file", "Error Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid format", "Error Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    file.Close();
                }
                else if (new System.IO.FileInfo(oplg.FileName).Extension.ToLower() == ".txtpng")
                {
                    System.IO.StreamReader file = new System.IO.StreamReader(oplg.FileName, Encoding.Unicode);
                    string strsize = file.ReadLine();
                    string colorhexs = file.ReadToEnd();
                    Match match = Regex.Match(strsize, @"(?<width>\d+)x(?<height>\d+)");
                    MatchCollection matches = Regex.Matches(colorhexs, @"\d{3}");
                    if (match.Success)
                    {
                        try
                        {
                            Size imgsize = new Size(int.Parse(match.Groups["width"].Value), int.Parse(match.Groups["height"].Value));
                            Bitmap openBitmap = new Bitmap(imgsize.Width, imgsize.Height);
                            for (int h = 0; h < imgsize.Height; h++)
                            {
                                for (int w = 0; w < imgsize.Width; w++)
                                {
                                    int R = int.Parse(matches[h * imgsize.Width * 4 + w * 4].Value);
                                    int G = int.Parse(matches[h * imgsize.Width * 4 + w * 4 + 1].Value);
                                    int B = int.Parse(matches[h * imgsize.Width * 4 + w * 4 + 2].Value);
                                    int A = int.Parse(matches[h * imgsize.Width * 4 + w * 4 + 3].Value);

                                    openBitmap.SetPixel(w, h, Color.FromArgb(A, R, G, B));
                                }
                            }
                            CurrentImage = (Bitmap)openBitmap;
                            UpdateCanvas();
                        }
                        catch
                        {
                            MessageBox.Show("Error while parse the file", "Error Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid format", "Error Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    file.Close();
                }
                else if (new System.IO.FileInfo(oplg.FileName).Extension.ToLower() == ".rjpg")
                {
                    System.IO.FileStream file = new System.IO.FileStream(oplg.FileName, System.IO.FileMode.Open);
                    byte[] format = System.Text.Encoding.Unicode.GetBytes("JPG");
                    file.Seek(format.Length, System.IO.SeekOrigin.Begin);
                    byte[] width = new byte[sizeof(int)];
                    byte[] height = new byte[sizeof(int)];
                    file.Read(width, 0, sizeof(int));
                    file.Read(height, 0, sizeof(int));
                    try
                    {
                        Size imgsize = new Size(BitConverter.ToInt32(width, 0), BitConverter.ToInt32(height, 0));
                        Bitmap openBitmap = new Bitmap(imgsize.Width, imgsize.Height);
                        for (int y = 0; y < imgsize.Height; y++)
                        {
                            for (int x = 0; x < imgsize.Width; x++)
                            {
                                byte[] colorBytes = new byte[3];
                                file.Read(colorBytes, 0, 3);

                                openBitmap.SetPixel(x, y, Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]));
                            }
                        }
                        CurrentImage = (Bitmap)openBitmap;
                        UpdateCanvas();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid format", "Error Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    file.Close();
                }
                else if (new System.IO.FileInfo(oplg.FileName).Extension.ToLower() == ".rpng")
                {
                    System.IO.FileStream file = new System.IO.FileStream(oplg.FileName, System.IO.FileMode.Open);
                    byte[] format = System.Text.Encoding.Unicode.GetBytes("JPG");
                    file.Seek(format.Length, System.IO.SeekOrigin.Begin);
                    byte[] width = new byte[sizeof(int)];
                    byte[] height = new byte[sizeof(int)];
                    file.Read(width, 0, sizeof(int));
                    file.Read(height, 0, sizeof(int));
                    try
                    {
                        Size imgsize = new Size(BitConverter.ToInt32(width, 0), BitConverter.ToInt32(height, 0));
                        Bitmap openBitmap = new Bitmap(imgsize.Width, imgsize.Height);
                        for (int y = 0; y < imgsize.Height; y++)
                        {
                            for (int x = 0; x < imgsize.Width; x++)
                            {
                                byte[] colorBytes = new byte[4];
                                file.Read(colorBytes, 0, 4);

                                openBitmap.SetPixel(x, y, Color.FromArgb(colorBytes[3], colorBytes[0], colorBytes[1], colorBytes[2]));
                            }
                        }
                        CurrentImage = (Bitmap)openBitmap;
                        UpdateCanvas();
                    }
                    catch
                    {
                        MessageBox.Show("Invalid format", "Error Stream", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    file.Close();
                }
                else
                {
                    CurrentImage = (Bitmap)Bitmap.FromFile(oplg.FileName);
                    path = oplg.FileName;
                    UpdateCanvas();
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.FileName = path;
            save.Filter = "JPG|*.jpg;*.jpeg|PNG|*.png|BMP|*.bmp|txtjpg|*.txtjpg|txtpng|*.txtpng|RawJPG|*.rjpg|RawPNG|*.rpng";
            if (save.ShowDialog() == DialogResult.OK)
            {
                if (save.FilterIndex == 1)
                {
                    CurrentImage.Save(save.FileName, ImageFormat.Jpeg);
                }else if(save.FilterIndex == 2)
                {
                    CurrentImage.Save(save.FileName, ImageFormat.Png);
                }
                else if (save.FilterIndex == 3)
                {
                    CurrentImage.Save(save.FileName, ImageFormat.Bmp);
                }
                else if (save.FilterIndex == 4)
                {
                    System.IO.FileStream file = new System.IO.FileStream(save.FileName, System.IO.FileMode.OpenOrCreate);
                    byte[] buff = System.Text.Encoding.Unicode.GetBytes("JPG" + CurrentImage.Width + "x" + CurrentImage.Height+"\n");
                    file.Write(buff, 0, buff.Length);
                    for(int y = 0; y < CurrentImage.Height; y++)
                    {
                        for (int x = 0; x < CurrentImage.Width; x++)
                        {
                            buff = Encoding.Unicode.GetBytes(CurrentImage.GetPixel(x,y).R.ToString("000")+ CurrentImage.GetPixel(x, y).G.ToString("000")+ CurrentImage.GetPixel(x, y).B.ToString("000"));
                            file.Write(buff, 0, buff.Length);
                        }
                    }
                    file.Close();

                }
                else if (save.FilterIndex == 5)
                {
                    System.IO.FileStream file = new System.IO.FileStream(save.FileName, System.IO.FileMode.OpenOrCreate);
                    byte[] buff = System.Text.Encoding.Unicode.GetBytes("PNG" + CurrentImage.Width + "x" + CurrentImage.Height + "\n");
                    file.Write(buff, 0, buff.Length);
                    for (int y = 0; y < CurrentImage.Height; y++)
                    {
                        for (int x = 0; x < CurrentImage.Width; x++)
                        {
                            buff = Encoding.Unicode.GetBytes(CurrentImage.GetPixel(x, y).R.ToString("000") + CurrentImage.GetPixel(x, y).G.ToString("000") + CurrentImage.GetPixel(x, y).B.ToString("000") + CurrentImage.GetPixel(x, y).A.ToString("000"));
                            file.Write(buff, 0, buff.Length);
                        }
                    }
                    file.Close();
                }
                else if (save.FilterIndex == 6)
                {
                    System.IO.FileStream file = new System.IO.FileStream(save.FileName, System.IO.FileMode.OpenOrCreate);
                    byte[] format = System.Text.Encoding.Unicode.GetBytes("JPG");
                    byte[] w = BitConverter.GetBytes(CurrentImage.Width);
                    byte[] h = BitConverter.GetBytes(CurrentImage.Height);
                    byte[] buff = new byte[format.Length + w.Length + h.Length];
                    Buffer.BlockCopy(format, 0, buff, 0, format.Length);
                    Buffer.BlockCopy(w, 0, buff, format.Length, w.Length);
                    Buffer.BlockCopy(h, 0, buff, format.Length + w.Length, h.Length);
                    file.Write(buff, 0, buff.Length);
                    for (int y = 0; y < CurrentImage.Height; y++)
                    {
                        for (int x = 0; x < CurrentImage.Width; x++)
                        {
                            buff = new byte[3] { CurrentImage.GetPixel(x,y).R, CurrentImage.GetPixel(x, y).G, CurrentImage.GetPixel(x, y).B };
                            file.Write(buff, 0, buff.Length);
                        }
                    }
                    file.Close();
                }
                else if (save.FilterIndex == 7)
                {
                    System.IO.FileStream file = new System.IO.FileStream(save.FileName, System.IO.FileMode.OpenOrCreate);
                    byte[] format = System.Text.Encoding.Unicode.GetBytes("PNG");
                    byte[] w = BitConverter.GetBytes(CurrentImage.Width);
                    byte[] h = BitConverter.GetBytes(CurrentImage.Height);
                    byte[] buff = new byte[format.Length + w.Length + h.Length];
                    Buffer.BlockCopy(format, 0, buff, 0, format.Length);
                    Buffer.BlockCopy(w, 0, buff, format.Length, w.Length);
                    Buffer.BlockCopy(h, 0, buff, format.Length + w.Length, h.Length);
                    file.Write(buff, 0, buff.Length);
                    for (int y = 0; y < CurrentImage.Height; y++)
                    {
                        for (int x = 0; x < CurrentImage.Width; x++)
                        {
                            buff = new byte[4] { CurrentImage.GetPixel(x, y).R, CurrentImage.GetPixel(x, y).G, CurrentImage.GetPixel(x, y).B, CurrentImage.GetPixel(x, y).A };
                            file.Write(buff, 0, buff.Length);
                        }
                    }
                    file.Close();
                }

            }
        }
    }
}
