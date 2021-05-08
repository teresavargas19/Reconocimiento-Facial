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
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Data.OleDb;
using System.Runtime.InteropServices;

namespace ReconocimientoFacial
{
    public partial class Frm_Registrar : Form
    {
        public string[] Labels;
        clsDA dbc = new clsDA();
        int con = 0, ini = 0, fin;
     
        Image<Bgr, Byte> currentFrame;
        Capture grabber;
        HaarCascade face;
        MCvFont font = new MCvFont(FONT.CV_FONT_HERSHEY_TRIPLEX, 0.5d, 0.5d);
        Image<Gray, byte> result, TrainedFace = null;
        Image<Gray, byte> gray = null;
        List<Image<Gray, byte>> trainingImages = new List<Image<Gray, byte>>();
        List<string> labels = new List<string>();
        List<string> labels1 = new List<string>();
        List<string> NamePersons = new List<string>();
        int ContTrain, NumLabels, t;
        public Frm_Registrar()
        {
            InitializeComponent();
            face = new HaarCascade("haarcascade_frontalface_default.xml");
            try
            {
                dbc.ObtenerBytesImagen();           
                Labels = dbc.Name; 
                NumLabels = dbc.TotalUser;
                ContTrain = NumLabels;


                for (int tf = 0; tf < NumLabels; tf++)
                {
                    con = tf;
                    Bitmap bmp = new Bitmap(dbc.ConvertByteToImg(con));
                    //LoadFaces = "face" + tf + ".bmp";
                    trainingImages.Add(new Image<Gray, byte>(bmp));
                    labels.Add(Labels[tf]);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e + " No hay ningún rostro en la Base de Datos, por favor añadir por lo menos una cara", "Cragar caras en tu Base de Datos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btn_detectar_Click(object sender, EventArgs e)
        {
            try
            {
                        
                grabber = new Capture();
                grabber.QueryFrame();

               
                Application.Idle += new EventHandler(FrameGrabber);
                this.button1.Enabled = true;
                btn_detectar.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Frm_Registrar_Load_1(object sender, EventArgs e)
        {

        }

        private void imageBoxFrameGrabber_Click(object sender, EventArgs e)
        {

        }

        void FrameGrabber(object sender, EventArgs e)
        {
            lblNumeroDetect.Text = "0";
            NamePersons.Add("");
            try
            {

                
                try
                {
                    currentFrame = grabber.QueryFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                }
                catch (Exception)
                {
                    imageBoxFrameGrabber.Image = null;
                }

               
                gray = currentFrame.Convert<Gray, Byte>();

                //Detector de Rostros
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    t = t + 1;
                    result = currentFrame.Copy(f.rect).Convert<Gray, byte>().Resize(640, 480, INTER.CV_INTER_CUBIC);
                    
                    currentFrame.Draw(f.rect, new Bgr(Color.LightGreen), 1);

                    NamePersons[t - 1] = Name;
                    NamePersons.Add("");
                    
                    lblNumeroDetect.Text = facesDetected[0].Length.ToString();
                    //lblNadie.Text = name;

                }
                t = 0;

               
                imageBoxFrameGrabber.Image = currentFrame;
                Name = "";
                          
                NamePersons.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            try
            {
             
                ContTrain = ContTrain + 1;

                
                gray = grabber.QueryGrayFrame().Resize(320, 240, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);

                //Face Detector
                MCvAvgComp[][] facesDetected = gray.DetectHaarCascade(face, 1.2, 10, Emgu.CV.CvEnum.HAAR_DETECTION_TYPE.DO_CANNY_PRUNING, new Size(20, 20));

                
                foreach (MCvAvgComp f in facesDetected[0])
                {
                    TrainedFace = currentFrame.Copy(f.rect).Convert<Gray, byte>();
                    break;
                }


                TrainedFace = result.Resize(100, 100, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC);
                trainingImages.Add(TrainedFace);
                labels.Add(txt_nombre.Text);

            
                imageBox2.Image = TrainedFace;
                dbc.ConvertImgToBinary(txt_nombre.Text, txt_codigo.Text, imageBox2.Image.Bitmap);
                //}                
                MessageBox.Show("Agregado correctamente", "Capturado", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void btnlimpiar_Click(object sender, EventArgs e)
        {
            imageBox2.Image = null;
            this.txt_codigo.Clear();
            this.txt_nombre.Clear();
        }
        private void btn_loadImgsBD_Click(object sender, EventArgs e)
        {
            //groupBox2.Enabled = true;
          //  pictureBox1.Image = dbc.ConvertByteToImg(0);
          // label4.Text = dbc.Name[0];
        }
        private void btn_siguiente_Click(object sender, EventArgs e)
        {
            if (ini < NumLabels - 1)
            {
                ini++;
                pictureBox1.Image = dbc.ConvertByteToImg(ini);
                label4.Text = dbc.Name[ini];
            }
        }

        private void btn_anterior_Click(object sender, EventArgs e)
        {
            if (ini > 0)
            {
                ini--;
                pictureBox1.Image = dbc.ConvertByteToImg(ini);
                label4.Text = dbc.Name[ini];
            }
        }

        private void btn_ultimo_Click(object sender, EventArgs e)
        {
            ini = NumLabels - 1;
            pictureBox1.Image = dbc.ConvertByteToImg(ini);
            label4.Text = dbc.Name[ini];
        }


        private void Frm_Registrar_Load(object sender, EventArgs e)
        {
            imageBoxFrameGrabber.ImageLocation = "img/1.png";
        }

    }



}

