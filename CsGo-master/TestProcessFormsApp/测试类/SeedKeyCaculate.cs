using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
namespace TestProcessFormsApp.测试类
{
    public class SeedKeyCaculate
    {
        private static string TokenStr = string.Empty;
        private static string CookiesStr = string.Empty;
        public enum IECS_Result
        {
            Success,
            AgentKeyNotAuthorized,
            SeedNotValid,
            InvalidRequestCheckYourParams,
            ResponseFormatError,
            ResponseTimeOut = 253,
            ExceptionError,
            UnknownError,
            MACResponseFailure = 1002,
            MACInvalidCertificate,
            MACCallerInternetError = 1007,
            MACSystemError,
            MACGeneralError = 1100,
            MACOpenCerError,
            MACFindCerError,
            MACNetworkConnectivity,
            MACIECSConnectivitr,
            MACOIECSTimeoutFailure
        }
        private static string InternalURL = "http://rds.apps.saic-gm.com:8080";
        private static bool IsInternalNetwork = true;
        private static string ExternalURL = "https://rds.apps.saic-gm.com";
        private static string ConvertMD5(string EncryptionString)
        {
            string arg_05_0 = string.Empty;
            byte[] result = Encoding.Default.GetBytes(EncryptionString.Trim());
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(result)).Replace("-", "");
        }
        protected static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
        private static string Get32ByteConnectError(string status, ref IECS_Result returnStatus)
        {
            if (status.Equals("1"))
            {
                returnStatus = IECS_Result.AgentKeyNotAuthorized;
                return "AgentKey Not Authorized.";
            }
            if (status.Equals("2"))
            {
                returnStatus = IECS_Result.SeedNotValid;
                return "Seed Not Valid.";
            }
            if (status.Equals("3"))
            {
                returnStatus = IECS_Result.InvalidRequestCheckYourParams;
                return "Invalid Request, Check your params";
            }
            returnStatus = IECS_Result.UnknownError;
            return "Unknown Error, Error Code =" + status;
        }
        private static string Base64Decryption(string EncryptionString)
        {
            string arg_05_0 = string.Empty;
            byte[] result = Convert.FromBase64String(EncryptionString);
            return Encoding.Default.GetString(result);
        }
        private static IECS_Result GetIECSToken()
        {
            string ErrorStr = string.Empty;
            IECS_Result result;
            try
            {
                string serviceAddress = InternalURL;
                if (!IsInternalNetwork)
                {
                    serviceAddress = ExternalURL;
                }
                serviceAddress += "/rds/interface/getTokenFromNorthUS";
                string enStr = ConvertMD5("DPS_P@SS").Replace("\"", "\\\"").ToLower();
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest expr_60 = (HttpWebRequest)WebRequest.Create(serviceAddress);
                expr_60.UseDefaultCredentials = true;
                expr_60.Proxy.Credentials = CredentialCache.DefaultCredentials;
                expr_60.Timeout = 6000;
                expr_60.Method = "POST";
                expr_60.ContentType = "application/json";
                expr_60.Headers.Add("agentkey", enStr);
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)4032;
                HttpWebResponse expr_BD = (HttpWebResponse)expr_60.GetResponse();
                string encoding = expr_BD.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                string retString = new StreamReader(expr_BD.GetResponseStream(), Encoding.GetEncoding(encoding)).ReadToEnd();
                if (retString.Split(new char[]
                {
                    ','
                }).Length < 2)
                {
                    ErrorStr = "Get IECS Token failure: IECS Response format Error:" + retString;
                    result = IECS_Result.ResponseFormatError;
                }
                else if (retString.Split(new char[]
                {
                    ','
                })[1].Split(new char[]
                {
                    ':'
                }).Length < 2)
                {
                    ErrorStr = "Get IECS Token failure: IECS Response format Error:" + retString;
                    result = IECS_Result.ResponseFormatError;
                }
                else if (retString.Split(new char[]
                {
                    ','
                })[0].Split(new char[]
                {
                    ':'
                }).Length < 2)
                {
                    ErrorStr = "Get IECS Token failure: IECS Response format Error:" + retString;
                    result = IECS_Result.ResponseFormatError;
                }
                else
                {
                    string status = retString.Split(new char[]
                    {
                        ','
                    })[0].Split(new char[]
                    {
                        ':'
                    })[1];
                    if (!status.Equals("0"))
                    {
                        IECS_Result retureStatu = IECS_Result.ResponseFormatError;
                        ErrorStr = "Get IECS Token failure: " + Get32ByteConnectError(status, ref retureStatu);
                        result = retureStatu;
                    }
                    else
                    {
                        string jsonKey = retString.Split(new char[]
                        {
                            ','
                        })[1].Split(new char[]
                        {
                            ':'
                        })[1];
                        string[] tokenArray = Base64Decryption(jsonKey.Substring(1, jsonKey.Length - 3)).Split(new char[]
                        {
                            ',',
                            ':',
                            '{',
                            '}'
                        }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokenArray.Length != 7)
                        {
                            ErrorStr = "Get IECS Token failure: IECS Response format Error:" + retString;
                            result = IECS_Result.ResponseFormatError;
                        }
                        else
                        {
                            CookiesStr = tokenArray[1] + "," + tokenArray[2];
                            TokenStr = tokenArray[6];
                            result = IECS_Result.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    if (((WebException)ex).Status == WebExceptionStatus.Timeout)
                    {
                        ErrorStr = "Get IECS Token failure: Connect IECS server time out, " + ex.ToString();
                        result = IECS_Result.ResponseTimeOut;
                    }
                    else
                    {
                        ErrorStr = "Get IECS Token failure: Application exception Error, Status:" + ((WebException)ex).Status.ToString() + "; Detail information:" + ex.ToString();
                        result = IECS_Result.ExceptionError;
                    }
                }
                else
                {
                    ErrorStr = "Get IECS Token failure: Application exception Error, " + ex.ToString();
                    result = IECS_Result.ExceptionError;
                }
            }
            return result;
        }
        public static IECS_Result ConnectIECS()
        {
            string arg_05_0 = string.Empty;
            IECS_Result result = GetIECSToken();
            if (result != IECS_Result.Success)
            {
                if (IsInternalNetwork)
                {
                    Console.WriteLine("Get IECS Token failure thru internal network!");
                }
                else
                {
                    Console.WriteLine("Get IECS Token failure thru external network!");
                }
            }
            else if (IsInternalNetwork)
            {
                Console.WriteLine("Get IECS Token success thru internal network!");
            }
            else
            {
                Console.WriteLine("Get IECS Token success thru external network!");
            }
            if (result == IECS_Result.Success)
            {
                return result;
            }
            IsInternalNetwork = !IsInternalNetwork;
            result = GetIECSToken();
            if (result == IECS_Result.ExceptionError)
            {
                result = GetIECSToken();
            }
            if (result != IECS_Result.Success)
            {
                if (IsInternalNetwork)
                {
                    Console.WriteLine("Get IECS Token failure thru internal network!");
                }
                else
                {
                    Console.WriteLine("Get IECS Token failure thru external network!");
                }
            }
            else if (IsInternalNetwork)
            {
                Console.WriteLine("Get IECS Token success thru internal network!");
            }
            else
            {
                Console.WriteLine("Get IECS Token success thru external network!");
            }
            return result;
        }
        private static string GetRadomVIN()
        {
            return "LSGEN83L0JA" + new Random().Next(100000, 999999).ToString();
        }
        private static string Base64Encryption(string OriginalString)
        {
            string arg_05_0 = string.Empty;
            return Convert.ToBase64String(Encoding.Default.GetBytes(OriginalString.Trim()));
        }
        public static Dictionary<string, string> JsonToDictionary(string p)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            p = p.Replace("{", "").Replace("}", "");
            string[] reqarray = p.Split(new string[]
            {
                "\","
            }, StringSplitOptions.None);
            for (int i = 0; i < reqarray.Length; i++)
            {
                dic.Add(reqarray[i].Split(new string[]
                {
                    "\":"
                }, StringSplitOptions.None)[0].Replace("\"", ""), reqarray[i].Split(new string[]
                {
                    "\":"
                }, StringSplitOptions.None)[1].Replace("\"", ""));
            }
            return dic;
        }
        public static List<byte> strToHexByte(string hexString)
        {
            string phexString = hexString.Replace(" ", "");
            if (phexString.Length % 2 != 0)
            {
                phexString = "0" + phexString;
            }
            List<byte> returnBytes = new List<byte>();
            for (int i = 0; i < phexString.Length / 2; i++)
            {
                string arg_45_0 = phexString.Substring(i * 2, 2);
                byte tempByte = 0;
                byte.TryParse(arg_45_0, NumberStyles.HexNumber, null, out tempByte);
                returnBytes.Add(tempByte);
            }
            return returnBytes;
        }
        public static IECS_Result GMAndPatac32ByteCaculate(string vin, byte ecuSourseID, string SAlevel, bool isGM32Byte, ref uint ulSeedkeySize, byte[] pSeedKey, ref string ErrorStr)
        {
            if (string.IsNullOrEmpty(TokenStr) || string.IsNullOrEmpty(CookiesStr))
            {
                IECS_Result getTokenResult = ConnectIECS();
                if (getTokenResult != IECS_Result.Success)
                {
                    ErrorStr = "Get IECS Token Error.";
                    return getTokenResult;
                }
            }
            IECS_Result result;
            try
            {
                ErrorStr = string.Empty;
                string serviceAddress = InternalURL;
                if (!IsInternalNetwork)
                {
                    serviceAddress = ExternalURL;
                }
                serviceAddress += "/rds/interface/unlockByNorthUS";
                string enStr = ConvertMD5("DPS_P@SS").Replace("\"", "\\\"").ToLower();
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceAddress);
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = CredentialCache.DefaultCredentials;
                request.Timeout = 13000;
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("agentkey", enStr);
                string VIN = string.Empty;
                if (string.IsNullOrEmpty(vin))
                {
                    VIN = GetRadomVIN();
                }
                else
                {
                    VIN = vin;
                }
                string DEVICEID = ecuSourseID.ToString("X2");
                string CALCULATIONTYPE = "cb";
                string TYPE = "gm";
                if (!isGM32Byte)
                {
                    TYPE = "gm";
                    CALCULATIONTYPE = "cb";
                }
                string Seed;
                if (string.IsNullOrEmpty(SAlevel) || SAlevel.Length != 2)
                {
                    Seed = "01";
                }
                else
                {
                    Seed = SAlevel;
                }
                int i = 0;
                while ((long)i < (long)((ulong)ulSeedkeySize))
                {
                    Seed += pSeedKey[i].ToString("X2");
                    i++;
                }
                Console.WriteLine("seed = " + Seed);
                string Algorithmindex = "00";
                string param = string.Concat(new string[]
                {
                    "{\"token\":",
                    TokenStr,
                    ",\"calculationType\":\"",
                    CALCULATIONTYPE,
                    "\",\"algorithmIndex\":\"",
                    Algorithmindex,
                    "\",\"seed\":\"",
                    Seed,
                    "\",\"vin\":\"",
                    VIN,
                    "\",\"type\":\"",
                    TYPE,
                    "\",\"deviceId\":\"",
                    DEVICEID,
                    "\",\"cookies\":",
                    CookiesStr,
                    "}"
                });
                Console.WriteLine("param = " + param);
                string json = Base64Encryption(param);
                string resultParam = "{\"VAL\":\"" + json + "\"}";
                using (StreamWriter dataStream = new StreamWriter(request.GetRequestStream()))
                {
                    dataStream.Write(resultParam);
                    dataStream.Close();
                }
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)4032;
                HttpWebResponse expr_285 = (HttpWebResponse)request.GetResponse();
                string encoding = expr_285.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8";
                }
                string retString = new StreamReader(expr_285.GetResponseStream(), Encoding.GetEncoding(encoding)).ReadToEnd();
                JsonToDictionary(retString);
                Console.WriteLine("retString = " + retString);
                string[] retArray = retString.Split(new char[]
                {
                    ',',
                    ':',
                    '"'
                }, StringSplitOptions.RemoveEmptyEntries);
                if (retArray.Length == 0)
                {
                    ErrorStr = "IECS server connect time out" + retString;
                    result = IECS_Result.ResponseTimeOut;
                }
                else if (retArray.Length != 6)
                {
                    ErrorStr = "IECS Response Error:" + retString;
                    result = IECS_Result.ResponseFormatError;
                }
                else
                {
                    string status = retArray[2];
                    string key = retArray[4];
                    if (!status.Equals("0"))
                    {
                        IECS_Result retureStatu = IECS_Result.ResponseFormatError;
                        ErrorStr = Get32ByteConnectError(status, ref retureStatu);
                        result = retureStatu;
                    }
                    else
                    {
                        List<byte> keyList = strToHexByte(Base64Decryption(key));
                        if (keyList.Count < 5)
                        {
                            ErrorStr = "Get a invalid Key." + retString;
                            result = IECS_Result.UnknownError;
                        }
                        else
                        {
                            for (int j = 0; j < keyList.Count; j++)
                            {
                                pSeedKey[j] = keyList[j];
                            }
                            if ((long)keyList.Count != (long)((ulong)ulSeedkeySize) && ulSeedkeySize == 31u && keyList.Count != 12)
                            {
                                result = IECS_Result.SeedNotValid;
                            }
                            else
                            {
                                ulSeedkeySize = (uint)keyList.Count;
                                result = IECS_Result.Success;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    if (((WebException)ex).Status == WebExceptionStatus.Timeout)
                    {
                        ErrorStr = "Connect IECS server time out:" + ex.ToString();
                        Console.WriteLine(ex.ToString());
                        result = IECS_Result.ResponseTimeOut;
                    }
                    else
                    {
                        ErrorStr = "Application exception Error, Status:" + ((WebException)ex).Status.ToString() + "; Detail information:" + ex.ToString();
                        Console.WriteLine(ex.ToString());
                        result = IECS_Result.ExceptionError;
                    }
                }
                else
                {
                    ErrorStr = "Application exception Error:" + ex.ToString();
                    Console.WriteLine(ex.ToString());
                    result = IECS_Result.ExceptionError;
                }
            }
            return result;
        }
    }

}

