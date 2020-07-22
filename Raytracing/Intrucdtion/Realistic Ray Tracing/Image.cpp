#include "Image.h"
#include "Color.h"

using namespace std;

Image::Image()
{
}

Image::Image(int width, int height)
{
	nx = width;
	ny = height;

	raster = new Color * [nx];
	for (int i = 0; i < nx; i++)
		raster[i] = new Color[ny];
}

Image::Image(int width, int height, Color background)
{
	nx = width;
	ny = height;
	raster = new Color * [nx];
	for (int i = 0; i < nx; i++)
	{
		raster[i] = new Color[ny];
		for (int j = 0; j < ny; j++)
		{
			raster[i][j] = background;
		}
	}
}

bool Image::set(int x, int y, const Color& color)
{
	if (x < 0 || x >= nx)return false;
	if (y < 0 || y >= ny)return false;

	raster[x][y] = color;
	return true;
}

void Image::GammaCorrect(float gamma)
{
	Color temp;
	RFLOAT power = 1.0 / gamma;
	for(int i =0 ; i <nx; i++)
		for (int j = 0; j < ny; j++)
		{
			temp = raster[i][j];
			raster[i][j] = Color(pow(temp.r(), power), (temp.g(), power), (temp.b(), power));			
		}
}

void Image::writePPM(std::ostream& out)
{
	out << "P6\n";
	out << nx << ' ' << ny << '\n';
	out << "255\n";

	int i, j;
	unsigned int ired, igreen, iblue;
	unsigned char red, green, blue;

	for(i = ny-1; i > 0; i--)
		for (j = 0; j < nx; j++)
		{
			ired = (unsigned int)(256 * raster[j][i].r());
			igreen = (unsigned int)(256 * raster[j][i].g());
			iblue = (unsigned int)(256 * raster[j][i].b());

			if (ired > 255) ired = 255;
			if (igreen > 255) igreen = 255;
			if (iblue > 255) iblue = 255;

			red = (unsigned char)(ired);
			green = (unsigned char)(igreen);
			blue = (unsigned char)(iblue);
			out.put(red);
			out.put(green);
			out.put(blue);		
		}
}

void Image::writePPM2(std::ostream& out)
{
	cout << "P3\n" << nx << ' ' << ny << "\n255\n";

	for (int j = ny - 1; j >= 0; --j) {
		for (int i = 0; i < nx; ++i) {
			auto r = (unsigned int)(256 * raster[j][i].r());
			auto g = (unsigned int)(256 * raster[j][i].g());
			auto b = (unsigned int)(256 * raster[j][i].b());

			cout << r << ' ' << g << ' ' << b << '\n';
		}
	}
}

void Image::readPPM(string file_name)
{
	ifstream in;
	in.open(file_name.c_str());
	if (!in.is_open())
	{
		cerr << "ERROR -- couldn't open file " << file_name << "\n";
		exit(-1);
	}

	char ch, type;
	char red, green, blue;
	int i, j, cols, rows;
	int num;

	in.get(ch);
	in.get(type);
	in >> cols >> rows >> num;

	nx = cols;
	ny = rows;

	raster = new Color * [nx];
	for (i = 0; i < nx; i++)
		raster[i] = new Color[ny];

	in.get(ch);

	for(i = ny - 1; i>=0; j--)
		for (j = 0; j < nx; j++)
		{
			in.get(red);
			in.get(green);
			in.get(blue);
			raster[j][i] = Color(
				(RFLOAT)((unsigned char)red) / 255.0,
				(RFLOAT)((unsigned char)green) / 255.0,
				(RFLOAT)((unsigned char)blue) / 255.0);
		}
	
}
