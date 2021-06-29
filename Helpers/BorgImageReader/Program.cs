using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace BorgImageReader
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var layers = ReadLayers("Images");
        }

        public static List<Layer> ReadLayers(string folderName)
        {
            var layers = new List<Layer>();

            var folders = Directory.GetDirectories(folderName)
                .OrderBy(x => x);

            int position = 0;

            foreach (var folder in folders)
            {
                var files = Directory.GetFiles(folder);

                var layersItems = new List<LayerItem>();

                foreach (var file in files)
                {
                    var bitmap = new Bitmap(file);
                    var histogram = new Histogram();

                    for (int i = 0; i < bitmap.Size.Height; i++)
                    {
                        for (int j = 0; j < bitmap.Size.Width; j++)
                        {
                            var pixal = bitmap.GetPixel(i, j);
                            var color = Color.FromArgb(pixal.A, pixal.R, pixal.G, pixal.B);
                            string hex = color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
                            var imgPosition = (i * bitmap.Size.Height) + j;
                            histogram.AddData(hex, imgPosition);
                        }
                    }

                    var actualFileName = Path.GetFileName(file);
                    var parsedChance = actualFileName.Substring(0, actualFileName.LastIndexOf('.')).Replace("_", string.Empty);
                    var chance = decimal.Parse(parsedChance);

                    layersItems.Add(new LayerItem() { Name = "", Chance = chance, Histogram = histogram });
                }

                layers.Add(new Layer() { Position = position++, LayerItems = layersItems });
            }

            return layers;
        }

        public static void ConvertToHistogram(List<string> flatArray)
        {

        }
    }
}
