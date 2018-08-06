using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WcfPDFService
{

    [DataContract]
   public class PictureFile
    {
        [DataMember]
        public string PictureName { get; set; }
        [DataMember]
        public byte[] PictureStream { get; set; }
    }
}
