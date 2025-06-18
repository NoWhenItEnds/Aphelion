using Godot;
using System;

namespace Aphelion.Models
{
	public record StarData
	{
		public Single CatalogNumber { get; private set; }

		public Double RightAscension { get; init; }

		public Double Declination { get; init; }

		public Double RaProperMotion { get; init; }

		public Double DecProperMotion { get; init; }

		public Single Magnitude { get; init; }

		public Vector3 Position { get; private set; }

		public Color Colour { get; private set; }

		public Single Scale { get; private set; }


		public StarData(Single catalogNumber, Double rightAscension, Double declination, Byte spectralType, Byte spectralIndex, Int16 magnitude, Single raProperMotion, Single decProperMotion)
		{
			CatalogNumber = catalogNumber;
			RightAscension = rightAscension;
			Declination = declination;
			RaProperMotion = raProperMotion;
			DecProperMotion = decProperMotion;

			Position = GetBasePosition(rightAscension, declination);
			Colour = SetColour(spectralType, spectralIndex);
			Scale = SetSize(magnitude);
		}


		private Vector3 GetBasePosition(Double rightAscension, Double declination)
		{
			Double x = Mathf.Cos(rightAscension);
			Double y = Mathf.Sin(declination);
			Double z = Mathf.Sin(rightAscension);
			Double yCos = Mathf.Cos(declination);
			x *= yCos;
			z *= yCos;

			return new Vector3((Single)x, (Single)y, (Single)z);
		}

		private Color SetColour(Byte spectralType, Byte spectralIndex)
		{
			// OBAFGKM colours from: https://arxiv.org/pdf/2101.06254.pdf
			Color[] colours = new Color[8];
			colours[0] = new Color(0x5c, 0x7c, 0xff); // O1
			colours[1] = new Color(0x5d, 0x7e, 0xff); // B0.5
			colours[2] = new Color(0x79, 0x96, 0xff); // A0
			colours[3] = new Color(0xb8, 0xc5, 0xff); // F0
			colours[4] = new Color(0xff, 0xef, 0xed); // G1
			colours[5] = new Color(0xff, 0xde, 0xc0); // K0
			colours[6] = new Color(0xff, 0xa2, 0x5a); // M0
			colours[7] = new Color(0xff, 0x7d, 0x24); // M9.5

			Int32 colourIndex = -1;
			if (spectralType == 'O')
				colourIndex = 0;
			else if (spectralType == 'B')
				colourIndex = 1;
			else if (spectralType == 'A')
				colourIndex = 2;
			else if (spectralType == 'F')
				colourIndex = 3;
			else if (spectralType == 'G')
				colourIndex = 4;
			else if (spectralType == 'K')
				colourIndex = 5;
			else if (spectralType == 'M')
				colourIndex = 6;

			// If unknown, make white.
			if (colourIndex == -1) {
				return new Color(1f, 1f, 1f, 1f);
			}

			// Map second part 0 -> 0, 10 -> 100
			Single percent = (spectralIndex - 0x30) / 10.0f;
			return colours[colourIndex].Lerp(colours[colourIndex + 1], percent) / 255f;
		}

		private Single SetSize(Int16 magnitude)
		{
			return 1f - Mathf.InverseLerp(-146f, 796f, magnitude);
		}
    }
}
