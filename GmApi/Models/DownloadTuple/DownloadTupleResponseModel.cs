using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmApi.Models.DownloadTuple
{
    public  class DownloadTupleResponseModel
    {
        public string RequestId { get; set; }

        public string DownloadFile { get; set; }

        public string ErrorType { get; set; }

        public string FileName { get; set; }

        public string ErrorMessage { get; set; }

        public string Status { get; set; }

        public string SupplierName { get; set; }


    }
}
