using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BingoGenerator
{
    public partial class Form1 : Form
    {
        int _pageBorder = 0;
        int _width = 1700;
        int _height = 1100;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonDoTheThing_Click(object sender, EventArgs e)
        {
            //ExtractImagesFromPdfs();

            GenerateBingoPairImages();

            foreach (String file in Directory.GetFiles(@"C:\Users\bmaye\Desktop\bingo\output\sheets"))
            {
                Bitmap b = new Bitmap(_width, _height);
                var g = Graphics.FromImage(b);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, b.Width, b.Height);
                g.Save();
                DrawBingoCard(g, file);
                DrawBorder(g);
                //DrawHeader(g);
                b.Save(@"C:\Users\bmaye\Desktop\bingo\output\final_images\" + Path.GetFileNameWithoutExtension(file) + ".png", ImageFormat.Png);
                g.Dispose();
                b.Dispose();
            }
        }

        private void GenerateBingoPairImages()
        {
            BingoNumberManager bnm = new BingoNumberManager();
            bnm.GenerateBingoGrids(320);

            Font font = new System.Drawing.Font("Arial", 50);

            int i = 1, sheet = 1;
            float x = 0, y = 0;

            Image blankCardImage = null;
            Graphics g = null;

            foreach (BingoGrid grid in bnm.BingoGrids)
            {
                if (i % 2 == 0)
                    x = 760;
                else
                {
                    blankCardImage = Image.FromFile(@"C:\Users\bmaye\OneDrive - University of California, Riverside\bingo\input\blank_card.png");
                    g = Graphics.FromImage(blankCardImage);
                    x = 35;
                }
                
                foreach (var column in grid.columns)
                {
                    y = 105;
                    foreach (var value in column)
                    {
                        if (column.IndexOf(value) == 2 && grid.columns.IndexOf(column) == 2)
                        {
                            y += 100;
                            continue;
                        }

                        int padding = 0;
                        if (value < 10)
                            padding = 20;

                        g.DrawString(value.ToString(), font, Brushes.Black, x + padding, y);
                        y += 100;
                    }

                    x += 105;
                }

                if (i % 2 == 0)
                {
                    blankCardImage.Save(@"C:\Users\bmaye\Desktop\Bingo\output\sheets\sheet" + sheet.ToString() + ".png");
                    sheet++;
                    g.Dispose();
                    blankCardImage.Dispose();
                }
                i++;
            }
        }

        private void DrawHeader(Graphics g)
        {
            var headerImage = Image.FromFile(@"C:\Users\bmaye\Desktop\bingo\text.png");
            int paddingTop = 25;
            int borderImageWidth = 100;

            int x = _width / 2;
            x = x - (headerImage.Width / 2);

            int y = _pageBorder + paddingTop + borderImageWidth;

            g.DrawImage(headerImage, x, y, headerImage.Width, headerImage.Height);
            headerImage.Dispose();
        }

        private void DrawBorder(Graphics g)
        {
            var borderImage = Image.FromFile(@"C:\Users\bmaye\OneDrive - University of California, Riverside\bingo\border2.png");

            //g.Transform.RotateAt(90, new PointF(_width / 2, _height / 2));
            g.DrawImage(borderImage, _pageBorder, _pageBorder, _width - (2 * _pageBorder), _height - (2 * _pageBorder));
            borderImage.Dispose();
            //g.Transform.RotateAt(-90, new PointF(_width / 2, _height / 2));
        }

        private void DrawBingoCard(Graphics g, String file)
        {
            var bingoImage = GrayScale(new Bitmap(Image.FromFile(file)));
            int paddingTop = 0;

            int x = _width / 2;
            x = x - (bingoImage.Width / 2);

            int y = _height / 2;
            y = y - (bingoImage.Height / 2);
            y += paddingTop;

            g.DrawImage(bingoImage, x, y, bingoImage.Width, bingoImage.Height);
            bingoImage.Dispose();
        }

        public Bitmap GrayScale(Bitmap Bmp)
        {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)((c.R + c.G + c.B) / 3);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
            return Bmp;
        }
    }
}
