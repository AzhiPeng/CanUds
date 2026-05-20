using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmApi.Models.DownloadTuple;
using GmApi.Models.Verification;
namespace GmApi
{
    public class IecsApi : IIecsApi
    {
        public string ServceName { get; private set; }
        public string ServerAddress { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }

        public string VerificationEcuidPath { get; private set; }
        public string DownloadEcuidPath { get; private set; }


        private IecsApi() { }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName">GM or SGM</param>
        /// <returns></returns>
        public static IIecsApi GetIecsApi(string serviceName)
        {
            var api = new IecsApi();
            api.ServceName = serviceName.ToUpper();
            switch (serviceName.ToUpper())
            {
                case "SGM":
                    api.ServerAddress = "https://apigateway-external.saic-gm.com";
                    api.CertificatePath = "SGM.pfx";
                    api.CertificatePassword = "123456";
                    api.VerificationEcuidPath = "/verify-tuples/v1/tuples/verification/";
                    api.DownloadEcuidPath = "/download-tuples/v1/tuples/";
                    break;
                case "GM":
                    api.ServerAddress = "https://apimmtls.gm.com";
                    api.CertificatePath = "GM.pfx";
                    api.CertificatePassword = "123456";
                    api.VerificationEcuidPath = "/api-0638/verifytuples/v1/tuples/verification/";
                    api.DownloadEcuidPath = "/api-0728/downloadtuples/v1/tuples/";
                    break;
                default:
                    throw new Exception("Unknown serverName.");

            }
            return api;

        }
        public CreateInfoResponseModel CreateVerificationRequset()
        {
            const string method = "POST";
            const string action = "requests";

            var response = default(CreateInfoResponseModel);

            try
            {
                string res = HttpHelper.PostUrl(
                    url: $"{ServerAddress.TrimEnd('/')}/{VerificationEcuidPath.Trim('/')}/{action.Trim('/')}",
                    postData: "",
                    headers: null,
                    method: method,
                    contenttype: "application/json",
                    p12certfile: this.CertificatePath,
                    certpassword: this.CertificatePassword
                    );
                response = Newtonsoft.Json.JsonConvert.DeserializeObject<CreateInfoResponseModel>(res);
            }
            catch (Exception ex) { }


            return response;

        }

        public GetInfoResponseModel GetVerificationRequset(string requstId)
        {
            const string method = "GET";
            const string action = "requests";

            var response = default(GetInfoResponseModel);


            string res = HttpHelper.PostUrl(
                url: $"{ServerAddress.TrimEnd('/')}/{VerificationEcuidPath.Trim('/')}/{action.Trim('/')}/{requstId}",
                postData: "",
                headers: null,
                method: method,
                contenttype: "application/json",
                p12certfile: this.CertificatePath,
                certpassword: this.CertificatePassword,
                out System.Net.HttpWebResponse httpResponse
                );
            switch (httpResponse.ContentType.ToLower())
            {
                case "application/json":
                    response = Newtonsoft.Json.JsonConvert.DeserializeObject<GetInfoResponseModel>(res);
                    break;
                case "application/octet-stream":
                    response = new GetInfoResponseModel()
                    {
                        errorMessage = "",
                        errorResponse = "",
                        status = "success",
                        fileContent = res,
                        requestId = requstId
                    };
                    response.fileName = System.Text.RegularExpressions.Regex.Match(httpResponse.Headers["Content-Disposition"], "filename=\".*\"").Value;
                    if (string.IsNullOrEmpty(response.fileName))
                    {
                        // 没有指定的文件名
                        response.fileName = requstId + ".xml";
                    }
                    else
                    {
                        // 有指定的文件名
                        response.fileName = response.fileName.Remove(0, 9).Trim('\"');
                    }
                    //response.fileName = System.Text.RegularExpressions.Regex.Match(httpResponse.Headers["Content-Disposition"], "filename=\".*\"").Value.Remove(0, 9).Trim('\"');
                    break;
                default:
                    response = null;
                    break;
            }
            return response;
        }

        public GetListResponseModel GetVerificationRequsetList()
        {
            const string method = "GET";
            const string action = "requests";

            var response = default(GetListResponseModel);


            string res = HttpHelper.PostUrl(
                url: $"{ServerAddress.TrimEnd('/')}/{VerificationEcuidPath.Trim('/')}/{action.Trim('/')}",
                postData: "",
                headers: null,
                method: method,
                contenttype: "application/json",
                p12certfile: this.CertificatePath,
                certpassword: this.CertificatePassword
                );
            response = Newtonsoft.Json.JsonConvert.DeserializeObject<GetListResponseModel>(res);

            return response;

        }

        public UploadFileResponse UploadVerificationFile(string requstId, string fileData, string fileName)
        {
            const string method = "POST";
            const string action = "requests";
            //var aa = new Dictionary<string, string>(); aa.Add("_csrf", fileName);
            var response = default(UploadFileResponse);
            string res = HttpHelper.UploadFilesToServer(
                url: $"{ServerAddress.TrimEnd('/')}/{VerificationEcuidPath.Trim('/')}/{action.Trim('/')}/{requstId}/data",
                data: null,
                fileContentType: "text/xml",
                fileName: fileName,
                fileData: fileData,
                p12certfile: this.CertificatePath,
                certpassword: this.CertificatePassword
                );

            //string res = HttpHelper.PostUrl(
            //    url: ServerAddress + Path.Combine(VerificationEcuidPath, action+"/"+requstId+"/data"),
            //    postData: fileData,
            //    headers: null,
            //    method: method,
            //    contenttype: "multipart/form-data",
            //    p12certfile: this.CertificatePath,
            //    certpassword: this.CertificatePassword
            //    );
            response = Newtonsoft.Json.JsonConvert.DeserializeObject<UploadFileResponse>(res);

            return response;
        }

        public async Task< Models.DownloadTuple.DownloadTupleResponseModel> GetDownloadTupleRequset(string procutionName, int num)
        {
            const string method = "POST";
            const string action = "request";

            var response = default(DownloadTupleResponseModel);


            string res = HttpHelper.PostUrl(
                url: $"{ServerAddress.TrimEnd('/')}/{DownloadEcuidPath.Trim('/')}/{action.Trim('/')}",
                postData: $"{{\"product\":\"{procutionName}\", \"nbrEcus\":\"{num}\"}}",
                headers: null,
                method: method,
                contenttype: "application/json",
                p12certfile: this.CertificatePath,
                certpassword: this.CertificatePassword
                );
            response = Newtonsoft.Json.JsonConvert.DeserializeObject<DownloadTupleResponseModel>(res);

            return response;
        }

        public async Task<DownloadTupleResponseModel> GetDownloadTupleFile(string requstId)
        {
            const string method = "GET";
            const string action = "download";

            var response = default(DownloadTupleResponseModel);


            string res = HttpHelper.PostUrl(
                url: $"{ServerAddress.TrimEnd('/')}/{DownloadEcuidPath.Trim('/')}/{action.Trim('/')}/{requstId}",
                postData: "",
                headers: null,
                method: method,
                contenttype: "application/json",
                p12certfile: this.CertificatePath,
                certpassword: this.CertificatePassword
                );
            response = Newtonsoft.Json.JsonConvert.DeserializeObject<DownloadTupleResponseModel>(res);

            return response;

        }
    }
}
