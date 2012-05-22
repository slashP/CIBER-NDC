/*!
 * Face.com Rest API C# Library v1.0.2
 * http://face.com/
 *
 * Copyright 2010, 
 * Written By Daren Willman
 *  
 * Date: Friday July 10
 * 
 * Note: XML Serialization not currently supported.
 *
 * v1.0.1 - add facebook auth to constrcutor
 * v1.0.2 - add attributes param

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.



 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace CiberNdc.Util
{
    public class FaceRestApi
    {
        private readonly string _apiKey;
        private readonly string _apiSecret;
        private readonly string _password;
        private readonly string _format;
        private const string ApiServer = "http://api.face.com/";

        private readonly Dictionary<string, string> _userAuth = new Dictionary<string, string>();
        public FaceRestApi()
        {
        }


        public FaceRestApi(string apiKey, string apiSecret, string password, bool getRawData, string format, string fbUser, string fbOauthToken)
        {
            this._apiKey = apiKey;
            this._apiSecret = apiSecret;
            this._password = password;

            _userAuth.Add("fb_user", fbUser);
            _userAuth.Add("fb_oauth_token", fbOauthToken);

            if (!getRawData)
            {
                this._format = "json";
            }
            else
            {
                this._format = format;
            }
        }

        // *************
        // Account Methods
        // *************
        public FaceApi AccountAuthenticate()
        {
            var d = new Dictionary<string, string>();
            return this.CallMethod("account/authenticate", d);
        }

        public FaceApi AccountUsers(List<string> ns)
        {
            var list = PrepLists(ns);
            var dict = new Dictionary<string, string> {{"namespaces", list[0]}};
            return CallMethod("account/users", dict);
        }

        public FaceApi AccountLimits()
        {
            var d = new Dictionary<string, string>();
            return CallMethod("account/limits", d);
        }


        // *************
        // Face Methods
        // *************
        public FaceApi FacesDetect(List<string> urls, string filename, List<string> ownerIds, List<string> attributes, string callBackUrl)
        {
            var list = this.PrepLists(urls, ownerIds, attributes);
            var dict = new Dictionary<string, string>
                           {
                               {"urls", list[0]},
                               {"owner_ids", list[1]},
                               {"attributes", list[2]},
                               {"_file", "@" + filename},
                               {"callback_url", callBackUrl}
                           };

            return CallMethod("faces/detect", dict);
        }

        public FaceApi FacesTrain(List<string> uids, string ns, string callbackUrl)
        {
            var list = PrepLists(uids);
            var dict = new Dictionary<string, string>
                           {{"uids", list[0]}, {"namespace", ns}, {"callback_url", callbackUrl}};

            return CallMethod("faces/train", dict);
        }

        public FaceApi FacesRecognize(List<string> urls, List<string> uids, string ns, string train, string filename, List<string> ownerIds, List<string> attributes, string callbackUrl)
        {
            var list = PrepLists(urls, uids, ownerIds, attributes);
            var dict = new Dictionary<string, string>
                           {
                               {"urls", list[0]},
                               {"uids", list[1]},
                               {"namespace", ns},
                               {"train", train},
                               {"owner_ids", list[2]},
                               {"attributes", list[3]},
                               {"_file", "@" + filename},
                               {"callback_url", callbackUrl}
                           };

            return CallMethod("faces/recognize", dict);

        }

        public FaceApi FacesStatus(List<string> uids, string ns)
        {
            var list = PrepLists(uids);
            var dict = new Dictionary<string, string>();

            dict.Add("uids", list[0]);
            dict.Add("namespace", ns);
            return CallMethod("faces/status", dict);
        }

        // ************
        // Tags Methods
        // ************
        public FaceApi TagsAdd(string url, string x, string y, string width, string height, string label, string uid, string pid, string taggerId, string ownerId)
        {
            var dict = new Dictionary<string, string>
                           {
                               {"url", url},
                               {"x", x},
                               {"y", y},
                               {"width", width},
                               {"height", height},
                               {"label", label},
                               {"uid", uid},
                               {"pid", pid},
                               {"tagger_id", taggerId},
                               {"owner_id", ownerId}
                           };
            return CallMethod("tags/add", dict);
        }

        public FaceApi TagsSave(List<string> tids, string uid, string label, string taggerId)
        {
            var list = PrepLists(tids);
            var dict = new Dictionary<string, string>
                           {{"tids", list[0]}, {"label", label}, {"uid", uid}, {"tagger_id", taggerId}};

            return CallMethod("tags/save", dict);
        }

        public FaceApi TagsRemove(List<string> tids, string taggerId)
        {
            var list = PrepLists(tids);
            var dict = new Dictionary<string, string> {{"tids", list[0]}, {"tagger_id", taggerId}};

            return CallMethod("tags/remove", dict);
        }

        public FaceApi TagsGet(List<string> urls, List<string> pids, string filename, List<string> ownerIds, List<string> uids, string ns, string filter, string limit, string together, string order)
        {
            var list = this.PrepLists(urls, pids, ownerIds, uids);
            var dict = new Dictionary<string, string>
                           {
                               {"urls", list[0]},
                               {"pids", list[1]},
                               {"owner_ids", list[2]},
                               {"_file", filename},
                               {"uids", list[3]},
                               {"together", together},
                               {"filter", filter},
                               {"order", order},
                               {"limit", limit},
                               {"namespace", ns}
                           };

            return CallMethod("tags/get", dict);
        }

        // ************
        // Facebook Methods
        // ************
        public FaceApi FacebookGet(List<string> uids, string filter, string limit, string together, string order)
        {
            var list = PrepLists(uids);
            var dict = new Dictionary<string, string>
                           {
                               {"uids", list[0]},
                               {"limit", limit},
                               {"together", together},
                               {"filter", filter},
                               {"order", order}
                           };

            return CallMethod("facebook/get", dict);
        }


        protected FaceApi CallMethod(string method, Dictionary<string, string> param)
        {
            // Remember keys for removal
            var keys = new List<string>();

            foreach (var s in param)
            {
                if (String.IsNullOrWhiteSpace(s.Value))
                {
                    keys.Add(s.Key);
                }
                else
                {
                    if (s.Key == "_file" && s.Value == "@")
                    {
                        keys.Add(s.Key);
                    }
                }
            }
            foreach (string s in keys)
            {
                param.Remove(s);
            }


            var authParams = new Dictionary<string, string>();

            if (!String.IsNullOrWhiteSpace(this._apiKey))
            {
                authParams.Add("api_key", this._apiKey);
            }

            if (!String.IsNullOrWhiteSpace(this._apiSecret))
            {
                authParams.Add("api_secret", this._apiSecret);
            }

            if (_userAuth.Count > 0)
            {
                authParams.Add("user_auth", getUserAuthString(this._userAuth));
            }

            if (!String.IsNullOrWhiteSpace(_password))
            {
                authParams.Add("password", _password);
            }


            var paramMerge = authParams.Union(param).ToDictionary(pair => pair.Key, pair => pair.Value);

            var request = method + "." + _format;

            return PostMethod(request, paramMerge);
        }

        private static string HttpBuildQuery(ICollection<KeyValuePair<string, string>> param)
        {
            var strRetVal = "";

            if (param.Count > 0) {
                strRetVal = param.Aggregate(strRetVal, (current, s) => current + (s.Key + "=" + s.Value + "&"));
                strRetVal = strRetVal.Substring(0, strRetVal.Length - 1);
            }

            return strRetVal;
        }

        private string getUserAuthString(Dictionary<string, string> userAuthReturn)
        {
            string strRetVal = "";

            if (userAuthReturn.Count > 0) {
                strRetVal = userAuthReturn.Aggregate(strRetVal, (current, s) => current + (s.Key + ":" + s.Value + ","));
                strRetVal = strRetVal.Substring(0, strRetVal.Length - 1);
            }

            return strRetVal;
        }

        private FaceApi PostMethod(string request, IDictionary<string, string> param)
        {
            string result;
            var url = ApiServer + request;
            var paramQS = HttpBuildQuery(param);

            var req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "POST";

            string filename;
            if (param.TryGetValue("_file", out filename))
            {
                string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                req.ContentType = "multipart/form-data; boundary=" + boundary;
                req.KeepAlive = true;

                req.Credentials = System.Net.CredentialCache.DefaultCredentials;



                var memStream = new MemoryStream();
                var boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");


                var formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

                foreach (var formitembytes in param.Select(s => string.Format(formdataTemplate, s.Key, s.Value)).Select(formitem => System.Text.Encoding.UTF8.GetBytes(formitem))) {
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }
                memStream.Write(boundarybytes, 0, boundarybytes.Length);

                const string headerTemplate = "Content-Disposition: form-data; name=\"_file\"; filename=\"_files\"\r\n Content-Type: application/octet-stream\r\n\r\n";

                var header = string.Format(headerTemplate, "file1", filename);
                var headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                memStream.Write(headerbytes, 0, headerbytes.Length);

                var fileStream = new FileStream(filename.Replace("@", ""), FileMode.Open,
                                                       FileAccess.Read);
                var buffer = new byte[1024];

                int bytesRead;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }


                memStream.Write(boundarybytes, 0, boundarybytes.Length);


                fileStream.Close();

                req.ContentLength = memStream.Length;
                var requestStream = req.GetRequestStream();

                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();


                var response = (HttpWebResponse)req.GetResponse();
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();

                response.Close();
                reader.Close();
                reader.Dispose();
            }
            else
            {
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = paramQS.Length;

                req.GetRequestStream().Write(Encoding.UTF8.GetBytes(paramQS), 0, paramQS.Length);
                req.GetRequestStream().Close();
                var response = (HttpWebResponse)req.GetResponse();
                var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
                response.Close();
                reader.Close();
                reader.Dispose();
            }

            FaceApi fd = null;
            if (_format == "json")
            {
                var jss = new JavaScriptSerializer();
                fd = jss.Deserialize<FaceApi>(result);
                fd.RawData = result;
            }
            if (_format == "xml")
            {
                //fd = DeserializeObject<FaceRestAPI.FaceAPI>(result);
                fd = new FaceApi {RawData = result};
            }
            return fd;
        }

        protected List<string> PrepLists(params object[] obj)
        {
            var list = new List<string>();

            foreach (var o in obj)
            {
                var l = (List<string>)o;

                var str = "";
                if (l != null)
                {
                    str = l.Aggregate(str, (current, s) => current + (s + ","));
                    if (str.Length > 0)
                    {
                        str = str.Substring(0, str.Length - 1);
                    }
                    list.Add(str);
                }
                else
                {
                    list.Add(str);
                }
            }
            return list;
        }


        // Face API Classes
        [Serializable()]
        public class FaceApi
        {
            public List<Photo> Photos { get; set; }

            public List<UserStatus> UserStatuses { get; set; }

            public string Status { get; set; }

            public string Authenticated { get; set; }

            public Usage Usage { get; set; }

            public string ErrorCode { get; set; }

            public string ErrorMessage { get; set; }

            public Dictionary<string, List<string>> Users { get; set; }

            public string RawData { get; set; }
        }

        public class Usage
        {
            public string Used { get; set; }

            public string Remaining { get; set; }

            public string Limit { get; set; }

            public string ResetTimeText { get; set; }

            public string ResetTime { get; set; }

            public string NamespaceUsed { get; set; }

            public string NamespaceRemaining { get; set; }

            public string NamespaceLimit { get; set; }
        }

        public class Photo
        {
            public string Url { get; set; }

            public string Pid { get; set; }

            public string Width { get; set; }

            public string Height { get; set; }

            public List<Tag> Tags { get; set; }

            public string ErrorCode { get; set; }

            public string ErrorMessage { get; set; }
        }

        public class Tag
        {
            public string tid { get; set; }

            public string threshold { get; set; }

            public List<UID> uids { get; set; }

            public string gid { get; set; }

            public string label { get; set; }

            public bool confirmed { get; set; }

            public bool manual { get; set; }

            public string tagger_id { get; set; }

            public float width { get; set; }

            public float height { get; set; }

            public Point center { get; set; }

            public Point eye_left { get; set; }

            public Point eye_right { get; set; }

            public Point mouth_left { get; set; }

            public Point mouth_right { get; set; }

            public Point mouth_center { get; set; }

            public Point nose { get; set; }

            public Point ear_left { get; set; }

            public Point ear_right { get; set; }

            public Point chin { get; set; }

            public string yaw { get; set; }

            public string roll { get; set; }

            public string pitch { get; set; }

            public Dictionary<string, Confidence> attributes { get; set; }

        }

        public class Point
        {
            public float x { get; set; }

            public float y { get; set; }
        }

        public class Confidence
        {
            public string value { get; set; }

            public float confidence { get; set; }
        }

        public class UID
        {
            public string uid { get; set; }

            public float confidence { get; set; }
        }

        public class Attributes
        {
            public Confidence face { get; set; }

            public Confidence gender { get; set; }

            public Confidence glasses { get; set; }

            public Confidence smiling { get; set; }
        }

        public class UserStatus
        {
            public string uid { get; set; }

            public string training_set_size { get; set; }

            public string last_trained { get; set; }

            public string training_in_progress { get; set; }
        }
    }
}

