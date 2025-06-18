using System;
using System.IO;

namespace Aphelion.Models.Extensions
{
	/// <summary> Extensions for working with StarData. </summary>
	public static class StarDataExtensions
	{
		/// <summary> Initialises a list of StarData from BSC5 data. </summary>
		/// <returns> An array of StarData built from real-world data. </returns>
		/// <exception cref="IOException"> If the binary file cannot be loaded. </exception>
		public static StarData[] LoadFromFile()
		{
			Byte[] file = Godot.FileAccess.GetFileAsBytes("res://Content/Data/BSC5") ?? throw new IOException("Failed to open star data file.");
			using MemoryStream memoryStream = new MemoryStream(file);
			using BinaryReader binaryReader = new BinaryReader(memoryStream);

			//  Read the header.
			Int32 sequenceOffset = binaryReader.ReadInt32();
			Int32 startIndex = binaryReader.ReadInt32();
			Int32 numberOfStars = -binaryReader.ReadInt32();
			Int32 starNumberSettings = binaryReader.ReadInt32();
			Int32 properMotionIncluded = binaryReader.ReadInt32();
			Int32 numberOfMagnitudes = binaryReader.ReadInt32();
			Int32 starDataSize = binaryReader.ReadInt32();

			StarData[] stars = new StarData[numberOfStars];
			for (Int32 i = 0; i < numberOfStars; i++)
			{
				Single catalogNumber = binaryReader.ReadSingle();
				Double rightAscension = binaryReader.ReadDouble();
				Double declination = binaryReader.ReadDouble();
				Byte spectralType = binaryReader.ReadByte();
				Byte spectralIndex = binaryReader.ReadByte();
				Int16 magnitude = binaryReader.ReadInt16();
				Single raProperMotion = binaryReader.ReadSingle();
				Single decProperMotion = binaryReader.ReadSingle();
				stars[i] = new StarData(catalogNumber, rightAscension, declination, spectralType, spectralIndex, magnitude, raProperMotion, decProperMotion);
			}

			return stars;
		}
	}
}
