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
        int _pageBorder = 20;
        int _width = 1700;
        int _height = 1100;

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonDoTheThing_Click(object sender, EventArgs e)
        {
            //ExtractImagesFromPdfs();

            foreach (String file in Directory.GetFiles(@"C:\Users\Brandon\Desktop\bingo\input"))
            {
                Bitmap b = new Bitmap(_width, _height);
                var g = Graphics.FromImage(b);
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, b.Width, b.Height);
                g.Save();
                DrawBorder(g);
                DrawBingoCard(g, file);
                DrawHeader(g);
                b.Save(@"C:\Users\Brandon\Desktop\bingo\output\" + Path.GetFileNameWithoutExtension(file) + ".jpg", ImageFormat.Jpeg);
            }
        }

        private void ExtractImagesFromPdfs()
        {
            foreach (String file in Directory.GetFiles(@"C:\Users\Brandon\Desktop\bingo\pdfs"))
            {
                ExtractImagesFromPDF(file, @"C:\Users\Brandon\Desktop\bingo\bingo_cards");
            }
        }

        private void GnerateBingoPairImages()
        {

        }

        private void DrawHeader(Graphics g)
        {
            var headerImage = Image.FromFile(@"C:\Users\Brandon\Desktop\bingo\text.png");
            int paddingTop = 25;
            int borderImageWidth = 100;

            int x = _width / 2;
            x = x - (headerImage.Width / 2);

            int y = _pageBorder + paddingTop + borderImageWidth;

            g.DrawImage(headerImage, x, y, headerImage.Width, headerImage.Height);
        }

        private void DrawBorder(Graphics g)
        {
            var borderImage = Image.FromFile(@"C:\Users\Brandon\Desktop\bingo\border.jpg");

            //g.Transform.RotateAt(90, new PointF(_width / 2, _height / 2));
            g.DrawImage(borderImage, _pageBorder, _pageBorder, _width - (2 * _pageBorder), _height - (2 * _pageBorder));
            //g.Transform.RotateAt(-90, new PointF(_width / 2, _height / 2));
        }

        private void DrawBingoCard(Graphics g, String file)
        {
            var bingoImage = GrayScale(new Bitmap(Image.FromFile(file)));
            int paddingTop = 75;

            int x = _width / 2;
            x = x - (bingoImage.Width / 2);

            int y = _height / 2;
            y = y - (bingoImage.Height / 2);
            y += paddingTop;

            g.DrawImage(bingoImage, x, y, bingoImage.Width, bingoImage.Height);
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

        public static void ExtractImagesFromPDF(string sourcePdf, string outputPath)
        {
            // NOTE:  This will only get the first image it finds per page.
            PdfReader pdf = new PdfReader(sourcePdf);
            RandomAccessFileOrArray raf = new iTextSharp.text.pdf.RandomAccessFileOrArray(sourcePdf);

            try
            {
                for (int pageNumber = 1; pageNumber <= pdf.NumberOfPages; pageNumber++)
                {
                    PdfDictionary pg = pdf.GetPageN(pageNumber);
                    PdfDictionary res =
                      (PdfDictionary)PdfReader.GetPdfObject(pg.Get(PdfName.RESOURCES));
                    PdfDictionary xobj =
                      (PdfDictionary)PdfReader.GetPdfObject(res.Get(PdfName.XOBJECT));
                    if (xobj != null)
                    {
                        foreach (PdfName name in xobj.Keys)
                        {
                            PdfObject obj = xobj.Get(name);
                            if (obj.IsIndirect())
                            {
                                PdfDictionary tg = (PdfDictionary)PdfReader.GetPdfObject(obj);
                                PdfName type =
                                  (PdfName)PdfReader.GetPdfObject(tg.Get(PdfName.SUBTYPE));
                                if (PdfName.IMAGE.Equals(type))
                                {

                                    int XrefIndex = Convert.ToInt32(((PRIndirectReference)obj).Number.ToString(System.Globalization.CultureInfo.InvariantCulture));
                                    PdfObject pdfObj = pdf.GetPdfObject(XrefIndex);
                                    PdfStream pdfStrem = (PdfStream)pdfObj;
                                    byte[] bytes = PdfReader.GetStreamBytesRaw((PRStream)pdfStrem);
                                    if ((bytes != null))
                                    {
                                        using (System.IO.MemoryStream memStream = new System.IO.MemoryStream(bytes))
                                        {
                                            memStream.Position = 0;
                                            System.Drawing.Image img = System.Drawing.Image.FromStream(memStream);
                                            // must save the file while stream is open.
                                            if (!Directory.Exists(outputPath))
                                                Directory.CreateDirectory(outputPath);

                                            string path = Path.Combine(outputPath, String.Format(@"{0}.jpg", pageNumber));
                                            System.Drawing.Imaging.EncoderParameters parms = new System.Drawing.Imaging.EncoderParameters(1);
                                            parms.Param[0] = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Compression, 0);
                                            // GetImageEncoder is found below this method
                                            System.Drawing.Imaging.ImageCodecInfo jpegEncoder = GetImageEncoder("JPEG");
                                            img.Save(path, jpegEncoder, parms);
                                            break;

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            catch
            {
                throw;
            }
            finally
            {
                pdf.Close();
            }


        }

        public static System.Drawing.Imaging.ImageCodecInfo GetImageEncoder(string imageType)
        {
            imageType = imageType.ToUpperInvariant();

            foreach (ImageCodecInfo info in ImageCodecInfo.GetImageEncoders())
            {
                if (info.FormatDescription == imageType)
                {
                    return info;
                }
            }

            return null;
        }
    }
}
