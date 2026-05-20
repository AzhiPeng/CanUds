using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmApi.Models.Verification
{
    public class GetInfoResponseModel: BaseResponseModel
    {
        //public string errorMessage { get; set; }

        //public string errorResponse { get; set; }

        //public string requestId { get; set; }

        public string fileName { get; set; }

        public string fileContent { get; set; }

        public string status { get; set; }
    }
}
