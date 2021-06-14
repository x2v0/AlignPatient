// $Id: TcpStreamHelper.cs 6879 2019-02-05 06:05:57Z onuchin $
//
// Copyright (C) 2018 Valeriy Onuchin

//#define LOCAL_DEBUG

using System;
using System.IO;

namespace P.Net
{
   public static class TcpStreamHelper
   {
      #region Public methods

      // This is not suitable for large files because the SEND() buffer may get filled up and throw an exception
      // if you attempt to write to it. You should change this to use the strongly typed networkstream and ensure
      // you have enough room to send data
      public static void CopyStreamToStream(Stream source, Stream destination, Action<Stream, Stream, Exception> completed)
      {
         var buffer = new byte[0x1000];
         int read;

         try {
            while ((read = source.Read(buffer, 0, buffer.Length)) > 0) {
               destination.Write(buffer, 0, read);
            }

            if (completed != null) {
               completed(source, destination, null);
            }
         } catch (Exception exc) {
            if (completed != null) {
               completed(source, destination, exc);
            }
         }
      }

      #endregion
   }
}
