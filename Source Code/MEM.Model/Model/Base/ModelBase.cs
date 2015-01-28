using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MEM.Domain.Model
{
    [Serializable]
    public class ModelBase
    {

        public ModelBase()
        {
            IsActive = true;
        }

        [DataMember]
        public virtual int Id { get; set; }

        [DataMember]
        public virtual DateTime? CreatedDate { get; set; }
        [DataMember]
        public virtual Guid? CreatedBy { get; set; }
        [DataMember]
        public virtual DateTime? ModifiedDate { get; set; }
        [DataMember]
        public virtual Guid? ModifiedBy { get; set; }
        [DataMember]
        
        public virtual bool IsActive { get; set; }
    }
}
