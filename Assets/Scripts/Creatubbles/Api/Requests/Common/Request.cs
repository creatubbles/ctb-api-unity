//
//  Request.cs
//  Creatubbles API Client Unity SDK
//
//  Copyright (c) 2017 Creatubbles Pte. Ltd.
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Creatubbles.Api.Requests
{
    public class RequestError
    {
        public string Message { get; set; }
        public Request.ErrorType Type { get; set; }

        public RequestError(string message, Request.ErrorType type)
        {
            Message = message;
            Type = type;
        }

        override public string ToString()
        {
            return Message;
        }
    }

    public enum HttpMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class Request
    {
        public enum ErrorType
        {
            Http, Parsing
        }

        public enum AuthorizationType
        {
            None, Public, Private
        }

        public bool IsDone { get { return www != null ? www.isDone : false; } }
        public bool IsError { get { return Errors.Any(); } }
        public readonly List<RequestError> Errors;
        public string TextResponse { get { return www != null ? www.text : string.Empty; } }
        public byte[] BinaryResponse { get { return www != null ? www.bytes : new byte[0]; } }
        public float DownloadProgress { get { return www != null ? www.progress : 0; } }
        public float UploadProgress { get { return www != null ? www.uploadProgress : 0; } } 
        public Dictionary<string, string> ResponseHeaders { get { return www != null ? www.responseHeaders : new Dictionary<string, string>(); } }

        public AuthorizationType Authorization { get; protected set; }
        public HttpMethod Method { get; protected set; }
        public string Path { get; protected set; }
        public string AbsoluteUrl { get; protected set; }
        public bool IgnoreHeaders = false;

        public Dictionary<string, string> Headers { get; private set; }
        public WWWForm Form { get; private set; }

        private WWW www;

        public Request()
        {
            Headers = new Dictionary<string, string>();
            Form = new WWWForm();
            Errors = new List<RequestError>();
        }

        public bool HasAnyHeaders()
        {
            return Headers.Keys.Any();
        }

        public void SetHeader(string key, string value)
        {
            Headers[key] = value;
        }

        public void AddField(string name, string contents)
        {
            Form.AddField(name, contents);
        }

        public void AddFieldIfNotNull(string name, string contents)
        {
            if (contents == null)
            {
                return;
            }

            AddField(name, contents);
        }

        public void AddBinaryData(string name, byte[] contents, string filename, string mimeType)
        {
            Form.AddBinaryData(name, contents, filename, mimeType);
        }

        public void SetBaseRequest(WWW baseRequest)
        {
            www = baseRequest;
        }

        virtual public void HandleResponse()
        {
            if (!string.IsNullOrEmpty(www.error))
            {
                Errors.Add(new RequestError(www.error, ErrorType.Http));
            }
        }
    }
}
