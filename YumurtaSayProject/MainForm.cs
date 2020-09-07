
// http://www.aforgenet.com/framework/


using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;

using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using AForge.Math.Geometry;

namespace ShapeChecker
{
    public partial class MainForm : Form
    {
        public MainForm( )
        {
            InitializeComponent( );
        }

        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        private void MainForm_Load( object sender, EventArgs e )
        {
            LoadDemo( "coins.jpg" );
        }

        private void openToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( openFileDialog.ShowDialog( ) == DialogResult.OK )
            {
                try
                {
                    ProcessImage( (Bitmap) Bitmap.FromFile( openFileDialog.FileName ) );
                }
                catch
                {
                    MessageBox.Show( "Failed loading selected image file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                }
            }
        }

        private void loadDemoImage1ToolStripMenuItem_Click( object sender, EventArgs e )
        {
            LoadDemo( "coins.jpg" );
        }

        private void loadDemoImage2ToolStripMenuItem_Click( object sender, EventArgs e )
        {
            LoadDemo( "demo.png" );
        }

        private void loadDemoImage3ToolStripMenuItem_Click( object sender, EventArgs e )
        {
            LoadDemo( "demo1.png" );
        }

        private void loadDemoImage4ToolStripMenuItem_Click( object sender, EventArgs e )
        {
            LoadDemo( "demo2.png" );
        }
      

        private void LoadDemo( string embeddedFileName )
        {
            Assembly assembly = this.GetType( ).Assembly;
            Bitmap image = new Bitmap( assembly.GetManifestResourceStream( "ShapeChecker." + embeddedFileName ) );
            ProcessImage( image );
        }

        
        private void ProcessImage( Bitmap bitmap )
        {
            int toplamSekilSayisi=0;
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle( 0, 0, bitmap.Width, bitmap.Height ),
                ImageLockMode.ReadWrite, bitmap.PixelFormat );

            ColorFiltering colorFilter = new ColorFiltering( );

            colorFilter.Red   = new IntRange( 100, 255 );
            colorFilter.Green = new IntRange( 0, 75 );
            colorFilter.Blue  = new IntRange( 0, 75 );

            colorFilter.ApplyInPlace( bitmapData );

            BlobCounter blobCounter = new BlobCounter( );

            blobCounter.FilterBlobs = true;
            blobCounter.MinHeight = 5;
            blobCounter.MinWidth = 5;

            blobCounter.ProcessImage( bitmapData );
            Blob[] blobs = blobCounter.GetObjectsInformation( );
            bitmap.UnlockBits( bitmapData );

            SimpleShapeChecker shapeChecker = new SimpleShapeChecker( );

            Graphics g = Graphics.FromImage( bitmap );
            Pen yellowPen = new Pen( Color.Yellow, 2 ); 
           
            for ( int i = 0, n = blobs.Length; i < n; i++ )
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints( blobs[i] );

                DoublePoint center;
                double radius;

                if ( shapeChecker.IsCircle( edgePoints, out center, out radius ) )
                {
                    g.DrawEllipse( yellowPen,
                        (float) ( center.X - radius ), (float) ( center.Y - radius ),
                        (float) ( radius * 2 ), (float) ( radius * 2 ) );
                    toplamSekilSayisi++;
                }
                label9.Text = "boşluk sayısı : " + (toplamSekilSayisi.ToString());
                
               


            }

            if (toplamSekilSayisi < 5 &&toplamSekilSayisi >= 1)
            {
                string message = "Yumurta sayısı azaldı." +
                    "Sipariş verilsin mi?";
                string title = "Yumurta Kontrol";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    MessageBox.Show("Siparişiniz verildi.");
                }
                else
                {
                }
            }



            yellowPen.Dispose( );
           
            g.Dispose( );

            Clipboard.SetDataObject( bitmap );
            pictureBox.Image = bitmap;

            UpdatePictureBoxPosition( );
        }

        private void splitContainer_Panel2_Resize( object sender, EventArgs e )
        {
            UpdatePictureBoxPosition( );
        }

        private void UpdatePictureBoxPosition( )
        {
            int imageWidth;
            int imageHeight;

            if ( pictureBox.Image == null )
            {
                imageWidth  = 320;
                imageHeight = 240;
            }
            else
            {
                imageWidth  = pictureBox.Image.Width;
                imageHeight = pictureBox.Image.Height;
            }

            Rectangle rc = splitContainer.Panel2.ClientRectangle;

            pictureBox.SuspendLayout( );
            pictureBox.Location = new Point( ( rc.Width - imageWidth - 2 ) / 2, ( rc.Height - imageHeight - 2 ) / 2 );
            pictureBox.Size = new Size( imageWidth + 2, imageHeight + 2 );
            pictureBox.ResumeLayout( );
        }

        private Point[] ToPointsArray( List<IntPoint> points )
        {
            Point[] array = new Point[points.Count];

            for ( int i = 0, n = points.Count; i < n; i++ )
            {
                array[i] = new Point( points[i].X, points[i].Y );
            }

            return array;
        }

        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AboutForm form = new AboutForm( );

            form.ShowDialog( );
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }

}
