using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoneMapORM
{
    /// <summary>
    /// Entity
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// id
        /// </summary>
        private int id;

        /// <summary>
        /// id
        /// </summary>
        public virtual int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }
    }

   


}
