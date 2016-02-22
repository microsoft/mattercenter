// ***********************************************************************
// Assembly         : Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
// Author           : v-prd
// Created          : 04-06-2014
//
// ***********************************************************************
// <copyright file="ConsoleCopy.cs" company="Microsoft">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary>This file performs various console operations.</summary>
// ***********************************************************************
namespace Microsoft.Legal.MatterCenter.UpdateTaxonomyFields
{
    #region using
    using System;
    using System.IO;
    using System.Text;
    #endregion

    /// <summary>
    /// Class to copy the Console output to log File
    /// </summary>
    internal class ConsoleCopy : IDisposable
    {
        /// <summary>
        /// File stream to perform writing operation on a text file
        /// </summary>
        private FileStream fileStream;

        /// <summary>
        /// File writer to perform writing operation on a text file
        /// </summary>
        private StreamWriter fileWriter;

        /// <summary>
        /// Double writer to perform writing operation on a text file
        /// </summary>
        private TextWriter doubleWriter;

        /// <summary>
        /// Old out to perform writing operation on a text file
        /// </summary>
        private TextWriter oldOut;

        /// <summary>
        /// This is a simple class which subclasses TextWriter to allow redirection of the input to both a file and the console.
        /// </summary>
        internal class DoubleWriter : TextWriter
        {
            /// <summary>
            /// Represent the object to write series of characters to console
            /// </summary>
            private TextWriter console_writer;

            /// <summary>
            /// Represent the object to write series of characters to file
            /// </summary>
            private TextWriter file_writer;

            /// <summary>
            /// Initializes a new instance of the <see cref="DoubleWriter" /> class.
            /// </summary>
            /// <param name="console_writer">Console writer object that can write series of characters</param>
            /// <param name="file_writer">File writer object that can write series of characters</param>
            public DoubleWriter(TextWriter console_writer, TextWriter file_writer)
            {
                this.console_writer = console_writer;
                this.file_writer = file_writer;
            }

            /// <summary>
            /// overriding the encoding
            /// </summary>
            public override Encoding Encoding
            {
                get
                {
                    return this.console_writer.Encoding;
                }
            }

            /// <summary>
            /// Overriding the Flush method
            /// </summary>
            public override void Flush()
            {
                this.file_writer.Flush();
            }

            /// <summary>
            /// Overriding the Write method
            /// </summary>
            /// <param name="value">char value</param>
            public override void Write(char value)
            {
                this.console_writer.Write(value);
                this.file_writer.Write(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleCopy" /> class.
        /// </summary>
        /// <param name="path">Path of file</param>
        public ConsoleCopy(string path)
        {
            //old TextWriter that only writes to console
            oldOut = Console.Out;

            try
            {
                fileStream = File.Create(path);

                fileWriter = new StreamWriter(fileStream);
                fileWriter.AutoFlush = true;
                //calling the constructor for DoubleWriter
                doubleWriter = new DoubleWriter(fileWriter, oldOut);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Cannot open file for writing");
                Console.WriteLine(exception.Message);
                return;
            }
            //setting the default TextWriter as DoubleWriter
            Console.SetOut(doubleWriter);
        }

        /// <summary>
        /// Dispose the all object
        /// </summary>
        public void Dispose()
        {
            Console.SetOut(oldOut);
            if (fileWriter != null)
            {
                fileWriter.Flush();
                fileWriter.Close();
                doubleWriter.Flush();
                doubleWriter.Close();
                fileWriter = null;
            }
            if (fileStream != null)
            {
                fileStream.Close();
                fileStream = null;
            }
        }
    }
}
