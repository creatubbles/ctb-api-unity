//
// ISecureStorage.cs
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
using System.Collections;

namespace Creatubbles.Api
{
    /// <summary>
    /// An interface for classes offering secure storage of persistent data like authentication tokens or user credentials.
    /// </summary>
    public interface ISecureStorage
    {
        /// <summary>
        /// Determines if value exists for specified key in the secure storage.
        /// </summary>
        /// <returns><c>true</c> value exists for specified key in the secure storage, otherwise, <c>false</c>.</returns>
        /// <param name="key">Key.</param>
        bool HasValue(string key);

        /// <summary>
        /// Loads value for a specific key from the secure storage.
        /// </summary>
        /// <returns>The value.</returns>
        /// <param name="key">Key.</param>
        string LoadValue(string key);

        /// <summary>
        /// Saves value for a specific key in the secure storage.
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        void SaveValue(string key, string value);

        /// <summary>
        /// Deletes a value for a specific key from the secure storage.
        /// </summary>
        /// <param name="key">Key.</param>
        void DeleteValue(string key);

        /// <summary>
        /// Removes all values from the secure storage.
        /// </summary>
        void Clear();
    }
}

