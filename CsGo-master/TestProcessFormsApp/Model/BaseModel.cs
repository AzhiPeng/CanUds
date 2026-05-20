using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyManageForGm.Model
{

    //[Serializable]
    public class BaseModel
    {
        //[Key]
        // [Display(Name = "ID", Description = "")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public virtual  int Id { get; set; }

    }
}
