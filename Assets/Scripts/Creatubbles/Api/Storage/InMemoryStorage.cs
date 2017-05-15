//
// InMemoryStorage.cs
// CreatubblesApiClient
//
// Copyright (c) 2017 Creatubbles Pte. Ltd.
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
using System.Collections.Generic;
using System.Collections;

namespace Creatubbles.Api.Storage
{
    /// <summary>
    /// In-memory implementation of <see cref="Creatubbles.Api.Storage.ISecureStorage"/>.
    /// </summary>
    /// <remarks>
    /// This storage class IS NOT a secure way of storing sensitive data and should ONLY be used for development and testing.
    /// For production an encrypted perstistent storage should be used instead. At the time of writing (15.11.2016) such secure storage needs to be implemented by the SDK consumer.
    /// </remarks>
    public class InMemoryStorage: ISecureStorage
    {
        private static Dictionary<string, string> store = new Dictionary<string, string>();

        public bool HasValue(string key)
        {
            return store.ContainsKey(key) && store[key] != null;
        }

        public string LoadValue(string key)
        {
            return store[key];
        }

        public void SaveValue(string key, string value)
        {
            store[key] = value;
        }

        public void DeleteValue(string key)
        {
            store.Remove(key);
        }

        public void Clear()
        {
            store.Clear();
        }
    }
}
