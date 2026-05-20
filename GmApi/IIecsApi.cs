using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmApi.Models.Verification;
namespace GmApi
{
    public interface IIecsApi
    {
        string ServceName { get; }
        
        //string ServerAddress { get; set; }

        //string CertificatePath { get; set; }

        //string CertificatePassword { get; set; }

        //string VerificationDataPath { get;  }

        CreateInfoResponseModel CreateVerificationRequset();

        GetListResponseModel GetVerificationRequsetList();

        GetInfoResponseModel GetVerificationRequset(string requstId);

        UploadFileResponse UploadVerificationFile(string requstId,string fileData,string fileName);

        Task<Models.DownloadTuple.DownloadTupleResponseModel> GetDownloadTupleRequset(string procutionName,int num);
        Task<Models.DownloadTuple.DownloadTupleResponseModel> GetDownloadTupleFile(string requstId);

    }
}
