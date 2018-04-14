using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace XCCloudService.Common
{
    public class QRHelper
    {
        /// <summary>
        /// 创建QR
        /// </summary>
        /// <param name="qrContent"></param>
        /// <returns></returns>
        public static byte[] CreateQR(string qrContent,int qrCodeScale,int qrCodeVersion)
        {
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = qrCodeScale;
            qrCodeEncoder.QRCodeVersion = qrCodeVersion;
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
            System.Drawing.Image image = qrCodeEncoder.Encode(qrContent);
            System.IO.MemoryStream MStream = new System.IO.MemoryStream();
            image.Save(MStream, System.Drawing.Imaging.ImageFormat.Png);

            System.IO.MemoryStream MStream1 = new System.IO.MemoryStream();
            CombinImage(image, HttpContext.Current.Server.MapPath("/Imgs/logo.png")).Save(MStream1, System.Drawing.Imaging.ImageFormat.Png);
            return MStream1.ToArray();
        }

        /// <summary>
        /// 创建QR
        /// </summary>
        /// <param name="qrContent"></param>
        /// <returns></returns>
        public static byte[] CreateQR(Image image, int qrCodeScale, int qrCodeVersion)
        {
            if (image == null) return null;

            System.IO.MemoryStream MStream = new System.IO.MemoryStream();
            image.Save(MStream, System.Drawing.Imaging.ImageFormat.Png);

            return MStream.ToArray();
        }


        /// <summary>     
        /// 调用此函数后使此两种图片合并，类似相册，有个     
        /// 背景图，中间贴自己的目标图片     
        /// </summary>     
        /// <param name="imgBack">粘贴的源图片</param>     
        /// <param name="destImg">粘贴的目标图片</param>     
        public static Image CombinImage(Image imgBack, string destImg)
        {
            int p_left = 0;
            int p_top = 0;
            int panelWidth = 0;
            int panelHeigh = 0;

            Image img = Image.FromFile(destImg);        //照片图片       
            if (img.Height != 45 || img.Width != 45)
            {
                img = KiResizeImage(img, 45, 45, 0);
            }

            //填充图片加边框
            panelWidth = img.Width + 10;
            panelHeigh = img.Height + 10;
            System.Drawing.Bitmap panel0 = new System.Drawing.Bitmap(panelWidth, panelHeigh);
            Color color = Color.FromArgb(128,191,255);
            for (int i = 0; i < panel0.Height; i++)
            {
                for (int j = 0; j < panel0.Width; j++)
                {
                    panel0.SetPixel(i,j,color);
                }
            }

            System.Drawing.Graphics graphic0 = System.Drawing.Graphics.FromImage(panel0);
            p_left = (panelWidth - img.Width) / 2;
            p_top = (panelWidth - img.Height) / 2;
            graphic0.DrawImage(img, p_left, p_top, img.Width, img.Height);
            img = new System.Drawing.Bitmap(panel0);


            Graphics g = Graphics.FromImage(imgBack);
            g.DrawImage(imgBack, 0, 0, imgBack.Width, imgBack.Height);      //g.DrawImage(imgBack, 0, 0, 相框宽, 相框高);      
            g.FillRectangle(System.Drawing.Brushes.White, imgBack.Width / 2 - img.Width / 2 - 1, imgBack.Width / 2 - img.Width / 2 - 1,1,1);//相片四周刷一层黑色边框     
            g.DrawImage(img, imgBack.Width / 2 - img.Width / 2, imgBack.Width / 2 - img.Width / 2, img.Width, img.Height);
            
            //加外边框
            panelWidth = imgBack.Width + 10;
            panelHeigh = imgBack.Height + 10;
            System.Drawing.Bitmap panel = new System.Drawing.Bitmap(panelWidth, panelWidth);
            System.Drawing.Graphics graphic1 = System.Drawing.Graphics.FromImage(panel);
            p_left = (panelWidth - imgBack.Width) / 2;
            p_top = (panelWidth - imgBack.Height) / 2;
            //将生成的二维码图像粘贴至绘图的中心位置
            graphic1.DrawImage(imgBack, p_left, p_top, imgBack.Width, imgBack.Height);
            imgBack = new System.Drawing.Bitmap(panel);


            GC.Collect();
            return imgBack;
        }


        /// <summary>     
        /// Resize图片     
        /// </summary>     
        /// <param name="bmp">原始Bitmap</param>     
        /// <param name="newW">新的宽度</param>     
        /// <param name="newH">新的高度</param>     
        /// <param name="Mode">保留着，暂时未用</param>     
        /// <returns>处理以后的图片</returns>     
        public static Image KiResizeImage(Image bmp, int newW, int newH, int Mode)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量     
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }  
    }
}
