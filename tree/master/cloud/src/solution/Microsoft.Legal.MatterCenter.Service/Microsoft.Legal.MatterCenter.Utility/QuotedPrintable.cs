// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.Utility
// Author           : v-diajme
// Created          : 07-02-2014
//
// ***********************************************************************
// <copyright file="QuotedPrintable.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file defines common function used for upload functionality.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.Utility
{
    #region using
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    #endregion

    /// <summary>
    /// The QuotedPrintable class encodes and decodes strings and files
    /// that either were encoded or need encoded in the Quoted-Printable
    /// MIME encoding for Internet mail. The encoding methods of the class
    /// use pointers wherever possible to guarantee the fastest possible 
    /// encoding times for any size file or string. The decoding methods 
    /// use only the .NET framework classes.
    /// </summary>
    public sealed class QuotedPrintable
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="QuotedPrintable"/> class from being created.
        /// </summary>
        private QuotedPrintable()
        {
        }

        /// <summary>
        /// Generate the hexadecimal code which needs to be replace in string when match found.
        /// </summary>
        /// <param name="matchValue">matching value</param>
        /// <returns>Hex decoded string</returns>
        public static string HexDecoderEvaluator(Match matchValue)
        {
            string result = string.Empty;
            if (matchValue != null && 2 < matchValue.Groups.Count)
            {
                string hex = matchValue.Groups[2].Value;
                int intEquivalent = Convert.ToInt32(hex, 16);
                char hexChar = (char)intEquivalent;
                result = Convert.ToString(hexChar, CultureInfo.InvariantCulture);
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Decode the string and generate the hexadecimal code for the input string.
        /// </summary>
        /// <param name="line">string to decode</param>
        /// <returns>Hex decoded string</returns>
        public static string HexDecoder(string line)
        {
            string result = string.Empty;
            if (null != line)
            {
                Regex re = new Regex(ServiceConstants.HEX_DECODER_REGEX, RegexOptions.IgnoreCase);
                result = re.Replace(line, new MatchEvaluator(HexDecoderEvaluator));
            }
            else
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Decodes a Quoted-Printable string of any size into 
        /// it's original text.
        /// </summary>
        /// <param name="encoded">
        /// The encoded string to decode.
        /// </param>
        /// <returns>The decoded string.</returns>      
        public static string Decode(string encoded)
        {
            string result = string.Empty;
            if (null != encoded)
            {
                string line = string.Empty;
                using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                {
                    using (StringReader stringReader = new StringReader(encoded))
                    {
                        while (null != (line = stringReader.ReadLine()))
                        {
                            if (line.EndsWith(ServiceConstants.OPERATOR_EQUAL, StringComparison.Ordinal))
                            {
                                stringWriter.Write(HexDecoder(line.Substring(0, line.Length - 1)));
                            }
                            else
                            {
                                stringWriter.WriteLine(HexDecoder(line));
                            }
                            stringWriter.Flush();
                        }
                        result = Convert.ToString(stringWriter, CultureInfo.InvariantCulture);
                    }
                }
            }
            else
            {
                result = null;
            }
            return result;
        }
    }
}
