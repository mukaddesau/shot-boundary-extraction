using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;


using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace WindowsFormsApplication1
{
    class Functions
    {
        private int w, h;

        // Görüntüyü axa boyutlarında bloklara ayırma.   
        public List<Bitmap> partitionImage(Bitmap bitmap, int x)
        {
            List<Bitmap> blocks = new List<Bitmap>();


            for (int i = 0; i < x; i++)
            {
                for (int y = 0; y < x; y++)
                {
                    // Rect(x-coordinate, y-coordinate, width,height)
                    Rectangle r = new Rectangle(i * (bitmap.Width / x), y * (bitmap.Height / x), bitmap.Width / x, bitmap.Height / x);
                    blocks.Add(cropImage(bitmap, r));
                }
            }

            return blocks;
        }

        public Bitmap cropImage(Bitmap bitmap, Rectangle cropArea)
        {
            Bitmap bmpCrop = bitmap.Clone(cropArea, System.Drawing.Imaging.PixelFormat.DontCare);
            return bmpCrop;
        }

        public double[][] computeHistogram(List<Bitmap> blocks)
        {
            double[][] block_features = new double[9][];            
            int block_no = 0;
            int hue;
            

            for (int i = 0; i < 9; i++)
                block_features[i] = new double[360];
            


                foreach (Bitmap block in blocks)
                {
                    w = block.Width;
                    h = block.Height;
                    for (int i = 0; i < w; i++)
                        for (int j = 0; j < h; j++)
                        {
                            hue = (int)block.GetPixel(i, j).GetHue();
                            //Console.WriteLine(hue);
                            block_features[block_no][hue]++;
                        }


                    block_no++;
                }

                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 360; j++)
                        block_features[i][j] /= (w * h);          


            
            return block_features;
        }

        public double bhattacharyyaDistance(double[] histT1, double[] histT2)
        {

            double bhatt_coef = 0;

            for (int m = 0; m < 360; m++)
                bhatt_coef += Math.Sqrt(histT1[m]*histT2[m]);
            //Console.WriteLine("bhat:" + bhatt_coef);
            return bhatt_coef;
        }

       
       
    }
}
