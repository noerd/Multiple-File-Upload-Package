using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using umbraco;
using umbraco.BasePages;
using umbraco.BusinessLogic;
using umbraco.BusinessLogic.console;
using umbraco.cms.businesslogic.datatype;
using umbraco.cms.businesslogic.media;

namespace noerd.Umb.DataTypes.multipleFileUpload
{
    /// <summary>
    /// The default Media factory for image files.
    /// </summary>
    public class DefaultImageMediaFactory : MediaFactory
    {
        // -------------------------------------------------------------------------
        // Public members
        // -------------------------------------------------------------------------

        // IMediaFactory Members
        // -------------------------------------------------------------------------

        #region IMediaFactory Members

        public override Media CreateMedia(IconI parent, HttpPostedFile uploadFile)
        {
            string filename = uploadFile.FileName;

            // Create new media object
            Media media = Media.MakeNew(filename, MediaType.GetByAlias("Image"),
                                      new User(0), parent.Id);

            // Get Image object, width and height
            Image image = Image.FromStream(uploadFile.InputStream);
            int fileWidth = image.Width;
            int fileHeight = image.Height;

            // Get umbracoFile property
            int propertyId = media.getProperty("umbracoFile").Id;

            // Get paths
            string relativeDestPath = ConstructRelativeDestPath(propertyId);
            string relativeDestFilePath = VirtualPathUtility.Combine(relativeDestPath, filename);
            string ext = VirtualPathUtility.GetExtension(filename).Substring(1);

            // Set media properties
            SetImageMediaProperties(media, relativeDestFilePath, fileWidth, fileHeight, uploadFile.ContentLength, ext);

            // Create directory
            if (UmbracoSettings.UploadAllowDirectories)
                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(relativeDestPath));

            // Generate thumbnail
            string absoluteDestPath = HttpContext.Current.Server.MapPath(relativeDestPath);
            string absoluteDestFilePath = Path.Combine(absoluteDestPath, Path.GetFileNameWithoutExtension(filename) + "_thumb");
            GenerateThumbnail(image, 100, fileWidth, fileHeight, absoluteDestFilePath + ".jpg");

            // Generate additional thumbnails based on PreValues set in DataTypeDefinition uploadField
            GenerateAdditionalThumbnails(image, fileWidth, fileHeight, absoluteDestFilePath);

            image.Dispose();

            return media;
        }

        // -------------------------------------------------------------------------
        // Private members
        // -------------------------------------------------------------------------

        private static void SetImageMediaProperties(Media media, string file, int fileWidth, int fileHeight, int bytes, string ext)
        {
            media.getProperty("umbracoFile").Value = file;
            media.getProperty("umbracoWidth").Value = fileWidth;
            media.getProperty("umbracoHeight").Value = fileHeight;
            media.getProperty("umbracoBytes").Value = bytes;
            media.getProperty("umbracoExtension").Value = ext;
        }

        // -------------------------------------------------------------------------

        private static void GenerateAdditionalThumbnails(Image image, int fileWidth, int fileHeight, string destFilePath)
        {
            Guid uploadFieldDataTypeId = new Guid("5032a6e6-69e3-491d-bb28-cd31cd11086c");
            // Get DataTypeDefinition of upload field
            DataTypeDefinition dataTypeDef = DataTypeDefinition.GetByDataTypeId(uploadFieldDataTypeId);
            // Get PreValues
            SortedList preValues = PreValues.GetPreValues(dataTypeDef.Id);

            string thumbnails = "";
            if (preValues.Count > 0)
                thumbnails = ((PreValue)preValues[0]).Value;

            if (thumbnails != "")
            {
                string[] thumbnailSizes = thumbnails.Split(";".ToCharArray());
                foreach (string thumb in thumbnailSizes)
                    if (thumb != "")
                    {
                        GenerateThumbnail(image, int.Parse(thumb), fileWidth, fileHeight, destFilePath + "_" + thumb + ".jpg");
                    }
            }
        }

        // -------------------------------------------------------------------------

        private static void GenerateThumbnail(Image image, int maxWidthHeight, int fileWidth, int fileHeight, string thumbnailFileName)
        {
            // Generate thumbnailee
            float fx = (float)fileWidth / maxWidthHeight;
            float fy = (float)fileHeight / maxWidthHeight;

            // must fit in thumbnail size
            float f = Math.Max(fx, fy); //if (f < 1) f = 1;
            int widthTh = (int)Math.Round(fileWidth / f);
            int heightTh = (int)Math.Round(fileHeight / f);

            // fixes for empty width or height
            if (widthTh == 0)
                widthTh = 1;
            if (heightTh == 0)
                heightTh = 1;

            // Create new image with best quality settings
            Bitmap bp = new Bitmap(widthTh, heightTh);
            Graphics g = Graphics.FromImage(bp);
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Copy the old image to the new and resized
            Rectangle rect = new Rectangle(0, 0, widthTh, heightTh);
            g.DrawImage(image, rect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

            // Copy metadata
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo codec = null;
            for (int i = 0; codec == null && i < codecs.Length; i++)
            {
                if (codecs[i].MimeType.Equals("image/jpeg"))
                    codec = codecs[i];
            }

            // Set compresion ratio to 90%
            EncoderParameters ep = new EncoderParameters();
            ep.Param[0] = new EncoderParameter(Encoder.Quality, 90L);

            // Save the new image
            if (codec != null)
            {
                bp.Save(thumbnailFileName, codec, ep);
            }
            else
            {
                // Log error
                Log.Add(LogTypes.Error, new User(0), -1, "Multiple file upload: Can't find appropriate codec");
            }

            bp.Dispose();
            g.Dispose();
        }

        #endregion
    }
}
