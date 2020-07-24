#pragma once

#include <string>
#include <fstream>
#include "Common.h"
class Color;

class Image
{
public:
	Image();
	Image(int width, int height);
	Image(int width, int height, Color background);

	bool set(int x, int y, const Color& color);
	Color& GetPixel(int x, int y) const;
	void GammaCorrect(float gamma);
	void writePPM(std::ostream& out);
	void writePPM2(std::ostream& out);
	void readPPM(std::string file_name);

	RFLOAT width() { return nx; }
	RFLOAT height() { return ny; }

		
private:

	Color** raster;
	int nx;
	int ny;
};

