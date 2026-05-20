using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;


internal class HttpHelper
{
    public static string PostUrl(
        string url,
        string postData,
        WebHeaderCollection headers,
        string method,
        string contenttype,
        string p12certfile,
        string certpassword) => PostUrl(
        url,
        postData,
        headers,
        method,
        contenttype,
        p12certfile,
        certpassword,
        out HttpWebResponse httpResponse);

    public static string PostUrl(
        string url,
        string postData,
        WebHeaderCollection headers,
        string method,
        string contenttype,
        string p12certfile,
        string certpassword,
        out HttpWebResponse httpResponse)
    {
        //string url = string.Format("https://apigateway-external.saic-gm.com/verify-tuples/v1/tuples/verification/requests");
        //method = "GET";
        //string url = string.Format($"{serverAddress}/{apiPath}");
        //if (isHttp)
        //{
        //    url = string.Format("http://{0}:{1}{2}", this.m_pltip, this.m_pltPort, apiPath);
        //}
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)2048;

        HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);

        X509Certificate2 cerCaiShang = new X509Certificate2(p12certfile, certpassword);

        httpRequest.ClientCertificates.Add(cerCaiShang);

        httpRequest.Method = method;
        httpRequest.ContentType = contenttype;
        //httpRequest.Referer = null;
        httpRequest.AllowAutoRedirect = true;
        //httpRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.2; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        //httpRequest.Accept = "*/*";
        for (int i = 0; i < headers?.Count; i++)
        {
            for (int j = 0; j < headers?.GetValues(i)?.Length; j++)
            {
                httpRequest.Headers.Add(headers.Keys[i], headers.GetValues(i)[j]);
            }

        }

        //if (isHttp) { httpRequest.ServicePoint.Expect100Continue = false; }
        //
        if (method != "GET")
        {
            Stream requestStem = httpRequest.GetRequestStream();
            byte[] buffer = UTF8Encoding.UTF8.GetBytes(postData);
            requestStem.Write(buffer, 0, buffer.Length);
            requestStem.Close();
            //StreamWriter sw = new StreamWriter(requestStem);
            //sw.Write(postData);
            //sw.Close();
        }
        //HttpWebResponse httpResponse = default;
        Stream receiveStream = default;
        string result = String.Empty;
        try
        {
            httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            receiveStream = httpResponse.GetResponseStream();

            using (StreamReader sr = new StreamReader(receiveStream))
            {
                result = sr.ReadToEnd();
            }
        }
        catch (System.Security.Cryptography.CryptographicException ex)
        {
            throw new Exception("证书错误。error:" + ex.Message);
        }
        catch (WebException ex)
        {
            switch (ex.Status)
            {
                case WebExceptionStatus.ProtocolError:
                    httpResponse = (HttpWebResponse)ex.Response;
                    receiveStream = httpResponse.GetResponseStream();

                    using (StreamReader sr = new StreamReader(receiveStream))
                    {
                        result = sr.ReadToEnd();
                    }
                    switch (httpResponse.StatusCode)
                    {
                        case HttpStatusCode.InternalServerError://500
                            if (result.Contains("{403} Forbidden"))
                            {
                                throw new Exception("认证失败。");
                            }
                            throw new Exception($"服务器返回错误，Error:{httpResponse.StatusCode},body:{result}"); ;

                    }
                    break;
                case WebExceptionStatus.Timeout:
                case WebExceptionStatus.ConnectionClosed:
                case WebExceptionStatus.ConnectFailure:
                case WebExceptionStatus.ProxyNameResolutionFailure:
                case WebExceptionStatus.NameResolutionFailure:
                case WebExceptionStatus.ReceiveFailure:
                    throw new Exception("请求GM服务器失败，请检查网络。Error:" + ex.Status.ToString());
                default:
                    throw new Exception("请求失败。Error:" + ex.Status.ToString()); ;

            }
        }

        return result;
    }
    public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {

        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        return true;

    }
    //private void HttpPost(byte[] file_bytes)
    //{
    //    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.myserver.com/upload.php");
    //    httpWebRequest.ContentType = "multipart/form-data";
    //    httpWebRequest.Method = "POST";
    //    var asyncResult = httpWebRequest.BeginGetRequestStream((ar) => { GetRequestStreamCallback(ar, file_bytes); }, httpWebRequest);
    //}

    //private void GetRequestStreamCallback(IAsyncResult asynchronousResult, byte[] postData)
    //{
    //    //DON'T KNOW HOW TO PASS "userid=some_user_id"  
    //    HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
    //    Stream postStream = request.EndGetRequestStream(asynchronousResult);
    //    postStream.Write(postData, 0, postData.Length);
    //    postStream.Close();
    //    var asyncResult = request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
    //}

    //private void GetResponseCallback(IAsyncResult asynchronousResult)
    //{
    //    HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
    //    HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
    //    Stream streamResponse = response.GetResponseStream();
    //    StreamReader streamRead = new StreamReader(streamResponse);
    //    string responseString = streamRead.ReadToEnd();
    //    streamResponse.Close();
    //    streamRead.Close();
    //    response.Close();
    //}
#if DEBUG
    static string debugTxt = string.Empty;
#endif
    //public static string UploadFilesToServer2(
    //string url,
    //Dictionary<string, string> data,
    //string fileName,
    //string fileContentType,
    //byte[] fileData,
    //string p12certfile,
    //string certpassword)
    //{
    //     using (var form = new MultipartFormDataContent())
    // {
    //     using (var fs = File.OpenRead(filePath))
    //     {
    //         using (var streamContent = new StreamContent(fs))
    //         {
    //             using (var fileContent = new ByteArrayContent(await streamContent.ReadAsByteArrayAsync()))
    //             {
    //                 fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");
    
    //                 // "file" parameter name should be the same as the server side input parameter name
    //                 form.Add(fileContent, "file", Path.GetFileName(filePath));
    //                 HttpResponseMessage response = await httpClient.PostAsync(url, form);
    //             }
    //         }
    //     }
    // }
    //}
    /// <summary>
    /// Creates HTTP POST request & uploads database to server. Author : Farhan Ghumra
    /// </summary>
    public static string UploadFilesToServer(
        string url,
        Dictionary<string, string> data,
        string fileName,
        string fileContentType,
        //byte[] fileData,
        string fileData,
        string p12certfile,
        string certpassword)
    {
        string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
        HttpWebRequest httpRequest = (HttpWebRequest)HttpWebRequest.Create(url);
        ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
        ServicePointManager.SecurityProtocol = (SecurityProtocolType)2048;
        X509Certificate2 cerCaiShang = new X509Certificate2(p12certfile, certpassword);

        httpRequest.ClientCertificates.Add(cerCaiShang);

        httpRequest.ContentType = "multipart/form-data; boundary=" + boundary;
        httpRequest.Method = "POST";
#if DEBUG
        debugTxt += "ContentType:multipart/form-data; boundary=" + boundary+"\r\n";
        debugTxt += "Method:POST" + "\r\n";
#endif
        Stream receiveStream = default;//httpRequest.GetRequestStream();
        try
        {
            using (Stream requestStream = httpRequest.GetRequestStream())
            {
                WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("#######################################################");
           System.Diagnostics.Debug.WriteLine(debugTxt);
            debugTxt=String.Empty;
#endif

            try
            {
                var response = httpRequest.GetResponse();
                var responseStream = response.GetResponseStream();
                using (var sr = new StreamReader(responseStream))
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string responseString = streamReader.ReadToEnd();
                        return responseString;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Writes multi part HTTP POST request. Author : Farhan Ghumra
    /// </summary>
    private static void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, string fileData)
    {
        /// The first boundary
        byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
        /// the last boundary.
        byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
        /// the form data, properly formatted
        string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
        /// the form-data file upload, properly formatted
        string fileheaderTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\";\r\n" +
            "Content-Type: {2}\r\n" +
            //"Content-Transfer-Encoding: base64\r\n" +
            "\r\n";

        /// Added to track if we need a CRLF or not.
        bool bNeedsCRLF = false;

        if (data != null)
        {
            foreach (string key in data.Keys)
            {
                /// if we need to drop a CRLF, do that.
                if (bNeedsCRLF)
                    WriteToStream(s, "\r\n");

#if DEBUG
                debugTxt += "\r\n";
                debugTxt += "--" + boundary + "\r\n";
                debugTxt += string.Format(formdataTemplate, key, data[key]);
#endif

                /// Write the boundary.
                WriteToStream(s, boundarybytes);

                /// Write the key.
                WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                bNeedsCRLF = true;
            }
        }

        /// If we don't have keys, we don't need a crlf.
        if (bNeedsCRLF)
            WriteToStream(s, "\r\n");

        WriteToStream(s, boundarybytes);
        WriteToStream(s, string.Format(fileheaderTemplate, "clientFile", fileName, fileContentType));
        
        /// Write the file data to the stream.
        WriteToStream(s, fileData);
        WriteToStream(s, trailer);
        
#if DEBUG
        debugTxt += "\r\n";
        debugTxt += "--" + boundary + "\r\n";
        debugTxt += string.Format(fileheaderTemplate, "clientFile", fileName, fileContentType);
        debugTxt +=  fileData;
        debugTxt += "\r\n--" + boundary + "--\r\n";
#endif

    }

    /// <summary>
    /// Writes string to stream. Author : Farhan Ghumra
    /// </summary>
    private static void WriteToStream(Stream s, string txt)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(txt);
        s.Write(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// Writes byte array to stream. Author : Farhan Ghumra
    /// </summary>
    private static void WriteToStream(Stream s, byte[] bytes)
    {
        s.Write(bytes, 0, bytes.Length);
    }

}
