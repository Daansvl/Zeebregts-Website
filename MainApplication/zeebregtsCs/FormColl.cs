using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace zeebregtsCs
{
    public  class FormsCollection : System.Collections.CollectionBase
    {

        public  Form Add(Form FormObject)
        {
            base.List.Add(FormObject);
            return (FormObject);
        }
        public void Remove(Form FormObject)
        {
            if(this.List.Contains(FormObject))
            {
             base.List.Remove(FormObject); 
            }
            
        }
        
    }
}
