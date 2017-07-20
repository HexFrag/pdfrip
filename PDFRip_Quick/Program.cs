using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfUtils;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFRip_Quick
{
    class Program
    {

        private static string pdfPath;
        private static string saveFolder = "pdftest";
        private static List<string> PDFText = new List<string>();
        public static readonly string ExecutingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static List<Dictionary<string, System.Drawing.Image>> PDFImages = new List<Dictionary<string, System.Drawing.Image>>();
        private static int imageCount = 0;
        private static string pdfName;



        static void Main(string[] args)
        {
            
            if(args.Length > 0)
            {

                Exec(args);
            }
            else
            {
                Console.WriteLine("Specifiy Path or run in same folder as file...any key to continue...");
                Console.ReadKey();
                Console.Clear();
                Exec(args);
            }



        }

        private static void Exec(string[] args)
        {
            pdfPath = args[0];
            if (!File.Exists(pdfPath))
            {
                Console.WriteLine("File Not Found");
                Console.ReadKey();
                Environment.Exit(0);
            }

            pdfName = System.IO.Path.GetFileNameWithoutExtension(pdfPath);

            saveFolder = System.IO.Path.Combine(ExecutingDirectory, string.Format("{0}_ripped", pdfName));
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

           


            RunApp();
        }

        private static void RunApp()
        {

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

           

            var reader = new PdfReader(pdfPath);
            var raf = new RandomAccessFileOrArray(pdfPath);



            Console.WriteLine("PDF Found, getting text.");
            for (var i = 1; i < reader.NumberOfPages; i++)
            {
                PDFText.Add(PdfTextExtractor.GetTextFromPage(reader, i));

                if (PdfImageExtractor.PageContainsImages(pdfPath, i))
                {
                    PDFImages.Add(PdfImageExtractor.ExtractImages(pdfPath, i));
                }

            }


            Console.WriteLine(string.Format("Found {0} pages of text, from {1} pages in PDF", PDFText.Count, reader.NumberOfPages));

            var saveFile = System.IO.Path.Combine(saveFolder, string.Format("{0}_Text.txt", pdfName));
            Console.WriteLine(string.Format("Saving text to {0}", saveFile));
            File.AppendAllLines(saveFile, PDFText);

            Console.WriteLine("Saving Images");

            foreach (var dict in PDFImages)
            {
                foreach (var key in dict.Keys)
                {
                    System.Drawing.Image img;
                    if (dict.TryGetValue(key, out img))
                    {
                        imageCount += dict.Count - 1;
                        try
                        {
                            img.Save(System.IO.Path.Combine(saveFolder, key));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                }
            }

            Console.WriteLine(string.Format("Saved {0} images in {1}", imageCount, saveFolder));
            Console.WriteLine("...any key to continue...");
            Console.ReadKey();
        }

        
        
    }
}
