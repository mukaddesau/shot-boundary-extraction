using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using System.Drawing.Imaging;
using System.Threading;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {       

        private List<Bitmap> bitmap_array = new List<Bitmap>();    
        VideoCapture _capture;

        private System.Windows.Forms.Timer My_Timer = new System.Windows.Forms.Timer();
        int FPS = 25; //Frame Rate 
        int flag = 0; 
        int height, width;
        private List<double> battDist = new List<double>();

        public Form1()
        {
            InitializeComponent();
            btn_compute.Enabled = false;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        

        private void My_Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (_capture.QueryFrame() != null && _capture.QueryFrame().ToImage<Bgr, Byte>() != null)
                {
                    Image<Bgr, Byte> imgOrg = _capture.QueryFrame().ToImage<Bgr, Byte>();
                    
                    pb_video.Image = imgOrg.ToBitmap();
                    bitmap_array.Add(imgOrg.ToBitmap());

                }
                else
                {
                    My_Timer.Stop();
                    flag = 1;
                    btn_open_file.Enabled = false;
                    btn_compute.Enabled = true;
                    MessageBox.Show("Video loaded successfully.");
                    Console.WriteLine("bitti");
                }

            }
            catch(NullReferenceException ne)
            {
                MessageBox.Show(ne.Message);
                return;
            }
        }

        private void pb_video_Click(object sender, EventArgs e)
        {

        }

        private void btn_open_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd_dosya_ac = new OpenFileDialog();
            if (ofd_dosya_ac.ShowDialog() == DialogResult.OK)
            {
                try
                {                    
                    Console.WriteLine(ofd_dosya_ac.FileName.ToString());
                    My_Timer.Interval = 1000 / FPS;
                    My_Timer.Tick += new EventHandler(My_Timer_Tick);
                    My_Timer.Start();
                    _capture = new VideoCapture(ofd_dosya_ac.FileName.ToString()); //"C:\\Users\\Mukaddes\\Desktop\\d.mp4"
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Form1", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MessageBox.Show("Error! File not opened!!");
                }
            }
        }
       

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void btn_compute_Click(object sender, EventArgs e)
        {
            if (flag == 1)
            {
                
                Functions func = new Functions();
                double[] dist = new double[9];
                double[][] histT1 = new double[9][];
                double[][] histT2 = new double[9][];
                Console.WriteLine("dim:" + bitmap_array.Count);
                
                for (int j=0; j<bitmap_array.Count-1; j++)
                {                   
                    List<Bitmap> btm_blocks_t1 = func.partitionImage(bitmap_array[j], 3);
                    histT1 = func.computeHistogram(btm_blocks_t1);  

                    List<Bitmap> btm_blocks_t2 = func.partitionImage(bitmap_array[j+1], 3);
                    
                    //Console.WriteLine(j);
                    //panel1.Visible = false;
                    //btm_parts[0].Save("C:\\Users\\Mukaddes\\Desktop\\Yeni klasör\\0.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    //pb1.Image = btm_parts[0];
                    //pb2.Image = btm_parts[1];
                    //pb3.Image = btm_parts[2];
                    //pb4.Image = btm_parts[3];
                    //pb5.Image = btm_parts[4];
                    //pb6.Image = btm_parts[5];
                    //pb7.Image = btm_parts[6];
                    //pb8.Image = btm_parts[7];
                    //pb9.Image = btm_parts[8];

                    histT2 = func.computeHistogram(btm_blocks_t2);

                    height = btm_blocks_t1[0].Height;
                    width = btm_blocks_t1[0].Width;                                  
                    
                    for (int i = 0; i < 9; i++)                    
                        dist[i] = func.bhattacharyyaDistance(histT1[i], histT2[i]);   

                    double mean_dist = dist.Average();
                    battDist.Add(mean_dist);                  

                }
                int dim = battDist.Count();
               
                double[] centers = new double[2];
               
                Matrix<float> distances = new Matrix<float>(dim, 1);
                Matrix<int> clusters = new Matrix<int>(dim, 1);

                for (int i = 0; i < dim; i++)                
                    distances[i, 0] = (float)battDist[i];     

                MCvTermCriteria termCrit = new MCvTermCriteria(100);
                CvInvoke.Kmeans(distances, 2, clusters, termCrit, 2, 0);
                               
               Console.WriteLine("yazmaya gecis");
                using (System.IO.StreamWriter file =
                   new System.IO.StreamWriter(@"C:\Users\Mukaddes\Desktop\outputdemo\distance_cluster.txt"))
                   {
                        for (int i = 0; i < dim; i++)  
                            file.WriteLine(distances[i,0] + " " + clusters[i,0]);                    
                    
                   }
                int count1 = 0;

                for (int i = 0; i < dim; i++)
                    if (clusters[i, 0] == 1)
                        count1++;
                
                for (int i = 0; i < dim; i++)
                {
                    if (count1 > (dim - count1) )
                    {//
                        if (clusters[i, 0] == 0)
                        {
                            if (i > 2)
                            {
                                string filename1 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_1.jpg");
                                string filename2 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_2.jpg");
                                string filename3 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_3.jpg");
                                string filename4 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_4.jpg");
                                string filename5 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_5.jpg");
                                bitmap_array[i - 2].Save(filename1, ImageFormat.Jpeg);
                                bitmap_array[i - 1].Save(filename2, ImageFormat.Jpeg);
                                bitmap_array[i].Save(filename3, ImageFormat.Jpeg);
                                bitmap_array[i + 1].Save(filename4, ImageFormat.Jpeg);
                                bitmap_array[i + 2].Save(filename5, ImageFormat.Jpeg);
                            }

                        }
                    }
                    else
                    { //&& c[i,0]<0.8
                        if (clusters[i, 0] == 1 )
                        {
                            if (i > 2)
                            {
                                string filename1 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_1.jpg");
                                string filename2 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_2.jpg");
                                string filename3 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_3.jpg");
                                string filename4 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_4.jpg");
                                string filename5 = (@"C:\Users\Mukaddes\Desktop\outputdemo\shot" + i + "_5.jpg");
                                bitmap_array[i - 2].Save(filename1, ImageFormat.Jpeg);
                                bitmap_array[i - 1].Save(filename2, ImageFormat.Jpeg);
                                bitmap_array[i].Save(filename3, ImageFormat.Jpeg);
                                bitmap_array[i + 1].Save(filename4, ImageFormat.Jpeg);
                                bitmap_array[i + 2].Save(filename5, ImageFormat.Jpeg);
                            }

                        }
                    }
                    
                       
                }

                MessageBox.Show("Shot boundary extraction completed successsfully.");   
          }
        }

        public void display(string text)
        {
            MessageBox.Show(text);
        }
        private void pb_computing_Click(object sender, EventArgs e)
        {

        }

       

    }
}
