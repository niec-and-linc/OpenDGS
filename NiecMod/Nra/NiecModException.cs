/*
 * Created by SharpDevelop.
 * User: Niec 2018
 * Date: 11/10/2018
 * Time: 8:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.Serialization;

namespace NiecMod.Nra
{
	/// <summary>
	/// Description of NiecModException.
	/// </summary>
	public class NiecModException : Exception, ISerializable
	{
        public NiecModException()
            //: base(Locale.GetText("Invalid format."))
		{
		}

	 	public NiecModException(string message) : base(message)
		{
		}

		public NiecModException(string message, Exception innerException) : base(message, innerException)
		{
		}

		// This constructor is needed for serialization.
		protected NiecModException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}