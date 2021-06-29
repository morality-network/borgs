﻿using System;
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
            //var test = ConvertBorgToBitmap(new List<string>() { "#FF0000", "", "#FF0000", "#FFFF00", "", "#FFFF00", "#A569BD", "", "#A569BD" });
            //test.Save("test.png");

            var layers = ReadLayers("Images");
        }

        /// <summary>
        /// Read layers from a directory/file structure
        /// </summary>
        /// <param name="folderName">The parent directory holding the sub directory/file structure required to parse</param>
        /// <returns>Borg layers</returns>
        public static List<Layer> ReadLayers(string folderName)
        {
            // Define layers
            var layers = new List<Layer>();

            // Get all directories to read images from
            var folders = Directory.GetDirectories(folderName)
                .OrderBy(x => x);

            // Define position
            var position = 0;

            // For each directory (acting as a layer) we read its files/images into layer items
            foreach (var folder in folders)
            {
                // Get the files/images to turn to historgrams
                var files = Directory.GetFiles(folder);

                // Add the layer
                layers.Add(new Layer() { Position = position++, LayerItems = BuildLayerItems(files.ToList()) });
            }

            // Return the built layers
            return layers;
        }

        /// <summary>
        /// Build the layer items for a layer
        /// </summary>
        /// <param name="files">The files to build the layer items from</param>
        /// <returns>A layers items</returns>
        public static List<LayerItem> BuildLayerItems(List<string> files)
        {
            // Define the items
            var layersItems = new List<LayerItem>();

            // For each of the files we want to build a histogram to add to the layer
            foreach (var file in files)
            {
                // Get the image from file location
                var bitmap = new Bitmap(file);
                
                // Define histogram
                var histogram = new Histogram();

                // Process the image
                for (int i = 0; i < bitmap.Size.Height; i++)
                {
                    for (int j = 0; j < bitmap.Size.Width; j++)
                    {
                        // Get the pixal for the current position
                        var pixal = bitmap.GetPixel(i, j);

                        // Determine the color
                        var color = Color.FromArgb(pixal.A, pixal.R, pixal.G, pixal.B);

                        // Convert to hex value
                        var hex = color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");

                        // Calculate the position as if it were in a flat array
                        var imgPosition = (i * bitmap.Size.Height) + j;

                        // Set in histogram
                        histogram.AddData(hex, imgPosition);
                    }
                }

                // Get the chance of the layer item being selected (from file name)
                var chance = ParseChance(file);

                // Once the image has been processed we can add the image (as a histogram) to the collection of layer items
                layersItems.Add(new LayerItem() { Name = Guid.NewGuid().ToString(), Chance = chance, Histogram = histogram });
            }

            // Return the items
            return layersItems;
        }  
        
        /// <summary>
        /// Parse a file name to get the chance it can be selected ie. fileName="35_.png" -> 35
        /// NOTE: "_" allows multiple file names with same chance
        /// </summary>
        /// <param name="file">A full file locations</param>
        /// <returns>A decimal value representing chance for the respective file</returns>
        public static decimal ParseChance(string file)
        {
            // We get the file name (which holds chance data)
            var actualFileName = Path.GetFileName(file);

            // Parse the chance
            var parsedChance = actualFileName.Substring(0, actualFileName.LastIndexOf('.')).Replace("_", string.Empty);

            // Return parsed value
            return decimal.Parse(parsedChance);
        }

        /// <summary>
        /// This is used to convert an array of hex pixals back into an argb image
        /// </summary>
        /// <param name="hexValues">The string hex values to convert to image (flat)</param>
        /// <returns>A 2d bitmap</returns>
        public static Bitmap ConvertBorgToBitmap(List<string> hexValues)
        {
            var sqrt = (int)Math.Sqrt(hexValues.Count());
            var bitmap = new Bitmap(sqrt, sqrt, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            for (int i = 0; i < hexValues.Count(); i++)
            {
                // Default white
                var pixal = Color.White;

                // If specified then isnt white
                if (!string.IsNullOrEmpty(hexValues[i]))
                    pixal = System.Drawing.ColorTranslator.FromHtml(hexValues[i]);

                // Define 2d coords
                var y = i / sqrt;
                var x = i % sqrt;

                // Set in place
                bitmap.SetPixel(x,y,pixal);
            }

            // Return the built up image
            return bitmap;
        }
    }
}

