#pragma once

#include <string>
#include <fstream>
class Color;

class Image
{
public:
	Image();
	Image(int width, int height);
	Image(int width, int height, Color background);

	bool set(int x, int y, const Color& color);
	void GammaCorrect(float gamma);
	void writePPM(std::ostream& out);
	void writePPM2(std::ostream& out);
	void readPPM(std::string file_name);
private:

	Color** raster;
	int nx;
	int ny;
};

