﻿//
// CreatubblesApiClientConfiguration.cs
// CreatubblesApiClient
//
// Copyright (c) 2016 Creatubbles Pte. Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
using System;
using Creatubbles.Api;

public class CreatubblesConfiguration: IApiConfiguration
{
	/*
        API base url. Should be:
        - "https://api.staging.creatubbles.com" for Debug builds
        - "https://api.creatubbles.com" for Release builds
	*/
    public string BaseUrl { get { return "https://api.staging.creatubbles.com"; } }

	/*
        Personal application identifier. Please contact support@creatubbles.com to obtain it.
    */
    public string AppId { get { return SecretData.AppId; } }

	/*
        Personal application secret. Please contact support@creatubbles.com to obtain it.
    */
    public string AppSecret { get { return SecretData.AppSecret; } }

	/*
        API version string.
    */
    public string ApiVersion { get { return "v2"; } }

	/*
        Locale code used for getting localized responses from servers. Example values: “en”, “pl”, “de”.
        Can be null.
        See: https://partners.creatubbles.com/api/#locales for details
    */
    public string Locale { get { return "en"; } }
}