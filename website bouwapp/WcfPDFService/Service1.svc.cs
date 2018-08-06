using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Web;
using System.Configuration;

namespace WcfPDFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class PdfService : IPdfService
    {
        public bool UploadImg(PictureFile picture)
        {
            FileStream filestream = null;
            BinaryWriter writer = null;
            string filepath;
            
          /*  try
            {
                 filestream = null;
                    writer = null;
                    filepath = String.Empty;
                    filepath = HttpContext.Current.Server.MapPath(".") + ConfigurationManager.AppSettings["UploadDir"] + picture.PictureName;
                    if (picture.PictureName != string.Empty)
                    {
                        filestream = File.Open(filepath, FileMode.Create);
                        writer = new BinaryWriter(filestream);
                        writer.Write(picture.PictureStream);
                    }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (filestream != null)
                    filestream.Close();
                if (writer != null)
                    writer.Close();
            }*/
            return true;
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
